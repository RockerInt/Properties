using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class WebUtilities
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };
        public enum Method
        {
            Get,
            Post,
            Put,
            Delete
        }

        public async static Task<HttpResponseMessage> ConectAsync(Method method, string baseAddress, string path, dynamic data, Dictionary<string, string> headers = null)
        {
            string httpContent = JsonConvert.SerializeObject(data);

            using var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> entry in headers)
                {
                    client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }
            else
                client.DefaultRequestHeaders.Accept.Clear();

            StringContent stringContent = null;

            if (!string.IsNullOrEmpty(httpContent?.ToString()))
            {
                stringContent = new StringContent(httpContent, Encoding.UTF8, "application/json");
            }

            return method switch
            {
                Method.Get => await client.GetAsync(path),
                Method.Put => await client.PutAsync(path, stringContent),
                Method.Delete => await client.DeleteAsync(path),
                _ => await client.PostAsync(path, stringContent),
            };
        }

        public async static Task<HttpResponseMessage> ConectAsync(Method method, string baseAddress, string path, string httpContent, Dictionary<string, string> headers = null)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> entry in headers)
                {
                    client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }
            else
                client.DefaultRequestHeaders.Accept.Clear();

            StringContent stringContent = null;

            if (!string.IsNullOrEmpty(httpContent))
            {
                stringContent = new StringContent(httpContent, Encoding.UTF8, "application/json");
            }

            return method switch
            {
                Method.Get => await client.GetAsync(path),
                Method.Put => await client.PutAsync(path, stringContent),
                Method.Delete => await client.DeleteAsync(path),
                _ => await client.PostAsync(path, stringContent),
            };
        }

        public static HttpResponseMessage Conect(Method method, string baseAddress, string path, dynamic data, Dictionary<string, string> headers = null)
            => ConectAsync(method, baseAddress, path, data, headers).Result;

        public static HttpResponseMessage Conect(Method method, string baseAddress, string path, string httpContent, Dictionary<string, string> headers = null)
            => ConectAsync(method, baseAddress, path, httpContent, headers).Result;

        public static string ValidateContent(this HttpResponseMessage httpResponse)
        {
            string resp = null;
            if (httpResponse.Content.Headers.ContentLength > 0)
            {
                Stream stream = httpResponse.Content.ReadAsStreamAsync().Result;
                StreamReader sr = new StreamReader(stream);
                resp = sr.ReadToEnd();
            }
            return resp;
        }

        public static string JsonSerialize(this object obj, JsonSerializerSettings settings = null) => JsonConvert.SerializeObject(obj, settings ?? _settings); 

        public static T ToEntity<T>(this string obj) where T : class, new()
        {
            PropertyInfo[] piPropiedades = typeof(T).GetProperties();

            T returnObj = new T();

            JObject jObj = JObject.Parse(obj);

            foreach (PropertyInfo piPropiedad in piPropiedades)
            {
                try
                {
                    string stringValue = (string)(jObj.SelectToken(piPropiedad.Name));
                    TypeConverter tc = TypeDescriptor.GetConverter(piPropiedad.PropertyType);
                    if (stringValue != null)
                    {
                        object value = tc.ConvertFromString(null, CultureInfo.InvariantCulture, stringValue);
                        piPropiedad.SetValue(returnObj, value);
                    }
                }
                catch { }
            }

            return returnObj;
        }

        public static T ToEntitySimple<T>(this string obj) => JsonConvert.DeserializeObject<T>(obj);
        
        public static List<T> ToEntityList<T, D>(this string objs) where T : class, new() where D : class, new()
        {
            PropertyInfo[] piPropiedades = typeof(T).GetProperties();

            List<T> returnObjList = new List<T>();

            var jArray = JsonConvert.DeserializeObject<dynamic[]>(objs);

            foreach (var obj in jArray)
            {
                JObject jObj = JObject.Parse(obj.ToString());
                T preReturnObj = new T();
                foreach (PropertyInfo piPropiedad in piPropiedades)
                {
                    if (!typeof(System.Collections.IList).IsAssignableFrom(piPropiedad.PropertyType))
                    {
                        string stringValue = (string)(jObj.SelectToken(piPropiedad.Name));
                        TypeConverter tc = TypeDescriptor.GetConverter(piPropiedad.PropertyType);
                        if (stringValue != null)
                        {
                            object value = tc.ConvertFromString(null, CultureInfo.InvariantCulture, stringValue);
                            piPropiedad.SetValue(preReturnObj, value);
                        }
                    }
                    else if (!typeof(T).Equals(typeof(D)))
                    {
                        PropertyInfo[] piPropiedadesSub = typeof(D).GetProperties();
                        if (!string.IsNullOrWhiteSpace(jObj.SelectToken(piPropiedad.Name).ToString()))
                        {
                            JArray jArraySub = JArray.Parse(jObj.SelectToken(piPropiedad.Name).ToString());

                            List<D> asingObjSub = ToEntityList<D, D>(jArraySub.ToString());
                            piPropiedad.SetValue(preReturnObj, asingObjSub);
                        }
                    }
                }
                returnObjList.Add(preReturnObj);
            }
            return returnObjList;
        }

        public static List<T> ToEntityListSimple<T>(this string objs) => JsonConvert.DeserializeObject<List<T>>(objs);

        public static Dictionary<T, D> ToDictionary<T, D>(this string objs) => JsonConvert.DeserializeObject<Dictionary<T, D>>(objs);

        public static bool TryParseJson<T>(this string json, out T result)
        {
            bool success = true;

            if (string.IsNullOrEmpty(json))
            {
                success = false;
                result = (T)(object)string.Empty;
                return success;
            }
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(json, settings);
            return success;
        }

        public static T MapResponse<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return response.ValidateContent().ToEntitySimple<T>();
            else
                throw HttpCallError(response);
        }

        public static IEnumerable<T> MapListResponse<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return response.ValidateContent().ToEntityListSimple<T>();
            else
                throw HttpCallError(response);
        }

        public static Exception HttpCallError(this HttpResponseMessage response) =>
            new($"StatusCode: {Convert.ToInt16(response.StatusCode)}, {Environment.NewLine} Messege: {response.ValidateContent()}");
    }
}
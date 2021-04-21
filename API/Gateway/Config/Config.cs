using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Properties.Gateway.Config
{
    public class UrlsConfig
    {
        public string PropertiesService { get; set; }

        public string PropertiesGrpc { get; set; }


        public class PropertyOperations
        {
            public static string Get(string queryString) => $"/api/v1/Properties/Get{queryString}";

            public static string GetById(Guid id) => $"/api/v1/Properties/Get/{id}";

            public static string Create() => $"/api/v1/Properties/Create";

            public static string Update() => $"/api/v1/Properties/Update";

            public static string Delete(Guid id) => $"/api/v1/Properties/Delete/{id}";
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Utilities
    {
        #region String
        public static Guid? ToGuid(this string id) => Guid.TryParse(id, out Guid guid) ? guid : null;
        #endregion

        #region DateTime
        public static DateTime SetKind(this DateTime dateTime, DateTimeKind kind) => 
            new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kind);
        #endregion

        #region Functions
        public static TResponse TryCatch<TResponse>(this Func<TResponse> func, Action<Exception> handleError = null)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                handleError?.Invoke(e);
                throw;
            }
        }
        public static TResponse TryCatch<TResponse>(this Func<TResponse> func, Func<Exception, TResponse> handleError)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return handleError(e);
            }
        }
        public static async Task<TResponse> TryCatchAsync<TResponse>(this Func<Task<TResponse>> func, Action<Exception> handleError = null)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                handleError?.Invoke(e);
                throw;
            }
        }
        public static async Task<TResponse> TryCatchAsync<TResponse>(this Func<Task<TResponse>> func, Func<Exception, Task<TResponse>> handleError)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                return await handleError(e);
            }
        }
        #endregion
    }
}

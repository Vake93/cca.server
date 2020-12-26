using CCA.Application.Services.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CCA.Application.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> DeserializeRequestAsync<T>(this HttpRequest request)
        {
            using var streamReader = new StreamReader(request.Body);
            var requestJson = await streamReader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(requestJson);
        }

        public static bool AuthenticateRequest(this HttpRequest request, out string id, out string email)
        {
            var token = request.Headers["Authorization"];
            return TokenManager.ValidateJSONWebToken(token, validateLifetime: true, out id, out email, out _);
        }

        public static T QueryParam<T>(this HttpRequest request, string paramName, T defaultValue)
        {
            var value = request.Query[paramName];

            if (!string.IsNullOrEmpty(value))
            {
                if ((typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?)) && DateTime.TryParse(value, out var date))
                {
                    return (dynamic)date;
                }

                if (typeof(T) == typeof(int) && int.TryParse(value, out var intValue))
                {
                    return (dynamic)intValue;
                }

                if (typeof(T) == typeof(string))
                {
                    return (dynamic)value;
                }

                throw new NotImplementedException($"{typeof(T).Name} is not implemented");
            }

            return defaultValue;
        }
    }
}

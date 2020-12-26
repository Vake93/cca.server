using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CCA.Models.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthenticationProviderType
    {
        Google,
        Microsoft
    }
}

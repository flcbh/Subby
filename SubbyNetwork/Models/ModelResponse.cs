using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SubbyNetwork.Models
{
    public class ModelResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Actions Action { get; set; } = Actions.Redirect;
        public string Url { get; set; }
        public object Data { get; set; }
        public string JavascriptAction { get; set; }

        public string JavascriptResponse
        {
            get
            {
                if (string.IsNullOrEmpty(JavascriptAction))
                {
                    return string.Empty;
                }

                if (JavascriptData == null)
                {
                    return JavascriptAction + "()";
                }

                return JavascriptAction + "('" + JsonConvert.SerializeObject(JavascriptData) + "')";
            }
        }
        public object JavascriptData { get; set; }
    }

    public enum Actions
    {
        Redirect,
        Reload,
        Js
    }
}
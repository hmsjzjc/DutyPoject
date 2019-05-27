using System;
using Newtonsoft.Json;

namespace zProject.Helper
{
    public class JsonHelper
    {
        public JsonHelper()
        {
        }
        public static class UserJson
        {
            public string result { get; set; }
            public string id { get; set; }
            public string truename { get; set; }
            public string img { get; set; }
            public string token { get; set; }
        }
    }
}

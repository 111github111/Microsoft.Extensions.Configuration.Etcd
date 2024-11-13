using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteEtcd.Helpers
{
    public class JsonHelpers
    {
        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings();

        static JsonHelpers()
        {
            Settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }
    }
}

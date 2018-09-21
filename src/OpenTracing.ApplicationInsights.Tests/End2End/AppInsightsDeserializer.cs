using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OpenTracing.ApplicationInsights.Tests.End2End
{
    public static class AppInsightsDeserializer
    {
        public static AppInsightsResult DeserializeResult(string appInsightsJson)
        {
            return JsonConvert.DeserializeObject<AppInsightsResult>(appInsightsJson);
        }
    }

    public class Column
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Table
    {
        public string name { get; set; }
        public List<Column> columns { get; set; }
        public List<List<object>> rows { get; set; }
    }

    public class AppInsightsResult
    {
        public List<Table> tables { get; set; }
    }
}

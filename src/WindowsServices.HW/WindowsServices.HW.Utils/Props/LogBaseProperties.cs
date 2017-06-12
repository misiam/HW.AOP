using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServices.HW.Utils.Props
{
    public class LogBaseProperties : BaseProperties
    {

        public LogBaseProperties(string propsArgs) : base(propsArgs)
        {
        }

        public LogBaseProperties(BaseProperties baseProperties) : base(baseProperties)
        {
        }

        public LogBaseProperties(IDictionary<string, string> properties) : base(properties)
        {
        }

        public string LogPath => Properties[PropsNames.LogPath];

        public bool UseCodeRewritingLogs => Properties.ContainsKey(PropsNames.CodeRewritingLogs) && Properties[PropsNames.CodeRewritingLogs] == "true";
        public bool UseDynamicProxyLogs => Properties.ContainsKey(PropsNames.DynamicProxyLogsLogs) && Properties[PropsNames.DynamicProxyLogsLogs] == "true";


        public static string GetLogPath(string[] args, string defaultPath)
        {
            var properties = BaseProperties.GetProperties(args);
            if (properties == null)
            {
                return defaultPath;
            }
            var logProperties = new LogBaseProperties(properties);

            return logProperties.LogPath;
        }


    }
}

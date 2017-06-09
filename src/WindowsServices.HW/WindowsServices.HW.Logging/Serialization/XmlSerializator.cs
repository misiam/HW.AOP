using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WindowsServices.HW.Logging;

namespace WindowsServices.HW.Utils.Serialization
{
    class XmlSerializator : ISerializer
    {
        public string Serialize(object arg)
        {
            IFormatter formatter = new NetDataContractSerializer();
            try
            {
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, arg);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var readerStream = new StreamReader(stream))
                    {
                        string result = readerStream.ReadToEnd();

                        return result;
                    }
                }
            }
            catch (InvalidDataContractException e)
            {
                return $"Not serializable [{arg.GetType()}]";
            }
            catch (Exception e)
            {
                var logger = Logger.Current;
                logger.LogInfo($"XmlSerializer error: {e}");
                throw;
            }
        }

    }
}

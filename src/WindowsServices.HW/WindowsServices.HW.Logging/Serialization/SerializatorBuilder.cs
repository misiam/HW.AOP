using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServices.HW.Utils.Serialization
{
    public static class SerializatorBuilder
    {
        public static ISerializer CreateSerializer(SerializatorType type)
        {
            switch (type)
            {
                    case SerializatorType.Xml:
                        return new XmlSerializator();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

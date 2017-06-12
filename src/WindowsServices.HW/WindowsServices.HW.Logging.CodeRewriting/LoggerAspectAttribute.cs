using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using WindowsServices.HW.Utils.Serialization;

namespace WindowsServices.HW.Logging.CodeRewriting
{
    [Serializable]
    public class LoggerAspectAttribute : OnMethodBoundaryAspect
    {
        public SerializatorType SerializatorType { get; set; } = SerializatorType.Xml;

        public override void OnEntry(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogInfo($"   [PostSharp] ON ENTRY: {args.Method.Name}");
            var serializer = SerializatorBuilder.CreateSerializer(this.SerializatorType);

            foreach (var arg in args.Arguments)
            {
                logger.LogInfo($"   [PostSharp] ON ENTRY arg: { serializer.Serialize(arg ?? "[null]")}");

            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogInfo($"   [PostSharp] ON SUCCESS: {args.Method.Name}");

            var serializer = SerializatorBuilder.CreateSerializer(this.SerializatorType);
            logger.LogInfo($"   [PostSharp] ON SUCCESS result: { serializer.Serialize(args.ReturnValue ?? "[null]")}");

            base.OnSuccess(args);
            //MemoryCache.Default.Set(key, args.ReturnValue, new CacheItemPolicy());
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogError($"   [PostSharp] ON EXCEPTION: {args.Method.Name} " + args.Exception);

            base.OnException(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogInfo($"   [PostSharp] ON EXIT: {args.Method.Name}");
            base.OnExit(args);
        }

    }
}

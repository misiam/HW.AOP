using System;
using PostSharp.Aspects;
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
            logger.LogInfo($"   [PostSharp] ON ENTRY: {GetMethodName(args)}");
            var serializer = SerializatorBuilder.CreateSerializer(this.SerializatorType);

            foreach (var arg in args.Arguments)
            {
                logger.LogInfo($"   [PostSharp] ON ENTRY arg: { serializer.Serialize(arg ?? "[null]")}");

            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogInfo($"   [PostSharp] ON SUCCESS: {GetMethodName(args)}");

            var serializer = SerializatorBuilder.CreateSerializer(this.SerializatorType);
            logger.LogInfo($"   [PostSharp] ON SUCCESS result: { serializer.Serialize(args.ReturnValue ?? "[null]")}");

            base.OnSuccess(args);
            //MemoryCache.Default.Set(key, args.ReturnValue, new CacheItemPolicy());
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogError($"   [PostSharp] ON EXCEPTION: {GetMethodName(args)} " + args.Exception);

            base.OnException(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var logger = Logger.CodeRewritingLogger;
            logger.LogInfo($"   [PostSharp] ON EXIT: {GetMethodName(args)}");
            base.OnExit(args);
        }

        private string GetMethodName(MethodExecutionArgs args)
        {
            string methodName = args.Method.DeclaringType.Name + "." + args.Method.Name;
            return methodName;
        }

    }
}

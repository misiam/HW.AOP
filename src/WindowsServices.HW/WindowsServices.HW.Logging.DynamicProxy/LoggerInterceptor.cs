using System;
using WindowsServices.HW.Utils.Serialization;
using Castle.DynamicProxy;

namespace WindowsServices.HW.Logging.DynamicProxy
{
    public class LoggerInterceptor : IInterceptor
    {
        private readonly bool _useDynamicProxyLogs;

        public LoggerInterceptor(bool useDynamicProxyLogs)
        {
            _useDynamicProxyLogs = useDynamicProxyLogs;
        }

        public SerializatorType SerializatorType { get; set; } = SerializatorType.Xml;


        public void Intercept(IInvocation invocation)
        {
            if (!_useDynamicProxyLogs)
            {
                invocation.Proceed();
                return;
            }

            string methodName = invocation.TargetType.Name + "." + invocation.Method.Name;

            var logger = Logger.Current;
            logger.LogInfo("   [DynamicProxy] ON ENTRY " + methodName);

            var serializer = SerializatorBuilder.CreateSerializer(SerializatorType);

            foreach (var arg in invocation.Arguments)
            {
                logger.LogInfo($"   [DynamicProxy] ON ENTRY arg: { serializer.Serialize(arg ?? "[null]")}");
            }

            try
            {
                invocation.Proceed();
                logger.LogInfo($"   [DynamicProxy] ON SUCCESS: {methodName}");
                logger.LogInfo($"   [DynamicProxy] ON SUCCESS result: { serializer.Serialize(invocation.ReturnValue ?? "[null]")}");

            }
            catch (Exception e)
            {
                logger.LogError($"   [DynamicProxy] ON EXCEPTION: {methodName} " + e);
            }

            logger.LogInfo($"   [DynamicProxy] ON EXIT: {methodName}");
        }
    }



}

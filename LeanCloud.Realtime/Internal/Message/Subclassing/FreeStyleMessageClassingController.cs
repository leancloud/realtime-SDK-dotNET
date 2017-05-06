using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LeanCloud.Realtime.Internal
{
    internal class FreeStyleMessageClassingController : IFreeStyleMessageClassingController
    {
        private static readonly string messageClassName = "_AVIMMessage";
        private readonly IDictionary<string, FreeStyleMessageClassInfo> registeredInterfaces;
        private readonly ReaderWriterLockSlim mutex;
        public FreeStyleMessageClassingController()
        {
            mutex = new ReaderWriterLockSlim();
            registeredInterfaces = new Dictionary<string, FreeStyleMessageClassInfo>();
        }
        public Type GetType(IDictionary<string, object> msg)
        {
            throw new NotImplementedException();
        }

        public IAVIMMessage Instantiate(string msgStr, IDictionary<string, object> buildInData)
        {
            FreeStyleMessageClassInfo info = null;
            mutex.EnterReadLock();
            var reverse = registeredInterfaces.Values.Reverse();
            foreach (var subInterface in reverse)
            {
                if (subInterface.Validate(msgStr))
                {
                    info = subInterface;
                    break;
                }
            }
            mutex.ExitReadLock();
            var rtn = info != null ? info.Instantiate(msgStr) : new AVIMMessage();

            if (buildInData.ContainsKey("timestamp"))
            {
                long timestamp = 0;
                if (long.TryParse(buildInData["timestamp"].ToString(), out timestamp))
                {
                    rtn.ServerTimestamp = timestamp;
                }
            }
            if (buildInData.ContainsKey("ackAt"))
            {
                long ackAt = 0;
                if (long.TryParse(buildInData["ackAt"].ToString(), out ackAt))
                {
                    rtn.RcpTimestamp = ackAt;
                }
            }
            if (buildInData.ContainsKey("from"))
            {
                rtn.FromClientId = buildInData["from"].ToString();
            }
            if (buildInData.ContainsKey("msgId"))
            {
                rtn.Id = buildInData["msgId"].ToString();
            }
            if (buildInData.ContainsKey("cid"))
            {
                rtn.ConversationId = buildInData["cid"].ToString();
            }
            if (buildInData.ContainsKey("fromPeerId"))
            {
                rtn.FromClientId = buildInData["fromPeerId"].ToString();
            }
            if (buildInData.ContainsKey("id"))
            {
                rtn.Id = buildInData["id"].ToString();
            }
            rtn.Deserialize(msgStr);
            return rtn;
        }
        public IDictionary<string, object> EncodeProperties(IAVIMMessage subclass)
        {
            var type = subclass.GetType();
            var result = new Dictionary<string, object>();
            var className = GetClassName(type);
            var propertMappings = GetPropertyMappings(className);
            foreach (var propertyPair in propertMappings)
            {
                var propertyInfo = ReflectionHelpers.GetProperty(type, propertyPair.Key);
                var operation = propertyInfo.GetValue(subclass, null);
                result[propertyPair.Value] = PointerOrLocalIdEncoder.Instance.Encode(operation);
            }
            return result;
        }
        public bool IsTypeValid(IDictionary<string, object> msg, Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterSubclass(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            if (!typeof(IAVIMMessage).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                throw new ArgumentException("Cannot register a type that is not a implementation of IAVIMMessage");
            }
            var className = GetClassName(type);
            try
            {
                mutex.EnterWriteLock();
                ConstructorInfo constructor = type.FindConstructor();
                if (constructor == null)
                {
                    throw new ArgumentException("Cannot register a type that does not implement the default constructor!");
                }
                registeredInterfaces[className] = new FreeStyleMessageClassInfo(type, constructor);
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }
        public String GetClassName(Type type)
        {
            return type == typeof(IAVIMMessage)
              ? messageClassName
              : FreeStyleMessageClassInfo.GetMessageClassName(type.GetTypeInfo());
        }
        public IDictionary<String, String> GetPropertyMappings(String className)
        {
            FreeStyleMessageClassInfo info = null;
            mutex.EnterReadLock();
            registeredInterfaces.TryGetValue(className, out info);
            if (info == null)
            {
                registeredInterfaces.TryGetValue(messageClassName, out info);
            }
            mutex.ExitReadLock();

            return info.PropertyMappings;
        }
    }
}

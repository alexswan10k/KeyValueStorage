using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace KeyValueStorage.ORM.Utility
{
    public class ProxyBuilder : IProxyBuilder
    {
        public Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            throw new NotImplementedException();
        }

        public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            throw new NotImplementedException();
        }

        public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, Type targetType, ProxyGenerationOptions options)
        {
            throw new NotImplementedException();
        }

        public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            throw new NotImplementedException();
        }

        public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            throw new NotImplementedException();
        }

        public Castle.Core.Logging.ILogger Logger
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ModuleScope ModuleScope
        {
            get { throw new NotImplementedException(); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using ServiceStack.Text;

namespace KeyValueStorage.ORM.Utility
{
    public class AssocTypeInterceptor : IInterceptor
    {
        public ContextBase Context { get; protected set; }
        public string EntityFullKey { get; protected set; }

        public AssocTypeInterceptor(ContextBase context)//, string fullKey)
        {
            Context = context;
            LazyLoadedProps = new Dictionary<string, string>();
            //EntityFullKey = fullKey;
        }

        public bool EntityIsLoaded = false;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("get_"))
            {
                //if (!EntityIsLoaded)
                //{
                //    //load!
                //    Context.Store.Get<Dictionary<string, string>>(EntityFullKey);
                //    EntityIsLoaded = true;
                //}

                var relMap = Context.ContextMap.RelationshipMaps.FirstOrDefault(q=>q.LocalObjectMap.EntityType == invocation.Method.ReturnType);

                if (relMap != null)
                {

                }

                //check the types
                //var relMap = Context.ContextMap.RelationshipMaps.FirstOrDefault(q => q.LocalObjectMap.EntityType == invocation.Method.ReturnType);
                //if (relMap != null)
                //{

                //}
            }
        }

        public Dictionary<string, string> LazyLoadedProps { get; protected set; }
    }
}

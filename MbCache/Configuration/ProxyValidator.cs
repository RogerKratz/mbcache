using System;
using System.Reflection;

namespace MbCache.Configuration
{
    public class ProxyValidator
    {
        private readonly IProxyFactory _proxyFactory;

        public ProxyValidator(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }

        public void Validate(Type proxyType)
        {
            if(!_proxyFactory.AllowNonVirtualMember)
                checkAccessibleMembersAreVirtual(proxyType);
        }

        private void checkAccessibleMembersAreVirtual(Type type)
        {
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var member in members)
            {
                if(!checkProperty(type, member))
                {
                    if(!checkMethod(type, member))
                    {
                        checkField(type, member);
                    }
                }
                else
                {
                    var field = member as FieldInfo;
                    if (field!=null)
                    {
                        if (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly)
                        {
                            throw new InvalidOperationException("Type " + type + "'s field " + member.Name + " is public. Proxy factory " +
                                                _proxyFactory.GetType().FullName + " does not allow this.");
                        }
                    }        
                }
            }
        }

        private bool checkField(Type type, MemberInfo member)
        {
            var field = member as FieldInfo;
            if(field!=null)
            {
                if (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly)
                {
                    throw new InvalidOperationException("Type " + type + "'s field " + member.Name + " is public. Proxy factory " +
                                        _proxyFactory.GetType().FullName + " does not allow this.");
                }
                return true;
            }
            return false;
        }

        private bool checkMethod(Type type, MemberInfo member)
        {
            var method = member as MethodInfo;
            if(method!=null)
            {
                if (method.Name != "GetType" || method.DeclaringType != typeof(object))
                {
                    checkMethodIsVirtual(type, method);
                }
                return true;
            }
            return false;
        }

        private bool checkProperty(Type type, MemberInfo member)
        {
            var prop = member as PropertyInfo;
            if (prop != null)
            {
                var accessors = prop.GetAccessors(false);

                foreach (var accessor in accessors)
                {
                    checkMethodIsVirtual(type, accessor);
                }
                return true;
            }
            return false;
        }

        private void checkMethodIsVirtual(Type type, MethodInfo method)
        {
            if (!isProxeable(method))
            {
                throw new InvalidOperationException("Type " + type + "'s member " + method.Name + " is non virtual. Proxy factory " +
                            _proxyFactory.GetType().FullName + " does not allow this.");
            }
        }

        private static bool isProxeable(MethodInfo method)
        {
            // In NET if IsVirtual is false or IsFinal is true, then the method cannot be overridden.
            return !(method.DeclaringType != typeof(object) && !isDisposeMethod(method)
                   && (method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly)
                   && (!method.IsVirtual || method.IsFinal || (method.IsVirtual && method.IsAssembly)));
        }

        private static bool isDisposeMethod(MethodBase method)
        {
            return method.Name.Equals("Dispose") && method.MemberType == MemberTypes.Method && method.GetParameters().Length == 0;
        }
    }
}

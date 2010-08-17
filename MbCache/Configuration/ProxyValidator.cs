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
            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var member in members)
            {
                if (member is PropertyInfo)
                {
                    var property = (PropertyInfo)member;
                    MethodInfo[] accessors = property.GetAccessors(false);

                    if (accessors != null)
                    {
                        foreach (var accessor in accessors)
                        {
                            checkMethodIsVirtual(type, accessor);
                        }
                    }
                }
                else if (member is MethodInfo)
                {
                    if (member.DeclaringType == typeof(object) && member.Name == "GetType")
                    {
                        // object.GetType is ignored
                        continue;
                    }
                    checkMethodIsVirtual(type, (MethodInfo)member);
                }
                else if (member is FieldInfo)
                {
                    var memberField = (FieldInfo)member;
                    if (memberField.IsPublic || memberField.IsAssembly || memberField.IsFamilyOrAssembly)
                    {
                        throw new InvalidOperationException("Type " + type + "'s field " + member.Name + " is public. Proxy factory " +
                                            _proxyFactory.Name + " does not allow this.");
                    }
                }
            }
        }

        private void checkMethodIsVirtual(Type type, MethodInfo method)
        {
            if (!isProxeable(method))
            {
                throw new InvalidOperationException("Type " + type + "'s member " + method.Name + " is non virtual. Proxy factory " +
                            _proxyFactory.Name + " does not allow this.");
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

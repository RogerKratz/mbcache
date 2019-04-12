using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic.Proxy
{
	public class ProxyFactory : IProxyFactory
	{
		private readonly ConcurrentDictionary<Type, Type> _proxyTypeCache = new ConcurrentDictionary<Type, Type>();
		
		public T CreateProxy<T>(T target, ConfigurationForType configurationForType) where T : class
		{
			var componentType = typeof(T);
			var proxyType = _proxyTypeCache.GetOrAdd(componentType,
				t => createProxyType(componentType));
			
			var ctor = proxyType.GetConstructors().Single();
			var proxyInterceptorArg = Expression.Parameter(typeof(ProxyInterceptor));
			var proxyInterceptor = new ProxyInterceptor(target, configurationForType, new CachingComponent(configurationForType));
			var ctorExpr = Expression.New(ctor, proxyInterceptorArg);
			var func = Expression.Lambda<Func<ProxyInterceptor, T>>(ctorExpr, proxyInterceptorArg).Compile();
			
			return func(proxyInterceptor);
		}


		private TypeInfo createProxyType(Type componentType)
		{
			var typeName = $"{componentType.Name}MbCacheProxy";
			var assemblyName = $"{typeName}Assembly";
			var asmFileName = $"{typeName}MbCacheModule.dll";

			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(asmFileName, asmFileName, true);
			
			const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

			Type parentType;
			Type[] interfaces;

			if (componentType.IsInterface)
			{
				parentType = typeof(object);
				interfaces = new[] {typeof(ICachingComponent), componentType};
			}
			else
			{
				parentType = componentType;
				interfaces = new[] {typeof(ICachingComponent)};
			}
			
			var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces);
			var proxyInterceptorField = typeBuilder.DefineField("__proxyInterceptor", typeof(ProxyInterceptor), FieldAttributes.Private);
			implementConstructor(typeBuilder, proxyInterceptorField);
			implementCachingComponent(typeBuilder, proxyInterceptorField);
			overrideComponentMethods(typeBuilder, componentType, proxyInterceptorField);

			var proxyType = typeBuilder.CreateTypeInfo();

			//TODO: not saving ass here!
			assemblyBuilder.Save(asmFileName);
			var file = "d:\\temp\\proxy.dll";
			File.Delete(file);
			File.Copy(asmFileName, file);
			//
			
			return proxyType;
		}

		private void implementConstructor(TypeBuilder typeBuilder, FieldBuilder proxyInterceptorField)
		{
			const MethodAttributes constructorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

			var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
			
			var ctor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new []{typeof(ProxyInterceptor)});
			var IL = ctor.GetILGenerator();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, objectCtor);

			IL.Emit(OpCodes.Ldarg_0); 
			IL.Emit(OpCodes.Ldarg_1); 
			IL.Emit(OpCodes.Stfld, proxyInterceptorField);
			
			IL.Emit(OpCodes.Ret);
		}
		
		private void implementCachingComponent(TypeBuilder typeBuilder, FieldBuilder proxyInterceptorField)
		{
			/*
				return this.__proxyInterceptor.Component.[SameMethod]([sameParams]);
			 */
			
			const MethodAttributes attributes = MethodAttributes.HideBySig | MethodAttributes.Virtual;
			foreach (var methodInfo in typeof(ICachingComponent).GetMethods())
			{
				var method = typeBuilder.DefineMethod(methodInfo.Name, attributes, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
				var typeArgs = methodInfo.GetGenericArguments();
				if (typeArgs.Length > 0)
				{
					var typeNames = GenerateTypeNames(typeArgs.Length);
					method.DefineGenericParameters(typeNames);
				}
				var IL = method.GetILGenerator();
				IL.Emit(OpCodes.Ldarg_0); 
				IL.Emit(OpCodes.Ldfld, proxyInterceptorField);
				IL.Emit(OpCodes.Callvirt, typeof(ProxyInterceptor).GetProperty(nameof(ProxyInterceptor.Component)).GetMethod);
				EmitCallMethod(IL, OpCodes.Callvirt, methodInfo);
				IL.Emit(OpCodes.Ret);
				
				typeBuilder.DefineMethodOverride(method, methodInfo);
			}
		}
		
		private void overrideComponentMethods(TypeBuilder typeBuilder, Type targetType, FieldBuilder proxyInterceptorField)
		{
			var getGenericMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }, null);
			var getMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
			var getTypeFromHandle = typeof (Type).GetMethod("GetTypeFromHandle");
			
			
			/*
			 	var methodInfo = (MethodInfo) MethodBase.GetCurrentMethod();
			 	var parameters = new []{param1, param2...};
			 				
				return this.__proxyInterceptor.DecorateMethodCall(methodInfo, parameters);
			 */

			const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
			foreach (var methodInfo in targetType.GetMethods())
			{
			
				if(methodInfo.DeclaringType == typeof(object))
					continue;
				if(!methodInfo.IsVirtual)
					continue;
				
				var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, attributes, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
				var typeArgs = methodInfo.GetGenericArguments();
				if (typeArgs.Length > 0)
				{
					var typeNames = GenerateTypeNames(typeArgs.Length);
					methodBuilder.DefineGenericParameters(typeNames);
					
				}

				var parameters = methodInfo.GetParameters();
				var IL = methodBuilder.GetILGenerator();
				var localMethodInfo = IL.DeclareLocal(typeof(MethodInfo));
				var localParameters = IL.DeclareLocal(typeof(object[]));
				var localParameterTypes = IL.DeclareLocal(typeof(Type));
				
			
				var declaringType = methodInfo.DeclaringType;
            
				IL.Emit(OpCodes.Ldtoken, methodInfo);
				if (declaringType.IsGenericType)
				{
					IL.Emit(OpCodes.Ldtoken, declaringType);
					IL.Emit(OpCodes.Call, getGenericMethodFromHandle);
				}
				else
				{
					IL.Emit(OpCodes.Call, getMethodFromHandle);
				}
				IL.Emit(OpCodes.Stloc_0, localMethodInfo); 
			
				
				//arguments as array
				IL.Emit(OpCodes.Ldc_I4_S, parameters.Length);
				IL.Emit(OpCodes.Newarr, typeof(object));
				for (var i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];
					IL.Emit(OpCodes.Dup);
					IL.Emit(OpCodes.Ldc_I4, i);
					IL.Emit(OpCodes.Ldarg, i+1);
					IL.Emit(OpCodes.Box, parameter.ParameterType); //TODO: why?
					IL.Emit(OpCodes.Stelem_Ref);
				}
				IL.Emit(OpCodes.Stloc_1, localParameters);
				//
				
				//argument types
				var typeParameters = methodInfo.GetGenericArguments();
				var genericTypeCount = typeParameters.Length;
				IL.Emit(OpCodes.Ldc_I4, genericTypeCount);
				IL.Emit(OpCodes.Newarr, typeof (Type));

				for (var index = 0; index < genericTypeCount; index++)
				{
					var currentType = typeParameters[index];

					IL.Emit(OpCodes.Dup);
					IL.Emit(OpCodes.Ldc_I4, index);
					IL.Emit(OpCodes.Ldtoken, currentType);
					IL.Emit(OpCodes.Call, getTypeFromHandle);
					IL.Emit(OpCodes.Stelem_Ref);
				}
				IL.Emit(OpCodes.Stloc_2, localParameterTypes);
				//
			
				IL.Emit(OpCodes.Ldarg_0);
				IL.Emit(OpCodes.Ldfld, proxyInterceptorField);
				IL.Emit(OpCodes.Ldloc_0, localMethodInfo);
				IL.Emit(OpCodes.Ldloc_2, localParameterTypes);
				IL.Emit(OpCodes.Ldloc_1, localParameters);
				IL.Emit(OpCodes.Callvirt, typeof(ProxyInterceptor).GetMethod(nameof(ProxyInterceptor.DecorateMethodCall)));
				if (methodBuilder.ReturnType == typeof(void))
				{
					IL.Emit(OpCodes.Pop);
				}
				else
				{
					IL.Emit(OpCodes.Unbox_Any, methodBuilder.ReturnType); //TODO: why?					
				}
				
				IL.Emit(OpCodes.Ret);
				
				typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
			}
		}
		
		private static string[] GenerateTypeNames(int count)
		{
			var result = new string[count];
			for (var index = 0; index < count; index++)
			{
				result[index] = $"T{index}"; 
			}
			return result;
		}

		private static void EmitCallMethod(ILGenerator IL, OpCode opCode, MethodInfo method)
		{
			for (var i = 0; i < method.GetParameters().Length; i++)
				IL.Emit(OpCodes.Ldarg_S, (sbyte) (1 + i));
			IL.Emit(opCode, method);
		}

	}
}
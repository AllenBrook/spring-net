#region License

/*
 * Copyright � 2002-2006 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using AopAlliance.Intercept;
using Spring.Aop;

namespace Spring.Aop.Framework
{
	
	/// <summary> Internal framework class.
	/// This class is required because if we put an interceptor that implements IInterceptionAdvice
	/// in the interceptor list passed to MethodInvocation, it may be mistaken for an
	/// advice that requires dynamic method matching.
	/// </summary>
    /// <author>Rod Johnson</author>
    /// <author>Aleksandar Seovic (.Net)</author>
    /// <version>$Id: InterceptorAndDynamicMethodMatcher.cs,v 1.3 2007/03/16 04:01:18 aseovic Exp $</version>
    [Serializable]
    internal class InterceptorAndDynamicMethodMatcher
	{
		internal IMethodMatcher MethodMatcher;
		internal IMethodInterceptor Interceptor;
		
		public InterceptorAndDynamicMethodMatcher(IMethodInterceptor interceptor, IMethodMatcher methodMatcher)
		{
			this.Interceptor = interceptor;
			this.MethodMatcher = methodMatcher;
		}
	}
}
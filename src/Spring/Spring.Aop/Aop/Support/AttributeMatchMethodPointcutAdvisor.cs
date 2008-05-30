#region License

/*
 * Copyright � 2002-2005 the original author or authors.
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

#region Imports

using System;
using AopAlliance.Aop;
using Spring.Core;

#endregion

namespace Spring.Aop.Support
{
	/// <summary> 
	/// Convenient class for attribute-match method pointcuts that hold an Interceptor,
	/// making them an Advisor.
	/// </summary>
    /// <author>Bruno Baia</author>
    /// <version>$Id: AttributeMatchMethodPointcutAdvisor.cs,v 1.3 2007/03/16 04:01:23 aseovic Exp $</version>
    [Serializable]
    public class AttributeMatchMethodPointcutAdvisor
		: AttributeMatchMethodPointcut, IPointcutAdvisor, IOrdered
	{
		private int _order = Int32.MaxValue;
		private IAdvice _advice;

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="AttributeMatchMethodPointcutAdvisor"/> class.
		/// </summary>
		public AttributeMatchMethodPointcutAdvisor()
		{
		}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="AttributeMatchMethodPointcutAdvisor"/> class
		/// for the supplied <paramref name="advice"/>.
		/// </summary>
		/// <param name="advice"></param>
		public AttributeMatchMethodPointcutAdvisor(IAdvice advice)
		{
			this._advice = advice;
		}

		/// <summary>
		/// Is this advice associated with a particular instance?
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this advice is associated with a
		/// particular instance.
		/// </value>
		/// <exception cref="System.NotSupportedException">
		/// Always; this property is not yet supported.
		/// </exception>
		public virtual bool IsPerInstance
		{
			get
			{
				throw new NotSupportedException(
					"The 'IsPerInstance' property of the IAdvisor interface is " +
					"not yet supported in Spring.NET.");
			}
		}

		/// <summary>
		/// Returns this <see cref="Spring.Aop.IAdvisor"/>s order in the
		/// interception chain.
		/// </summary>
		/// <returns>
		/// This <see cref="Spring.Aop.IAdvisor"/>s order in the
		/// interception chain.
		/// </returns>
		public virtual int Order
		{
			get { return this._order; }
			set { this._order = value; }
		}

		/// <summary>
		/// Return the advice part of this advisor.
		/// </summary>
		/// <returns>
		/// The advice that should apply if the pointcut matches.
		/// </returns>
		/// <see cref="Spring.Aop.IAdvisor.Advice"/>
		public virtual IAdvice Advice
		{
			get { return this._advice; }
			set { this._advice = value; }
		}

		/// <summary>
		/// The <see cref="Spring.Aop.IPointcut"/> that drives this advisor.
		/// </summary>
		public virtual IPointcut Pointcut
		{
			get { return this; }
		}
	}
}
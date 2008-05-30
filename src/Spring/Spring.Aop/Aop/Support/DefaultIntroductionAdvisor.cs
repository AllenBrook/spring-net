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
using Spring.Collections;

#endregion

namespace Spring.Aop.Support
{
	/// <summary>
	/// Simple <seealso cref="Spring.Aop.IIntroductionAdvisor"/> implementation that
	/// by default applies to any class.
	/// </summary>
	/// <author>Rod Johnson</author>
	/// <author>Aleksandar Seovic (.NET)</author>
	/// <version>$Id: DefaultIntroductionAdvisor.cs,v 1.11 2007/03/16 04:01:23 aseovic Exp $</version>
	[Serializable]
    public class DefaultIntroductionAdvisor : IIntroductionAdvisor, ITypeFilter
	{
		private IAdvice _introduction;
		private ISet _interfaces = new HybridSet();

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Aop.Support.DefaultIntroductionAdvisor"/> class using
		/// the supplied <paramref name="introduction"/>
		/// </summary>
		/// <remarks>
		/// <p>
		/// This constructor adds all interfaces implemented by the supplied
		/// <paramref name="introduction"/> (except the
		/// <see cref="AopAlliance.Aop.IAdvice"/> interface) to the list of
		/// interfaces to introduce.
		/// </p>
		/// </remarks>
		/// <param name="introduction">The introduction to use.</param>
		public DefaultIntroductionAdvisor(IAdvice introduction)
			: this(introduction, introduction.GetType().GetInterfaces())
		{
		}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Aop.Support.DefaultIntroductionAdvisor"/> class using
		/// the supplied <paramref name="introduction"/>
		/// </summary>
		/// <param name="introduction">The introduction to use.</param>
		/// <param name="intf">
		/// The interface to introduce.
		/// </param>
		public DefaultIntroductionAdvisor(IAdvice introduction, Type intf)
			: this(introduction, new Type[] {intf})
		{
		}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Aop.Support.DefaultIntroductionAdvisor"/> class using
		/// the supplied <paramref name="introduction"/>
		/// </summary>
		/// <param name="introduction">The introduction to use.</param>
		/// <param name="interfaces">
		/// The interfaces to introduce.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// If the supplied <paramref name="introduction"/> is <see langword="null"/>.
		/// </exception>
		public DefaultIntroductionAdvisor(IAdvice introduction, Type[] interfaces)
		{
			if (introduction == null)
			{
				throw new ArgumentNullException("introduction", "Introduction cannot be null");
			}
			_introduction = introduction;
			foreach (Type intf in interfaces)
			{
				if (intf != typeof (IAdvice))
				{
					AddInterface(intf);
				}
			}
		}

		/// <summary>
		/// Returns the filter determining which target classes this
		/// introduction should apply to. 
		/// </summary>
		/// <value>
		/// The filter determining which target classes this introduction
		/// should apply to. 
		/// </value>
		public virtual ITypeFilter TypeFilter
		{
			get { return this; }
		}

		/// <summary>
		/// Gets the interfaces introduced by this
		/// <see cref="Spring.Aop.IAdvisor"/>.
		/// </summary>
		/// <value>
		/// The interfaces introduced by this
		/// <see cref="Spring.Aop.IAdvisor"/>.
		/// </value>
		public virtual Type[] Interfaces
		{
			get
			{
				Type[] interfaces = new Type[_interfaces.Count];
				_interfaces.CopyTo(interfaces, 0);
				return interfaces;
			}
		}

		/// <summary>
		/// Is this advice associated with a particular instance?
		/// </summary>
		/// <remarks>
		/// <p>
		/// Default for an introduction is per-instance interception.
		/// </p>
		/// </remarks>
		/// <value>
		/// <see langword="true"/> if this advice is associated with a
		/// particular instance.
		/// </value>
		public virtual bool IsPerInstance
		{
			get { return true; }
		}

		/// <summary>
		/// Return the advice part of this aspect.
		/// </summary>
		/// <returns>
		/// The advice that should apply if the pointcut matches.
		/// </returns>
		public virtual IAdvice Advice
		{
			get { return this._introduction; }
		}

		/// <summary>
		/// Adds the supplied <paramref name="intf"/> to the list of
		/// introduced interfaces.
		/// </summary>
		/// <param name="intf">The interface to add.</param>
		/// <exception cref="System.ArgumentException">
		/// If any of the <see cref="Interfaces"/> are not interface <see cref="System.Type"/>.
		/// </exception>
		public virtual void AddInterface(Type intf)
		{
			if(intf != null) 
			{
				BailIfNotAnInterfaceType(intf);
				_interfaces.Add(intf);
			}
		}

		/// <summary>
		/// Should the pointcut apply to the supplied
		/// <see cref="System.Type"/>?
		/// </summary>
		/// <remarks>
		/// <p>
		/// This, the default, implementation always returns <see langword="true"/>.
		/// </p>
		/// </remarks>
		/// <param name="type">
		/// The candidate <see cref="System.Type"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the advice should apply to the supplied
		/// <paramref name="type"/>
		/// </returns>
		public virtual bool Matches(Type type)
		{
			return true;
		}

		/// <summary>
		/// Can the advised interfaces be implemented by the introduction
		/// advice?
		/// </summary>
		/// <remarks>
		/// <p>
		/// Invoked <b>before</b> adding an
		/// <seealso cref="Spring.Aop.IIntroductionAdvisor"/>.
		/// </p>
		/// </remarks>
		/// <exception cref="System.ArgumentException">
		/// If the advised interfaces cannot be implemented by the introduction
		/// advice.
		/// </exception>
		/// <seealso cref="Spring.Aop.IIntroductionAdvisor.Interfaces"/>
		/// <exception cref="System.ArgumentException">
		/// If any of the <see cref="Interfaces"/> are not interface <see cref="System.Type"/>.
		/// </exception>
		public virtual void ValidateInterfaces()
		{
			foreach (Type intf in _interfaces)
			{
				BailIfNotAnInterfaceType(intf);
				if (! intf.IsAssignableFrom(_introduction.GetType()))
				{
					throw new ArgumentException("Introduction [" + _introduction.GetType().FullName + "] " +
						"does not implement interface '" + intf.FullName + "' specified in introduction advice.");
				}
			}
		}

		private static void BailIfNotAnInterfaceType(Type intf)
		{
			if (intf != null && !intf.IsInterface)
			{
				throw new ArgumentException("Type [" + intf.FullName + "] is not an interface; cannot be used in an introduction.");
			}
		}
	}
}
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
using System.Collections;
using AopAlliance.Aop;

#endregion

namespace Spring.Aop
{
	/// <summary>
	/// AOP Aspect abstraction, holding a list of <see cref="Spring.Aop.IAdvisor"/>s
	/// </summary>
    /// <seealso cref="IAdvisor"/>
    /// <author>Aleksandar Seovic (.NET)</author>
	/// <version>$Id: IAdvisors.cs,v 1.1 2007/08/03 14:38:30 markpollack Exp $</version>
	public interface IAdvisors
	{
        /// <summary>
        /// Gets or sets a list of advisors.
        /// </summary>
        /// <value>
        /// A list of advisors.
        /// </value>
		IList Advisors { get; set; }
	}
}
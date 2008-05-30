#region License

/*
 * Copyright 2004 the original author or authors.
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

using NUnit.Framework;
using Spring.Core;

namespace Spring.Objects
{
	/// <summary>
	/// Unit tests for the TypeMismatchException class.
    /// </summary>
    /// <author>Rick Evans</author>
    /// <version>$Id: TypeMismatchExceptionTests.cs,v 1.4 2007/07/31 00:09:24 markpollack Exp $</version>
	[TestFixture]
    public sealed class TypeMismatchExceptionTests
    {
		[Test]
		public void InstantiationSupplyingPropertyChangeArgsType()
		{
			TypeMismatchException ex = new TypeMismatchException(new PropertyChangeEventArgs("Doctor", new NestedTestObject("Foo"), new TestObject("Hershey", 12)), typeof (INestedTestObject));
			Assert.AreEqual("typeMismatch", ex.ErrorCode);
		}

		[Test]
		public void InstantiationSupplyingPropertyChangeArgsTypeAndRootException()
		{
			TypeMismatchException ex = new TypeMismatchException(new PropertyChangeEventArgs("Doctor", new NestedTestObject("Foo"), new TestObject("Hershey", 12)), typeof (INestedTestObject), null);
			Assert.AreEqual("typeMismatch", ex.ErrorCode);
		}

		[Test]
		public void InstantiationSupplyingNullPropertyChangeArgsAndNullType()
		{
			TypeMismatchException ex = new TypeMismatchException(null, null, null);
			Assert.AreEqual("typeMismatch", ex.ErrorCode);
		}
	}
}
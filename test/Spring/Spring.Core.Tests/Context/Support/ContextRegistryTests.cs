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
using System.Xml;
using NUnit.Framework;

#endregion

namespace Spring.Context.Support
{
	/// <summary>
	/// Unit tests for the ContextRegistry class.
	/// </summary>
	/// <author>Rick Evans</author>
	/// <version>$Id: ContextRegistryTests.cs,v 1.9 2008/03/21 10:49:38 oakinger Exp $</version>
	[TestFixture]
	public sealed class ContextRegistryTests
	{
		[SetUp]
		public void SetUp() 
		{
			ContextRegistry.Clear();
		}


        /// <summary>
        /// This handler simulates calls to ContextRegistry during context creation
        /// </summary>
        private static object GetContextRecursive(object parent, object context, XmlNode section)
        {
            return ContextRegistry.GetContext(); // this must fail!
        }

        [Test]
        public void ThrowsInvalidOperationExceptionOnRecursiveCallsToGetContext()
        {
            HookableContextHandler.CreateContextFromSectionHandler prevInst = HookableContextHandler.SetSectionHandler(
                new HookableContextHandler.CreateContextFromSectionHandler(GetContextRecursive));
            try
            {
                ContextRegistry.GetContext("somename");                
            }
            catch(Exception ex)
            {
                InvalidOperationException rootCause = ex.GetBaseException() as InvalidOperationException;
                Assert.IsNotNull(rootCause);
                Assert.AreEqual("root context is currently in creation.", rootCause.Message.Substring(0, 38));
            }
            finally
            {
                HookableContextHandler.SetSectionHandler(prevInst);
            }
        }

		[Test]
		public void RegisterRootContext()
		{
			MockApplicationContext ctx = new MockApplicationContext();
			ContextRegistry.RegisterContext(ctx);
			IApplicationContext context = ContextRegistry.GetContext();
			Assert.IsNotNull(context,
				"Root context is null even though a context has been registered.");
			Assert.IsTrue(Object.ReferenceEquals(ctx, context),
				"Root context was not the same as the first context registered (it must be).");
		}

        [Test]
        public void RegisterNamedRootContext()
        {
            const string ctxName = "bingo";
            MockApplicationContext ctx = new MockApplicationContext(ctxName);
            ContextRegistry.RegisterContext(ctx);
            IApplicationContext rootContext = ContextRegistry.GetContext();
            Assert.IsNotNull(rootContext,
                "Root context is null even though a context has been registered.");
            Assert.AreEqual(ctxName, rootContext.Name,
                "Root context name is different even though the root context has been registered under the lookup name.");
        }

		[Test]
		public void RegisterNamedContext()
		{
			const string ctxName = "bingo";
			MockApplicationContext ctx = new MockApplicationContext(ctxName);
			ContextRegistry.RegisterContext(ctx);
			IApplicationContext context = ContextRegistry.GetContext(ctxName);
			Assert.IsNotNull(context,
				"Named context is null even though a context has been registered under the lookup name.");
			Assert.IsTrue(Object.ReferenceEquals(ctx, context),
				"Named context was not the same as the registered context (it must be).");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetContextWithNullName()
		{
			ContextRegistry.GetContext(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetContextWithEmptyName()
		{
			ContextRegistry.GetContext("");
		}

		[Test]
		public void GetContextNotRegisteredReturnsNull()
		{
			IApplicationContext context = ContextRegistry.GetContext("bingo");
			Assert.IsNull(context,
				"Named context is not null even though a context has not been registered under the lookup name.");
		}
#if NET_2_0
        // TODO : Add support for .NET 1.x
        [Test]
        public void ClearWithConfigurationSection()
        {
            IApplicationContext ctx1 = ContextRegistry.GetContext();
            ContextRegistry.Clear();
            IApplicationContext ctx2 = ContextRegistry.GetContext();

            Assert.AreNotSame(ctx1, ctx2);
        }
#endif

		[Test(Description="SPRNET-105")]
		[ExpectedException(typeof(ApplicationContextException))]
		public void ChokesIfChildContextRegisteredUnderNameOfAnExistingContext()
		{
			MockApplicationContext original = new MockApplicationContext("original");
			ContextRegistry.RegisterContext(original);
			MockApplicationContext duplicate = new MockApplicationContext("original");
			ContextRegistry.RegisterContext(duplicate);
		}
	}
}
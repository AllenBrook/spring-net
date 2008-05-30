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
using Microsoft.Win32;
using NUnit.Framework;

#endregion

namespace Spring.Objects.Factory.Config
{
	/// <summary>
    /// Unit tests for the EnvironmentVariableSource class.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: EnvironmentVariableSourceTests.cs,v 1.1 2007/03/10 01:44:40 aseovic Exp $</version>
    [TestFixture]
    public sealed class EnvironmentVariableSourceTests
    {
        [Test]
        public void TestVariablesResolution()
        {
            EnvironmentVariableSource vs = new EnvironmentVariableSource();

            // existing vars
            Assert.AreEqual(Environment.GetEnvironmentVariable("path"), vs.ResolveVariable("PATH"));
            Assert.AreEqual(Environment.GetEnvironmentVariable("PATH"), vs.ResolveVariable("path"));
            Assert.AreEqual(Environment.GetEnvironmentVariable("ComputerName"), vs.ResolveVariable("computerName"));

            // non-existant variable
            Assert.IsNull(vs.ResolveVariable("dummy"));
        }
    }
}
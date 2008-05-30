#if NET_2_0

#region License

/*
 * Copyright � 2002-2007 the original author or authors.
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

using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Spring.Data.Core;
using Spring.Objects;
using Spring.Transaction;
using Spring.Transaction.Support;

#endregion

namespace Spring.Data.Core
{
    /// <summary>
    /// This class contains tests for TxScopeTransactionManager and will directly a real TransactionScope object
    /// but does not access any database
    /// </summary>
    /// <author>Mark Pollack</author>
    /// <version>$Id: TxScopeTransactionManagerIntegrationTests.cs,v 1.1 2007/11/30 18:38:59 markpollack Exp $</version>
    [TestFixture]
    public class TxScopeTransactionManagerIntegrationTests
    {
        private MockRepository mocks;
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            Assert.IsTrue(TransactionSynchronizationManager.ResourceDictionary.Count == 0);
            Assert.IsFalse(TransactionSynchronizationManager.SynchronizationActive);
            Assert.IsNull(TransactionSynchronizationManager.CurrentTransactionName);
            Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
            Assert.AreEqual(System.Data.IsolationLevel.Unspecified, TransactionSynchronizationManager.CurrentTransactionIsolationLevel);
            Assert.IsFalse(TransactionSynchronizationManager.ActualTransactionActive);

        }

        [Test]
        public void Commit()
        {
            TxScopeTransactionManager tm = new TxScopeTransactionManager();
            TransactionTemplate tt = new TransactionTemplate(tm);

            //tt.Name = "txName";

            Assert.AreEqual(TransactionSynchronizationState.Always, tm.TransactionSynchronization);
            Assert.IsFalse(TransactionSynchronizationManager.SynchronizationActive);
            Assert.IsNull(TransactionSynchronizationManager.CurrentTransactionName);
            Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
            tt.Execute(CommitTxDelegate);
            Assert.IsFalse(TransactionSynchronizationManager.SynchronizationActive);
            Assert.IsNull(TransactionSynchronizationManager.CurrentTransactionName);
            Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
        }

        public object CommitTxDelegate(ITransactionStatus status)
        {
            Assert.IsTrue(TransactionSynchronizationManager.SynchronizationActive);
            Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
           
            return null;
        }

        [Test]
        public void TransactionInformation()
        {
            TxScopeTransactionManager tm = new TxScopeTransactionManager();
            TransactionTemplate tt = new TransactionTemplate(tm);
            tt.TransactionIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
            tt.Execute(TransactionInformationTxDelegate);
        }

        public object TransactionInformationTxDelegate(ITransactionStatus status)
        {

            Assert.AreEqual(System.Transactions.IsolationLevel.ReadUncommitted,
                            System.Transactions.Transaction.Current.IsolationLevel);

            Assert.AreEqual(System.Data.IsolationLevel.ReadUncommitted,
                            TransactionSynchronizationManager.CurrentTransactionIsolationLevel);
            return null;
        }


        [Test]
        public void Rollback()
        {
            ITransactionSynchronization sync =
                (ITransactionSynchronization) mocks.DynamicMock(typeof (ITransactionSynchronization));
            sync.BeforeCompletion();
            LastCall.On(sync).Repeat.Once();
            sync.AfterCompletion(TransactionSynchronizationStatus.Rolledback);
            LastCall.On(sync).Repeat.Once();
            mocks.ReplayAll();


            TxScopeTransactionManager tm = new TxScopeTransactionManager();
            TransactionTemplate tt = new TransactionTemplate(tm);
            tt.TransactionTimeout = 10;
            tt.Name = "txName";

            Assert.IsFalse(TransactionSynchronizationManager.SynchronizationActive);
            Assert.IsNull(TransactionSynchronizationManager.CurrentTransactionName);
            Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
            tt.Execute(delegate (ITransactionStatus status)
                           {
                               Assert.IsTrue(TransactionSynchronizationManager.SynchronizationActive);
                               TransactionSynchronizationManager.RegisterSynchronization(sync);
                               Assert.AreEqual("txName", TransactionSynchronizationManager.CurrentTransactionName);
                               Assert.IsFalse(TransactionSynchronizationManager.CurrentTransactionReadOnly);
                               status.RollbackOnly = true;
                               return null;
                           }
            );

            mocks.VerifyAll();

        }


    }
}
#endif
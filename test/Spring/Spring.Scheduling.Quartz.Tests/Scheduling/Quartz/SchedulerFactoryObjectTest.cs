/*
* Copyright 2002-2005 the original author or authors.
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

using System;
using System.Collections;
using System.Threading;

using NUnit.Framework;

using Quartz;
using Quartz.Impl;

using Rhino.Mocks;

namespace Spring.Scheduling.Quartz
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Marko Lahma (.NET)</author>
    [TestFixture]
    public class SchedulerFactoryObjectTest
    {
        private MockRepository mockery = null;
        private SchedulerFactoryObject factory;

        [SetUp]
        public void SetUp()
        {
            factory = new SchedulerFactoryObject();
            TestSchedulerFactory.Mockery.BackToRecordAll();
        }

        [Test]
        public void TestAfterPropertiesSet_Defaults()
        {
            factory.AfterPropertiesSet();
            TestSchedulerFactory.Mockery.ReplayAll();
        }

        [Test]
        public void TestAfterPropertiesSet_NullJobFactory()
        {
            factory.JobFactory = null;  
            factory.AfterPropertiesSet();
            TestSchedulerFactory.Mockery.ReplayAll();
        }

        [Test]
        public void TestAfterPropertiesSet_NoAutoStartup()
        {
            // set expectations
            TestSchedulerFactory.MockScheduler.JobFactory = null;
            LastCall.IgnoreArguments();
            TestSchedulerFactory.Mockery.ReplayAll();

            factory.SchedulerFactoryType = typeof(TestSchedulerFactory);
            factory.AutoStartup = false;
            factory.AfterPropertiesSet();
        }

        [Test]
        public void TestAfterPropertiesSet_AutoStartup()
        {
            InitForAfterPropertiesSetTest();

            TestSchedulerFactory.MockScheduler.Start();
            TestSchedulerFactory.Mockery.ReplayAll();

            factory.SchedulerFactoryType = typeof (TestSchedulerFactory);
            factory.AutoStartup = true;
            factory.AfterPropertiesSet();
        }

        [Test]
        public void TestAfterPropertiesSet_AddListeners()
        {
            mockery = new MockRepository();
            InitForAfterPropertiesSetTest();
            
            factory.SchedulerListeners = new ISchedulerListener[] { (ISchedulerListener)mockery.CreateMock(typeof(ISchedulerListener)) };
            TestSchedulerFactory.MockScheduler.AddSchedulerListener(null);
            LastCall.IgnoreArguments();
            
            factory.GlobalJobListeners = new IJobListener[] { (IJobListener)mockery.CreateMock(typeof(IJobListener)) };
            TestSchedulerFactory.MockScheduler.AddGlobalJobListener(null);
            LastCall.IgnoreArguments();

            factory.JobListeners = new IJobListener[] { (IJobListener)mockery.CreateMock(typeof(IJobListener)) };
            TestSchedulerFactory.MockScheduler.AddJobListener(null);
            LastCall.IgnoreArguments();

            factory.GlobalTriggerListeners = new ITriggerListener[] { (ITriggerListener)mockery.CreateMock(typeof(ITriggerListener)) };
            TestSchedulerFactory.MockScheduler.AddGlobalTriggerListener(null);
            LastCall.IgnoreArguments();
            
            factory.TriggerListeners = new ITriggerListener[] { (ITriggerListener)mockery.CreateMock(typeof(ITriggerListener)) };
            TestSchedulerFactory.MockScheduler.AddTriggerListener(null);
            LastCall.IgnoreArguments();

            TestSchedulerFactory.Mockery.ReplayAll();
            mockery.ReplayAll();
            factory.AfterPropertiesSet();
        }

        [Test]
        public void TestAfterPropertiesSet_Calendars()
        {
            mockery = new MockRepository();
            InitForAfterPropertiesSetTest();

            const string calendarName = "calendar";
            ICalendar cal = (ICalendar) mockery.CreateMock(typeof (ICalendar));
            Hashtable calTable = new Hashtable();
            calTable[calendarName] = cal;
            factory.Calendars = calTable;
            TestSchedulerFactory.MockScheduler.AddCalendar(calendarName, cal, true, true);

            TestSchedulerFactory.Mockery.ReplayAll();
            mockery.ReplayAll();
            factory.AfterPropertiesSet();
        }

        [Test]
        public void TestAfterPropertiesSet_Trigger_TriggerExists()
        {
            mockery = new MockRepository();
            InitForAfterPropertiesSetTest();

            const string TRIGGER_NAME = "trigName";
            const string TRIGGER_GROUP = "trigGroup";
            SimpleTrigger trigger = new SimpleTrigger(TRIGGER_NAME, TRIGGER_GROUP);
            factory.Triggers = new Trigger[] { trigger };

            Expect.Call(TestSchedulerFactory.MockScheduler.GetTrigger(TRIGGER_NAME, TRIGGER_GROUP)).Return(trigger);

            TestSchedulerFactory.Mockery.ReplayAll();
            mockery.ReplayAll();
            factory.AfterPropertiesSet();
        }

        [Test]
        public void TestAfterPropertiesSet_Trigger_TriggerDoesntExist()
        {
            mockery = new MockRepository();
            InitForAfterPropertiesSetTest();

            const string TRIGGER_NAME = "trigName";
            const string TRIGGER_GROUP = "trigGroup";
            SimpleTrigger trigger = new SimpleTrigger(TRIGGER_NAME, TRIGGER_GROUP);
            factory.Triggers = new Trigger[] { trigger };

            Expect.Call(TestSchedulerFactory.MockScheduler.GetTrigger(TRIGGER_NAME, TRIGGER_GROUP)).Return(null);
            TestSchedulerFactory.MockScheduler.ScheduleJob(trigger);
            LastCall.IgnoreArguments().Return(DateTime.UtcNow);
                

            TestSchedulerFactory.Mockery.ReplayAll();
            mockery.ReplayAll();
            factory.AfterPropertiesSet();

        }


        private void InitForAfterPropertiesSetTest()
        {
            factory.AutoStartup = false;
            // set expectations
            factory.SchedulerFactoryType = typeof(TestSchedulerFactory);
            TestSchedulerFactory.MockScheduler.JobFactory = null;
            LastCall.IgnoreArguments();
        }

        [Test]
        public void TestAfterPropertiesSet_AutoStartup_WithDelay()
        {
            // set expectations
            TestSchedulerFactory.MockScheduler.JobFactory = null;
            LastCall.IgnoreArguments();
            Expect.Call(TestSchedulerFactory.MockScheduler.SchedulerName).Return("schedName");
            TestSchedulerFactory.MockScheduler.Start();
            TestSchedulerFactory.Mockery.ReplayAll();

            factory.SchedulerFactoryType = typeof(TestSchedulerFactory);
            factory.AutoStartup = true;
            factory.StartupDelay = 2;
            factory.AfterPropertiesSet();
            Thread.Sleep(TimeSpan.FromSeconds(3));

        }

        [Test]
        public void TestStart()
        {
            // set expectations
            TestSchedulerFactory.MockScheduler.JobFactory = null;
            LastCall.IgnoreArguments();
            TestSchedulerFactory.MockScheduler.Start();
            TestSchedulerFactory.Mockery.ReplayAll();

            factory.SchedulerFactoryType = typeof(TestSchedulerFactory);
            factory.AutoStartup = false;
            factory.AfterPropertiesSet();
            factory.Start();
        }

        [Test]
        public void TestStop()
        {
            // set expectations
            TestSchedulerFactory.MockScheduler.JobFactory = null;
            LastCall.IgnoreArguments();
            TestSchedulerFactory.MockScheduler.Standby();
            TestSchedulerFactory.Mockery.ReplayAll();

            factory.SchedulerFactoryType = typeof(TestSchedulerFactory);
            factory.AutoStartup = false;
            factory.AfterPropertiesSet();
            factory.Stop();
        }

        [Test]
        public void TestGetObject()
        {
            factory.AfterPropertiesSet();
            TestSchedulerFactory.Mockery.ReplayAll(); 
            IScheduler sched = (IScheduler)factory.GetObject();
            Assert.IsNotNull(sched, "scheduler was null");
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(ArgumentException))]
        public void TestSchedulerFactoryType_InvalidType()
        {
            TestSchedulerFactory.Mockery.ReplayAll(); 
            factory.SchedulerFactoryType = typeof(SchedulerFactoryObjectTest);
        }

        [Test]
        public void TestSchedulerFactoryType_ValidType()
        {
            TestSchedulerFactory.Mockery.ReplayAll(); 
            factory.SchedulerFactoryType = typeof(StdSchedulerFactory);
        }

        [TearDown]
        public void TearDown()
        {
            TestSchedulerFactory.Mockery.VerifyAll();
            if (mockery != null)
            {
                mockery.VerifyAll();
            }
        }


    }

    public class TestSchedulerFactory : ISchedulerFactory
    {
        private static readonly MockRepository mockery = new MockRepository();
        private static readonly IScheduler mockScheduler;

        static TestSchedulerFactory()
        {
            mockScheduler = (IScheduler) mockery.CreateMock(typeof (IScheduler));
        }

        public static MockRepository Mockery
        {
            get { return mockery; }
        }

        public static IScheduler MockScheduler
        {
            get { return mockScheduler; }
        }

        public IScheduler GetScheduler()
        {
            return mockScheduler;
        }

        public IScheduler GetScheduler(string schedName)
        {
            return mockScheduler;
        }

        public ICollection AllSchedulers
        {
            get { return new ArrayList(); }
        }
    }


}
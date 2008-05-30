/*
* Copyright 2002-2007 the original author or authors.
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
using Quartz;
using Spring.Objects.Factory;

namespace Spring.Scheduling.Quartz
{
	/// <summary> 
	/// Convenience subclass of Quartz's <see cref="SimpleTrigger" />
	/// class, making properties based usage easier.
	/// </summary>
	/// <remarks>
	/// <p>
	/// SimpleTrigger itself is already a PONO but lacks sensible defaults.
	/// This class uses the Spring object name as job name, the Quartz default group
	/// ("DEFAULT") as job group, the current time as start time, and indefinite
	/// repetition, if not specified.
	/// </p>
	/// 
	/// <p>
	/// This class will also register the trigger with the job name and group of
	/// a given <see cref="JobDetail" />. This allows <see cref="SchedulerFactoryObject" />
	/// to automatically register a trigger for the corresponding JobDetail,
	/// instead of registering the JobDetail separately.
	/// </p>
    /// </remarks>
	/// <author>Juergen Hoeller</author>
	/// <seealso cref="Trigger.Name" />
    /// <seealso cref="Trigger.Group" />
    /// <seealso cref="Trigger.StartTimeUtc" />
    /// <seealso cref="Trigger.JobName" />
    /// <seealso cref="Trigger.JobGroup" />
    /// <seealso cref="SimpleTriggerObject.JobDetail" />
	/// <seealso cref="SchedulerFactoryObject.Triggers" />
	/// <seealso cref="SchedulerFactoryObject.JobDetails" />
	/// <seealso cref="CronTriggerObject" />
	public class SimpleTriggerObject : SimpleTrigger, IJobDetailAwareTrigger, IObjectNameAware, IInitializingObject
	{
		private long startDelay = 0;
		private JobDetail jobDetail;
		private string objectName;
        private readonly Constants constants = new Constants(typeof(MisfirePolicy.SimpleTrigger), typeof(MisfirePolicy));

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTriggerObject"/> class.
        /// </summary>
        public SimpleTriggerObject()
        {
            RepeatCount = REPEAT_INDEFINITELY;
        }

		/// <summary> 
		/// Register objects in the JobDataMap via a given Map.
		/// <p>
		/// These objects will be available to this Trigger only,
		/// in contrast to objects in the JobDetail's data map.
		/// </p>
		/// </summary>
		/// <seealso cref="JobDetailObject.JobDataAsMap" />
		public virtual IDictionary JobDataAsMap
		{
			set { JobDataMap.PutAll(value); }
		}

		/// <summary> 
		/// Set the misfire instruction via the name of the corresponding
		/// constant in the SimpleTrigger class.
        /// Default is <see cref="MisfirePolicy.SmartPolicy" />.
		/// </summary>
		/// <seealso cref="MisfirePolicy.SimpleTrigger.FireNow" />
        /// <seealso cref="MisfirePolicy.SimpleTrigger.RescheduleNextWithExistingCount" />
        /// <seealso cref="MisfirePolicy.SimpleTrigger.RescheduleNextWithRemainingCount" />
        /// <seealso cref="MisfirePolicy.SimpleTrigger.RescheduleNowWithExistingRepeatCount" />
        /// <seealso cref="MisfirePolicy.SimpleTrigger.RescheduleNowWithRemainingRepeatCount" />
        /// <seealso cref="MisfirePolicy.SmartPolicy" />
		public virtual string MisfireInstructionName
		{
			set 
			{ 
				MisfireInstruction = constants.AsNumber(value); 
			}
		}

		/// <summary> 
		/// Set the delay before starting the job for the first time.
		/// The given number of milliseconds will be added to the current
		/// time to calculate the start time. Default is 0.
		/// </summary>
		/// <remarks>
		/// This delay will just be applied if no custom start time was
		/// specified. However, in typical usage within a Spring context,
		/// the start time will be the container startup time anyway.
		/// Specifying a relative delay is appropriate in that case.
		/// </remarks>
		/// <seealso cref="Trigger.StartTimeUtc" />
		public virtual long StartDelay
		{
			set { startDelay = value; }
		}

        /// <summary>
        /// Set the name of the object in the object factory that created this object.
        /// </summary>
        /// <value>The name of the object in the factory.</value>
        /// <remarks>
        /// <p>
        /// Invoked after population of normal object properties but before an init
        /// callback like <see cref="Spring.Objects.Factory.IInitializingObject"/>'s
        /// <see cref="Spring.Objects.Factory.IInitializingObject.AfterPropertiesSet"/>
        /// method or a custom init-method.
        /// </p>
        /// </remarks>
		public virtual string ObjectName
		{
			set { objectName = value; }
		}
        
		/// <summary> 
		/// Set the JobDetail that this trigger should be associated with.
		/// <p>
		/// This is typically used with a object reference if the JobDetail
		/// is a Spring-managed object. Alternatively, the trigger can also
		/// be associated with a job by name and group.
		/// </p>
		/// </summary>
		public virtual JobDetail JobDetail
		{
			get { return jobDetail; }
			set { jobDetail = value; }
		}


        /// <summary>
        /// Invoked by an <see cref="Spring.Objects.Factory.IObjectFactory"/>
        /// after it has injected all of an object's dependencies.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method allows the object instance to perform the kind of
        /// initialization only possible when all of it's dependencies have
        /// been injected (set), and to throw an appropriate exception in the
        /// event of misconfiguration.
        /// </p>
        /// <p>
        /// Please do consult the class level documentation for the
        /// <see cref="Spring.Objects.Factory.IObjectFactory"/> interface for a
        /// description of exactly <i>when</i> this method is invoked. In
        /// particular, it is worth noting that the
        /// <see cref="Spring.Objects.Factory.IObjectFactoryAware"/>
        /// and <see cref="Spring.Context.IApplicationContextAware"/>
        /// callbacks will have been invoked <i>prior</i> to this method being
        /// called.
        /// </p>
        /// </remarks>
        /// <exception cref="System.Exception">
        /// In the event of misconfiguration (such as the failure to set a
        /// required property) or if initialization fails.
        /// </exception>
		public virtual void AfterPropertiesSet()
		{
			if (Name == null)
			{
				Name = objectName;
			}
			if (Group == null)
			{
                Group = SchedulerConstants.DEFAULT_GROUP;
			}
			if (StartTimeUtc == DateTime.MinValue)
			{
				StartTimeUtc = DateTime.UtcNow.AddMilliseconds(startDelay);
			}
			if (jobDetail != null)
			{
				JobName = jobDetail.Name;
				JobGroup = jobDetail.Group;
			}
		}
	}
}
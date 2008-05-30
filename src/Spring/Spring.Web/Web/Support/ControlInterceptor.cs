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

#region Imports

using System.Collections;
using System.Web.UI;
using Spring.Context;
using Spring.Util;

#endregion

namespace Spring.Web.Support
{
    /// <summary>
    /// Support Class providing a method to ensure a control has been intercepted
    /// </summary>
    /// <author>Erich Eichinger</author>
    /// <version>$Id: ControlInterceptor.cs,v 1.2 2008/05/13 14:22:47 oakinger Exp $</version>
    internal sealed class ControlInterceptor
    {				
        /// <summary>
        /// Holds all available interception strategies
        /// </summary>
        private static readonly IInterceptionStrategy[] s_availableInterceptionStrategies = new IInterceptionStrategy[]
            {
                new InterceptControlCollectionStrategy()
                , new InterceptControlCollectionOwnerStrategy()
            };
		
        /// <summary>
        /// Holds a control.GetType()->IInterceptionStrategy table.
        /// </summary>
        private static readonly Hashtable s_cachedInterceptionStrategies = new Hashtable();
		
        private ControlInterceptor()
        {
        }

        /// <summary>
        /// Ensures, a control has been intercepted to support Web-DI. If not, the control will be intercepted.
        /// </summary>
        public static void EnsureControlIntercepted(IApplicationContext defaultApplicationContext, Control control)
        {
            if (control is LiteralControl)
            {
                return; // nothing more to do
            }

            // check control itself
            if (IsDependencyInjectionAware(defaultApplicationContext, control))
            {
                return; // nothing more to do
            }

            // check control's ControlCollection
            EnsureControlCollectionIntercepted(defaultApplicationContext, control);
        }

        private static void EnsureControlCollectionIntercepted(IApplicationContext defaultApplicationContext, Control control)
        {
            // check the collection
            ControlAccessor ctlAccessor = new ControlAccessor(control);
            ControlCollection childControls = ctlAccessor.Controls;
            if (IsDependencyInjectionAware(defaultApplicationContext, childControls))
            {
                return; // nothing more to do				
            }

            // check, if the collection's owner has already been intercepted
            ControlCollectionAccessor ctlColAccessor = new ControlCollectionAccessor(childControls);
            if (IsDependencyInjectionAware(defaultApplicationContext, ctlColAccessor.Owner))
            {
                return; // nothing more to do				
            }

            // lookup strategy in cache
            IInterceptionStrategy strategy = null;
            lock(s_cachedInterceptionStrategies)
            {
                strategy = (IInterceptionStrategy) s_cachedInterceptionStrategies[control.GetType()];
            }
			
            if (strategy != null)
            {
                strategy.Intercept(defaultApplicationContext, ctlAccessor, ctlColAccessor);
            }
            else
            {
                // probe for a strategy
                for(int i=0;i<s_availableInterceptionStrategies.Length;i++)
                {
                    bool bOk = s_availableInterceptionStrategies[i].Intercept(defaultApplicationContext, ctlAccessor, ctlColAccessor);
                    if (bOk) break;
                }

                lock(s_cachedInterceptionStrategies)
                {					
                    s_cachedInterceptionStrategies[control.GetType()] = strategy;
                }
            }			
        }

        private static bool IsDependencyInjectionAware(IApplicationContext defaultApplicationContext, object o)
        {
            ISupportsWebDependencyInjection diAware = o as ISupportsWebDependencyInjection;
            if (diAware != null)
            {
                // If the ControlCollection is alread DI-aware ensure appContext is set
                if (diAware.DefaultApplicationContext == null)
                {
                    diAware.DefaultApplicationContext = defaultApplicationContext;
                }
                return true; // nothing more to do				
            }
            return false;
        }
    }
}
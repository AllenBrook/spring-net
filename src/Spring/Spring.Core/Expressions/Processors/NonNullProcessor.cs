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

using System.Collections;

namespace Spring.Expressions.Processors
{
    /// <summary>
    /// Implementation of the non-null processor.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: NonNullProcessor.cs,v 1.2 2007/07/31 21:43:52 markpollack Exp $</version>
    public class NonNullProcessor : ICollectionProcessor
    {
        /// <summary>
        /// Returns non-null items from the collection.
        /// </summary>
        /// <param name="source">
        /// The source collection to process.
        /// </param>
        /// <param name="args">
        /// Ignored.
        /// </param>
        /// <returns>
        /// A collection containing non-null source collection elements.
        /// </returns>
        public object Process(ICollection source, object[] args)
        {
            if (source == null)
            {
                return null;
            }

            ArrayList list = new ArrayList();
            foreach (object item in source)
            {
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }
    }
}
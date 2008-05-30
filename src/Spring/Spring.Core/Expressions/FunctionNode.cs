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

using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Spring.Expressions
{
    /// <summary>
    /// Represents parsed function node.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: FunctionNode.cs,v 1.7 2008/03/20 23:58:16 oakinger Exp $</version>
    [Serializable]
    public class FunctionNode : NodeWithArguments
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        public FunctionNode()
        {
        }

        /// <summary>
        /// Create a new instance from SerializationInfo
        /// </summary>
        protected FunctionNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Evaluates function represented by this node.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <returns>Result of the function evaluation.</returns>
        protected override object Get(object context, EvaluationContext evalContext)
        {
            string name = this.getText();
            LambdaExpressionNode lambda = evalContext.Variables[name] as LambdaExpressionNode;
            Delegate function = evalContext.Variables[name] as Delegate;

            if (lambda == null && function == null)
            {
                throw new InvalidOperationException("Function '" + name + "' is not defined.");
            }

            object[] argValues = ResolveArguments(evalContext);

            // delegate?
            if (function != null)
            {
                return function.DynamicInvoke(argValues);
            }

            // lambda!
            string[] argNames = lambda.ArgumentNames;

            if (argValues.Length != argNames.Length)
            {
                throw new InvalidOperationException(
                    "Function '" + name + "' requires " + argNames.Length + " arguments.");
            }

            IDictionary arguments = new Hashtable();
            for (int i = 0; i < argValues.Length; i++)
            {
                arguments[argNames[i]] = argValues[i];
            }

            return lambda.GetValueInternal(context, evalContext, arguments);
        }
    }
}
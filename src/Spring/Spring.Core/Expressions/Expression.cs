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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using antlr;
using antlr.collections;
using Spring.Core;
using Spring.Util;
using StringUtils=Spring.Util.StringUtils;

namespace Spring.Expressions
{
    /// <summary>
    /// Container object for the parsed expression.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Preparing this object once and reusing it many times for expression
    /// evaluation can result in significant performance improvements, as 
    /// expression parsing and reflection lookups are only performed once. 
    /// </p>
    /// </remarks>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: Expression.cs,v 1.19 2008/03/20 10:35:54 oakinger Exp $</version>
    [Serializable]
    public class Expression : BaseNode
    {
        /// <summary>
        /// Contains a list of reserved variable names.
        /// You must not use any variable names with the reserved prefix!
        /// </summary>
        public class ReservedVariableNames
        {
            /// <summary>
            /// Variable Names using this prefix are reserved for internal framework use
            /// </summary>
            public static readonly string RESERVEDPREFIX = "____spring_";

            /// <summary>
            /// variable name of the currently processed object factory, if any
            /// </summary>
            internal static readonly string CurrentObjectFactory = RESERVEDPREFIX + "CurrentObjectFactory";
        }

        private class SpringASTFactory : ASTFactory
        {
            public SpringASTFactory(Type t) : base(t.FullName)
            {
                base.defaultASTNodeTypeObject_ = t;
                base.typename2creator_ = new Hashtable(32, 0.3f);
                base.typename2creator_[t.FullName] = SpringAST.Creator;
            }
        }

        private class SpringExpressionParser : ExpressionParser
        {
            public SpringExpressionParser(TokenStream lexer)
                : base(lexer)
            {
                base.astFactory = new SpringASTFactory(typeof(SpringAST));
                base.initialize();
            }
        }

        static Expression()
        {
            // Ensure antlr is loaded (fixes GAC issues)!
            Assembly antlrAss = typeof(antlr.LLkParser).Assembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class
        /// by parsing specified expression string.
        /// </summary>
        /// <param name="expression">Expression to parse.</param>
        public static IExpression Parse(string expression)
        {
            if (StringUtils.HasText(expression))
            {
                ExpressionLexer lexer = new ExpressionLexer(new StringReader(expression));
                ExpressionParser parser = new SpringExpressionParser(lexer);

                parser.expr();

                return (IExpression) parser.getAST();
            }
            else
            {
                return new Expression();
            }
        }

        /// <summary>
        /// Registers lambda expression under the specified <paramref name="functionName"/>.
        /// </summary>
        /// <param name="functionName">Function name to register expression as.</param>
        /// <param name="lambdaExpression">Lambda expression to register.</param>
        /// <param name="variables">Variables dictionary that the function will be registered in.</param>
        public static void RegisterFunction(string functionName, string lambdaExpression, IDictionary variables)
        {
            AssertUtils.ArgumentHasText(functionName, "functionName");
            AssertUtils.ArgumentHasText(lambdaExpression, "lambdaExpression");

            ExpressionLexer lexer = new ExpressionLexer(new StringReader(lambdaExpression));
            ExpressionParser parser = new SpringExpressionParser(lexer);

            parser.lambda();
            variables[functionName] = parser.getAST();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class
        /// by parsing specified primary expression string.
        /// </summary>
        /// <param name="expression">Primary expression to parse.</param>
        internal static IExpression ParsePrimary(string expression)
        {
            if (StringUtils.HasText(expression))
            {
                ExpressionLexer lexer = new ExpressionLexer(new StringReader(expression));
                ExpressionParser parser = new SpringExpressionParser(lexer);

                parser.primaryExpression();
                return (IExpression) parser.getAST();
            }
            else
            {
                return new Expression();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class
        /// by parsing specified property expression string.
        /// </summary>
        /// <param name="expression">Property expression to parse.</param>
        internal static IExpression ParseProperty(string expression)
        {
            if (StringUtils.HasText(expression))
            {
                ExpressionLexer lexer = new ExpressionLexer(new StringReader(expression));
                ExpressionParser parser = new SpringExpressionParser(lexer);

                parser.property();
                return (IExpression) parser.getAST();
            }
            else
            {
                return new Expression();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class.
        /// </summary>
        public Expression()
        {}

        /// <summary>
        /// Create a new instance from SerializationInfo
        /// </summary>
        protected Expression(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        /// <summary>
        /// Evaluates this expression for the specified root object and returns 
        /// value of the last node.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <returns>Value of the last node.</returns>
        protected override object Get(object context, EvaluationContext evalContext)
        {
            object result = context;

            if (this.getNumberOfChildren() > 0)
            {
                AST node = this.getFirstChild();
                while (node != null)
                {
                    result = ((BaseNode)node).GetValueInternal( result, evalContext );

                    node = node.getNextSibling();
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates this expression for the specified root object and sets 
        /// value of the last node.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <param name="newValue">Value to set last node to.</param>
        /// <exception cref="NotSupportedException">If navigation expression is empty.</exception>
        protected override void Set(object context, EvaluationContext evalContext, object newValue)
        {
            object target = context;

            if (this.getNumberOfChildren() > 0)
            {
                AST node = this.getFirstChild();

                for (int i = 0; i < this.getNumberOfChildren() - 1; i++)
                {
                    try
                    {
                        target = ((BaseNode) node).GetValueInternal(target, evalContext);
                        node = node.getNextSibling();
                    }
                    catch (NotReadablePropertyException e)
                    {
                        throw new NotWritablePropertyException("Cannot read the value of '" + node.getText() + "' property in the expression.", e);
                    }
                }
                ((BaseNode) node).SetValueInternal(target, evalContext, newValue);
            }
            else
            {
                throw new NotSupportedException("You cannot set the value for an empty expression.");
            }
        }

        /// <summary>
        /// Evaluates this expression for the specified root object and returns 
        /// <see cref="PropertyInfo"/> of the last node, if possible.
        /// </summary>
        /// <param name="context">Context to evaluate expression against.</param>
        /// <param name="variables">Expression variables map.</param>
        /// <returns>Value of the last node.</returns>
        internal PropertyInfo GetPropertyInfo(object context, IDictionary variables)
        {
            if (this.getNumberOfChildren() > 0)
            {
                object target = context;
                AST node = this.getFirstChild();

                for (int i = 0; i < this.getNumberOfChildren() - 1; i++)
                {
                    target = ((IExpression) node).GetValue(target, variables);
                    node = node.getNextSibling();
                }

                if (node is PropertyOrFieldNode)
                {
                    return (PropertyInfo) ((PropertyOrFieldNode) node).GetMemberInfo(target);
                }
                else if (node is IndexerNode)
                {
                    return ((IndexerNode)node).GetPropertyInfo(target, variables);
                }
                else
                {
                    throw new FatalReflectionException("Cannot obtain PropertyInfo from an expression that does not resolve to a property or an indexer.");
                }
            }

            throw new FatalReflectionException("Cannot obtain PropertyInfo for empty property name.");
        }
    }
}
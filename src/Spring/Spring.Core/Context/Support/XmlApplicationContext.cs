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

namespace Spring.Context.Support
{
	/// <summary>
	/// An <see cref="Spring.Context.IApplicationContext"/> implementation that
	/// reads context definitions from XML based resources.
	/// </summary>
	/// <remarks>
	/// <p>
	/// Currently, the resources that are supported are the <c>file</c>,
	/// <c>http</c>, <c>ftp</c>, <c>config</c> and <c>assembly</c> resource
	/// types.
	/// </p>
	/// <p>
	/// You can provide custom implementations of the
	/// <see cref="Spring.Core.IO.IResource"/> interface and and register them
	/// with any <see cref="Spring.Context.IApplicationContext"/> that inherits
	/// from the
	/// <see cref="Spring.Context.Support.AbstractApplicationContext"/>
	/// interface.
	/// </p>
	/// <note>
	/// In case of multiple config locations, later object definitions will
	/// override ones defined in previously loaded resources. This can be
	/// leveraged to deliberately override certain object definitions via an
	/// extra XML file.
	/// </note>
	/// </remarks>
	/// <example>
	/// <p>
	/// Find below some examples of instantiating an
	/// <see cref="Spring.Context.Support.XmlApplicationContext"/> using a
	/// variety of different XML resources.
	/// </p>
	/// <code language="C#">
	/// // an XmlApplicationContext that reads its object definitions from an
	/// //    XML file that has been embedded in an assembly...
	/// IApplicationContext context = new XmlApplicationContext
	///		(
	///			"assembly://AssemblyName/NameSpace/ResourceName"
	///		);
	///		
	/// // an XmlApplicationContext that reads its object definitions from a
	/// //    number of disparate XML resources...
	/// IApplicationContext context = new XmlApplicationContext
	///		(
	///			// from an XML file that has been embedded in an assembly...
	///			"assembly://AssemblyName/NameSpace/ResourceName",
	///			// and from a (relative) filesystem-based resource...
	///			"file://Objects/services.xml",
	///			// and from an App.config / Web.config resource...
	///			"config://spring/objects"
	///		);
	/// </code>
	/// </example>
	/// <author>Rod Johnson</author>
	/// <author>Juergen Hoeller</author>
	/// <author>Griffin Caprio (.NET)</author>
	/// <version>$Id: XmlApplicationContext.cs,v 1.18 2007/08/08 17:46:37 bbaia Exp $</version>
	/// <seealso cref="Spring.Core.IO.IResource"/>
	/// <seealso cref="Spring.Core.IO.IResourceLoader"/>
	/// <seealso cref="Spring.Core.IO.ConfigurableResourceLoader"/>
	public class XmlApplicationContext : AbstractXmlApplicationContext
	{
		private string[] _configurationLocations;

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
        /// loading the definitions from the supplied XML resource locations.
        /// </summary>
        /// <remarks>The created context will be case sensitive.</remarks>
        /// <param name="configurationLocations">
        /// Any number of XML based object definition resource locations.
        /// </param>
        public XmlApplicationContext(params string[] configurationLocations)
            : this(true, null, true, null, configurationLocations)
        { }

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
		/// loading the definitions from the supplied XML resource locations.
		/// </summary>
		/// <param name="caseSensitive">Flag specifying whether to make this context case sensitive or not.</param>
		/// <param name="configurationLocations">
		/// Any number of XML based object definition resource locations.
		/// </param>
        public XmlApplicationContext(bool caseSensitive,
            params string[] configurationLocations)
            : this(true, null, caseSensitive, null, configurationLocations)
        { }
        
        /// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
		/// loading the definitions from the supplied XML resource locations.
		/// </summary>
		/// <param name="name">The application context name.</param>
        /// <param name="caseSensitive">Flag specifying whether to make this context case sensitive or not.</param>
        /// <param name="configurationLocations">
		/// Any number of XML based object definition resource locations.
		/// </param>
        public XmlApplicationContext(string name, bool caseSensitive,
            params string[] configurationLocations)
            : this(true, name, caseSensitive, null, configurationLocations)
        { }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
        /// loading the definitions from the supplied XML resource locations,
        /// with the given <paramref name="parentContext"/>.
        /// </summary>
        /// <param name="parentContext">
        /// The parent context (may be <see langword="null"/>).
        /// </param>
        /// <param name="configurationLocations">
        /// Any number of XML based object definition resource locations.
        /// </param>
        public XmlApplicationContext(
            IApplicationContext parentContext,
            params string[] configurationLocations)
            : this(true, null, true, parentContext, configurationLocations)
        { }

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
		/// loading the definitions from the supplied XML resource locations,
		/// with the given <paramref name="parentContext"/>.
		/// </summary>
		/// <param name="caseSensitive">Flag specifying whether to make this context case sensitive or not.</param>
		/// <param name="parentContext">
		/// The parent context (may be <see langword="null"/>).
		/// </param>
		/// <param name="configurationLocations">
		/// Any number of XML based object definition resource locations.
		/// </param>
        public XmlApplicationContext(
            bool caseSensitive,
            IApplicationContext parentContext,
            params string[] configurationLocations)
            : this(true, null, caseSensitive, parentContext, configurationLocations)
        { }
        
        /// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
		/// loading the definitions from the supplied XML resource locations,
		/// with the given <paramref name="parentContext"/>.
		/// </summary>
		/// <param name="name">The application context name.</param>
        /// <param name="caseSensitive">Flag specifying whether to make this context case sensitive or not.</param>
        /// <param name="parentContext">
		/// The parent context (may be <see langword="null"/>).
		/// </param>
		/// <param name="configurationLocations">
		/// Any number of XML based object definition resource locations.
		/// </param>
        public XmlApplicationContext(
            string name,
            bool caseSensitive,
            IApplicationContext parentContext,
            params string[] configurationLocations)
            : this(true, name, caseSensitive, parentContext, configurationLocations)
        {}

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="Spring.Context.Support.XmlApplicationContext"/> class,
        /// loading the definitions from the supplied XML resource locations,
        /// with the given <paramref name="parentContext"/>.
        /// </summary>
        /// <remarks>
        /// This constructor is meant to be used by derived classes. By passing <paramref name="refresh"/>=false, it is
        /// the responsibility of the deriving class to call <see cref="AbstractApplicationContext.Refresh()"/> to initialize the context instance.
        /// </remarks>
        /// <param name="refresh">if true, <see cref="AbstractApplicationContext.Refresh()"/> is called automatically.</param>
        /// <param name="name">The application context name.</param>
        /// <param name="caseSensitive">Flag specifying whether to make this context case sensitive or not.</param>
        /// <param name="parentContext">
        /// The parent context (may be <see langword="null"/>).
        /// </param>
        /// <param name="configurationLocations">
        /// Any number of XML based object definition resource locations.
        /// </param>
        public XmlApplicationContext(
            bool refresh,
            string name,
            bool caseSensitive,
            IApplicationContext parentContext,
            params string[] configurationLocations)
            : base(name, caseSensitive, parentContext )
        {
            _configurationLocations = configurationLocations;
            if (refresh)
            {
                Refresh();
            }
        }

	    /// <summary>
	    /// An array of resource locations, referring to the XML object
	    /// definition files that this context is to be built with.
	    /// </summary>
	    /// <returns>
	    /// An array of resource locations, or <see langword="null"/> if none.
	    /// </returns>
	    /// <seealso cref="Spring.Context.Support.AbstractXmlApplicationContext.ConfigurationLocations"/>
	    protected override string[] ConfigurationLocations
	    {
	        get { return _configurationLocations; }
	    }
	}
}
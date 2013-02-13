using System;
using System.Collections.Generic;
using System.Text;

using Sprocket;
using Sprocket.Web;

namespace MyWebsite
{
	// add zero or more ModuleDependency attributes to ensure that your module
	// is sorted and initialised after modules that have higher precedence.
	[ModuleDependency(typeof(WebEvents))]
	[ModuleTitle("MyWebsite Main Request Handler")] // required
	[ModuleDescription("This line is a quick one-sentence overview of your module")] // optional
	public class MyWebsiteHandler : ISprocketModule // implement ISprocketModule to make your class a part of the Sprocket module registry
	{
		/// <summary>
		/// This allows you to hook into events of any other modules. This method is called once when the application thread is
		/// first initialised. This module instance is held alive for the lifetime of the application thread. Remember: Don't use
		/// static variables unless you are prepared to write thread-safe code, or other worker processes may cause thread-related
		/// problems. Remember that each worker process (HttpApplication instance) is only serving one request at a time, so while
		/// being executed, all of its instance members are isolated for the use of that one request.
		/// </summary>
		/// <param name="registry">A quick reference to the modules in the module registry. You can also request a module reference
		/// any time by accessing Core.Instance[typeof(SomeModule)].</param>
		public void AttachEventHandlers(ModuleRegistry registry)
		{
		}
	}
}

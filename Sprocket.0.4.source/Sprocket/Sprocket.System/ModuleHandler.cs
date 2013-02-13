using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Web;

namespace Sprocket
{
	/// <summary>
	/// Handles discovery, loading, registration and dependency checking of Sprocket modules.
	/// </summary>
	public class ModuleHandler
	{
		private ModuleRegistry registry = new ModuleRegistry();
		private Dictionary<Type, List<Type>> interfaceImplementations = new Dictionary<Type, List<Type>>();

		internal Dictionary<Type, List<Type>> InterfaceImplementations
		{
			get { return interfaceImplementations; }
		}

		public List<Type> GetInterfaceImplementations(Type interfaceType)
		{
			if (!interfaceImplementations.ContainsKey(interfaceType))
				return new List<Type>();
			return interfaceImplementations[interfaceType];
		}

		/// <summary>
		/// Gets a reference to the specified Sprocket module.
		/// </summary>
		/// <param name="moduleRegCode">The namespace of the module to retrieve.</param>
		/// <returns>A reference to the module, or null if the module doesn't exist.</returns>
		public RegisteredModule this[string moduleNamespace]
		{
			get { return registry[moduleNamespace]; }
		}

		/// <summary>
		/// Searches for assemblies containing Sprocket modules.
		/// </summary>
		public void RegisterModules()
		{
			string dir = HttpContext.Current.Request.PhysicalApplicationPath + "bin";
			RegisterModules(dir);
		}

		/// <summary>
		/// Searches for assemblies containing Sprocket modules.
		/// </summary>
		/// <param name="path">The physical disk location to search for assemblies.</param>
		private void RegisterModules(string path)
		{
			string[] paths = Directory.GetFileSystemEntries(path, "*.dll");
			for(int i=0; i<paths.Length; i++)
			{
				try		{ RegisterFromAssembly(Assembly.LoadFrom(paths[i])); }
				catch	{ continue; }
			}
			foreach(string dir in Directory.GetDirectories(path))
				RegisterModules(dir);
		}

		/// <summary>
		/// Searches an assembly for Sprocket modules and adds them to the module registry.
		/// </summary>
		/// <param name="asm">A reference to the assembly to search</param>
		private void RegisterFromAssembly(Assembly asm)
		{
			foreach(Type t in asm.GetTypes())
			{
				if (t.GetInterface("ISprocketModule") != null)
				{
					ISprocketModule module;
					try
					{
						module = (ISprocketModule)asm.CreateInstance(t.FullName);
						registry.RegisterModule(module);
					}
					catch { continue; }
				}
				// here we register all the extra interfaces we find, and the types that implement them.
				// we do this to allow modules to have easy access to find types that implement the
				// interfaces they expose.
				foreach (Type i in t.GetInterfaces())
				{
					if (!interfaceImplementations.ContainsKey(i))
						interfaceImplementations.Add(i, new List<Type>());
					interfaceImplementations[i].Add(t);
				}
			}
		}

		/// <summary>
		/// Unregisters all modules that are dependent on modules that have not been
		/// loaded into the module registry.
		/// </summary>
		public void CheckDependencies()
		{
			// find all the modules that have a missing dependency...
			List<RegisteredModule> badModules = new List<RegisteredModule>();
			foreach(RegisteredModule module in registry)
			{
				ModuleDependencyAttribute[] atts = (ModuleDependencyAttribute[])Attribute.GetCustomAttributes(module.Module.GetType(), typeof(ModuleDependencyAttribute), true);
				foreach(ModuleDependencyAttribute att in atts)
					if(!registry.IsRegistered(att.ModuleType.FullName))
					{
						badModules.Add(module);
						break;
					}
			}

			// ...then unregister them
			foreach (RegisteredModule badmod in badModules)
				registry.Unregister(badmod.Module);

			foreach (RegisteredModule module in registry)
				module.Importance = registry.Count;

			// assign an importance value to each remaining module based on dependence heirarchy
			Dictionary<string, RegisteredModule> assignedModules = new Dictionary<string, RegisteredModule>();
			Dictionary<string, RegisteredModule> preAssignedModules = new Dictionary<string, RegisteredModule>();

			int mostImportantSoFar = registry.Count;
			while (assignedModules.Count < registry.Count)
			{
				foreach (RegisteredModule registeredModule in registry)
				{
					bool notyet = false;
					ModuleDependencyAttribute[] atts = (ModuleDependencyAttribute[])Attribute.GetCustomAttributes(registeredModule.Module.GetType(), typeof(ModuleDependencyAttribute), true);
					foreach (ModuleDependencyAttribute att in atts)
						if (!assignedModules.ContainsKey(att.ModuleType.FullName))
							notyet = true; // this module doesn't get its importance increased until all of its dependencies are accounted for

					if (notyet)
						continue;

					if (registeredModule.Importance == registry.Count)
						preAssignedModules.Add(registeredModule.Module.GetType().FullName, registeredModule);
					registeredModule.Importance--;
					if (registeredModule.Importance < mostImportantSoFar)
						mostImportantSoFar = registeredModule.Importance;
				}
				foreach (KeyValuePair<string, RegisteredModule> kvp in preAssignedModules)
					assignedModules.Add(kvp.Key, kvp.Value);
				preAssignedModules.Clear();
			}

			// fix the importance values to start from 0 now that we know the depth of the heirarchy
			foreach (RegisteredModule m in registry)
				m.Importance -= mostImportantSoFar;

			// sort the registry so that iterating happens in order of dependence heirarchy depth
			registry.SortByImportance();
		}

		/// <summary>
		/// Tells each module to hook into system events and events of other modules.
		/// </summary>
		public void AttachEventHandlers()
		{
			foreach(RegisteredModule module in registry)
				module.Module.AttachEventHandlers(registry);
		}

		public ModuleRegistry ModuleRegistry
		{
			get { return registry; }
		}
	}

	#region ModuleRegistry

	public class RegisteredModule
	{
		private int importance = 0;
		private ISprocketModule module = null;

		public int Importance
		{
			get { return importance; }
			set { importance = value; }
		}

		public ISprocketModule Module
		{
			get { return module; }
			set { module = value; }
		}

		private string nspace;
		public string Namespace
		{
			get { return nspace; }
		}

		private string title;
		public string Title
		{
			get { return title; }
		}

		private string description;
		public string Description
		{
			get { return description; }
		}

		public RegisteredModule(ISprocketModule module)
		{
			this.module = module;
			nspace = module.GetType().FullName;
			ModuleDescriptionAttribute att = (ModuleDescriptionAttribute)Attribute.GetCustomAttribute(module.GetType(), typeof(ModuleDescriptionAttribute));
			description = att == null ? "" : att.Description;
			ModuleTitleAttribute att2 = (ModuleTitleAttribute)Attribute.GetCustomAttribute(module.GetType(), typeof(ModuleTitleAttribute));
			title = att2 == null ? nspace : att2.Title;
		}
	}

	/// <summary>
	/// Encapsulates the list of modules that have been registered for use by Sprocket.
	/// </summary>
	public class ModuleRegistry : IEnumerable
	{
		private Dictionary<string, RegisteredModule> moduleRegistry = new Dictionary<string, RegisteredModule>();
		/// <summary>
		/// Adds the specified module instance to the registry.
		/// </summary>
		/// <param name="moduleNamespace">The registration code for the module</param>
		/// <param name="module">A reference to the module to register</param>
		public void RegisterModule(ISprocketModule module)
		{
			moduleRegistry.Add(module.GetType().FullName, new RegisteredModule(module));
		}

		/// <summary>
		/// Gets a reference to the registered module with the specified registration code.
		/// </summary>
		/// <param name="moduleNamespace">The registration code for the module</param>
		/// <returns>A reference to a module, or null if it doesn't exist</returns>
		public RegisteredModule this[string moduleNamespace]
		{
			get
			{
				try { return moduleRegistry[moduleNamespace]; }
				catch (Exception ex) { throw new SprocketException("The registration code " + moduleNamespace + " does not exist in the module registry.", ex); }
			}
		}

		/// <summary>
		/// Gets the number of registered modules.
		/// </summary>
		public int Count
		{
			get { return moduleRegistry.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether or not a module with the specified registration code
		/// exists in the module registry.
		/// </summary>
		/// <param name="moduleNamespace">The registration code to look for</param>
		/// <returns>True if found, otherwise false</returns>
		public bool IsRegistered(string moduleNamespace)
		{
			return moduleRegistry.ContainsKey(moduleNamespace);
		}

		/// <summary>
		/// Removes the specified module from the registry. Used for unregistering
		/// modules whose dependencies are missing.
		/// </summary>
		/// <param name="module">A reference to the module to remove.</param>
		public void Unregister(ISprocketModule module)
		{
			string name = module.GetType().FullName;
			if(moduleRegistry.ContainsKey(name))
				moduleRegistry.Remove(name);
		}

		private bool sorted = false;
		private List<RegisteredModule> sortedRegistry = new List<RegisteredModule>();
		/// <summary>
		/// Sorts all of the modules in the module registry in order of
		/// dependency hierarchy.
		/// </summary>
		public void SortByImportance()
		{
			sortedRegistry.InsertRange(0, moduleRegistry.Values);
			sortedRegistry.Sort(new ModuleImportanceComparer());
			sorted = true;
			
		}

		/// <summary>
		/// Used for comparing the importance values of individual modules.
		/// </summary>
		public class ModuleImportanceComparer : IComparer<RegisteredModule>
		{
			public int Compare(RegisteredModule x, RegisteredModule y)
			{
				if (x.Importance > y.Importance)
					return 1;
				else if (x.Importance < y.Importance)
					return -1;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets an enumerator for iterating through the module registry.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			if (sorted)
				return new ModuleRegistryEnumerator(sortedRegistry);
			else
				return new ModuleRegistryEnumerator(moduleRegistry.Values);
		}
	}
	#endregion

	#region ModuleRegistryEnumerator
	public class ModuleRegistryEnumerator : IEnumerator<RegisteredModule>
	{
		IEnumerator<RegisteredModule> regEnum;
		public ModuleRegistryEnumerator(IEnumerable<RegisteredModule> registry)
		{
			regEnum = registry.GetEnumerator();
		}

		public void Reset()
		{
			regEnum.Reset();
		}

		public RegisteredModule Current
		{
			get
			{
				return regEnum.Current;
			}
		}

		public bool MoveNext()
		{
			return regEnum.MoveNext();
		}

		public void Dispose()
		{
		}

		object IEnumerator.Current
		{
			get { return regEnum.Current; }
		}
	}
	#endregion
}

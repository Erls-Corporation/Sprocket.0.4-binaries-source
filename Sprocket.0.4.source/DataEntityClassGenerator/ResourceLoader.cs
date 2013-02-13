using System;
using System.IO;
using System.Text;
using System.Reflection;

namespace ClassGenerator
{
	/// <summary>
	/// Utility class for loading resources from an assembly.
	/// </summary>
	public static class ResourceLoader
	{
		/// <summary>
		/// Loads a text-based resource from the calling assembly.
		/// </summary>
		/// <param name="fullNamespaceFilename">The namespace of the resource.</param>
		/// <returns>A string containing the full text of the resource</returns>
		public static string LoadTextResource(string filename)
		{
			Assembly asm = Assembly.GetCallingAssembly();
			return LoadTextResource(asm, "ClassGenerator.templates." + filename);
		}

		/// <summary>
		/// Loads a text-based resource from the specified assembly.
		/// </summary>
		/// <param name="asm">A reference to the assembly from which to load the resource</param>
		/// <param name="fullNamespaceFilename">The namespace of the resource.</param>
		/// <returns>A string containing the full text of the resource</returns>
		public static string LoadTextResource(Assembly asm, string fullNamespaceFilename)
		{
			StreamReader r;
			try
			{
				r = new StreamReader(asm.GetManifestResourceStream(fullNamespaceFilename));
			}
			catch
			{
				throw new Exception("Cannot load resource " + fullNamespaceFilename + " from assembly " + asm.FullName);
			}
			string str = r.ReadToEnd();
			r.Close();
			return str;
		}
	}
}

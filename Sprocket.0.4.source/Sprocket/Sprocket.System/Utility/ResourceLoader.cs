using System;
using System.IO;
using System.Text;
using System.Reflection;

namespace Sprocket.Utility
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
        public static string LoadTextResource(string fullNamespaceFilename)
        {
			Assembly asm = Assembly.GetCallingAssembly();
            return LoadTextResource(asm, fullNamespaceFilename);
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
                throw new SprocketException("Cannot load resource " + fullNamespaceFilename + " from assembly " + asm.FullName);
            }
            string str = r.ReadToEnd();
            r.Close();
            return str;
        }

		public static Stream LoadResource(string fullNamespaceFilename)
		{
			return LoadResource(Assembly.GetCallingAssembly(), fullNamespaceFilename);
		}

		public static Stream LoadResource(Assembly asm, string fullNamespaceFilename)
		{
			try
			{
				return asm.GetManifestResourceStream(fullNamespaceFilename);
			}
			catch
			{
				throw new SprocketException("Cannot load resource " + fullNamespaceFilename + " from assembly " + asm.FullName);
			}
		}
	}
}

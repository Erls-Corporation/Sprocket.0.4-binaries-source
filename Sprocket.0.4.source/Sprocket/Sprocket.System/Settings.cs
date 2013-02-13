using System;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sprocket;
using Sprocket.Utility;
using System.Xml;

namespace Sprocket
{
	/// <summary>
	/// Handles application configuration settings in the .config file and any other files with a similar purpose.
	/// </summary>
	[ModuleTitle("Sprocket Settings")]
	[ModuleDescription("Handles general application settings, mainly in the application's .config file.")]
	public sealed class SprocketSettings : ISprocketModule
	{
		/// <summary>
		/// Designed for settings-related events so that other modules
		/// can assign errors to an errors collection or react to errors
		/// that have been flagged in the collection.
		/// </summary>
		/// <param name="errors">An instance of an errors collection</param>
		public delegate void CheckSettingsHandler(SettingsErrors errors);
		/// <summary>
		/// Fires to allow each Sprocket module to specify errors in settings
		/// that should be cause for the application to halt. Errors can also
		/// be flagged if they are non-critical.
		/// </summary>
		public event CheckSettingsHandler OnCheckingSettings;
		/// <summary>
		/// Fires after the OnCheckingSettings event fires and only if one or
		/// more critical errors have been flagged.
		/// </summary>
		public event CheckSettingsHandler OnCriticalSettingsErrorsFound;
		/// <summary>
		/// Fires after the OnCheckingSettings event fires and only if errors
		/// have been found, none of which were critical.
		/// </summary>
		public event CheckSettingsHandler OnSettingsErrorsFound;
		/// <summary>
		/// Fires after the OnCheckingSettings event fires and only if no
		/// errors were found.
		/// </summary>
		public event EmptyHandler OnSettingsVerified;

		/// <summary>
		/// Gets the value from a key/value pair in the .config file
		/// </summary>
		/// <param name="key">The key to return the value for</param>
		/// <returns>The settings value</returns>
		public string this[string key]
		{
			get
			{
				return ConfigurationManager.AppSettings[key];
			}
		}

		/// <summary>
		/// Retrieves a value from the .config file for the application.
		/// </summary>
		/// <param name="key">The key to return the value for</param>
		/// <returns>The settings value</returns>
		public static string GetValue(string key)
		{
			return Instance[key];
		}

		/// <summary>
		/// Retrieves a value from the .config file and returns a boolean indicating
		/// if the value was (case insensitive) "true", "yes", "on" or "1". Negative
		/// values are "false", "no", "off" and "0".
		/// </summary>
		/// <param name="key">The key to return the boolean value for</param>
		/// <returns>A boolean representation of the settings value</returns>
		public static bool GetBooleanValue(string key)
		{
			string val = Instance[key];
			if (val == null) return false;
			return StringUtilities.MatchesAny(val.ToLower(), "true", "yes", "on", "1");
		}

		/// <summary>
		/// Returns the current instance of the SprocketSettings module
		/// </summary>
		public static SprocketSettings Instance
		{
			get { return (SprocketSettings)Core.Instance[typeof(SprocketSettings)].Module; }
		}

		public void AttachEventHandlers(ModuleRegistry registry)
		{
		}

		/// <summary>
		/// Gets a reference to the collection of errors found during the OnCheckingSettings event
		/// </summary>
		public SettingsErrors ErrorList
		{
			get { return errors; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the errors collection contains any errors
		/// </summary>
		public bool HasErrors
		{
			get { return errors.List.Count > 0; }
		}

		/// <summary>
		/// Gets a value indicating whether any of the errors found during the OnCheckingSettings
		/// event were critical and thus should prevent further program execution.
		/// </summary>
		public bool HasCriticalErrors
		{
			get { return errors.HasCriticalError; }
		}

		/// <summary>
		/// A container class that holds a list of settings-related errors found by the
		/// various registered Sprocket modules.
		/// </summary>
		public class SettingsErrors
		{
			private Dictionary<RegisteredModule, List<string>> errors = new Dictionary<RegisteredModule, List<string>>();
			private bool critical = false;

			/// <summary>
			/// Adds an error description to the list, mapped to a specific module registration code.
			/// </summary>
			/// <param name="moduleRegCode">The registration code for the module that found the error</param>
			/// <param name="error">A description of the error</param>
			public void Add(ISprocketModule module, string error)
			{
				RegisteredModule m = Core.Instance[module];
				if (!errors.ContainsKey(m))
					errors[m] = new List<string>();
				errors[m].Add(error);
			}

			/// <summary>
			/// Gets a reference to the list of errors
			/// </summary>
			public Dictionary<RegisteredModule, List<string>> List
			{
				get { return errors; }
			}

			/// <summary>
			/// Informs the system that a critical error is present in the settings and that
			/// Sprocket should terminate execution of the application as a result.
			/// </summary>
			public void SetCriticalError()
			{
				critical = true;
			}

			/// <summary>
			/// Gets a boolean value specifying whether or not the application settings have
			/// a critical error that will prevent proper operation of the application.
			/// </summary>
			public bool HasCriticalError
			{
				get { return critical; }
			}
		}

		#region ISprocketModule Members

		private static SettingsErrors errors = null;
		public static SettingsErrors Errors
		{
			get { return SprocketSettings.errors; }
		}

		internal void ValidateSettings()
		{
			errors = new SettingsErrors();
			if (OnCheckingSettings != null)
				OnCheckingSettings(errors);
			if (errors.HasCriticalError && OnCriticalSettingsErrorsFound != null)
				OnCriticalSettingsErrorsFound(errors);
			else if (errors.List.Count > 0 && OnSettingsErrorsFound != null)
				OnSettingsErrorsFound(errors);
			else if (OnSettingsVerified != null && !errors.HasCriticalError)
				OnSettingsVerified();
		}

		#endregion
	}
}

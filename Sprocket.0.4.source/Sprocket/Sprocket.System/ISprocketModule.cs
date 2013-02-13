using System;
using Sprocket;

namespace Sprocket
{
	public interface ISprocketModule
	{
		void AttachEventHandlers(ModuleRegistry registry);
	}

	public delegate void ModuleEnabledStateChangingHandler(ISprocketModule module, StateChangingEventArgs e);

	public interface IOptionalModule
	{
		void DisableModule(bool preserveData);
		void EnableModule();
		event ModuleEnabledStateChangingHandler OnModuleEnabledStateChanging;
		event ModuleEventHandler OnModuleEnabledStateChanged;
	}
}

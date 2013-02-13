using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.FileManager
{
	public interface IFileManagerDataLayer
	{
		Type DatabaseHandlerType { get; }
		void InitialiseDatabase(Result result);

		#region Definitions for SprocketFile

		event InterruptableEventHandler<SprocketFile> OnBeforeDeleteSprocketFile;
		event NotificationEventHandler<SprocketFile> OnSprocketFileDeleted;
		Result Store(SprocketFile sprocketFile);
		Result Delete(SprocketFile sprocketFile);
		SprocketFile SelectSprocketFile(long id, bool getFileData);

		#endregion
	}
}

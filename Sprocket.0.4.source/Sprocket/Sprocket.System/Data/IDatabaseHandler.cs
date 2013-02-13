using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Sprocket.Data
{
	public interface IDatabaseHandler
	{
		/// <summary>
		/// Create tables, check that required data exists, etc.
		/// </summary>
		/// <returns>A <see cref="Result" /> indicating success or
		/// failure</returns>
		Result Initialise();

		/// <summary>
		/// Check configuration settings, if any, and return a result
		/// indicating whether the database has enough information to
		/// successfully call <see cref="Initialise"/>.
		/// </summary>
		/// <returns>A <see cref="Result"/> value indicating if the
		/// <c cref="Initialise" /> method can be called</returns>
		Result CheckConfiguration();

		/// <summary>
		/// Gets the default connection string to use for this database
		/// </summary>
		string ConnectionString { get; }

		/// <summary>
		/// This should return a short descriptive title for the database
		/// provider, e.g. "SQL Server 2005" or "SQLite 3"
		/// </summary>
		string Title { get; }

		/// <summary>
		/// This event should be called from inside the <c cref="Initialise" />
		/// method. It is used to allow each module to perform initialisation
		/// tasks relating to this database handler.
		/// </summary>
		event InterruptableEventHandler OnInitialise;

		/// <summary>
		/// This should implement some form of generator that returns a unique long
		/// value for use as a database table row identifier.
		/// </summary>
		/// <returns>A unique long value</returns>
		long GetUniqueID();

		/// <summary>
		/// This should first check to see if a connection has been created and held
		/// by PersistConnection(), and if so, return that. Otherwise, simply return
		/// a new connection initialised and opened with the default ConnectionString.
		/// </summary>
		IDbConnection GetConnection();
		
		/// <summary>
		/// This will request that the database handler open and hold a reference to a
		/// connection object which will be used in place of new connections in
		/// subsequent queries. Use this to ensure that all queries in a
		/// TransactionScope use the same connection object and don't escalate their
		/// operation to MSDTC. PersistConnection() should be called immediately
		/// after the TransactionScope using statement. An accompanying call to
		/// ReleaseConnection should be made outside the TransactionScope closing
		/// brace. PersistConnection should generally not be used inside a data layer
		/// class, rather it is intended for for the purpose of linking multiple calls
		/// to different unrelated data handler methods to a single transaction.
		/// </summary>
		void PersistConnection();

		/// <summary>
		/// This should remove any reference to a connection object created by
		/// PersistConnection(), including closing and disposing the reference.
		/// </summary>
		void ReleaseConnection();

		/// <summary>
		/// This should close the specified connection if it was not created by PersistConnection().
		/// </summary>
		/// <param name="conn">The connection to close</param>
		void ReleaseConnection(IDbConnection conn);
	}
}

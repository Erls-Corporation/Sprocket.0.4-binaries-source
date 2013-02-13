using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Data.SQLite;
using System.Transactions;

namespace Sprocket.Data
{
	public class SQLiteDatabase : IDatabaseHandler
	{
		private string connectionString;
		public string ConnectionString
		{
			get
			{
				if (connectionString == null)
					connectionString = "Data Source=" + PhysicalPath + ";UseUTF16Encoding=True;";
				return connectionString;
			}
		}

		public Result Initialise()
		{
			try
			{
				if (!File.Exists(PhysicalPath))
				{
					FileInfo info = new FileInfo(PhysicalPath);
					if (!info.Directory.Exists)
						info.Directory.Create();
					SQLiteConnection.CreateFile(PhysicalPath);
				}
			}
			catch (Exception ex)
			{
				return new Result("Unable to initialise the default SQLite database: " + ex.Message);
			}

			Result result = new Result();

			if (OnInitialise != null)
				OnInitialise(result);
			return result;
		}

		private string physicalPath = null;
		private string PhysicalPath
		{
			get
			{
				if (physicalPath == null)
					physicalPath = Web.WebUtility.MapPath("datastore/databases/main.s3db");
				return physicalPath;
			}
		}

		public Result CheckConfiguration()
		{
			return new Result();
		}

		public string Title
		{
			get { return "SQLite 3"; }
		}

		public event InterruptableEventHandler OnInitialise;

		public long GetUniqueID()
		{
			return -1;
		}

		#region IDatabaseHandler Members


		public IDbConnection GetConnection()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void PersistConnection()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ReleaseConnection(IDbConnection conn)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IDatabaseHandler Members


		public void ReleaseConnection()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}

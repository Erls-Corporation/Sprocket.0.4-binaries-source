using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using System.Text.RegularExpressions;

using Sprocket.Web;
using Sprocket.Utility;

namespace Sprocket.Data
{
	public class SqlServer2005Database : IDatabaseHandler
	{
		private string connectionString = null;

		public Result Initialise()
		{
			Result result;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					DatabaseManager.DatabaseEngine.PersistConnection();
					SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					result = ExecuteScript(conn, ResourceLoader.LoadTextResource("Sprocket.Data.SqlServer2005.scripts.sql"));
					if (result.Succeeded && OnInitialise != null)
						OnInitialise(result);
					if (result.Succeeded)
						scope.Complete();
					return result;
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection();
			}
		}

		public string ConnectionString
		{
			get { return connectionString; }
		}

		public Result CheckConfiguration()
		{
			connectionString = SprocketSettings.GetValue("ConnectionString");
			if (connectionString == null)
				return new Result("No value exists in Web.config for ConnectionString. SqlServer2005Database requires a valid connection string.");
			try
			{
				SqlConnection conn = new SqlConnection(connectionString);
				conn.Open();
				conn.Close();
				conn.Dispose();
			}
			catch (Exception ex)
			{
				return new Result("The ConnectionString value was unable to be used to open the database. The error was: " + ex.Message);
			}
			return new Result();
		}

		public string Title
		{
			get { return "SQL Server 2005"; }
		}

		public event InterruptableEventHandler OnInitialise;

		public long GetUniqueID()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand("GetUniqueID", conn);
				SqlParameter prm = new SqlParameter("@ID", SqlDbType.BigInt);
				prm.Direction = ParameterDirection.Output;
				cmd.Parameters.Add(prm);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.ExecuteNonQuery();
				conn.Close();
				return (long)prm.Value;
			}
		}

		public Result ExecuteScript(SqlConnection conn, string script)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				string[] sql = Regex.Split(script, @"^[ \t]*go[ \t\r\n]*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
				for (int i = 0; i < sql.Length; i++)
				{
					if (sql[i].Trim() == "")
						continue;
					using (TransactionScope innerscope = new TransactionScope())
					{
						SqlCommand cmd = conn.CreateCommand();
						cmd.CommandText = sql[i];
						cmd.CommandType = CommandType.Text;
						try
						{
							cmd.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							return new Result(ex.Message);
						}
						innerscope.Complete();
					}
				}
				scope.Complete();
				return new Result();
			}
		}

		public IDbConnection GetConnection()
		{
			SqlConnection conn = Conn as SqlConnection;
			if (conn == null)
				conn = new SqlConnection(ConnectionString);
			if (conn.State == ConnectionState.Closed)
				conn.Open();
			return conn;
		}

		private Stack<bool> persistenceStack = new Stack<bool>();
		public void PersistConnection()
		{
			if (persistenceStack.Count == 0)
			{
				SqlConnection conn = new SqlConnection(ConnectionString);
				conn.Open();
				Conn = conn;
			}
			persistenceStack.Push(true);
		}

		public void ReleaseConnection(IDbConnection conn)
		{
			if (conn != Conn && conn != null)
				if (conn.State != ConnectionState.Closed)
					conn.Close();
		}

		public void ReleaseConnection()
		{
			if (persistenceStack.Count > 0)
				persistenceStack.Pop();
			if (persistenceStack.Count == 0)
			{
				SqlConnection conn = Conn as SqlConnection;
				if (conn != null)
				{
					if (conn.State != ConnectionState.Closed)
						conn.Close();
					conn.Dispose();
				}
				Conn = null;
			}
		}

		private SqlConnection Conn
		{
			get { return CurrentRequest.Value["PersistedSqlConnection"] as SqlConnection; }
			set { CurrentRequest.Value["PersistedSqlConnection"] = value; }
		}
	}
}

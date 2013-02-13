using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Sprocket.Data;
using Sprocket.Utility;

namespace Sprocket.Web.FileManager
{
	public class SqlServer2005FileManagerDataLayer : IFileManagerDataLayer
	{
		public Type DatabaseHandlerType
		{
			get { return typeof(SqlServer2005Database); }
		}

		public void InitialiseDatabase(Result result)
		{
			if (!result.Succeeded)
				return;

			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlServer2005Database db = (SqlServer2005Database)DatabaseManager.DatabaseEngine;
					Result r = db.ExecuteScript(conn, ResourceLoader.LoadTextResource("Sprocket.Web.FileManager.SqlServer2005.scripts.sql"));
					if (!r.Succeeded)
					{
						result.SetFailed(r.Message);
						return;
					}
					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return;
		}

		public SqlParameter NewSqlParameter(string name, object value, SqlDbType dbType)
		{
			SqlParameter prm = new SqlParameter(name, value);
			prm.SqlDbType = dbType;
			return prm;
		}

		public Result Store(SprocketFile sprocketFile)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("SprocketFile_Store", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlParameter prm = new SqlParameter("@SprocketFileID", sprocketFile.SprocketFileID);
					prm.Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(prm);
					cmd.Parameters.Add(NewSqlParameter("@ClientSpaceID", sprocketFile.ClientSpaceID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@FileData", sprocketFile.FileData, SqlDbType.VarBinary));
					cmd.Parameters.Add(NewSqlParameter("@FileTypeExtension", sprocketFile.FileTypeExtension, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@OriginalFileName", sprocketFile.OriginalFileName, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@ContentType", sprocketFile.ContentType, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Title", sprocketFile.Title, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Description", sprocketFile.Description, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@UploadDate", sprocketFile.UploadDate, SqlDbType.DateTime));
					cmd.ExecuteNonQuery();
					sprocketFile.SprocketFileID = (long)prm.Value;
					scope.Complete();
				}
			}
			catch (Exception ex)
			{
				return new Result(ex.Message);
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public event InterruptableEventHandler<SprocketFile> OnBeforeDeleteSprocketFile;
		public event NotificationEventHandler<SprocketFile> OnSprocketFileDeleted;
		public Result Delete(SprocketFile sprocketFile)
		{
			Result result = new Result();
			if (OnBeforeDeleteSprocketFile != null)
				OnBeforeDeleteSprocketFile(sprocketFile, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						SqlCommand cmd = new SqlCommand("SprocketFile_Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@SprocketFileID", sprocketFile.SprocketFileID));
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
				}
				catch (Exception ex)
				{
					return new Result(ex.Message);
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
				if (OnSprocketFileDeleted != null)
					OnSprocketFileDeleted(sprocketFile);
			}
			return result;
		}

		public SprocketFile SelectSprocketFile(long id, bool getFileData)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SprocketFile_Select", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@SprocketFileID", id));
					cmd.Parameters.Add(new SqlParameter("@GetFileData", getFileData));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					SprocketFile entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new SprocketFile(reader);
					reader.Close();
					return entity;
				}
			}
		}
	}
}

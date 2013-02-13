using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Sprocket.Data;
using Sprocket.Utility;

namespace Sprocket.Web.Forums.SqlServer2005
{
	public class SqlServer2005ForumDataHandler : IForumDataHandler
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
					string[] scripts = new string[]{
						"Sprocket.Web.Forums.SqlServer2005.schema.sql",
						"Sprocket.Web.Forums.SqlServer2005.Generated.Entity.Procedures.sql",
						"Sprocket.Web.Forums.SqlServer2005.procedures.sql",
						"Sprocket.Web.Forums.SqlServer2005.data.sql"
					};
					foreach (string sql in scripts)
					{
						Result r = db.ExecuteScript(conn, ResourceLoader.LoadTextResource(sql));
						if (!r.Succeeded)
						{
							result.SetFailed(sql + ": " + r.Message);
							return;
						}
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

		#region Members for Forum

		public Result Store(Forum forum)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("Forum_Store", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlParameter prm = new SqlParameter("@ForumID", forum.ForumID);
					prm.Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(prm);
					cmd.Parameters.Add(NewSqlParameter("@ForumCategoryID", forum.ForumCategoryID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@ForumCode", forum.ForumCode, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Name", forum.Name, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Description", forum.Description, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@URLToken", forum.URLToken, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@DateCreated", forum.DateCreated, SqlDbType.DateTime));
					cmd.Parameters.Add(NewSqlParameter("@Rank", forum.Rank, SqlDbType.Int));
					cmd.Parameters.Add(NewSqlParameter("@PostWriteAccess", forum.PostWriteAccess, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@ReplyWriteAccess", forum.ReplyWriteAccess, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@ReadAccess", forum.ReadAccess, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@PostWriteAccessRoleID", forum.PostWriteAccessRoleID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@ReplyWriteAccessRoleID", forum.ReplyWriteAccessRoleID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@ReadAccessRoleID", forum.ReadAccessRoleID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@ModeratorRoleID", forum.ModeratorRoleID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@MarkupLevel", forum.MarkupLevel, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@ShowSignatures", forum.ShowSignatures, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@AllowImagesInMessages", forum.AllowImagesInMessages, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@AllowImagesInSignatures", forum.AllowImagesInSignatures, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@RequireModeration", forum.RequireModeration, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@AllowVoting", forum.AllowVoting, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@TopicDisplayOrder", forum.TopicDisplayOrder, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@Locked", forum.Locked, SqlDbType.Bit));
					cmd.ExecuteNonQuery();
					forum.ForumID = (long)prm.Value;
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

		public event InterruptableEventHandler<Forum> OnBeforeDeleteForum;
		public event NotificationEventHandler<Forum> OnForumDeleted;
		public Result Delete(Forum forum)
		{
			Result result = new Result();
			if (OnBeforeDeleteForum != null)
				OnBeforeDeleteForum(forum, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						SqlCommand cmd = new SqlCommand("Forum_Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ForumID", forum.ForumID));
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
				if (OnForumDeleted != null)
					OnForumDeleted(forum);
			}
			return result;
		}

		public Forum SelectForum(long id)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("Forum_Select", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumID", id));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					Forum entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new Forum(reader);
					reader.Close();
					return entity;
				}
			}
		}

		#endregion
		#region Members for ForumCategory

		public Result Store(ForumCategory forumCategory)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("ForumCategory_Store", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlParameter prm = new SqlParameter("@ForumCategoryID", forumCategory.ForumCategoryID);
					prm.Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(prm);
					cmd.Parameters.Add(NewSqlParameter("@ClientSpaceID", forumCategory.ClientSpaceID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@CategoryCode", forumCategory.CategoryCode, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Name", forumCategory.Name, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@URLToken", forumCategory.URLToken, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@DateCreated", forumCategory.DateCreated, SqlDbType.DateTime));
					cmd.Parameters.Add(NewSqlParameter("@Rank", forumCategory.Rank, SqlDbType.Int));
					cmd.Parameters.Add(NewSqlParameter("@InternalUseOnly", forumCategory.InternalUseOnly, SqlDbType.Bit));
					cmd.ExecuteNonQuery();
					forumCategory.ForumCategoryID = (long)prm.Value;
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

		public event InterruptableEventHandler<ForumCategory> OnBeforeDeleteForumCategory;
		public event NotificationEventHandler<ForumCategory> OnForumCategoryDeleted;
		public Result Delete(ForumCategory forumCategory)
		{
			Result result = new Result();
			if (OnBeforeDeleteForumCategory != null)
				OnBeforeDeleteForumCategory(forumCategory, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						SqlCommand cmd = new SqlCommand("ForumCategory_Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ForumCategoryID", forumCategory.ForumCategoryID));
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
				if (OnForumCategoryDeleted != null)
					OnForumCategoryDeleted(forumCategory);
			}
			return result;
		}

		public ForumCategory SelectForumCategory(long id)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumCategory_Select", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumCategoryID", id));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ForumCategory entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new ForumCategory(reader);
					reader.Close();
					return entity;
				}
			}
		}

		#endregion
		#region Members for ForumTopic

		public Result Store(ForumTopic forumTopic)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("ForumTopic_Store", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlParameter prm = new SqlParameter("@ForumTopicID", forumTopic.ForumTopicID);
					prm.Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(prm);
					cmd.Parameters.Add(NewSqlParameter("@ForumID", forumTopic.ForumID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@AuthorUserID", forumTopic.AuthorUserID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@AuthorName", forumTopic.AuthorName, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@Subject", forumTopic.Subject, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@DateCreated", forumTopic.DateCreated, SqlDbType.DateTime));
					cmd.Parameters.Add(NewSqlParameter("@Sticky", forumTopic.Sticky, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@ModerationState", forumTopic.ModerationState, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@Locked", forumTopic.Locked, SqlDbType.Bit));
					cmd.Parameters.Add(NewSqlParameter("@URLToken", forumTopic.URLToken, SqlDbType.NVarChar));
					cmd.ExecuteNonQuery();
					forumTopic.ForumTopicID = (long)prm.Value;
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

		public event InterruptableEventHandler<ForumTopic> OnBeforeDeleteForumTopic;
		public event NotificationEventHandler<ForumTopic> OnForumTopicDeleted;
		public Result Delete(ForumTopic forumTopic)
		{
			Result result = new Result();
			if (OnBeforeDeleteForumTopic != null)
				OnBeforeDeleteForumTopic(forumTopic, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						SqlCommand cmd = new SqlCommand("ForumTopic_Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ForumTopicID", forumTopic.ForumTopicID));
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
				if (OnForumTopicDeleted != null)
					OnForumTopicDeleted(forumTopic);
			}
			return result;
		}

		public ForumTopic SelectForumTopic(long id)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumTopic_Select", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumTopicID", id));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ForumTopic entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new ForumTopic(reader);
					reader.Close();
					return entity;
				}
			}
		}

		#endregion
		#region Members for ForumTopicMessage

		public Result Store(ForumTopicMessage forumTopicMessage)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("ForumTopicMessage_Store", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlParameter prm = new SqlParameter("@ForumTopicMessageID", forumTopicMessage.ForumTopicMessageID);
					prm.Direction = ParameterDirection.InputOutput;
					cmd.Parameters.Add(prm);
					cmd.Parameters.Add(NewSqlParameter("@ForumTopicID", forumTopicMessage.ForumTopicID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@AuthorUserID", forumTopicMessage.AuthorUserID, SqlDbType.BigInt));
					cmd.Parameters.Add(NewSqlParameter("@AuthorName", forumTopicMessage.AuthorName, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@DateCreated", forumTopicMessage.DateCreated, SqlDbType.DateTime));
					cmd.Parameters.Add(NewSqlParameter("@BodySource", forumTopicMessage.BodySource, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@BodyOutput", forumTopicMessage.BodyOutput, SqlDbType.NVarChar));
					cmd.Parameters.Add(NewSqlParameter("@ModerationState", forumTopicMessage.ModerationState, SqlDbType.SmallInt));
					cmd.Parameters.Add(NewSqlParameter("@MarkupType", forumTopicMessage.MarkupType, SqlDbType.SmallInt));
					cmd.ExecuteNonQuery();
					forumTopicMessage.ForumTopicMessageID = (long)prm.Value;
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

		public event InterruptableEventHandler<ForumTopicMessage> OnBeforeDeleteForumTopicMessage;
		public event NotificationEventHandler<ForumTopicMessage> OnForumTopicMessageDeleted;
		public Result Delete(ForumTopicMessage forumTopicMessage)
		{
			Result result = new Result();
			if (OnBeforeDeleteForumTopicMessage != null)
				OnBeforeDeleteForumTopicMessage(forumTopicMessage, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						SqlCommand cmd = new SqlCommand("ForumTopicMessage_Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ForumTopicMessageID", forumTopicMessage.ForumTopicMessageID));
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
				if (OnForumTopicMessageDeleted != null)
					OnForumTopicMessageDeleted(forumTopicMessage);
			}
			return result;
		}

		public ForumTopicMessage SelectForumTopicMessage(long id)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumTopicMessage_Select", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumTopicMessageID", id));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ForumTopicMessage entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new ForumTopicMessage(reader);
					reader.Close();
					return entity;
				}
			}
		}

		#endregion

		public Forum SelectForumByCode(string forumCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("Forum_SelectByCode", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumCode", forumCode));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					Forum entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new Forum(reader);
					reader.Close();
					return entity;
				}
			}
		}

		public Forum SelectForumByURLToken(string urlToken)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("Forum_SelectByURLToken", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@URLToken", urlToken));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					Forum entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new Forum(reader);
					reader.Close();
					return entity;
				}
			}
		}

		public ForumCategory SelectForumCategoryByCode(string categoryCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumCategory_SelectByCode", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@CategoryCode", categoryCode));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ForumCategory entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new ForumCategory(reader);
					reader.Close();
					return entity;
				}
			}
		}

		public ForumCategory SelectForumCategoryByURLToken(string urlToken)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumCategory_SelectByURLToken", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@URLToken", urlToken));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ForumCategory entity;
					if (!reader.Read())
						entity = null;
					else
						entity = new ForumCategory(reader);
					reader.Close();
					return entity;
				}
			}
		}

		public List<Forum> ListForums(long forumCategoryID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumCategory_ListForums", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ForumCategoryID", forumCategoryID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<Forum> list = new List<Forum>();
					while (reader.Read())
						list.Add(new Forum(reader));
					reader.Close();
					return list;
				}
			}
		}

		public List<ForumSummary> ListForumSummary(string categoryCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ForumCategory_ListForumSummary", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@CategoryCode", categoryCode));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<ForumSummary> list = new List<ForumSummary>();
					while (reader.Read())
						list.Add(new ForumSummary(reader));
					reader.Close();
					return list;
				}
			}
		}
	}
}

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassGenerator
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			try
			{
				LoadDatabases();
			}
			catch
			{
			}
		}

		void LoadDatabases()
		{
			SQLInfoEnumerator s = new SQLInfoEnumerator();
			s.Password = Password.Text;
			s.Username = Username.Text;
			s.SQLServer = Server.Text;
			DatabaseList.Items.Clear();
			foreach (string db in s.EnumerateSQLServersDatabases())
				DatabaseList.Items.Add(db);
		}

		string ConnectionString
		{
			get
			{
				return string.Format("Server={0};UID={1};PWD={2};Initial Catalog={3};",
					Server.Text, Username.Text, Password.Text, DatabaseList.SelectedItem);
			}
		}

		private void DatabaseList_SelectedIndexChanged(object sender, EventArgs e)
		{
			SqlConnection conn;
			try
			{
				conn = new SqlConnection(ConnectionString);
				conn.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Connection String Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = "select name from sysobjects where type='U' order by name";
			cmd.CommandType = CommandType.Text;
			TablesList.Items.Clear();
			SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			while (dr.Read())
				TablesList.Items.Add(dr[0]);
			dr.Close();
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			try
			{
				LoadDatabases();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}

		private string className = "";
		private void TablesList_SelectedValueChanged(object sender, EventArgs e)
		{
			string cs, sql, csoutline, csinterface, csdatalayer, csmain, csexpr, csexpr2;
			GenerateCode(TablesList.SelectedItem.ToString(), out cs, out sql, out csmain, out csoutline, out csexpr, out csexpr2, out csdatalayer, out csinterface);
			Output.Text = cs;
			Procedures.Text = sql;
			DataLayerInterface.Text = csinterface;
			DataLayerMethods.Text = csdatalayer;
			ClassMain.Text = csmain;
			ClassOutline.Text = csoutline;
			exprTextBox.Text = csexpr;
			exprExtender.Text = csexpr2;
		}

		void GenerateCode(string tableName, out string cs, out string sql, out string csmain, out string csoutline, out string csexpr,
			out string csexpr_extender, out string csdatalayer, out string csinterface)
		{
			SaveButton.Enabled = true;
			ExecuteSQLButton.Enabled = true;
			SaveSQLFileButton.Enabled = true;
			SqlConnection conn;
			try
			{
				conn = new SqlConnection(ConnectionString);
				conn.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Connection String Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cs = ""; sql = ""; csmain = ""; csdatalayer = ""; csoutline = ""; csinterface = ""; csexpr = ""; csexpr_extender = "";
				return;
			}
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = "select top 0 * from " + tableName;
			cmd.CommandType = CommandType.Text;
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();
			da.FillSchema(ds, SchemaType.Mapped);
			da.Fill(ds);
			conn.Close();

			string sField = ResourceLoader.LoadTextResource("field.txt");
			string sProperty = ResourceLoader.LoadTextResource("property.txt");
			string sPropertyReadOnly = ResourceLoader.LoadTextResource("property-readonly.txt");
			string sClass = ResourceLoader.LoadTextResource("class.txt");
			string sExpressionClass = ResourceLoader.LoadTextResource("expressionclass.txt");
			string sExpressionClassExtended = ResourceLoader.LoadTextResource("expression_classextender.txt");
			string sRow = ResourceLoader.LoadTextResource("fromrow.txt");
			string sProcs = ResourceLoader.LoadTextResource("procs.txt");
			string sKeyVal = ResourceLoader.LoadTextResource("keyvalmethods.txt");
			string sJSON = ResourceLoader.LoadTextResource("jsonmethods.txt");

			string sClassDataLayer = ResourceLoader.LoadTextResource("class_datalayer.txt");
			string sClassMain = ResourceLoader.LoadTextResource("class_main.txt");
			string sClassOutline = ResourceLoader.LoadTextResource("class_outline.txt");
			string sClassInterface = ResourceLoader.LoadTextResource("datalayer_interface.txt");

			className = tableName.ToString();
			string classNameLower = className.ToLower();
			//if (className.ToLower().EndsWith("s"))
			//    className = className.Substring(0, className.Length - 1);
			string lesserClassName = className.Substring(0, 1).ToLower() + className.Substring(1, className.Length - 1);
			string primaryKey = ds.Tables[0].PrimaryKey[0].ColumnName;
			string classIDField = primaryKey.Substring(0, 1).ToLower() + primaryKey.Substring(1, primaryKey.Length - 1);
			string classIDFieldType = GetTypeName(ds.Tables[0].PrimaryKey[0], false);

			string classFields = "", classProperties = "", classconstructorparams = "", constructorFieldsFromParams = "", constructorFieldAssignsFromDataRow = "";
			string insertProcParams = "", updateProcParams = "", updateSqlSets = "", updateExplicitSqlSets = "";
			string sqlInsertFieldsListByCommas = "", sqlInsertValuesListByCommas = "", prms = "", filterCommandParams = "", cases = "";
			string filterProcSelectConditions = "", filterProcParams = "", filterMethodParams = "", filterMethodParamsValsOnly = "";
			string updateExplicitProcParams = "", fieldNameCaseStatements = "", enumFieldNames = "", filterOrderByClause = "";
			string jsonWrites = "", jsonReads = "", fieldCopies = ""; ;
			string exprPrepareProperty_CaseStatements = "", exprEvaluateProperty_CaseStatements = "", exprEvaluate_CaseStatements = "", exprPropertyTypes = "";
			string exprEvaluateProperty_CaseStatements2 = "", exprEvaluate_CaseStatements2 = "";
			int n = 0;
			foreach (DataColumn c in ds.Tables[0].Columns)
			{
				bool columnIsID = object.ReferenceEquals(ds.Tables[0].PrimaryKey[0], c);
				bool isIdentity = c.AutoIncrement;
				string fldtype = GetTypeName(c, false);
				string defVal = GetDefaultValue(c);
				string fldName = c.ColumnName.Substring(0, 1).ToLower() + c.ColumnName.Substring(1);
				string prName = c.ColumnName.Substring(0, 1).ToUpper() + c.ColumnName.Substring(1);
				fldName = fldName.Replace("#", "amount");
				prName = prName.Replace("#", "amount");

				if (cases.Length > 0) cases += Environment.NewLine;
				cases += "\t\t\t\tcase \"" + c.ColumnName + "\": "; //fldName + " = " + StringToNativeField(;
				if (c.DataType.Name == "String")
					cases += fldName + " = value; return true;";
				else
				{
					cases += "if(value.Length > 0) " + fldName + " = ";
					switch (c.DataType.Name)
					{
						case "Boolean": cases += "Boolean.Parse(value);"; break;
						case "Byte": cases += "Byte.Parse(value);"; break;
						case "Char": cases += "Char.Parse(value);"; break;
						case "DateTime": cases += "ParseDateTime(value);"; break;
						case "Decimal": cases += "Decimal.Parse(value);"; break;
						case "Double": cases += "Double.Parse(value);"; break;
						case "Float": cases += "Float.Parse(value);"; break;
						case "Guid": cases += "new Guid(value);"; break;
						case "String": cases += "String.Parse(value);"; break;
						case "Int16": cases += "Int16.Parse(value);"; break;
						case "Int32": cases += "Int32.Parse(value);"; break;
						case "Int64": cases += "Int64.Parse(value);"; break;
						case "UInt16": cases += "UInt16.Parse(value);"; break;
						case "UInt32": cases += "UInt32.Parse(value);"; break;
						case "UInt64": cases += "UInt64.Parse(value);"; break;
						default: break;
					}
					cases += " return true;";
				}

				if (jsonWrites.Length > 0)
					jsonWrites += Environment.NewLine + "\t\t\twriter.Write(\",\");" + Environment.NewLine + "\t\t\t";
				jsonWrites += "JSON.EncodeNameValuePair(writer, \"" + prName + "\"," + fldName + ");";

				if (jsonReads.Length > 0)
					jsonReads += Environment.NewLine + "\t\t\t";
				jsonReads += fldName + " = (" + fldtype + ")values[\"" + prName + "\"];";

				if (enumFieldNames.Length > 0)
					enumFieldNames += "," + Environment.NewLine + "\t\t\t";
				enumFieldNames += prName + " = " + n;

				if (fieldCopies.Length > 0)
					fieldCopies += Environment.NewLine;
				fieldCopies += "\t\t\tcopy." + fldName + " = " + fldName + ";";

				if (fieldNameCaseStatements.Length > 0)
					fieldNameCaseStatements += Environment.NewLine + "\t\t\t\t\t";
				fieldNameCaseStatements += "case Field." + prName + ": return \"" + prName + "\";";

				if (c.DataType.Name != "Guid")
				{
					if (filterOrderByClause.Length > 0)
						filterOrderByClause += Environment.NewLine + "\t\t\t";
					if(c.DataType.Name == "DateTime")
						filterOrderByClause += "WHEN '" + prName + "' THEN Convert(varchar,[" + c.ColumnName + "],121)";
					else if (c.DataType.Name == "Boolean")
						filterOrderByClause += "WHEN '" + prName + "' THEN Convert(varchar,[" + c.ColumnName + "])";
					else if (c.DataType.Name.StartsWith("Int") || c.DataType.Name.StartsWith("UInt"))
						filterOrderByClause += "WHEN '" + prName + "' THEN Convert(varchar,[" + c.ColumnName + "])";
					else
						filterOrderByClause += "WHEN '" + prName + "' THEN [" + c.ColumnName + "]";
				}

				if (!isIdentity && !columnIsID)
				{
					if (prms.Length > 0) prms += Environment.NewLine + "\t\t\t\t\t";
					prms += "cmd.Parameters.Add(NewSqlParameter(\"@" + c.ColumnName + "\", " + lesserClassName + "." + c.ColumnName + ", SqlDbType." + SqlDbTypeFromType(c.DataType).ToString() + "));";
				}

				if (filterCommandParams.Length > 0) filterCommandParams += Environment.NewLine + "\t\t\t";
				if (c.DataType.Name == "DateTime")
				{
					filterCommandParams += "Database.Main.AddParameter(cmd, \"@" + c.ColumnName + "_Min\", " + fldName + "Min, DbType." + SqlDbTypeFromType(c.DataType).ToString() + ");";
					filterCommandParams += Environment.NewLine + "\t\t\t";
					filterCommandParams += "Database.Main.AddParameter(cmd, \"@" + c.ColumnName + "_Max\", " + fldName + "Max, DbType." + SqlDbTypeFromType(c.DataType).ToString() + ");";
				}
				else
					filterCommandParams += "Database.Main.AddParameter(cmd, \"@" + c.ColumnName + "\", " + fldName + ", DbType." + SqlDbTypeFromType(c.DataType).ToString() + ");";

				if (!isIdentity)
				{
					if (insertProcParams.Length > 0) insertProcParams += "," + Environment.NewLine + "\t";
					insertProcParams += "@" + c.ColumnName + " " + GetSqlDataType(c);
					if (!columnIsID && c.AllowDBNull)
						insertProcParams += " = null";
					else if (columnIsID)
						insertProcParams += " OUTPUT";
				}

				if (!isIdentity)
				{
					if (updateProcParams.Length > 0) updateProcParams += "," + Environment.NewLine + "\t";
					updateProcParams += "@" + c.ColumnName + " " + GetSqlDataType(c);
					if (!columnIsID) updateProcParams += " = null";
				}

				if (!isIdentity)
				{
					if (updateExplicitProcParams.Length > 0) updateExplicitProcParams += "," + Environment.NewLine + "\t\t";
					updateExplicitProcParams += "@" + c.ColumnName + " " + GetSqlDataType(c);
				}

				if (filterProcParams.Length > 0) filterProcParams += "," + Environment.NewLine + "\t";
				if (c.DataType.Name == "DateTime")
				{
					filterProcParams += "@" + c.ColumnName + "_Min " + GetSqlDataType(c) + " = null," + Environment.NewLine + "\t";
					filterProcParams += "@" + c.ColumnName + "_Max " + GetSqlDataType(c) + " = null";
				}
				else
					filterProcParams += "@" + c.ColumnName + " " + GetSqlDataType(c) + " = null";

				if (!columnIsID && !isIdentity)
				{
					if (updateSqlSets.Length > 0)
						updateSqlSets += "," + Environment.NewLine + "\t\t";
					updateSqlSets += c.ColumnName + " = COALESCE(@" + c.ColumnName + ", " + c.ColumnName + ")";
				}

				if (!columnIsID && !isIdentity)
				{
					if (updateExplicitSqlSets.Length > 0)
						updateExplicitSqlSets += "," + Environment.NewLine + "\t\t\t";
					updateExplicitSqlSets += c.ColumnName + " = @" + c.ColumnName;
				}

				if (filterProcSelectConditions.Length > 0) filterProcSelectConditions += "\r\n\t\t   AND ";
				if (c.DataType.Name == "DateTime")
					filterProcSelectConditions +=
						"((@" + c.ColumnName + "_Min IS NULL AND @" + c.ColumnName + "_Max IS NULL) OR\r\n\t\t\t" +
						"(@" + c.ColumnName + "_Min IS NULL AND @" + c.ColumnName + "_Max IS NOT NULL AND @" + c.ColumnName + "_Max >= " + c.ColumnName + ") OR\r\n\t\t\t" +
						"(@" + c.ColumnName + "_Max IS NULL AND @" + c.ColumnName + "_Min IS NOT NULL AND @" + c.ColumnName + "_Min <= " + c.ColumnName + ") OR\r\n\t\t\t" +
						"(@" + c.ColumnName + "_Max IS NOT NULL AND @" + c.ColumnName + "_Min IS NOT NULL AND " + c.ColumnName + " BETWEEN @" + c.ColumnName + "_Min AND @" + c.ColumnName + "_Max))";
				else
					filterProcSelectConditions += "(@" + c.ColumnName + " IS NULL OR " + c.ColumnName + " = @" + c.ColumnName + ")";

				//input parameters for filter procedure
				if (filterMethodParams.Length > 0) filterMethodParams += ", ";
				if (c.DataType.Name == "DateTime")
				{
					filterMethodParams += GetTypeName(c, true) + " " + fldName + "Min, ";
					filterMethodParams += GetTypeName(c, true) + " " + fldName + "Max";
				}
				else
					filterMethodParams += GetTypeName(c, true) + " " + fldName;

				if (filterMethodParamsValsOnly.Length > 0) filterMethodParamsValsOnly += ", ";
				if (c.DataType.Name == "DateTime")
				{
					filterMethodParamsValsOnly += fldName + "Min, ";
					filterMethodParamsValsOnly += fldName + "Max";
				}
				else
					filterMethodParamsValsOnly += fldName;

				//full list of table fields separated by commas
				if (!isIdentity)
				{
					if (sqlInsertFieldsListByCommas.Length > 0) sqlInsertFieldsListByCommas += ", ";
					sqlInsertFieldsListByCommas += c.ColumnName;
				}

				//full list of parameters for table fields separated by commas
				if (!isIdentity)
				{
					if (sqlInsertValuesListByCommas.Length > 0) sqlInsertValuesListByCommas += ", ";
					sqlInsertValuesListByCommas += "@" + c.ColumnName;
				}

				if (classFields.Length > 0) classFields += Environment.NewLine + "\t\t";
				classFields += sField
					.Replace("[datatype]", fldtype)
					.Replace("[defaultvalue]", defVal)
					.Replace("[fieldname]", fldName)
					;

				if (classProperties.Length > 0) classProperties += Environment.NewLine + Environment.NewLine;
				classProperties += (isIdentity ? sPropertyReadOnly : sProperty)
					.Replace("[datatype]", fldtype)
					.Replace("[propertyname]", prName)
					.Replace("[fieldname]", fldName)
					;

				if (!isIdentity)
				{
					if (classconstructorparams.Length > 0) classconstructorparams += ", ";
					classconstructorparams += fldtype + " " + fldName;
				}

				if (!isIdentity)
				{
					if (constructorFieldsFromParams.Length > 0) constructorFieldsFromParams += Environment.NewLine + "\t\t\t";
					constructorFieldsFromParams += "this." + fldName + " = " + fldName + ";";
				}

				if (constructorFieldAssignsFromDataRow.Length > 0) constructorFieldAssignsFromDataRow += Environment.NewLine + "\t\t\t";
				constructorFieldAssignsFromDataRow += sRow
					.Replace("[columnname]", c.ColumnName)
					.Replace("[fldname]", fldName)
					.Replace("[fldtype]", fldtype)
					;

				if (exprPropertyTypes.Length > 0)
				{
					exprEvaluate_CaseStatements += Environment.NewLine + "\t\t\t\t";
					exprEvaluateProperty_CaseStatements += Environment.NewLine + "\t\t\t\t";
					exprPrepareProperty_CaseStatements += Environment.NewLine + "\t\t\t\t";
					exprEvaluate_CaseStatements2 += Environment.NewLine + "\t\t\t\t";
					exprEvaluateProperty_CaseStatements2 += Environment.NewLine + "\t\t\t\t";
				}
				exprPropertyTypes += "," + Environment.NewLine + "\t\t\t" + prName;
				exprEvaluate_CaseStatements += string.Format("case PropertyType.{0}: return {1}.{0};", prName, lesserClassName);
				exprEvaluateProperty_CaseStatements += string.Format("case \"{0}\": return {2}.{1};", prName.ToLower(), prName, lesserClassName);
				exprPrepareProperty_CaseStatements += string.Format("case \"{0}\": propertyType = PropertyType.{1}; return true;", prName.ToLower(), prName);
				exprEvaluate_CaseStatements2 += string.Format("case PropertyType.{0}: return {0};", prName);
				exprEvaluateProperty_CaseStatements2 += string.Format("case \"{0}\": return {1};", prName.ToLower(), prName);

				n++;
			}

			string keyvalmethods = "";
			if (AllowKeyValueMethodGeneration.Checked)
				keyvalmethods = sKeyVal.Replace("[case-statements]", cases);

			string jsonMethods = "", ijsonencoder = "";
			if (JsonEncodable.Checked)
			{
				jsonMethods = sJSON
					.Replace("[json-writes]", jsonWrites)
					.Replace("[json-reads]", jsonReads);
				ijsonencoder = " : IJSONEncoder, IJSONReader";
			}

			cs = sClass
				.Replace("[classname]", className)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[classidfield]", classIDField)
				.Replace("[classidfieldtype]", classIDFieldType)
				.Replace("[namespace]", Namespace.Text)
				.Replace("[fields]", classFields)
				.Replace("[commandparameters]", prms)
				.Replace("[filtercommandparameters]", filterCommandParams)
				.Replace("[properties]", classProperties)
				.Replace("[fieldlistparams]", classconstructorparams)
				.Replace("[fieldparamassignments]", constructorFieldsFromParams)
				.Replace("[rowassigns]", constructorFieldAssignsFromDataRow)
				.Replace("[keyvalmethods]", keyvalmethods)
				.Replace("[jsonmethods]", jsonMethods)
				.Replace("[:IJSONEncoder]", ijsonencoder)
				.Replace("[primarykey]", primaryKey)
				.Replace("[filterparams]", filterMethodParams)
				.Replace("[filterparamsvalsonly]", filterMethodParamsValsOnly)
				.Replace("[fieldnamecases]", fieldNameCaseStatements)
				.Replace("[enumfieldnames]", enumFieldNames)
				.Replace("[fieldcopies]", fieldCopies)
				;

			sql = sProcs
				.Replace("[primarykeytype]", GetSqlDataType(ds.Tables[0].PrimaryKey[0]))
				.Replace("[primarykey]", primaryKey)
				.Replace("[classname]", className)
				.Replace("[tablename]", tableName)
				.Replace("[fieldnames]", sqlInsertFieldsListByCommas)
				.Replace("[fieldvalues]", sqlInsertValuesListByCommas)
				.Replace("[insertparameters]", insertProcParams)
				.Replace("[updateparameters]", updateProcParams)
				.Replace("[filterparameters]", filterProcParams)
				.Replace("[updateexplicitparameters]", updateExplicitProcParams)
				.Replace("[updatesets]", updateSqlSets)
				.Replace("[updateexplicitsets]", updateExplicitSqlSets)
				.Replace("[filterconditions]", filterProcSelectConditions)
				.Replace("[filterorderbyclause]", filterOrderByClause)
				;

			csmain = sClassMain
				.Replace("[classname]", className)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[classidfield]", classIDField)
				.Replace("[classidfieldtype]", classIDFieldType)
				.Replace("[namespace]", Namespace.Text)
				.Replace("[fields]", classFields)
				.Replace("[commandparameters]", prms)
				.Replace("[filtercommandparameters]", filterCommandParams)
				.Replace("[properties]", classProperties)
				.Replace("[fieldlistparams]", classconstructorparams)
				.Replace("[fieldparamassignments]", constructorFieldsFromParams)
				.Replace("[rowassigns]", constructorFieldAssignsFromDataRow)
				.Replace("[keyvalmethods]", keyvalmethods)
				.Replace("[jsonmethods]", jsonMethods)
				.Replace("[:IJSONEncoder]", ijsonencoder)
				.Replace("[primarykey]", primaryKey)
				.Replace("[filterparams]", filterMethodParams)
				.Replace("[filterparamsvalsonly]", filterMethodParamsValsOnly)
				.Replace("[fieldnamecases]", fieldNameCaseStatements)
				.Replace("[enumfieldnames]", enumFieldNames)
				.Replace("[fieldcopies]", fieldCopies)
				;

			csdatalayer = sClassDataLayer
				.Replace("[classname]", className)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[classidfield]", classIDField)
				.Replace("[classidfieldtype]", classIDFieldType)
				.Replace("[namespace]", Namespace.Text)
				.Replace("[fields]", classFields)
				.Replace("[commandparameters]", prms)
				.Replace("[filtercommandparameters]", filterCommandParams)
				.Replace("[properties]", classProperties)
				.Replace("[fieldlistparams]", classconstructorparams)
				.Replace("[fieldparamassignments]", constructorFieldsFromParams)
				.Replace("[rowassigns]", constructorFieldAssignsFromDataRow)
				.Replace("[keyvalmethods]", keyvalmethods)
				.Replace("[jsonmethods]", jsonMethods)
				.Replace("[:IJSONEncoder]", ijsonencoder)
				.Replace("[primarykey]", primaryKey)
				.Replace("[filterparams]", filterMethodParams)
				.Replace("[filterparamsvalsonly]", filterMethodParamsValsOnly)
				.Replace("[fieldnamecases]", fieldNameCaseStatements)
				.Replace("[enumfieldnames]", enumFieldNames)
				;

			csinterface = sClassInterface
				.Replace("[classname]", className)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[classidfield]", classIDField)
				.Replace("[classidfieldtype]", classIDFieldType)
				.Replace("[namespace]", Namespace.Text)
				.Replace("[fields]", classFields)
				.Replace("[commandparameters]", prms)
				.Replace("[filtercommandparameters]", filterCommandParams)
				.Replace("[properties]", classProperties)
				.Replace("[fieldlistparams]", classconstructorparams)
				.Replace("[fieldparamassignments]", constructorFieldsFromParams)
				.Replace("[rowassigns]", constructorFieldAssignsFromDataRow)
				.Replace("[keyvalmethods]", keyvalmethods)
				.Replace("[jsonmethods]", jsonMethods)
				.Replace("[:IJSONEncoder]", ijsonencoder)
				.Replace("[primarykey]", primaryKey)
				.Replace("[filterparams]", filterMethodParams)
				.Replace("[filterparamsvalsonly]", filterMethodParamsValsOnly)
				.Replace("[fieldnamecases]", fieldNameCaseStatements)
				.Replace("[enumfieldnames]", enumFieldNames)
				;

			csoutline = sClassOutline
				.Replace("[classname]", className)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[classidfield]", classIDField)
				.Replace("[classidfieldtype]", classIDFieldType)
				.Replace("[namespace]", Namespace.Text)
				.Replace("[fields]", classFields)
				.Replace("[commandparameters]", prms)
				.Replace("[filtercommandparameters]", filterCommandParams)
				.Replace("[properties]", classProperties)
				.Replace("[fieldlistparams]", classconstructorparams)
				.Replace("[fieldparamassignments]", constructorFieldsFromParams)
				.Replace("[rowassigns]", constructorFieldAssignsFromDataRow)
				.Replace("[keyvalmethods]", keyvalmethods)
				.Replace("[jsonmethods]", jsonMethods)
				.Replace("[:IJSONEncoder]", ijsonencoder)
				.Replace("[primarykey]", primaryKey)
				.Replace("[filterparams]", filterMethodParams)
				.Replace("[filterparamsvalsonly]", filterMethodParamsValsOnly)
				.Replace("[fieldnamecases]", fieldNameCaseStatements)
				.Replace("[enumfieldnames]", enumFieldNames)
				.Replace("[fieldcopies]", fieldCopies)
				;

			csexpr = sExpressionClass
				.Replace("[namespace]", Namespace.Text)
				.Replace("[classname]", className)
				.Replace("[classname_lower]", classNameLower)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[propertytypes]", exprPropertyTypes)
				.Replace("[evaluateproperty_casestatements]", exprEvaluateProperty_CaseStatements)
				.Replace("[prepareproperty_casestatements]", exprPrepareProperty_CaseStatements)
				.Replace("[evaluate_casestatements]", exprEvaluate_CaseStatements)
				;

			csexpr_extender = sExpressionClassExtended
				.Replace("[namespace]", Namespace.Text)
				.Replace("[classname]", className)
				.Replace("[classname_lower]", classNameLower)
				.Replace("[lesserclassname]", lesserClassName)
				.Replace("[propertytypes]", exprPropertyTypes)
				.Replace("[evaluateproperty_casestatements]", exprEvaluateProperty_CaseStatements2)
				.Replace("[prepareproperty_casestatements]", exprPrepareProperty_CaseStatements)
				.Replace("[evaluate_casestatements]", exprEvaluate_CaseStatements2)
				;
		}

		private string GetDefaultValue(DataColumn c)
		{
			string t;
			switch (c.DataType.Name)
			{
				case "Boolean":
					t = c.AllowDBNull ? "null" : c.DefaultValue == null || c.DefaultValue == DBNull.Value ? "false" : ((bool)c.DefaultValue).ToString().ToLower();
					break;

				case "String":
					t = c.AllowDBNull ? "null" : "\"\"";
					break;

				case "Guid":
					t = c.AllowDBNull ? "null" : "Guid.NewGuid()";
					break;

				case "Int16":
				case "Int32":
				case "Int64":
				case "Single":
				case "Decimal":
				case "Double":
					t = c.AllowDBNull ? "null" : "0";
					break;

				case "DateTime":
					t = c.AllowDBNull ? "null" : "DateTime.MinValue";
					break;

				default:
					t = "null";
					break;
			}

			return t;
		}

		private string GetTypeName(DataColumn c, bool forceNullable)
		{
			string t;
			switch (c.DataType.Name)
			{
				case "Guid":
					t = "Guid";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Boolean":
					t = "bool";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Int16":
					t = "short";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Int32":
					t = "int";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Int64":
					t = "long";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "String":
					t = "string";
					break;

				case "Single":
					t = "float";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Decimal":
					t = "decimal";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "Double":
					t = "double";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				case "DateTime":
					t = "DateTime";
					if (c.AllowDBNull || forceNullable) t += "?";
					break;

				default:
					t = c.DataType.Name;
					break;
			}

			return t;
		}

		private string GetSqlDataType(DataColumn c)
		{
			Type t = c.DataType;
			string name = t.IsGenericType ? Nullable.GetUnderlyingType(t).Name : t.Name;
			switch (name)
			{
				case "Guid":
					return "uniqueidentifier";
				case "String":
					if (c.MaxLength == -1)
						return "ntext";
					else
						return "nvarchar(" + (c.MaxLength > 8000 ? "max" : c.MaxLength.ToString()) + ")";
				case "Boolean":
					return "bit";
				case "Int16":
					return "smallint";
				case "Int32":
					return "int";
				case "Int64":
					return "bigint";
				case "Decimal":
					return "money";
				case "Single":
					return "real";
				case "Double":
					return "float";
				case "DateTime":
					return "datetime";
				default:
					return t.Name;
			}
		}

		private SqlDbType SqlDbTypeFromType(Type t)
		{
			switch (t.Name)
			{
				case "Boolean": return SqlDbType.Bit;
				case "Byte": return SqlDbType.VarBinary;
				case "Byte[]": return SqlDbType.VarBinary;
				case "Char": return SqlDbType.Char;
				case "DateTime": return SqlDbType.DateTime;
				case "Decimal": return SqlDbType.Decimal;
				case "Double": return SqlDbType.Float;
				case "Float": return SqlDbType.Real;
				case "Guid": return SqlDbType.UniqueIdentifier;
				case "Object": return SqlDbType.VarBinary;
				case "String": return SqlDbType.NVarChar;
				case "TimeSpan": return SqlDbType.DateTime;
				case "Int16": return SqlDbType.SmallInt;
				case "Int32": return SqlDbType.Int;
				case "Int64": return SqlDbType.BigInt;
				case "UInt16": return SqlDbType.SmallInt;
				case "UInt32": return SqlDbType.Int;
				case "UInt64": return SqlDbType.BigInt;
				default: return SqlDbType.NVarChar;
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = className + ".Entity.cs";
			dlg.SupportMultiDottedExtensions = true;
			dlg.DefaultExt = ".cs";
			dlg.AddExtension = true;
			dlg.Filter = "C# Code Files (*.cs)|*.cs|All Files (*.*)|*.*";
			dlg.OverwritePrompt = true;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(dlg.FileName, false);
				sw.Write(Output.Text);
				sw.Flush();
				sw.Close();
			}
		}

		private void SaveSQLFileButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = className + ".Entity.Procedures.sql";
			dlg.SupportMultiDottedExtensions = true;
			dlg.DefaultExt = ".sql";
			dlg.AddExtension = true;
			dlg.Filter = "SQL Script Files (*.sql)|*.sql|All Files (*.*)|*.*";
			dlg.OverwritePrompt = true;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(dlg.FileName, false);
				sw.Write(Procedures.Text);
				sw.Flush();
				sw.Close();
			}
		}

		private void ExecuteSQLButton_Click(object sender, EventArgs e)
		{
			ExecuteSQL(Procedures.Text);
			ExecuteSQLButton.Enabled = false;
		}

		void ExecuteSQL(string sqltext)
		{
			SqlConnection conn;
			try
			{
				conn = new SqlConnection(ConnectionString);
				conn.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Connection String Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			string[] sqlarr = Regex.Split(sqltext, @"\sgo\s", RegexOptions.IgnoreCase);
			for (int i=0; i<sqlarr.Length; i++)
			{
				if (sqlarr[i] == "")
					continue;
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandText = sqlarr[i];
				cmd.CommandType = CommandType.Text;
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			conn.Close();
		}

		private void ExecuteAllSQLButton_Click(object sender, EventArgs e)
		{
			foreach (string name in TablesList.CheckedItems)
			{
				string cs, sql, cs1, cs2, cs3, cs4, cs5, cs6;
				GenerateCode(name, out cs, out sql, out cs1, out cs2, out cs5, out cs3, out cs4, out cs6);
				ExecuteSQL(sql);
			}
		}

		private void SaveCSFilesButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select the location in which the generated C# files will be saved.";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				foreach (string name in TablesList.CheckedItems)
				{
					string cname = name;
					if (cname.ToLower().EndsWith("s"))
						cname = cname.Substring(0, cname.Length - 1);
					string cs, sql, cs1, cs2, cs3, cs4, cs5, cs6;
					GenerateCode(name, out cs, out sql, out cs1, out cs2, out cs3, out cs4, out cs5, out cs6);
					StreamWriter writer = new StreamWriter(dlg.SelectedPath + "\\" + cname + ".Entity.cs");
					writer.Write(cs);
					writer.Flush();
					writer.Close();
				}
			}
		}

		private void SaveSQLFilesButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = "Generated.Entity.Procedures.sql";
			dlg.SupportMultiDottedExtensions = true;
			dlg.DefaultExt = ".sql";
			dlg.AddExtension = true;
			dlg.Filter = "SQL Script Files (*.sql)|*.sql|All Files (*.*)|*.*";
			dlg.OverwritePrompt = true;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string allsql = "";
				foreach (string name in TablesList.CheckedItems)
				{
					string cname = name;
					if (cname.ToLower().EndsWith("s"))
						cname = cname.Substring(0, cname.Length - 1);
					string cs, sql, cs1, cs2, cs3, cs4, cs5, cs6;
					GenerateCode(name, out cs, out sql, out cs1, out cs2, out cs3, out cs4, out cs5, out cs6);
					allsql += sql;
				}
				StreamWriter writer = new StreamWriter(dlg.FileName);
				writer.Write(allsql);
				writer.Flush();
				writer.Close();
			}
		}
	}
}
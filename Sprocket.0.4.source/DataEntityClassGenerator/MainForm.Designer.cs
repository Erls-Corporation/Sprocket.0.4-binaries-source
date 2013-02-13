namespace ClassGenerator
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.JsonEncodable = new System.Windows.Forms.CheckBox();
			this.AllowKeyValueMethodGeneration = new System.Windows.Forms.CheckBox();
			this.LoadButton = new System.Windows.Forms.Button();
			this.Namespace = new System.Windows.Forms.TextBox();
			this.Server = new System.Windows.Forms.TextBox();
			this.Password = new System.Windows.Forms.TextBox();
			this.Username = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.DatabaseList = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.SaveCSFilesButton = new System.Windows.Forms.Button();
			this.SaveSQLFilesButton = new System.Windows.Forms.Button();
			this.ExecuteAllSQLButton = new System.Windows.Forms.Button();
			this.TablesList = new System.Windows.Forms.CheckedListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.Output = new System.Windows.Forms.RichTextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SaveButton = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.ExecuteSQLButton = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.SaveSQLFileButton = new System.Windows.Forms.Button();
			this.Procedures = new System.Windows.Forms.RichTextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.DataLayerInterface = new System.Windows.Forms.RichTextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.DataLayerMethods = new System.Windows.Forms.RichTextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.ClassMain = new System.Windows.Forms.RichTextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.ClassOutline = new System.Windows.Forms.RichTextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tabPage7 = new System.Windows.Forms.TabPage();
			this.exprTextBox = new System.Windows.Forms.RichTextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.exprExtender = new System.Windows.Forms.RichTextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabPage7.SuspendLayout();
			this.tabPage8.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.JsonEncodable);
			this.splitContainer1.Panel1.Controls.Add(this.AllowKeyValueMethodGeneration);
			this.splitContainer1.Panel1.Controls.Add(this.LoadButton);
			this.splitContainer1.Panel1.Controls.Add(this.Namespace);
			this.splitContainer1.Panel1.Controls.Add(this.Server);
			this.splitContainer1.Panel1.Controls.Add(this.Password);
			this.splitContainer1.Panel1.Controls.Add(this.Username);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.label4);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(1002, 649);
			this.splitContainer1.SplitterDistance = 101;
			this.splitContainer1.TabIndex = 0;
			// 
			// JsonEncodable
			// 
			this.JsonEncodable.AutoSize = true;
			this.JsonEncodable.Checked = true;
			this.JsonEncodable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.JsonEncodable.Location = new System.Drawing.Point(113, 79);
			this.JsonEncodable.Name = "JsonEncodable";
			this.JsonEncodable.Size = new System.Drawing.Size(284, 17);
			this.JsonEncodable.TabIndex = 7;
			this.JsonEncodable.Text = "Generate methods to make the entity JSON encodable";
			this.JsonEncodable.UseVisualStyleBackColor = true;
			// 
			// AllowKeyValueMethodGeneration
			// 
			this.AllowKeyValueMethodGeneration.AutoSize = true;
			this.AllowKeyValueMethodGeneration.Location = new System.Drawing.Point(113, 58);
			this.AllowKeyValueMethodGeneration.Name = "AllowKeyValueMethodGeneration";
			this.AllowKeyValueMethodGeneration.Size = new System.Drawing.Size(419, 17);
			this.AllowKeyValueMethodGeneration.TabIndex = 6;
			this.AllowKeyValueMethodGeneration.Text = "Generate methods to allow fields to be set via a string represention of the field" +
				" name";
			this.AllowKeyValueMethodGeneration.UseVisualStyleBackColor = true;
			// 
			// LoadButton
			// 
			this.LoadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LoadButton.Location = new System.Drawing.Point(895, 6);
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(101, 23);
			this.LoadButton.TabIndex = 4;
			this.LoadButton.Text = "Load Databases";
			this.LoadButton.UseVisualStyleBackColor = true;
			this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
			// 
			// Namespace
			// 
			this.Namespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Namespace.Location = new System.Drawing.Point(113, 31);
			this.Namespace.Name = "Namespace";
			this.Namespace.Size = new System.Drawing.Size(776, 20);
			this.Namespace.TabIndex = 3;
			this.Namespace.Text = "Sprocket";
			// 
			// Server
			// 
			this.Server.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Server.Location = new System.Drawing.Point(408, 7);
			this.Server.Name = "Server";
			this.Server.Size = new System.Drawing.Size(481, 20);
			this.Server.TabIndex = 2;
			this.Server.Text = "SNOWMAN\\SQL2005";
			// 
			// Password
			// 
			this.Password.Location = new System.Drawing.Point(268, 7);
			this.Password.Name = "Password";
			this.Password.PasswordChar = '*';
			this.Password.Size = new System.Drawing.Size(87, 20);
			this.Password.TabIndex = 1;
			this.Password.Text = "nathan";
			// 
			// Username
			// 
			this.Username.Location = new System.Drawing.Point(113, 7);
			this.Username.Name = "Username";
			this.Username.Size = new System.Drawing.Size(87, 20);
			this.Username.TabIndex = 0;
			this.Username.Text = "sa";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(6, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Namespace:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(206, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Password:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(361, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Server:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Admin Username:";
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer2.Size = new System.Drawing.Size(1002, 544);
			this.splitContainer2.SplitterDistance = 200;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.DatabaseList);
			this.splitContainer3.Panel1.Controls.Add(this.label7);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.SaveCSFilesButton);
			this.splitContainer3.Panel2.Controls.Add(this.SaveSQLFilesButton);
			this.splitContainer3.Panel2.Controls.Add(this.ExecuteAllSQLButton);
			this.splitContainer3.Panel2.Controls.Add(this.TablesList);
			this.splitContainer3.Panel2.Controls.Add(this.label8);
			this.splitContainer3.Size = new System.Drawing.Size(200, 544);
			this.splitContainer3.SplitterDistance = 156;
			this.splitContainer3.TabIndex = 0;
			// 
			// DatabaseList
			// 
			this.DatabaseList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DatabaseList.FormattingEnabled = true;
			this.DatabaseList.IntegralHeight = false;
			this.DatabaseList.Location = new System.Drawing.Point(9, 16);
			this.DatabaseList.Name = "DatabaseList";
			this.DatabaseList.Size = new System.Drawing.Size(191, 140);
			this.DatabaseList.TabIndex = 0;
			this.DatabaseList.SelectedIndexChanged += new System.EventHandler(this.DatabaseList_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(6, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Database List:";
			// 
			// SaveCSFilesButton
			// 
			this.SaveCSFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveCSFilesButton.Location = new System.Drawing.Point(9, 319);
			this.SaveCSFilesButton.Name = "SaveCSFilesButton";
			this.SaveCSFilesButton.Size = new System.Drawing.Size(188, 23);
			this.SaveCSFilesButton.TabIndex = 6;
			this.SaveCSFilesButton.Text = "Save C# Files";
			this.SaveCSFilesButton.UseVisualStyleBackColor = true;
			this.SaveCSFilesButton.Click += new System.EventHandler(this.SaveCSFilesButton_Click);
			// 
			// SaveSQLFilesButton
			// 
			this.SaveSQLFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveSQLFilesButton.Location = new System.Drawing.Point(9, 348);
			this.SaveSQLFilesButton.Name = "SaveSQLFilesButton";
			this.SaveSQLFilesButton.Size = new System.Drawing.Size(188, 23);
			this.SaveSQLFilesButton.TabIndex = 6;
			this.SaveSQLFilesButton.Text = "Save SQL Files";
			this.SaveSQLFilesButton.UseVisualStyleBackColor = true;
			this.SaveSQLFilesButton.Click += new System.EventHandler(this.SaveSQLFilesButton_Click);
			// 
			// ExecuteAllSQLButton
			// 
			this.ExecuteAllSQLButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ExecuteAllSQLButton.Location = new System.Drawing.Point(9, 290);
			this.ExecuteAllSQLButton.Name = "ExecuteAllSQLButton";
			this.ExecuteAllSQLButton.Size = new System.Drawing.Size(188, 23);
			this.ExecuteAllSQLButton.TabIndex = 6;
			this.ExecuteAllSQLButton.Text = "Execute all SQL";
			this.ExecuteAllSQLButton.UseVisualStyleBackColor = true;
			this.ExecuteAllSQLButton.Click += new System.EventHandler(this.ExecuteAllSQLButton_Click);
			// 
			// TablesList
			// 
			this.TablesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TablesList.CheckOnClick = true;
			this.TablesList.FormattingEnabled = true;
			this.TablesList.IntegralHeight = false;
			this.TablesList.Location = new System.Drawing.Point(9, 24);
			this.TablesList.Name = "TablesList";
			this.TablesList.Size = new System.Drawing.Size(188, 260);
			this.TablesList.TabIndex = 1;
			this.TablesList.SelectedValueChanged += new System.EventHandler(this.TablesList_SelectedValueChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(6, 8);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(67, 13);
			this.label8.TabIndex = 0;
			this.label8.Text = "Table List:";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage6);
			this.tabControl1.Controls.Add(this.tabPage7);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Location = new System.Drawing.Point(3, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(789, 538);
			this.tabControl1.TabIndex = 8;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.Output);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.SaveButton);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(781, 512);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Entity Class";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// Output
			// 
			this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Output.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Output.Location = new System.Drawing.Point(6, 19);
			this.Output.Name = "Output";
			this.Output.Size = new System.Drawing.Size(769, 458);
			this.Output.TabIndex = 0;
			this.Output.Text = "";
			this.Output.WordWrap = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(6, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(119, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "C# Class Definition:";
			// 
			// SaveButton
			// 
			this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveButton.Enabled = false;
			this.SaveButton.Location = new System.Drawing.Point(674, 483);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(101, 23);
			this.SaveButton.TabIndex = 5;
			this.SaveButton.Text = "Save Class File";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.ExecuteSQLButton);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.SaveSQLFileButton);
			this.tabPage2.Controls.Add(this.Procedures);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(781, 512);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Stored Procedures";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// ExecuteSQLButton
			// 
			this.ExecuteSQLButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ExecuteSQLButton.Enabled = false;
			this.ExecuteSQLButton.Location = new System.Drawing.Point(567, 483);
			this.ExecuteSQLButton.Name = "ExecuteSQLButton";
			this.ExecuteSQLButton.Size = new System.Drawing.Size(101, 23);
			this.ExecuteSQLButton.TabIndex = 6;
			this.ExecuteSQLButton.Text = "Execute SQL";
			this.ExecuteSQLButton.UseVisualStyleBackColor = true;
			this.ExecuteSQLButton.Click += new System.EventHandler(this.ExecuteSQLButton_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(6, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(116, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Stored Procedures:";
			// 
			// SaveSQLFileButton
			// 
			this.SaveSQLFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveSQLFileButton.Enabled = false;
			this.SaveSQLFileButton.Location = new System.Drawing.Point(674, 483);
			this.SaveSQLFileButton.Name = "SaveSQLFileButton";
			this.SaveSQLFileButton.Size = new System.Drawing.Size(101, 23);
			this.SaveSQLFileButton.TabIndex = 5;
			this.SaveSQLFileButton.Text = "Save SQL File";
			this.SaveSQLFileButton.UseVisualStyleBackColor = true;
			this.SaveSQLFileButton.Click += new System.EventHandler(this.SaveSQLFileButton_Click);
			// 
			// Procedures
			// 
			this.Procedures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Procedures.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Procedures.Location = new System.Drawing.Point(9, 19);
			this.Procedures.Name = "Procedures";
			this.Procedures.Size = new System.Drawing.Size(766, 458);
			this.Procedures.TabIndex = 0;
			this.Procedures.Text = "";
			this.Procedures.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.DataLayerInterface);
			this.tabPage3.Controls.Add(this.label9);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(781, 512);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Data Layer Interface";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// DataLayerInterface
			// 
			this.DataLayerInterface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DataLayerInterface.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DataLayerInterface.Location = new System.Drawing.Point(6, 22);
			this.DataLayerInterface.Name = "DataLayerInterface";
			this.DataLayerInterface.Size = new System.Drawing.Size(769, 484);
			this.DataLayerInterface.TabIndex = 2;
			this.DataLayerInterface.Text = "";
			this.DataLayerInterface.WordWrap = false;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(6, 6);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(194, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "Section for Data Layer Interface:";
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.DataLayerMethods);
			this.tabPage4.Controls.Add(this.label10);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(781, 512);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Data Layer Methods";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// DataLayerMethods
			// 
			this.DataLayerMethods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DataLayerMethods.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DataLayerMethods.Location = new System.Drawing.Point(6, 22);
			this.DataLayerMethods.Name = "DataLayerMethods";
			this.DataLayerMethods.Size = new System.Drawing.Size(769, 484);
			this.DataLayerMethods.TabIndex = 4;
			this.DataLayerMethods.Text = "";
			this.DataLayerMethods.WordWrap = false;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(6, 6);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(173, 13);
			this.label10.TabIndex = 3;
			this.label10.Text = "Section for Data Layer Class:";
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.ClassMain);
			this.tabPage5.Controls.Add(this.label11);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(781, 512);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Class Main Region";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// ClassMain
			// 
			this.ClassMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ClassMain.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClassMain.Location = new System.Drawing.Point(6, 22);
			this.ClassMain.Name = "ClassMain";
			this.ClassMain.Size = new System.Drawing.Size(769, 484);
			this.ClassMain.TabIndex = 6;
			this.ClassMain.Text = "";
			this.ClassMain.WordWrap = false;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(6, 6);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(116, 13);
			this.label11.TabIndex = 5;
			this.label11.Text = "Class Main Region:";
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.ClassOutline);
			this.tabPage6.Controls.Add(this.label12);
			this.tabPage6.Location = new System.Drawing.Point(4, 22);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(781, 512);
			this.tabPage6.TabIndex = 5;
			this.tabPage6.Text = "Class Outline";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// ClassOutline
			// 
			this.ClassOutline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ClassOutline.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClassOutline.Location = new System.Drawing.Point(6, 22);
			this.ClassOutline.Name = "ClassOutline";
			this.ClassOutline.Size = new System.Drawing.Size(769, 484);
			this.ClassOutline.TabIndex = 6;
			this.ClassOutline.Text = "";
			this.ClassOutline.WordWrap = false;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(6, 6);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(81, 13);
			this.label12.TabIndex = 5;
			this.label12.Text = "Class Outline";
			// 
			// tabPage7
			// 
			this.tabPage7.Controls.Add(this.exprTextBox);
			this.tabPage7.Controls.Add(this.label13);
			this.tabPage7.Location = new System.Drawing.Point(4, 22);
			this.tabPage7.Name = "tabPage7";
			this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage7.Size = new System.Drawing.Size(781, 512);
			this.tabPage7.TabIndex = 6;
			this.tabPage7.Text = "Expression Class";
			this.tabPage7.UseVisualStyleBackColor = true;
			// 
			// exprTextBox
			// 
			this.exprTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.exprTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.exprTextBox.Location = new System.Drawing.Point(6, 22);
			this.exprTextBox.Name = "exprTextBox";
			this.exprTextBox.Size = new System.Drawing.Size(769, 484);
			this.exprTextBox.TabIndex = 8;
			this.exprTextBox.Text = "";
			this.exprTextBox.WordWrap = false;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(6, 6);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(190, 13);
			this.label13.TabIndex = 7;
			this.label13.Text = "SprocketScript Expression Class";
			// 
			// tabPage8
			// 
			this.tabPage8.Controls.Add(this.exprExtender);
			this.tabPage8.Controls.Add(this.label14);
			this.tabPage8.Location = new System.Drawing.Point(4, 22);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage8.Size = new System.Drawing.Size(781, 512);
			this.tabPage8.TabIndex = 7;
			this.tabPage8.Text = "Expression Methods";
			this.tabPage8.UseVisualStyleBackColor = true;
			// 
			// exprExtender
			// 
			this.exprExtender.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.exprExtender.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.exprExtender.Location = new System.Drawing.Point(6, 22);
			this.exprExtender.Name = "exprExtender";
			this.exprExtender.Size = new System.Drawing.Size(769, 484);
			this.exprExtender.TabIndex = 10;
			this.exprExtender.Text = "";
			this.exprExtender.WordWrap = false;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(6, 6);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(190, 13);
			this.label14.TabIndex = 9;
			this.label14.Text = "SprocketScript Expression Class";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1002, 649);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "Sprocket Data Entity Generator";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel1.PerformLayout();
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.Panel2.PerformLayout();
			this.splitContainer3.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.tabPage6.ResumeLayout(false);
			this.tabPage6.PerformLayout();
			this.tabPage7.ResumeLayout(false);
			this.tabPage7.PerformLayout();
			this.tabPage8.ResumeLayout(false);
			this.tabPage8.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.RichTextBox Output;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox Username;
		private System.Windows.Forms.Button LoadButton;
		private System.Windows.Forms.ListBox DatabaseList;
		private System.Windows.Forms.TextBox Namespace;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.TextBox Password;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox Server;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.RichTextBox Procedures;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox AllowKeyValueMethodGeneration;
		private System.Windows.Forms.Button ExecuteSQLButton;
		private System.Windows.Forms.Button SaveSQLFileButton;
		private System.Windows.Forms.CheckedListBox TablesList;
		private System.Windows.Forms.CheckBox JsonEncodable;
		private System.Windows.Forms.Button SaveCSFilesButton;
		private System.Windows.Forms.Button SaveSQLFilesButton;
		private System.Windows.Forms.Button ExecuteAllSQLButton;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.RichTextBox DataLayerInterface;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RichTextBox DataLayerMethods;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.RichTextBox ClassMain;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.RichTextBox ClassOutline;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TabPage tabPage7;
		private System.Windows.Forms.RichTextBox exprTextBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TabPage tabPage8;
		private System.Windows.Forms.RichTextBox exprExtender;
		private System.Windows.Forms.Label label14;

	}
}


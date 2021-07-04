//--------------------------------------------------------------------------------------
// File: frmMain.cs
//
// Main form.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Formula1
{

	//-----------------------------------------------------------------------------
	//-----------------------------------------------------------------------------

	/// <summary>
	/// Main form.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		//	This delegate enables asynchronous calls for setting
		//	the text property on a Label control.
		//
		delegate void SetLabelCallback1(string inText);
		delegate void SetLabelCallback2(string inText);
		delegate void SetLabelCallback3(string inText);

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		#region Form variables

		//	misc window
		//
		private const string sz_NOTRUNNING_FIND = "No find file in progress";
		private const string sz_RUNNING_FIND	= "Directory: ";
		private const string sz_NOTRUNNING_GREP = "No analyze in progress";
		private const string sz_IDLE_GREP		= "(0 left) Analizing: none";
		private const string sz_RUNNING_GREP	= " left) Analizing: ";
		private const string sz_ASK_DELETE		= "Delete selected file: \n";
		private const string sz_CHOOSEDIR		= "Select Directory";
		private const string sz_DEF_GREP_TEXT	= "Grep Found List";

		//	errors
		//
		private const string szERR_IO_Exception		= "An I/O error occurred: ";
		private const string szERR_IO_Permission	= "The caller does not have the required permission: ";
		private const string szERR_No_FileFilter	= "No file filter specified";
		private const string szERR_No_FileGrep		= "No grep criteria specified";

		private const int ONE_KILOBYTE = 1024;
		private const int ONE_MEGABYTE = ONE_KILOBYTE * ONE_KILOBYTE;

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		frmHistory m_DlgFileEntries;					//	shows file search history
		frmHistory m_DlgGrepEntries;					//	shows expressions' history
		FolderSelect m_fFolders;						//  shows folder m_Selection tool
		System.Text.Encoding m_encoder = System.Text.Encoding.UTF8;

		private bool m_bAnalyzerRunning;				//	the analyzer background thread
		private bool m_bScannerRunning;					//  the scanner background thread
		private bool bReentry;							//	reentry lock for the checked listbox

		private clsOptions m_Options;					//	program m_Options
		public clsUniqueEntries m_FindEntries;			//	history for find file
		public clsUniqueEntries m_GrepEntries;			//	history for grep file content
		public clsUniqueEntries m_FoldersEntries;		//	history for folders to scan

		public clsMsgBox m_MsgBox;						//	common message box
		private Regex m_GrepFilter;						//	filter for grep
		private clsTotals m_Totals;						//	statistics
		private string[] m_Directories;					//	array of directories
		private readonly object m_LockObj = new object();// locker for synchronization
		private clsPreparser m_Preparser;				//	decide if file is text or binary
		private clsTokenizer m_FileFilter;				//	list of file filters
		private clsTokenizer m_ExcludeFilter;			//	list of file exclusion
		private ListViewItemComparer m_ListSorter;		//	routine to sort the filename list

		private Color colorMatches	= Color.Peru;
		private Color colorEdNormal = Color.Black;
		private Color colorEdSelect = Color.Red;

		//	editor m_Selection
		//
		private clsLastSelected m_Selection;
		private int iEditorIndex	= -1;				//	start of m_Selection
		private int iEditorLength	= -1;				//	length of m_Selection

		// threaded application
		//
		Thread m_ThreadScanner;
		Thread m_ThreadAnalyzer;

		#endregion

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		#region Form Components

		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TextBox edTextGrep;
		private System.Windows.Forms.TextBox edFileFilter;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.ListView lbDrives;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox groupBoxFiles;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.GroupBox gbFileFind;
		private System.Windows.Forms.GroupBox groupBoxProps;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.ListView lsFilenames;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.Label lblUpdate2;
		private System.Windows.Forms.Label lblUpdate1;
		private System.Windows.Forms.TreeView trMatches;
		private System.Windows.Forms.Button btnRunGrep;
		private System.Windows.Forms.Splitter splitter3;
		private System.Windows.Forms.ColumnHeader hdrives;
		private System.Windows.Forms.RadioButton rbm_encoder_UTF32;
		private System.Windows.Forms.RadioButton rbm_encoder_UTF8;
		private System.Windows.Forms.RadioButton rbm_encoder_UTF7;
		private System.Windows.Forms.RadioButton rbm_encoder_BigEU;
		private System.Windows.Forms.RadioButton rbm_encoder_Unicode;
		private System.Windows.Forms.RadioButton rbm_encoder_ASCII;
		private System.Windows.Forms.CheckedListBox lbRegexOptions;
		private System.Windows.Forms.TextBox edExcludeFilter;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.RichTextBox edLog;
		private System.Windows.Forms.Label lblUpdate3;
		private System.Windows.Forms.RichTextBox edFile;

		#endregion

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		#region Windows Form Designer generated code

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.pnlControls = new System.Windows.Forms.Panel();
			this.lblUpdate1 = new System.Windows.Forms.Label();
			this.lblUpdate2 = new System.Windows.Forms.Label();
			this.lblUpdate3 = new System.Windows.Forms.Label();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.edExcludeFilter = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRunGrep = new System.Windows.Forms.Button();
			this.btnRun = new System.Windows.Forms.Button();
			this.edTextGrep = new System.Windows.Forms.TextBox();
			this.edFileFilter = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lbDrives = new System.Windows.Forms.ListView();
			this.hdrives = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBoxFiles = new System.Windows.Forms.GroupBox();
			this.gbFileFind = new System.Windows.Forms.GroupBox();
			this.edFile = new System.Windows.Forms.RichTextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.edLog = new System.Windows.Forms.RichTextBox();
			this.lbRegexOptions = new System.Windows.Forms.CheckedListBox();
			this.rbm_encoder_BigEU = new System.Windows.Forms.RadioButton();
			this.rbm_encoder_Unicode = new System.Windows.Forms.RadioButton();
			this.rbm_encoder_ASCII = new System.Windows.Forms.RadioButton();
			this.rbm_encoder_UTF32 = new System.Windows.Forms.RadioButton();
			this.rbm_encoder_UTF8 = new System.Windows.Forms.RadioButton();
			this.rbm_encoder_UTF7 = new System.Windows.Forms.RadioButton();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.lsFilenames = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.groupBoxProps = new System.Windows.Forms.GroupBox();
			this.trMatches = new System.Windows.Forms.TreeView();
			this.pnlControls.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBoxFiles.SuspendLayout();
			this.gbFileFind.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBoxProps.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlControls
			// 
			this.pnlControls.BackColor = System.Drawing.Color.LightSteelBlue;
			this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlControls.Controls.Add(this.lblUpdate1);
			this.pnlControls.Controls.Add(this.lblUpdate2);
			this.pnlControls.Controls.Add(this.lblUpdate3);
			this.pnlControls.Controls.Add(this.btnSelectAll);
			this.pnlControls.Controls.Add(this.edExcludeFilter);
			this.pnlControls.Controls.Add(this.label3);
			this.pnlControls.Controls.Add(this.btnRunGrep);
			this.pnlControls.Controls.Add(this.btnRun);
			this.pnlControls.Controls.Add(this.edTextGrep);
			this.pnlControls.Controls.Add(this.edFileFilter);
			this.pnlControls.Controls.Add(this.label2);
			this.pnlControls.Controls.Add(this.label1);
			this.pnlControls.Controls.Add(this.lbDrives);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlControls.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pnlControls.Location = new System.Drawing.Point(0, 0);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(2503, 163);
			this.pnlControls.TabIndex = 10;
			// 
			// lblUpdate1
			// 
			this.lblUpdate1.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdate1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblUpdate1.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUpdate1.ForeColor = System.Drawing.Color.Black;
			this.lblUpdate1.Location = new System.Drawing.Point(0, 95);
			this.lblUpdate1.Name = "lblUpdate1";
			this.lblUpdate1.Size = new System.Drawing.Size(1870, 22);
			this.lblUpdate1.TabIndex = 20;
			this.lblUpdate1.Text = "lblUpdate1";
			// 
			// lblUpdate2
			// 
			this.lblUpdate2.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdate2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblUpdate2.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUpdate2.ForeColor = System.Drawing.Color.Black;
			this.lblUpdate2.Location = new System.Drawing.Point(0, 117);
			this.lblUpdate2.Name = "lblUpdate2";
			this.lblUpdate2.Size = new System.Drawing.Size(1870, 22);
			this.lblUpdate2.TabIndex = 16;
			this.lblUpdate2.Text = "lblUpdate2";
			// 
			// lblUpdate3
			// 
			this.lblUpdate3.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdate3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblUpdate3.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUpdate3.ForeColor = System.Drawing.Color.DarkRed;
			this.lblUpdate3.Location = new System.Drawing.Point(0, 139);
			this.lblUpdate3.Name = "lblUpdate3";
			this.lblUpdate3.Size = new System.Drawing.Size(1870, 22);
			this.lblUpdate3.TabIndex = 15;
			this.lblUpdate3.Text = "lblUpdate3";
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.BackColor = System.Drawing.Color.SteelBlue;
			this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSelectAll.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelectAll.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectAll.Image")));
			this.btnSelectAll.Location = new System.Drawing.Point(915, 45);
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size(68, 31);
			this.btnSelectAll.TabIndex = 5;
			this.btnSelectAll.UseVisualStyleBackColor = false;
			this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
			// 
			// edExcludeFilter
			// 
			this.edExcludeFilter.BackColor = System.Drawing.Color.WhiteSmoke;
			this.edExcludeFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edExcludeFilter.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edExcludeFilter.ForeColor = System.Drawing.Color.Black;
			this.edExcludeFilter.Location = new System.Drawing.Point(547, 9);
			this.edExcludeFilter.Name = "edExcludeFilter";
			this.edExcludeFilter.Size = new System.Drawing.Size(282, 30);
			this.edExcludeFilter.TabIndex = 1;
			this.edExcludeFilter.WordWrap = false;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(450, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 22);
			this.label3.TabIndex = 24;
			this.label3.Text = "Exclude";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnRunGrep
			// 
			this.btnRunGrep.BackColor = System.Drawing.Color.SteelBlue;
			this.btnRunGrep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRunGrep.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRunGrep.Image = ((System.Drawing.Image)(resources.GetObject("btnRunGrep.Image")));
			this.btnRunGrep.Location = new System.Drawing.Point(836, 45);
			this.btnRunGrep.Name = "btnRunGrep";
			this.btnRunGrep.Size = new System.Drawing.Size(68, 31);
			this.btnRunGrep.TabIndex = 4;
			this.btnRunGrep.UseVisualStyleBackColor = false;
			this.btnRunGrep.Click += new System.EventHandler(this.btnRunGrep_Click);
			// 
			// btnRun
			// 
			this.btnRun.BackColor = System.Drawing.Color.SteelBlue;
			this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRun.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRun.Image = ((System.Drawing.Image)(resources.GetObject("btnRun.Image")));
			this.btnRun.Location = new System.Drawing.Point(836, 9);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(147, 30);
			this.btnRun.TabIndex = 3;
			this.btnRun.UseMnemonic = false;
			this.btnRun.UseVisualStyleBackColor = false;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// edTextGrep
			// 
			this.edTextGrep.BackColor = System.Drawing.Color.WhiteSmoke;
			this.edTextGrep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edTextGrep.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edTextGrep.ForeColor = System.Drawing.Color.Black;
			this.edTextGrep.Location = new System.Drawing.Point(93, 45);
			this.edTextGrep.Name = "edTextGrep";
			this.edTextGrep.Size = new System.Drawing.Size(737, 30);
			this.edTextGrep.TabIndex = 2;
			this.edTextGrep.WordWrap = false;
			this.edTextGrep.DoubleClick += new System.EventHandler(this.edTextGrep_DoubleClick);
			this.edTextGrep.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EdTextGrepKeyPress);
			// 
			// edFileFilter
			// 
			this.edFileFilter.BackColor = System.Drawing.Color.WhiteSmoke;
			this.edFileFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edFileFilter.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edFileFilter.ForeColor = System.Drawing.Color.Black;
			this.edFileFilter.Location = new System.Drawing.Point(93, 9);
			this.edFileFilter.Name = "edFileFilter";
			this.edFileFilter.Size = new System.Drawing.Size(282, 30);
			this.edFileFilter.TabIndex = 0;
			this.edFileFilter.Text = "*.*";
			this.edFileFilter.WordWrap = false;
			this.edFileFilter.DoubleClick += new System.EventHandler(this.edFileFilter_DoubleClick);
			this.edFileFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EdFileFilterKeyPress);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(5, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 22);
			this.label2.TabIndex = 11;
			this.label2.Text = "Grep";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(3, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 22);
			this.label1.TabIndex = 10;
			this.label1.Text = "Filter";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbDrives
			// 
			this.lbDrives.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lbDrives.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lbDrives.CheckBoxes = true;
			this.lbDrives.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdrives});
			this.lbDrives.Dock = System.Windows.Forms.DockStyle.Right;
			this.lbDrives.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbDrives.ForeColor = System.Drawing.Color.Black;
			this.lbDrives.FullRowSelect = true;
			this.lbDrives.GridLines = true;
			this.lbDrives.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lbDrives.HideSelection = false;
			this.lbDrives.LabelWrap = false;
			this.lbDrives.Location = new System.Drawing.Point(1870, 0);
			this.lbDrives.Name = "lbDrives";
			this.lbDrives.ShowGroups = false;
			this.lbDrives.ShowItemToolTips = true;
			this.lbDrives.Size = new System.Drawing.Size(631, 161);
			this.lbDrives.TabIndex = 6;
			this.lbDrives.UseCompatibleStateImageBehavior = false;
			this.lbDrives.View = System.Windows.Forms.View.Details;
			this.lbDrives.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbDrives_ItemCheck);
			this.lbDrives.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lbDrives_KeyUp);
			// 
			// hdrives
			// 
			this.hdrives.Text = "drives";
			this.hdrives.Width = 365;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Title = "Choose pathname";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBoxFiles);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 163);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(2503, 1174);
			this.panel1.TabIndex = 12;
			// 
			// groupBoxFiles
			// 
			this.groupBoxFiles.BackColor = System.Drawing.Color.Silver;
			this.groupBoxFiles.Controls.Add(this.gbFileFind);
			this.groupBoxFiles.Controls.Add(this.splitter2);
			this.groupBoxFiles.Controls.Add(this.groupBoxProps);
			this.groupBoxFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxFiles.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxFiles.ForeColor = System.Drawing.Color.Black;
			this.groupBoxFiles.Location = new System.Drawing.Point(0, 0);
			this.groupBoxFiles.Name = "groupBoxFiles";
			this.groupBoxFiles.Size = new System.Drawing.Size(2503, 1174);
			this.groupBoxFiles.TabIndex = 18;
			this.groupBoxFiles.TabStop = false;
			// 
			// gbFileFind
			// 
			this.gbFileFind.BackColor = System.Drawing.Color.DarkRed;
			this.gbFileFind.Controls.Add(this.edFile);
			this.gbFileFind.Controls.Add(this.panel2);
			this.gbFileFind.Controls.Add(this.splitter3);
			this.gbFileFind.Controls.Add(this.lsFilenames);
			this.gbFileFind.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbFileFind.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbFileFind.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.gbFileFind.Location = new System.Drawing.Point(182, 24);
			this.gbFileFind.Name = "gbFileFind";
			this.gbFileFind.Size = new System.Drawing.Size(2318, 1147);
			this.gbFileFind.TabIndex = 15;
			this.gbFileFind.TabStop = false;
			this.gbFileFind.Text = "Find file";
			// 
			// edFile
			// 
			this.edFile.BackColor = System.Drawing.Color.WhiteSmoke;
			this.edFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edFile.DetectUrls = false;
			this.edFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.edFile.Font = new System.Drawing.Font("Liberation Mono", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edFile.ForeColor = System.Drawing.Color.Black;
			this.edFile.HideSelection = false;
			this.edFile.Location = new System.Drawing.Point(3, 229);
			this.edFile.Multiline = false;
			this.edFile.Name = "edFile";
			this.edFile.Size = new System.Drawing.Size(2312, 915);
			this.edFile.TabIndex = 1;
			this.edFile.Text = "";
			this.edFile.WordWrap = false;
			this.edFile.ZoomFactor = 2F;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.LightSteelBlue;
			this.panel2.Controls.Add(this.edLog);
			this.panel2.Controls.Add(this.lbRegexOptions);
			this.panel2.Controls.Add(this.rbm_encoder_BigEU);
			this.panel2.Controls.Add(this.rbm_encoder_Unicode);
			this.panel2.Controls.Add(this.rbm_encoder_ASCII);
			this.panel2.Controls.Add(this.rbm_encoder_UTF32);
			this.panel2.Controls.Add(this.rbm_encoder_UTF8);
			this.panel2.Controls.Add(this.rbm_encoder_UTF7);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 127);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(2312, 102);
			this.panel2.TabIndex = 4;
			this.panel2.ClientSizeChanged += new System.EventHandler(this.panel2_ClientSizeChanged);
			// 
			// edLog
			// 
			this.edLog.BackColor = System.Drawing.Color.LightSteelBlue;
			this.edLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.edLog.DetectUrls = false;
			this.edLog.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.edLog.ForeColor = System.Drawing.Color.Black;
			this.edLog.Location = new System.Drawing.Point(821, 0);
			this.edLog.Name = "edLog";
			this.edLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.edLog.Size = new System.Drawing.Size(909, 101);
			this.edLog.TabIndex = 7;
			this.edLog.Text = "";
			// 
			// lbRegexOptions
			// 
			this.lbRegexOptions.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lbRegexOptions.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbRegexOptions.FormattingEnabled = true;
			this.lbRegexOptions.IntegralHeight = false;
			this.lbRegexOptions.Location = new System.Drawing.Point(318, -3);
			this.lbRegexOptions.Name = "lbRegexOptions";
			this.lbRegexOptions.ScrollAlwaysVisible = true;
			this.lbRegexOptions.Size = new System.Drawing.Size(300, 104);
			this.lbRegexOptions.TabIndex = 6;
			this.lbRegexOptions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbRegexOptions_ItemCheck);
			// 
			// rbm_encoder_BigEU
			// 
			this.rbm_encoder_BigEU.AutoSize = true;
			this.rbm_encoder_BigEU.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_BigEU.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_BigEU.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_BigEU.Location = new System.Drawing.Point(115, 67);
			this.rbm_encoder_BigEU.Name = "rbm_encoder_BigEU";
			this.rbm_encoder_BigEU.Size = new System.Drawing.Size(210, 24);
			this.rbm_encoder_BigEU.TabIndex = 5;
			this.rbm_encoder_BigEU.Text = "BigEndianUnicode";
			this.rbm_encoder_BigEU.UseVisualStyleBackColor = false;
			this.rbm_encoder_BigEU.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// rbm_encoder_Unicode
			// 
			this.rbm_encoder_Unicode.AutoSize = true;
			this.rbm_encoder_Unicode.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_Unicode.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_Unicode.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_Unicode.Location = new System.Drawing.Point(115, 37);
			this.rbm_encoder_Unicode.Name = "rbm_encoder_Unicode";
			this.rbm_encoder_Unicode.Size = new System.Drawing.Size(111, 24);
			this.rbm_encoder_Unicode.TabIndex = 4;
			this.rbm_encoder_Unicode.Text = "Unicode";
			this.rbm_encoder_Unicode.UseVisualStyleBackColor = false;
			this.rbm_encoder_Unicode.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// rbm_encoder_ASCII
			// 
			this.rbm_encoder_ASCII.AutoSize = true;
			this.rbm_encoder_ASCII.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_ASCII.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_ASCII.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_ASCII.Location = new System.Drawing.Point(115, 6);
			this.rbm_encoder_ASCII.Name = "rbm_encoder_ASCII";
			this.rbm_encoder_ASCII.Size = new System.Drawing.Size(89, 24);
			this.rbm_encoder_ASCII.TabIndex = 3;
			this.rbm_encoder_ASCII.Text = "ASCII";
			this.rbm_encoder_ASCII.UseVisualStyleBackColor = false;
			this.rbm_encoder_ASCII.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// rbm_encoder_UTF32
			// 
			this.rbm_encoder_UTF32.AutoSize = true;
			this.rbm_encoder_UTF32.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_UTF32.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_UTF32.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_UTF32.Location = new System.Drawing.Point(5, 67);
			this.rbm_encoder_UTF32.Name = "rbm_encoder_UTF32";
			this.rbm_encoder_UTF32.Size = new System.Drawing.Size(89, 24);
			this.rbm_encoder_UTF32.TabIndex = 2;
			this.rbm_encoder_UTF32.Text = "UTF32";
			this.rbm_encoder_UTF32.UseVisualStyleBackColor = false;
			this.rbm_encoder_UTF32.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// rbm_encoder_UTF8
			// 
			this.rbm_encoder_UTF8.AutoSize = true;
			this.rbm_encoder_UTF8.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_UTF8.Checked = true;
			this.rbm_encoder_UTF8.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_UTF8.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_UTF8.Location = new System.Drawing.Point(5, 37);
			this.rbm_encoder_UTF8.Name = "rbm_encoder_UTF8";
			this.rbm_encoder_UTF8.Size = new System.Drawing.Size(78, 24);
			this.rbm_encoder_UTF8.TabIndex = 1;
			this.rbm_encoder_UTF8.TabStop = true;
			this.rbm_encoder_UTF8.Text = "UTF8";
			this.rbm_encoder_UTF8.UseVisualStyleBackColor = false;
			this.rbm_encoder_UTF8.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// rbm_encoder_UTF7
			// 
			this.rbm_encoder_UTF7.AutoSize = true;
			this.rbm_encoder_UTF7.BackColor = System.Drawing.Color.Transparent;
			this.rbm_encoder_UTF7.Font = new System.Drawing.Font("Liberation Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbm_encoder_UTF7.ForeColor = System.Drawing.Color.Black;
			this.rbm_encoder_UTF7.Location = new System.Drawing.Point(5, 6);
			this.rbm_encoder_UTF7.Name = "rbm_encoder_UTF7";
			this.rbm_encoder_UTF7.Size = new System.Drawing.Size(78, 24);
			this.rbm_encoder_UTF7.TabIndex = 0;
			this.rbm_encoder_UTF7.Text = "UTF7";
			this.rbm_encoder_UTF7.UseVisualStyleBackColor = false;
			this.rbm_encoder_UTF7.CheckedChanged += new System.EventHandler(this.EncodingOptions_CheckedChanged);
			// 
			// splitter3
			// 
			this.splitter3.BackColor = System.Drawing.Color.Silver;
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(3, 122);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(2312, 5);
			this.splitter3.TabIndex = 2;
			this.splitter3.TabStop = false;
			// 
			// lsFilenames
			// 
			this.lsFilenames.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lsFilenames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lsFilenames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
			this.lsFilenames.Cursor = System.Windows.Forms.Cursors.Default;
			this.lsFilenames.Dock = System.Windows.Forms.DockStyle.Top;
			this.lsFilenames.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lsFilenames.FullRowSelect = true;
			this.lsFilenames.HideSelection = false;
			this.lsFilenames.Location = new System.Drawing.Point(3, 26);
			this.lsFilenames.Name = "lsFilenames";
			this.lsFilenames.Size = new System.Drawing.Size(2312, 96);
			this.lsFilenames.TabIndex = 0;
			this.lsFilenames.UseCompatibleStateImageBehavior = false;
			this.lsFilenames.View = System.Windows.Forms.View.Details;
			this.lsFilenames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsFilenames_ColumnClick);
			this.lsFilenames.SelectedIndexChanged += new System.EventHandler(this.lsFilenames_SelectedIndexChanged);
			this.lsFilenames.DoubleClick += new System.EventHandler(this.lsFilenames_DoubleClick);
			this.lsFilenames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lsFilenames_KeyUp);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Hits";
			this.columnHeader7.Width = 35;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Filename";
			this.columnHeader1.Width = 450;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Attrs";
			this.columnHeader2.Width = 45;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Size";
			this.columnHeader3.Width = 70;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Created";
			this.columnHeader4.Width = 120;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Accessed";
			this.columnHeader5.Width = 120;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Modified";
			this.columnHeader6.Width = 120;
			// 
			// splitter2
			// 
			this.splitter2.BackColor = System.Drawing.Color.Silver;
			this.splitter2.Location = new System.Drawing.Point(177, 24);
			this.splitter2.MinExtra = 0;
			this.splitter2.MinSize = 120;
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(5, 1147);
			this.splitter2.TabIndex = 14;
			this.splitter2.TabStop = false;
			// 
			// groupBoxProps
			// 
			this.groupBoxProps.BackColor = System.Drawing.Color.DarkRed;
			this.groupBoxProps.Controls.Add(this.trMatches);
			this.groupBoxProps.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBoxProps.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxProps.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.groupBoxProps.Location = new System.Drawing.Point(3, 24);
			this.groupBoxProps.Name = "groupBoxProps";
			this.groupBoxProps.Size = new System.Drawing.Size(174, 1147);
			this.groupBoxProps.TabIndex = 13;
			this.groupBoxProps.TabStop = false;
			this.groupBoxProps.Text = "Grep found list";
			// 
			// trMatches
			// 
			this.trMatches.BackColor = System.Drawing.Color.LightSteelBlue;
			this.trMatches.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.trMatches.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trMatches.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.trMatches.ForeColor = System.Drawing.Color.Black;
			this.trMatches.HideSelection = false;
			this.trMatches.Location = new System.Drawing.Point(3, 26);
			this.trMatches.Name = "trMatches";
			this.trMatches.ShowNodeToolTips = true;
			this.trMatches.Size = new System.Drawing.Size(168, 1118);
			this.trMatches.TabIndex = 0;
			this.trMatches.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trMatches_AfterSelect);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(9, 24);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(2503, 1337);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pnlControls);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmMain";
			this.Text = "Formula1";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.pnlControls.ResumeLayout(false);
			this.pnlControls.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.groupBoxFiles.ResumeLayout(false);
			this.gbFileFind.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupBoxProps.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		#region Main form init/term code

		public frmMain()
		{
			InitializeComponent();
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Initialize all member variables.
		/// </summary>
		private void _Initialize()
		{
			try
			{
				m_Totals = new clsTotals();				//	statistics
				m_Selection = new clsLastSelected();
				m_Preparser = new clsPreparser();

				m_MsgBox = new clsMsgBox(this, Application.ProductName);
				m_Options = new clsOptions();
				m_Options.Load_Options(Application.LocalUserAppDataPath);

				m_FindEntries = new clsUniqueEntries();
				m_FindEntries.Filename = Application.LocalUserAppDataPath + "\\FindEntries.xml";

				m_GrepEntries = new clsUniqueEntries();
				m_GrepEntries.Filename = Application.LocalUserAppDataPath + "\\GrepEntries.xml";

				m_FoldersEntries = new clsUniqueEntries();
				m_FoldersEntries.Filename = Application.LocalUserAppDataPath + "\\FoldersList.xml";

				m_FileFilter = new clsTokenizer();
				m_ExcludeFilter = new clsTokenizer();

				m_ListSorter = new ListViewItemComparer();

				//	main form m_Options ( location and size are coded in the load event handler )
				//
				this.Text = Application.ProductName + " (ver. " + Application.ProductVersion + ")";
				edFileFilter.Text = m_Options.FileFilter;
				edTextGrep.Text = m_Options.FileGrep;
				StartPosition = FormStartPosition.Manual;

				//	editor m_Options
				//
				edFile.Multiline = true;
				edFile.WordWrap = false;
				edFile.Multiline = true;
				edFile.ScrollBars = RichTextBoxScrollBars.ForcedBoth;

				lblUpdate1.Text = sz_NOTRUNNING_FIND;
				lblUpdate2.Text = sz_NOTRUNNING_GREP;
				lblUpdate3.Text = "";
				edLog.Clear();

				System.Windows.Forms.ListView.CheckForIllegalCrossThreadCalls = false;
				System.Windows.Forms.TreeView.CheckForIllegalCrossThreadCalls = false;

				lbRegexOptions.Items.Add("Culture Invariant");
				lbRegexOptions.Items.Add("ECMA Script");
				lbRegexOptions.Items.Add("Explicit Capture");
				lbRegexOptions.Items.Add("Ignore Case", true);
				lbRegexOptions.Items.Add("Ignore Pattern Whitespace");
				lbRegexOptions.Items.Add("Multiline", true);
				lbRegexOptions.Items.Add("Right To Left");
				lbRegexOptions.Items.Add("Singleline");

				//	assign the sort routine
				//
				lsFilenames.ListViewItemSorter = m_ListSorter;

				//	load entries from xml files
				//
				m_FindEntries.LoadFromFile();
				m_GrepEntries.LoadFromFile();
				m_FoldersEntries.LoadFromFile();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Set the form's options read from file
		/// </summary>
		private void _SetFormOptions()
		{
			try
			{
				Left = m_Options.Win_Left;
				Top = m_Options.Win_Top;
				Width = m_Options.Win_Width;
				Height = m_Options.Win_Height;

				//					TopMost			= (m_Options.StayOnTop == 1) ;
				WindowState = (FormWindowState)m_Options.Win_State;

				edFile.Height = m_Options.Splitter1;
				groupBoxProps.Width = m_Options.Splitter2;
				lsFilenames.Height = m_Options.Splitter3;

				lsFilenames.Columns[0].Width = m_Options.ColumnSize0;
				lsFilenames.Columns[1].Width = m_Options.ColumnSize1;
				lsFilenames.Columns[2].Width = m_Options.ColumnSize2;
				lsFilenames.Columns[3].Width = m_Options.ColumnSize3;
				lsFilenames.Columns[4].Width = m_Options.ColumnSize4;
				lsFilenames.Columns[5].Width = m_Options.ColumnSize5;
				lsFilenames.Columns[6].Width = m_Options.ColumnSize6;
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// The form is loading.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				_Initialize();
				_Load_Drives();
				_SetFormOptions();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}

			Cursor.Current = Cursors.Default;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// The form is closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				if (m_bScannerRunning == true || m_bAnalyzerRunning == true)
				{
					_Reset_Threads();					//	let threads exit cleanly
					Thread.Sleep(3000);					//	give time to threads to quit ...
				}

				// save form's options
				//
				m_Options.Splitter1 = edFile.Height;
				m_Options.Splitter2 = groupBoxProps.Width;
				m_Options.Splitter3 = lsFilenames.Height;

				m_Options.ColumnSize0 = lsFilenames.Columns[0].Width;
				m_Options.ColumnSize1 = lsFilenames.Columns[1].Width;
				m_Options.ColumnSize2 = lsFilenames.Columns[2].Width;
				m_Options.ColumnSize3 = lsFilenames.Columns[3].Width;
				m_Options.ColumnSize4 = lsFilenames.Columns[4].Width;
				m_Options.ColumnSize5 = lsFilenames.Columns[5].Width;
				m_Options.ColumnSize6 = lsFilenames.Columns[6].Width;

				if (WindowState == FormWindowState.Normal)
				{
					m_Options.Win_Left = Location.X;
					m_Options.Win_Top = Location.Y;
					m_Options.Win_Width = Width;
					m_Options.Win_Height = Height;
				}
				m_Options.Win_State = (int)WindowState;
				m_Options.FileFilter = edFileFilter.Text;
				m_Options.FileGrep = edTextGrep.Text;

				m_Options.Save_Options();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}

			Cursor.Current = Cursors.Default;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Dispose the form
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (true == disposing)
			{
				if (null != components)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		[STAThread]
		static void Main()
		{
			Application.Run(new frmMain());
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Called by a child form when it closes
		/// </summary>
		/// <param name="inWhich"></param>
		public void Closing_Child(Form inWhich)
		{
			try
			{
				if (inWhich == m_DlgFileEntries)
				{
					m_DlgFileEntries = null;
				}
				else if (inWhich == m_DlgGrepEntries)
				{
					m_DlgGrepEntries = null;
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Append text to the logging panel.
		/// </summary>
		/// <param name="inText"></param>
		private void _AppendLogText(string inText)
		{
			if (0 == inText.Length)
			{
				return;
			}

			try
			{
				if (edLog.InvokeRequired)
				{
					SetLabelCallback1 d = new SetLabelCallback1(_AppendLogText);
					this.Invoke(d, new object[] { inText });
				}
				else
				{
					if (0 < edLog.Text.Length)
					{
						edLog.Text += "\n";
					}

					edLog.Text += inText;
				}
			}
			catch (Exception)
			{
				// m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Enumerate the drives on the local machine
		/// </summary>
		private void _Load_Drives()
		{
			//	cleanup and add the default "Select ..." option
			//
			lbDrives.Items.Clear();
			lbDrives.Items.Add(sz_CHOOSEDIR);

			try
			{
				//	add the entire list of drives installed
				//
				string[] szDrives = System.IO.Directory.GetLogicalDrives();

				foreach (string sz in szDrives)
				{
					lbDrives.Items.Add(sz);
				}

				//	add the list of previous searched folders
				//
				for (int i = 0; i < m_FoldersEntries.Count; i++)
				{
					lbDrives.Items.Add((string)m_FoldersEntries[i]);
				}
			}
			catch (System.IO.IOException ce)
			{
				_AppendLogText(szERR_IO_Exception + ce.Message);
			}
			catch (System.Security.SecurityException ce)
			{
				_AppendLogText(szERR_IO_Exception + ce.Message);
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Get the regex m_Options. Deletes the object if are different.
		/// </summary>
		/// <returns></returns>
		private RegexOptions _Get_Regex_Options()
		{
			RegexOptions retValue = RegexOptions.None;

			foreach (string text in lbRegexOptions.CheckedItems)
			{
				if ("Culture Invariant" == text)
				{
					retValue |= RegexOptions.CultureInvariant;
					continue;
				}
				else if ("ECMA Script" == text)
				{
					retValue |= RegexOptions.ECMAScript;
					continue;
				}
				else if ("Explicit Capture" == text)
				{
					retValue |= RegexOptions.ExplicitCapture;
					continue;
				}
				else if ("Ignore Pattern Whitespace" == text)
				{
					retValue |= RegexOptions.IgnorePatternWhitespace;
					continue;
				}
				else if ("Ignore Case" == text)
				{
					retValue |= RegexOptions.IgnoreCase;
					continue;
				}
				else if ("Multiline" == text)
				{
					retValue |= RegexOptions.Multiline;
					continue;
				}
				else if ("Right To Left" == text)
				{
					retValue |= RegexOptions.RightToLeft;
					continue;
				}
				else if ("Singleline" == text)
				{
					retValue |= RegexOptions.Singleline;
					continue;
				}
			}

			return retValue;
		}

		//---------------------------------------------------------------------
		//---------------------------------------------------------------------
		// This method demonstrates a pattern for making thread-safe
		// calls on a Windows Forms control. 
		//
		// If the calling thread is different from the thread that
		// created the TextBox control, this method creates a
		// SetTextCallback and calls itself asynchronously using the
		// Invoke method.
		//
		// If the calling thread is the same as the thread that created
		// the TextBox control, the Text property is set directly. 
		//
		private void SetTextLabel1(string inText)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (lblUpdate1.InvokeRequired)
			{
				SetLabelCallback1 d = new SetLabelCallback1(SetTextLabel1);
				this.Invoke(d, new object[] { inText });
			}
			else
			{
				lblUpdate1.Text = inText;
			}
		}

		private void SetTextLabel2(string inText)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (lblUpdate2.InvokeRequired)
			{
				SetLabelCallback2 d = new SetLabelCallback2(SetTextLabel2);
				this.Invoke(d, new object[] { inText });
			}
			else
			{
				lblUpdate2.Text = inText;
			}
		}

		private void SetTextLabel3(string inText)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (lblUpdate3.InvokeRequired)
			{
				SetLabelCallback3 d = new SetLabelCallback3(SetTextLabel3);
				this.Invoke(d, new object[] { inText });
			}
			else
			{
				lblUpdate3.Text = inText;
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Update the statistics label.
		/// </summary>
		private void _Update_Stats()
		{
			lock (m_LockObj)
			{
				string text = "Directories: " + m_Totals.iDirs.ToString()
								+ " Files: " + m_Totals.iFiles.ToString()
								+ " Matches: " + m_Totals.iGreps.ToString();

				if (text != lblUpdate3.Text)
				{
					SetTextLabel3(text);
				}
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of file listed as result
		/// </summary>
		/// <returns></returns>
		private int FilesListed()
		{
			lock (m_LockObj)
			{
				return lsFilenames.Items.Count;
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public void _Analyzer()
		{
			int iNewCount = 0;
			int iOldCount = 0;
			int iCounter = 0;
			int iMatches = 0;
			ListViewItem item = null;

			if (0 == edTextGrep.Text.Length)
			{
				_AppendLogText(szERR_No_FileGrep);
				m_bAnalyzerRunning = false;
				m_ListSorter.Enable = true;
				return;
			}

			try
			{
				m_bAnalyzerRunning = true;
				m_ListSorter.Enable = false;

				while (m_bAnalyzerRunning)
				{
					iNewCount = FilesListed();

					//	no items left to check and scanner has exited
					//	safe to quit ...
					//
					if (iNewCount == iOldCount && m_bScannerRunning == false)
						break;

					if (iNewCount == iOldCount)
					{
						SetTextLabel2(sz_IDLE_GREP);
						_Update_Stats();
						Thread.Sleep(100);
						continue;
					}

					try
					{
						for (; iCounter < iNewCount; iCounter++)
						{
							if (false == m_bAnalyzerRunning)
							{
								break;
							}

							if (null == (item = lsFilenames.Items[iCounter]))
							{
								break;
							}

							if (item.Checked == true)
							{
								continue;
							}

							SetTextLabel2("(" + (iNewCount - iOldCount).ToString() + sz_RUNNING_GREP + item.SubItems[1].Text);
							iMatches = _Analyze_File(item, false);

							lock (m_LockObj)
							{
								iOldCount++;
								item.Checked = true;
								item.SubItems[0].Text = iMatches.ToString();

								if (iMatches > 0)
								{
									item.BackColor = colorMatches;
									item.EnsureVisible();
									m_Totals.iGreps++;
								}
							}
						}

						if (true == m_bScannerRunning)
						{
							Thread.Sleep(25);
						}
					}
					catch (Exception ce)
					{
						//	may have problems with item matrix indexes ...
						//
						SetTextLabel3(ce.Message);
						break;
					}

					//	faile safe
					//
					iOldCount = iNewCount;
					_Update_Stats();
				}
			}
			catch (Exception ce)
			{
				SetTextLabel3(ce.Message);
			}

			//	here we release resources
			//
			m_bAnalyzerRunning = false;
			m_ListSorter.Enable = true;
			_Update_Stats();
			SetTextLabel2(sz_NOTRUNNING_GREP);
			lsFilenames.EndUpdate();
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Run grep on the selected file. Can be called in background.
		/// </summary>
		/// <param name="item">The filename on which to run grep</param>
		/// <param name="bUpdateTree">Run a full analisys</param>
		/// <returns>Number of grep matches</returns>
		public int _Analyze_File(ListViewItem item, bool bUpdateTree)
		{
			FileStream fIn = null;
			MatchCollection mColl = null;
			byte[] buffer = null;
			string szText = "";
			int parents = 0;
			int iRet = 0;
			int iFileSize = 0;

			try
			{
				//	don't bother changing if the file size is zero
				//
				iFileSize = System.Convert.ToInt32(item.Tag.ToString(), 10);

				if (iFileSize <= 0 || edTextGrep.Text.Length == 0)
					return iRet;

				//	check the grep object if needs to be rebuilt
				//
				if (null == m_GrepFilter)
				{
					m_GrepFilter = new Regex(edTextGrep.Text, _Get_Regex_Options());
				}
				else if (edTextGrep.Text != m_GrepFilter.ToString())
				{
					m_GrepFilter = new Regex(edTextGrep.Text, _Get_Regex_Options());
				}
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
				return iRet;
			}

			try
			{
				//	let the framework decide how much big is the buffer size
				//
				fIn = new FileStream(item.SubItems[1].Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, iFileSize, FileOptions.SequentialScan);

				if (fIn == null)
				{
					m_MsgBox.Say("Error opening \n" + item.SubItems[1].Text);
					return iRet;
				}

				//	grab all the data
				//
				buffer = new byte[iFileSize];

				fIn.Read(buffer, 0, iFileSize);
				fIn.Close();
				fIn = null;
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
				return iRet;
			}

			try
			{
				if (true == bUpdateTree)
				{
					//	simple check over the file, won't modify the buffer
					//
					if (true == m_Preparser.IsKnownBinary(buffer))
					{
						m_MsgBox.Say("Known binary file !");
					}

					if (true == m_Preparser.IsText(buffer))
					{
						buffer = m_Preparser.StripCarriages(buffer);
					}
				}

				//	make text out of the read data and run the grep over it
				//
				szText = m_encoder.GetString(buffer);
				mColl = m_GrepFilter.Matches(szText);

				if (mColl.Count > 0)
				{
					iRet = mColl.Count;
				}

				if (false == bUpdateTree)
				{
					return iRet;
				}

				//	------------------------------
				//  full update of tree
				//
				TreeNode parent;
				TreeNode node;

				trMatches.BeginUpdate();
				Cleanup_Tree();

				foreach (Match match in mColl)
				{
					//  get a parent
					//
					parent = null;
					foreach (TreeNode t in trMatches.Nodes)
					{
						szText = ((string)t.Tag);
						if (szText.CompareTo(match.Value) == 0)
						{
							parent = t;
							break;
						}
					}

					//  if not found create new
					//
					if (null == parent)
					{
						++parents;
						parent = new TreeNode(match.Value + " [0]");
						parent.Tag = match.Value;
						trMatches.Nodes.Add(parent);
					}
				}

				foreach (Match match in mColl)
				{
					//  get a parent
					//
					parent = null;
					foreach (TreeNode t in trMatches.Nodes)
					{
						szText = ((string)t.Tag);
						if (szText.CompareTo(match.Value) == 0)
						{
							parent = t;
							parent.Text = szText + " [" + (parent.Nodes.Count + 1).ToString() + "]";
							break;
						}
					}

					//  add a new node
					//
					if (parent.Nodes.Count == 0)
					{
						szText = match.Value.ToString() + "("
							   + match.Index.ToString() + ","
							   + match.Length.ToString() + ")";
					}
					else
					{
						szText = "("
							   + match.Index.ToString() + ","
							   + match.Length.ToString() + ")";
					}

					node = new TreeNode(szText);
					node.Tag = match;
					parent.Nodes.Add(node);
				}

				//	report stats and finish up
				//
				groupBoxProps.Text = parents.ToString() + " words, " + mColl.Count.ToString() + " matches";
				trMatches.Sort();
				trMatches.EndUpdate();
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
			}

			return iRet;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// For each directory in the directories' list, query its contents.
		/// </summary>
		public void _Scanner()
		{
			try
			{
				m_bScannerRunning = true;

				//	check for a file filter
				//
				if (0 == edFileFilter.Text.Length)
				{
					_AppendLogText(szERR_No_FileFilter);
				}
				else
				{
					foreach (string item in m_Directories)
					{
						if (false == m_bScannerRunning)
						{
							break;
						}

						//	enumerate directories and files
						//							
						_AppendLogText("Start scanner on [" + item + "] at: " + m_Totals.dtStartTime.ToLongTimeString());

						// _Enum_Path_Files(worker, item);
						_Enum_Path_Files(item);
						_Update_Stats();
					}
				}

				m_Totals.dtStopTime = System.DateTime.Now;

				//	print a report in the logger
				//
				_AppendLogText("Stop scanner at: " + m_Totals.dtStopTime.ToLongTimeString());
				_AppendLogText("(Done in: " + (m_Totals.dtStopTime - m_Totals.dtStartTime).ToString() + ")");
				_AppendLogText("Directories inspected total: " + m_Totals.iDirs.ToString());
				_AppendLogText("Files matching total: " + m_Totals.iFiles.ToString());
				_AppendLogText("Files with grep specified total: " + m_Totals.iGreps.ToString());
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}

			//	here we release resources
			//
			SetTextLabel1(sz_NOTRUNNING_FIND);
			m_bScannerRunning = false;
			_Update_Stats();
			lsFilenames.EndUpdate();
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Scan the disk for files.
		/// </summary>
		/// <param name="szPathname">root pathname</param>
		private void _Enum_Path_Files(string szPathname)
		{
			string[] szDirs = null;		//	enumeration of directories			

			try
			{
				//	update counters and labels
				//
				++m_Totals.iDirs;
				_Update_Stats();
				SetTextLabel1(sz_RUNNING_FIND + szPathname);

				try
				{
					//	get the directories for the given pathname
					//
					szDirs = System.IO.Directory.GetDirectories(szPathname);

				}
				catch (System.IO.DirectoryNotFoundException ce)
				{
					szDirs = null;

					_AppendLogText(ce.Message);
					throw;
				}
				catch (System.IO.IOException ce)
				{
					szDirs = null;

					_AppendLogText(ce.Message);
					throw;
				}

				if (null != szDirs && 0 < szDirs.Length)
				{
					foreach (string szDirectory in szDirs)
					{
						bool bScanDir = true;  //	decide if scan the directory

						//	chech if there's an exclude list
						//
						if (0 < m_ExcludeFilter.Count)
						{
							foreach (string szExclusion in m_ExcludeFilter)
							{
								if (-1 != szDirectory.IndexOf(szExclusion, 0, StringComparison.CurrentCultureIgnoreCase))
								{
									bScanDir = false;
									break;
								}
							}
						}

						if (false == m_bScannerRunning)
						{
							return;
						}

						if (true == bScanDir)
						{
							_Enum_Path_Files(szDirectory);
						}
					}
				}

				//	list of filters is delimited by the ; sign
				//
				string[] szFiles;

				foreach (string szFileFilter in m_FileFilter)
				{
					if (false == m_bScannerRunning)
					{
						return;
					}

					//	get all files in current directory
					//
					szFiles = System.IO.Directory.GetFiles(szPathname, szFileFilter);

					if (null != szFiles && 0 < szFiles.Length)
					{
						if (0 < m_ExcludeFilter.Count)
						{
							//	collected files after exclusion criteria
							//
							string[] szFilesList = new string[szFiles.Length];	//	builds a list of filtered items
							int i = 0;											//  counter
							bool bAddFile;										//	decide if add the file to list

							//	exclude files ...
							//
							foreach (string szCurrent in szFiles)
							{
								bAddFile = true;

								foreach (string szExclusion in m_ExcludeFilter)
								{
									if (-1 != szCurrent.IndexOf(szExclusion, 0, StringComparison.CurrentCultureIgnoreCase))
									{
										bAddFile = false;
										break;
									}
								}

								if (true == bAddFile)
								{
									szFilesList[i++] = szCurrent;
								}
							}

							//	add the filtered file list
							//
							_Add_Files(szFilesList);
							szFilesList = null;
						}
						else
						{
							_Add_Files(szFiles);
							szFiles = null;
						}
					}
				}
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
			}

			_Update_Stats();

			szDirs = null;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Adds an array of full filenames to the list view.
		/// Will retrieve file information for each file.
		/// </summary>
		/// <param name="szFiles">array of filenames</param>
		private void _Add_Files(string[] szFiles)
		{
			if (szFiles == null || szFiles.Length == 0)
			{
				return;
			}

			try
			{
				lsFilenames.BeginUpdate();

				foreach (string szFilename in szFiles)
				{
					//	note that here the array may contain nulls because of the exclusion list
					//
					if (null == szFilename)
					{
						break;
					}

					FileInfo fInfo = new FileInfo(szFilename);
					ListViewItem item = new ListViewItem("?");
					DateTime dtTemp;

					item.Checked = false;
					item.Tag = fInfo.Length.ToString();

					item.SubItems.Add(szFilename);
					item.SubItems.Add(Attributes(fInfo.Attributes));

					if (fInfo.Length > ONE_MEGABYTE)
					{
						item.SubItems.Add((fInfo.Length / ONE_MEGABYTE).ToString() + " Mb");
					}
					else if (fInfo.Length > ONE_KILOBYTE)
					{
						item.SubItems.Add((fInfo.Length / ONE_KILOBYTE).ToString() + " Kb");
					}
					else
					{
						item.SubItems.Add(fInfo.Length.ToString());
					}

					dtTemp = fInfo.CreationTime;
					item.SubItems.Add(dtTemp.ToShortDateString() + " " + dtTemp.ToLongTimeString());
					dtTemp = fInfo.LastAccessTime;
					item.SubItems.Add(dtTemp.ToShortDateString() + " " + dtTemp.ToLongTimeString());
					dtTemp = fInfo.LastWriteTime;
					item.SubItems.Add(dtTemp.ToShortDateString() + " " + dtTemp.ToLongTimeString());

					//	add the item to the list control
					//
					lsFilenames.Items.Add(item);

					//	note that the total list may contain nulls
					//	must increment to add only valid names
					//
					++m_Totals.iFiles;
				}

				lsFilenames.EndUpdate();
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Give a string from a file attribute set
		/// </summary>
		/// <param name="fa">File attribute</param>
		/// <returns>String built with file's attributes</returns>
		private string Attributes(System.IO.FileAttributes fa)
		{
			string szRet = "";

			try
			{
				if ((fa & System.IO.FileAttributes.Archive) > 0)
					szRet += "A";
				if ((fa & System.IO.FileAttributes.Compressed) > 0)
					szRet += "C";
				//				if( (fa & System.IO.FileAttributes.Device) > 0 )
				//					szRet += "d" ;
				if ((fa & System.IO.FileAttributes.Directory) > 0)
					szRet += "D";
				if ((fa & System.IO.FileAttributes.Encrypted) > 0)
					szRet += "E";
				if ((fa & System.IO.FileAttributes.Hidden) > 0)
					szRet += "H";
				if ((fa & System.IO.FileAttributes.Normal) > 0)
					szRet += "N";
				if ((fa & System.IO.FileAttributes.Offline) > 0)
					szRet += "O";
				if ((fa & System.IO.FileAttributes.ReadOnly) > 0)
					szRet += "R";
				if ((fa & System.IO.FileAttributes.System) > 0)
					szRet += "S";
				if ((fa & System.IO.FileAttributes.Temporary) > 0)
					szRet += "T";
			}
			catch (Exception ce)
			{
				_AppendLogText(ce.Message);
			}
			return szRet;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Set up check state for items in the list box
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">The item checked</param>
		private void lbDrives_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if (bReentry == true)
			{
				return;
			}

			bReentry = true;

			try
			{
				if (e.Index == 0)
				{
					//	if the index is 0 the open the directory dialog box
					//	and add a new item to the list
					//
					m_fFolders = new FolderSelect();
					if (DialogResult.OK == m_fFolders.ShowDialog())
					{
						ListViewItem item;

						item = new ListViewItem();
						item.Text = m_fFolders.fullPath;
						item.Checked = true;
						e.NewValue = CheckState.Unchecked;
						m_fFolders = null;
						lbDrives.Items.Add(item);

						//	add the item to the history
						//
						m_FoldersEntries.Add(item.Text);
						m_FoldersEntries.SaveToFile();
					}
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}

			bReentry = false;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Open the File Filter dialog box
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void edFileFilter_DoubleClick(object sender, System.EventArgs e)
		{
			try
			{
				if (m_DlgFileEntries == null)
				{
					m_DlgFileEntries = new frmHistory();
					m_DlgFileEntries.Mainform = this;
					m_DlgFileEntries.Text = "File find history";
					m_DlgFileEntries.UniqueEntries = m_FindEntries;
				}

				m_DlgFileEntries.Show();
				m_DlgFileEntries.BringToFront();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Open the File Grep dialog box
		/// </summary>
		/// <param name="sender">CAller</param>
		/// <param name="e">args</param>
		private void edTextGrep_DoubleClick(object sender, System.EventArgs e)
		{
			try
			{
				if (m_DlgGrepEntries == null)
				{
					m_DlgGrepEntries = new frmHistory();
					m_DlgGrepEntries.Mainform = this;
					m_DlgGrepEntries.Text = "Expressions history";
					m_DlgGrepEntries.UniqueEntries = m_GrepEntries;
				}

				m_DlgGrepEntries.Show();
				m_DlgGrepEntries.BringToFront();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Cancel any pending operation
		/// </summary>
		private void _Reset_Threads()
		{
			bool bMessage = m_bScannerRunning || m_bAnalyzerRunning;

			try
			{
				if (true == m_bScannerRunning)
				{
					m_bScannerRunning = false;
					while (ThreadState.Running == m_ThreadScanner.ThreadState && false == m_ThreadScanner.Join(50))
					{
						Thread.Sleep(2);
					}
					SetTextLabel1(sz_NOTRUNNING_FIND);
				}

				if (true == m_bAnalyzerRunning)
				{
					m_bAnalyzerRunning = false;
					while (ThreadState.Running == m_ThreadAnalyzer.ThreadState && false == m_ThreadAnalyzer.Join(50))
					{
						Thread.Sleep(2);
					}
					SetTextLabel2(sz_NOTRUNNING_GREP);
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}

			if (true == bMessage)
			{
				m_MsgBox.Say("Running threads stopped !");
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Select/Unselect all results
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			int i;
			bool bSelected = false;

			try
			{
				//	nothing to be done
				//
				if (0 == lsFilenames.Items.Count)
				{
					return;
				}

				//	here we can be sure one item exists
				//	check if an item is selected
				//	if so, then choose UNSELECT ALL
				//	otherwise SELECT ALL
				//
				for (i = 0; i < lsFilenames.Items.Count; i++)
				{
					if (true == lsFilenames.Items[i].Selected)
					{
						bSelected = true;
						break;
					}
				}

				//	here we revert the state we've found
				//
				bSelected = !bSelected;

				for (i = 0; i < lsFilenames.Items.Count; i++)
				{
					lsFilenames.Items[i].Selected = bSelected;
				}

			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Run the background threads
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			//	if the user presses the run button whilst doing a scan
			//	then stop the threads
			//
			if (m_bScannerRunning == true || m_bAnalyzerRunning == true)
			{
				_Reset_Threads();
				return;
			}

			try
			{
				edLog.Clear();
				lsFilenames.Items.Clear();
				Cleanup_Editor();
				Cleanup_Tree();
				m_Totals.Reset();

				//	check for a file filter
				//
				if (0 == edFileFilter.Text.Length)
				{
					_AppendLogText(szERR_No_FileFilter);
					return;
				}

				//	get the file filter and the exclude file filter
				//
				m_FileFilter.Assign(edFileFilter.Text);
				m_ExcludeFilter.Assign(edExcludeFilter.Text);

				//	history of what has been searched
				//
				m_FindEntries.Add(edFileFilter.Text);
				m_FindEntries.SaveToFile();

				if (edTextGrep.Text.Length > 0)
				{
					m_GrepEntries.Add(edTextGrep.Text);
					m_GrepEntries.SaveToFile();
				}

				//	count the number of elements that we need to create
				//
				int i = 0;

				foreach (ListViewItem item in lbDrives.Items)
				{
					if (item.Checked == true)
						i++;
				}

				m_Directories = new string[i];
				i = 0;
				foreach (ListViewItem item in lbDrives.Items)
				{
					if (item.Checked == true)
						m_Directories[i++] = item.Text;
				}

				// starts the working threads
				//
				m_ThreadAnalyzer	= new Thread(_Analyzer);
				m_ThreadScanner		= new Thread(_Scanner);

				m_ThreadScanner.Start();
				m_ThreadAnalyzer.Start();

				//  Start the asynchronous operation for both the scanner and the analyzer
				//
				//bkScanner.RunWorkerAsync(this);
				//bkAnalyzer.RunWorkerAsync(this);
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Check which filename has been selected and update the editor and the tree 
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void lsFilenames_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (m_Selection.iSelection > -1 && m_Selection.iSelection < FilesListed())
				{
					lsFilenames.Items[m_Selection.iSelection].BackColor = m_Selection.color;
				}

				if (lsFilenames.SelectedItems.Count > 0)
				{
					ListViewItem item = lsFilenames.SelectedItems[0];
					string szFilename = item.SubItems[1].Text;

					Cursor.Current = Cursors.WaitCursor;

					Cleanup_Editor();
					Cleanup_Tree();
					gbFileFind.Text = "Editor open on: " + szFilename;
					edFile.LoadFile(szFilename, RichTextBoxStreamType.PlainText);

					item.SubItems[0].Text = _Analyze_File(item, true).ToString();
					if (item.SubItems[0].Text != "0")
						item.BackColor = colorMatches;
					else
						item.BackColor = lsFilenames.BackColor;

					m_Selection.iSelection = item.Index;
					m_Selection.color = item.BackColor;
				}
				else
				{
					m_Selection.iSelection = -1;
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
			Cursor.Current = Cursors.Default;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Run the grep utility alone, files are needed ...
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void btnRunGrep_Click(object sender, System.EventArgs e)
		{
			if (m_bScannerRunning == true || m_bAnalyzerRunning == true)
			{
				_Reset_Threads();
				return;
			}

			try
			{
				Cleanup_Editor();
				Cleanup_Tree();

				m_Totals.iFiles = lsFilenames.Items.Count;
				m_Totals.iGreps = 0;

				_Update_Stats();

				foreach (ListViewItem item in lsFilenames.Items)
				{
					item.Checked = false;
					item.Selected = false;
					item.SubItems[0].Text = "?";
					item.BackColor = lsFilenames.BackColor;
				}

				if (edTextGrep.Text.Length > 0)
				{
					m_GrepEntries.Add(edTextGrep.Text);
					m_GrepEntries.SaveToFile();
				}

				//	start the analyzer alone now
				//
				m_ThreadAnalyzer = new Thread(_Analyzer);
				m_ThreadAnalyzer.Start();

			}
			catch (Exception ce)
			{
				_Reset_Threads();
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Update the file filter or the grep text fields, depending on the calling form
		/// </summary>
		/// <param name="which">The calling form</param>
		/// <param name="szText">New text for the entry box</param>
		public void Update_From_History(Form which, string szText)
		{
			if (szText == null || which == null)
				return;

			try
			{
				if (which == m_DlgFileEntries)
				{
					edFileFilter.Text = szText;
				}
				else if (which == m_DlgGrepEntries)
				{
					edTextGrep.Text = szText;
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Cleanup the editor pane
		/// </summary>
		private void Cleanup_Editor()
		{
			try
			{
				edFile.Clear();
				edFile.Refresh();
				iEditorIndex = -1;				//	start of m_Selection
				iEditorLength = -1;				//	length of m_Selection

				gbFileFind.Text = "File find";
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Cleanup the items found pane
		/// </summary>
		private void Cleanup_Tree()
		{
			try
			{
				groupBoxProps.Text = sz_DEF_GREP_TEXT;
				foreach (TreeNode node in trMatches.Nodes)
				{
					if (node.Tag != null)
						node.Tag = null;
				}
				trMatches.Nodes.Clear();
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.Message);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Catch the key press event and see if the user has requested a file delete
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">args</param>
		private void lsFilenames_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			string szFilename;
			FileInfo fInfo;
			bool bAsked = false;

			if (e.KeyData != Keys.Delete)
			{
				return;
			}

			try
			{
				while (lsFilenames.SelectedIndices.Count > 0)
				{
					szFilename = lsFilenames.Items[lsFilenames.SelectedIndices[0]].SubItems[1].Text;
					fInfo = new FileInfo(szFilename);

					if (false == bAsked)
					{
						bAsked = true;
						if (false == m_MsgBox.Ask(sz_ASK_DELETE + szFilename))
						{
							return;
						}
					}

					fInfo.Delete();
					lsFilenames.Items.RemoveAt(lsFilenames.SelectedIndices[0]);
					_AppendLogText("Deleted: " + szFilename);
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}

		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Get the match item in the tree and update the editor pane, selecting text
		/// </summary>
		/// <param name="sender">Caller</param>
		/// <param name="e">Args</param>
		private void trMatches_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			try
			{
				//	restore old color if some text has been selected
				//
				if (iEditorIndex != -1)
				{
					edFile.Select(iEditorIndex, iEditorLength);
					edFile.SelectionColor = colorEdNormal;

					iEditorIndex = iEditorLength = -1;
				}

				if (e.Node.Tag != null)
				{
					Match match;

					match = (Match)e.Node.Tag;
					iEditorIndex = match.Index;
					iEditorLength = match.Length;

					edFile.Select(iEditorIndex, iEditorLength);
					edFile.SelectionBackColor = colorEdSelect;
				}
			}
			catch (InvalidCastException)
			{
				//  do nothing with this ...
				//
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Executes the file find when the user presses enter on the entry box.
		/// </summary>
		/// <param name="sender">passed on</param>
		/// <param name="e">passed on</param>		
		void EdFileFilterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 0x0d || e.KeyChar == 0x0a)
			{
				btnRun_Click(sender, e);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Executes the grep when the user presses enter on the entry box.
		/// </summary>
		/// <param name="sender">passed on</param>
		/// <param name="e">passed on</param>
		void EdTextGrepKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 0x0d || e.KeyChar == 0x0a)
			{
				btnRun_Click(sender, e);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Check which encondig to use.
		/// </summary>
		/// <param name="sender">ignored</param>
		/// <param name="e">ignored</param>
		private void EncodingOptions_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (true == rbm_encoder_ASCII.Checked)
				{
					m_encoder = System.Text.Encoding.ASCII;
				}
				else if (true == rbm_encoder_BigEU.Checked)
				{
					m_encoder = System.Text.Encoding.BigEndianUnicode;
				}
				else if (true == rbm_encoder_Unicode.Checked)
				{
					m_encoder = System.Text.Encoding.Unicode;
				}
				else if (true == rbm_encoder_UTF32.Checked)
				{
					m_encoder = System.Text.Encoding.UTF32;
				}
				else if (true == rbm_encoder_UTF7.Checked)
				{
					m_encoder = System.Text.Encoding.UTF7;
				}
				else if (true == rbm_encoder_UTF8.Checked)
				{
					m_encoder = System.Text.Encoding.UTF8;
				}
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Automatically called when an item is checked, will rebuild the regex options.
		/// Doesn't accept changes if the analyzer is already active.
		/// </summary>
		/// <param name="sender">ignored</param>
		/// <param name="e">new value</param>
		private void lbRegexOptions_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (true == m_bAnalyzerRunning)
			{
				//	disallow change if already running the analyzer
				//
				e.NewValue = e.CurrentValue;
				_AppendLogText("Cannot apply change at the moment !");
			}
			else
			{
				//	ask to rebuild the object
				//
				m_GrepFilter = null;
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Check if the user presses the delete key, then remove any selected list entry.
		/// If the item is a system drive the procedure stops with an error.
		/// </summary>
		/// <param name="sender">ignored</param>
		/// <param name="e">key code</param>
		private void lbDrives_KeyUp(object sender, KeyEventArgs e)
		{
			string szFilename;

			if (e.KeyData != Keys.Delete)
			{
				return;
			}

			while (lbDrives.SelectedIndices.Count > 0)
			{
				try
				{
					//	for each item selected, remove it
					//
					szFilename = lbDrives.Items[lbDrives.SelectedIndices[0]].SubItems[0].Text;

					if (0 <= m_FoldersEntries.IndexOf(szFilename))
					{
						m_FoldersEntries.Remove(szFilename);
						lbDrives.Items.RemoveAt(lbDrives.SelectedIndices[0]);
					}
					else
					{
						m_MsgBox.Say("Cannot delete System Drive");
						break;
					}

					_AppendLogText("Removed: " + szFilename);
				}
				catch (Exception ce)
				{
					m_MsgBox.Say(ce.ToString());
					break;
				}
			}

			//	record changes
			//
			m_FoldersEntries.SaveToFile();
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Resize controls
		/// </summary>
		/// <param name="sender">ignored</param>
		/// <param name="e">ignored</param>
		private void panel2_ClientSizeChanged(object sender, EventArgs e)
		{
			try
			{
				edLog.Left = lbRegexOptions.Right;
				edLog.Width = panel2.Width - edLog.Left;
			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Executes the highlighed item, NOT IMPLEMENTED.
		/// </summary>
		/// <param name="sender">ignored</param>
		/// <param name="e"></param>
		private void lsFilenames_DoubleClick(object sender, EventArgs e)
		{
			try
			{

			}
			catch (Exception ce)
			{
				m_MsgBox.Say(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Provides sorting for the file list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lsFilenames_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (true == m_ListSorter.Enable)
			{
				Cursor.Current = Cursors.WaitCursor;

				m_ListSorter.SetColumn(e.Column);
				lsFilenames.Sort();

				Cursor.Current = Cursors.Default;
			}
		}
	}

	//-----------------------------------------------------------------------------
	//-----------------------------------------------------------------------------

	// Implements the manual sorting of items by columns.
	class ListViewItemComparer : IComparer
	{
		enum eSortOrder { eSortAsc, eSortDesc } ;

		private eSortOrder sort = eSortOrder.eSortAsc;
		private int col;
		private bool bEnable = true;

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public ListViewItemComparer()
		{
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Store the current column selected
		/// clicking twice the same column toggles the ordering (asc/desc)
		/// </summary>
		/// <param name="column">the column clicked by the user</param>
		public void SetColumn(int column)
		{
			if (column == col)
			{
				//	toggle sort ordering
				//
				if (eSortOrder.eSortAsc == sort)
				{
					sort = eSortOrder.eSortDesc;
				}
				else
				{
					sort = eSortOrder.eSortAsc;
				}
			}

			col = column;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// The enable status for ordering items
		/// </summary>
		public bool Enable
		{
			set
			{
				bEnable = value;
			}

			get
			{
				return bEnable;
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Perform the ordering based on the user selection
		/// </summary>
		/// <param name="x">left item</param>
		/// <param name="y">right item</param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			if (false == bEnable)
			{
				return 0;
			}

			string szLeft = ((ListViewItem)x).SubItems[col].Text;
			string szRight = ((ListViewItem)y).SubItems[col].Text;

			//	check for troubles with numeric conversion
			//
			if ("?" == szLeft)
			{
				szLeft = "0";
			}

			if ("?" == szRight)
			{
				szRight = "0";
			}

			//	descending
			//
			if (eSortOrder.eSortDesc == sort)
			{
				if (0 == col)
				{
					return int.Parse(szRight) - int.Parse(szLeft);
				}

				return String.Compare(szRight, szLeft);
			}

			//	ascending
			//
			if (0 == col)
			{
				return int.Parse(szLeft) - int.Parse(szRight);
			}

			return String.Compare(szLeft, szRight);
		}
	}

	//-----------------------------------------------------------------------------
	//-----------------------------------------------------------------------------

}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------



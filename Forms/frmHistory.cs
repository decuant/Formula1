//--------------------------------------------------------------------------------------
// File: frmHistory.cs
//
// Hisory form.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;

namespace Formula1
{
	/// <summary>
	/// Lists history entries.
	/// </summary>
	public class frmHistory : System.Windows.Forms.Form
	{
		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox lbHistory;

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		private frmMain fMainform = null;   //	owner form
		private clsUniqueEntries m_uniqueEntries;

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public frmHistory()
		{
			InitializeComponent();
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public frmMain Mainform
		{
			set
			{
				fMainform = value;

				try
				{
					if (null != fMainform)
					{
						this.Owner = fMainform;
					}
				}
				catch (Exception ce)
				{
					MessageBox.Show(ce.ToString());
				}
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		#region Windows Form Designer generated code
		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHistory));
			this.lbHistory = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lbHistory
			// 
			this.lbHistory.BackColor = System.Drawing.Color.DarkSlateGray;
			this.lbHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lbHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbHistory.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbHistory.ForeColor = System.Drawing.Color.White;
			this.lbHistory.IntegralHeight = false;
			this.lbHistory.ItemHeight = 23;
			this.lbHistory.Location = new System.Drawing.Point(0, 0);
			this.lbHistory.Name = "lbHistory";
			this.lbHistory.Size = new System.Drawing.Size(320, 500);
			this.lbHistory.TabIndex = 0;
			this.lbHistory.DoubleClick += new System.EventHandler(this.lbHistory_DoubleClick);
			this.lbHistory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lbHistory_KeyUp);
			// 
			// frmHistory
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(13, 23);
			this.ClientSize = new System.Drawing.Size(320, 500);
			this.Controls.Add(this.lbHistory);
			this.Font = new System.Drawing.Font("Liberation Mono", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmHistory";
			this.Opacity = 0.9D;
			this.Text = "History";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmHistory_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmHistory_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (null != fMainform)
			{
				fMainform.Closing_Child(this);
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// assing the history list
		/// (container for history items read from file)
		/// </summary>
		public clsUniqueEntries UniqueEntries
		{
			get
			{
				return m_uniqueEntries;
			}

			set
			{
				m_uniqueEntries = value;

				lbHistory.Items.Clear();

				if (null != m_uniqueEntries)
				{
					foreach (string szItem in m_uniqueEntries)
					{
						lbHistory.Items.Add(szItem);
					}
				}
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbHistory_DoubleClick(object sender, System.EventArgs e)
		{
			try
			{
				if (null != fMainform && 0 < lbHistory.SelectedItems.Count)
				{
					string selText = lbHistory.SelectedItems[0].ToString();

					if (0 < selText.Length)
					{
						fMainform.Update_From_History(this, selText);
					}
				}
			}
			catch (Exception ce)
			{
				MessageBox.Show(ce.ToString());
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbHistory_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
			{
				if (lbHistory.SelectedIndex <= -1)
				{
					return;
				}

				try
				{
					if (null != m_uniqueEntries && lbHistory.SelectedIndex < m_uniqueEntries.Count)
					{
						m_uniqueEntries.RemoveAt(lbHistory.SelectedIndex);
						m_uniqueEntries.SaveToFile();
					}

					lbHistory.Items.RemoveAt(lbHistory.SelectedIndex);
				}
				catch (Exception ce)
				{
					MessageBox.Show(ce.ToString());
				}
			}
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

	}

	//-----------------------------------------------------------------------------
	//-----------------------------------------------------------------------------
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------

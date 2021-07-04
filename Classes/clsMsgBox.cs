//--------------------------------------------------------------------------------------
// File: clsMsgBox.cs
//
// Shows message box.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System ;
using System.Windows.Forms ;

namespace Formula1
{
	/// <summary>
	/// Msg box helpers
	/// </summary>
	public sealed class clsMsgBox
	{
		private Form m_frmCaller = null;
		private string m_szAppTitle = "Application";

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="frm">Calling form</param>
		/// <param name="szApplication">Default message box title</param>
		public clsMsgBox(Form frm, string szApplication)
		{
			m_frmCaller = frm;
			m_szAppTitle = szApplication;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Static version of the Say method, display a simple message box
		/// </summary>
		/// <param name="szText"></param>
		/// <param name="szTitle"></param>
		public static void Say(string szText, string szTitle)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(null, szText, szTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Display a simple message box
		/// </summary>
		/// <param name="szText">The message for the user</param>
		public void Say(string szText)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(m_frmCaller, szText, m_szAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Display a question message box
		/// </summary>
		/// <param name="szText">The text for the question</param>
		/// <returns>True if the user selects the yes button, false otherwise</returns>
		public bool Ask(string szText)
		{
			Cursor.Current = Cursors.Default;
			DialogResult ret = MessageBox.Show(m_frmCaller, szText, m_szAppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			return (ret == DialogResult.Yes);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Display an error message box
		/// </summary>
		/// <param name="szText">Text for the message box</param>
		public void Error(string szText)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(m_frmCaller, szText, m_szAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Display an exception
		/// </summary>
		/// <param name="ce">An exception to show</param>
		public void Exception(Exception ce)
		{
			string szMessage;

			Cursor.Current = Cursors.Default;

			szMessage = "Eccezione : " + ce.Message + "\nSorgente : " + ce.Source + "\nStack Trace :\n" + ce.StackTrace;
			MessageBox.Show(m_frmCaller, szMessage, m_szAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
	}
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------


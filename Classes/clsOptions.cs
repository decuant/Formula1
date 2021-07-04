//--------------------------------------------------------------------------------------
// File: clsOptions.cs
//
// Handling of application's options.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Serialization;

namespace Formula1
{
	/// <summary>
	/// Program options.
	/// </summary>
	public sealed class clsOptions
	{
		public const string sz_FILENAME = "options.xml";
		private string szFullFilename = sz_FILENAME;

		private int iSplitter1 = 70;
		private int iSplitter2 = 100;
		private int iSplitter3 = 100;
		private int[] iColumnSize = { 35, 450, 45, 70, 120, 120, 120 };

		private int iWinLeft;
		private int iWinTop;
		private int iWinWidth = 600;
		private int iWinHeight = 400;
		private int iWinState;

		private string szFileFilter = "*.*";
		private string szFileGrep = "";

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public clsOptions()
		{
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public bool Load_Options(string szAppPath)
		{
			try
			{
				szFullFilename = szAppPath + "\\" + sz_FILENAME;

				XmlSerializer serializer = new XmlSerializer(typeof(clsOptions));
				StreamReader fs = new StreamReader(szFullFilename);
				clsOptions options = new clsOptions();

				options = (clsOptions)serializer.Deserialize(fs);
				fs.Close();

				iSplitter1 = options.iSplitter1;
				iSplitter2 = options.iSplitter2;
				iSplitter3 = options.iSplitter3;
				iColumnSize = options.iColumnSize;
				iWinLeft = options.iWinLeft;
				iWinTop = options.iWinTop;
				iWinWidth = options.iWinWidth;
				iWinHeight = options.iWinHeight;
				iWinState = options.iWinState;
				szFileFilter = options.szFileFilter;
				szFileGrep = options.szFileGrep;

			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception exc)
			{
				clsMsgBox.Say(exc.Message, "Exception");
				return false;
			}

			return true;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public bool Save_Options()
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(clsOptions));
				StreamWriter writer = new StreamWriter(szFullFilename, false);

				serializer.Serialize(writer, this);
				writer.Close();
			}
			catch (Exception exc)
			{
				clsMsgBox.Say(exc.Message, "Exception");
				return false;
			}

			return true;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		static int _Force_TrueFalse(int aValue)
		{
			if (aValue != 0 && aValue != 1 && aValue != -1)
				return 0;
			return aValue;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		static int _Force_GT(int aValue, int aMinimum)
		{
			return ((aValue < aMinimum) ? aMinimum : aValue);
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Splitter1
		{
			get { return iSplitter1; }
			set { iSplitter1 = _Force_GT(value, 70); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Splitter2
		{
			get { return iSplitter2; }
			set { iSplitter2 = _Force_GT(value, 80); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Splitter3
		{
			get { return iSplitter3; }
			set { iSplitter3 = _Force_GT(value, 100); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize0
		{
			get { return iColumnSize[0]; }
			set { iColumnSize[0] = _Force_GT(value, 35); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize1
		{
			get { return iColumnSize[1]; }
			set { iColumnSize[1] = _Force_GT(value, 450); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize2
		{
			get { return iColumnSize[2]; }
			set { iColumnSize[2] = _Force_GT(value, 45); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize3
		{
			get { return iColumnSize[3]; }
			set { iColumnSize[3] = _Force_GT(value, 70); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize4
		{
			get { return iColumnSize[4]; }
			set { iColumnSize[4] = _Force_GT(value, 120); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize5
		{
			get { return iColumnSize[5]; }
			set { iColumnSize[5] = _Force_GT(value, 120); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int ColumnSize6
		{
			get { return iColumnSize[6]; }
			set { iColumnSize[6] = _Force_GT(value, 120); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Win_Left
		{
			get { return iWinLeft; }
			set { iWinLeft = _Force_GT(value, 0); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Win_Top
		{
			get { return iWinTop; }
			set { iWinTop = _Force_GT(value, 0); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Win_Width
		{
			get { return iWinWidth; }
			set { iWinWidth = _Force_GT(value, 100); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Win_Height
		{
			get { return iWinHeight; }
			set { iWinHeight = _Force_GT(value, 100); }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public int Win_State
		{
			get { return iWinState; }
			set { iWinState = value; }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public string FileFilter
		{
			get { return szFileFilter; }
			set { szFileFilter = value; }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public string FileGrep
		{
			get { return szFileGrep; }
			set { szFileGrep = value; }
		}

	}
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------

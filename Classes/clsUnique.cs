//--------------------------------------------------------------------------------------
// File: clsUnique.cs
//
// Manages unique list entries.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace Formula1
{

	//-----------------------------------------------------------------------------
	//-----------------------------------------------------------------------------

	/// <summary>
	/// Descrizione di riepilogo per clsUniqueEntries.
	/// </summary>
	public sealed class clsUniqueEntries : ArrayList
	{
		private string szFilename = "uniqueEntries.xml";

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public clsUniqueEntries()
		{

		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override int Add(object value)
		{
			try
			{
				if (-1 == IndexOf(value))
				{
					base.Add(value);
				}
			}
			catch (Exception)
			{
			}

			return Count;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public string Filename
		{
			get { return szFilename; }
			set { szFilename = value; }
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool SaveToFile()
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(clsUniqueEntries));
				StreamWriter writer = new StreamWriter(szFilename, false);

				serializer.Serialize(writer, this);
				writer.Close();
				return true;
			}
			catch (Exception)
			{
			}

			return false;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool LoadFromFile()
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(clsUniqueEntries));
				StreamReader reader = new StreamReader(szFilename);
				clsUniqueEntries dictionary = (clsUniqueEntries)serializer.Deserialize(reader);
				reader.Close();

				Clear();
				foreach (string szElem in dictionary)
				{
					Add(szElem);
				}

				return true;
			}
			catch (Exception)
			{
			}

			return false;
		}
	}
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------


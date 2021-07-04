//--------------------------------------------------------------------------------------
// File: clsPreparser.cs
//
//	Format buffer adding or stripping chars. 
//	Heuristical text file recognition.
//	Experimental heuristical binary file recognition.
//
// decuant@protonmail.com
//--------------------------------------------------------------------------------------

using System;

namespace Formula1
{
	public sealed class clsPreparser
	{
		private byte[] m_Array;			//	holder for trated data
		private const int m_MinTextChars = 15;				//	minimum number of chars for text recognition
		private const int m_AllowErros = 5;
		private int[,] m_BinaryPatterns = 
		{ 					
			{ 0x4d,0x5a,0x90,0x00,0x03,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xff,0xff },	//			mz ...	exec
			{ 0x42,0x4d,0xe6,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x36,0x00,0x00,0x00 },	//	+/- 2	bm ...	bmp file
			{ 0x00,0x00,0x01,0x00,0x01,0x00,0x20,0x20,0x00,0x00,0x01,0x00,0x08,0x00 },	//					ico file
			{ 0x47,0x49,0x46,0x38,0x39,0x61,0xbc,0x00,0x26,0x01,0x80,0x00,0x00,0x00 },	//	+/-5	GIF89a	gif file
			{ 0x47,0x49,0x46,0x38,0x39,0x61,0x52,0x02,0x3d,0x00,0xd5,0x00,0x00,0x00 },	//
			{ 0xff,0xd8,0xff,0xe0,0x00,0x10,0x4a,0x46,0x49,0x46,0x00,0x01,0x01,0x01 },	//					jpg file
			{ 0x4d,0x5a,0x8e,0x01,0x01,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xff,0xff },	//			mz ...	16 bit dll
			{ 0x4d,0x5a,0x90,0x00,0x03,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xff,0xff },	//			mz ...	32 bit dll
			{ 0x49,0x54,0x53,0x46,0x03,0x00,0x00,0x00,0x60,0x00,0x00,0x00,0x01,0x00 },	//			it ...	chm help file
			{ 0x3f,0x5f,0x03,0x00,0x3f,0x04,0x00,0x00,0xff,0xff,0xff,0xff,0xf0,0x84 }	//			?- ...	hlp help file
		};

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public clsPreparser()
		{

		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Check if a buffer matches almost n characters against a known pattern.
		/// </summary>
		/// <param name="inBuffer">Memory to check</param>
		/// <param name="inPattern">Pattern number</param>
		/// <param name="inCharsToCheck">Number of characters to use for the check</param>
		/// <returns>If the match succeeded</returns>
		private bool IsMatchingPattern(byte[] inBuffer, int inPattern, int inCharsToCheck)
		{
			int iMatches = 0;

			try
			{
				for (int i = 0; i < inCharsToCheck; i++)
				{
					if (inBuffer[i] == m_BinaryPatterns[inPattern, i])
					{
						++iMatches;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return (iMatches >= (inCharsToCheck - m_AllowErros));
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Try a match against known binaries.
		/// </summary>
		/// <param name="inBuffer">Buffer read from file</param>
		/// <returns>If any match was found</returns>
		public bool IsKnownBinary(byte[] inBuffer)
		{
			try
			{
				int maxSize = Math.Min(inBuffer.GetUpperBound(0), m_BinaryPatterns.GetLength(1));

				for (int pattern = 0; maxSize > 0 && pattern < m_BinaryPatterns.GetLength(0); pattern++)
				{
					if (true == IsMatchingPattern(inBuffer, pattern, maxSize))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return false;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Check if the very first bytes of a file are almost text or almost binary.
		/// </summary>
		/// <param name="inBuffer">Buffer read from file</param>
		/// <returns>If the match succeded</returns>
		public bool IsText(byte[] inBuffer)
		{
			int matches = 0;
			int minBytes = 0;

			try
			{
				//	pass a constant here to get a new minBytes value
				//
				minBytes = Math.Min(inBuffer.GetUpperBound(0), m_MinTextChars);

				for (int i = 0; i < minBytes; i++)
				{
					if (0x1f >= inBuffer[i])
					{
						if (0x0d == inBuffer[i] || 0x0a == inBuffer[i])
						{
							++matches;
						}
						continue;
					}

					if (0xb0 <= inBuffer[i])
					{
						continue;
					}

					++matches;
				}
			}
			catch (Exception)
			{
				throw;
			}

			//	can be a match against  negative value
			//
			return (matches > (minBytes - m_AllowErros));
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public byte[] AddLineFeed(byte[] inBuffer)
		{
			int bufSize = inBuffer.GetUpperBound(0);
			bool bCrFound = false;
			bool bLfFound = false;
			int y = 0;
			byte[] temp = null;

			if (0 == bufSize)
			{
				return inBuffer;
			}

			temp = new byte[bufSize * 2];

			for (int i = 0; i < bufSize; i++)
			{
				if (0x0d == inBuffer[i])
				{
					if (true == bCrFound && false == bLfFound)
					{
						temp[y++] = 0x0a;
						bLfFound = true;
					}

					bCrFound = true;
				}

				if (0x0a == inBuffer[i])
				{
					bLfFound = true;
					bCrFound = false;
				}

				temp[y++] = inBuffer[i];
			}

			m_Array = new byte[y + 1];
			for (int i = 0; i < y; i++)
			{
				m_Array[i] = temp[i];
			}

			return m_Array;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------

		public byte[] StripCarriages(byte[] inBuffer)
		{
			try
			{
				int bufSize = inBuffer.GetUpperBound(0);
				int y = 0;

				//	arrays are initialized with zeros by default
				//	so there is no need to pad them
				//	( not only when in debug mode )
				//
				m_Array = new byte[bufSize];

				for (int i = 0; i < bufSize; i++)
				{
					if (0x0d == inBuffer[i])
					{
						continue;
					}

					m_Array[y++] = inBuffer[i];
				}
			}
			catch (Exception)
			{
				throw;
			}

			return m_Array;
		}

		//-----------------------------------------------------------------------------
		//-----------------------------------------------------------------------------
	}
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------


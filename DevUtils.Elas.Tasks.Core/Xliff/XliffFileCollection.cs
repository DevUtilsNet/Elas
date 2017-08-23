using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> Collection of xliff files. This class cannot be inherited. </summary>
	public sealed class XliffFileCollection : IEnumerable<XliffFile>
	{
		private readonly XliffDocument _document;
		private readonly List<XliffFile> _xliffFiles = new List<XliffFile>();

		/// <summary> Constructor. </summary>
		///
		/// <param name="document"> The document. </param>
		public XliffFileCollection(XliffDocument document)
		{
			_document = document;
		}

		/// <summary> Adds xliffUnit. </summary>
		///
		/// <param name="xliffFile"> The xliff unit to add. </param>
		public void Add(XliffFile xliffFile)
		{
			xliffFile.SetParent(_document);
			_xliffFiles.Add(xliffFile);

			_document.IsDirty = true;
		}

		/// <summary> Clears this object to its blank/initial state. </summary>
		public void Clear()
		{
			if (_xliffFiles.Count > 0)
			{
				_document.IsDirty = true;

				foreach (var item in this)
				{
					item.SetParent(null);
				}
				_xliffFiles.Clear();
			}
		}

		/// <summary> Creates a file. </summary>
		///
		/// <param name="original">			 	Original file - The original specifies the name of the original
		/// 															file from which the contents of a file has been extracted. </param>
		/// <param name="sourceLanguage">	The language for the <source/> elements in the given
		/// 															<see cref="XliffFile"/>. </param>
		/// <param name="targetLanguage"> The language for the <see cref="XliffTarget"/> class in the given <see cref="XliffFile"/> class. </param>
		/// <param name="datatype">			 	The datatype attribute specifies the kind of text contained in
		/// 															the <see cref="XliffFile"/>. Depending on that type, you may
		/// 															apply different processes to the data. For example:
		/// 															<paramref name="datatype"/>=<see cref="XliffDataType.Winres"/>
		/// 															specifies that the content is Windows resources which would allow
		/// 															using the Win32 API in rendering the content. </param>
		///
		/// <returns> The or create file. </returns>
		public XliffFile GetOrCreateFile(string original, CultureInfo sourceLanguage, CultureInfo targetLanguage, XliffDataType datatype)
		{
			var ret = this.FirstOrDefault(f =>
				f.Original.Equals(original, StringComparison.InvariantCultureIgnoreCase) &&
				Equals(f.SourceLanguage, sourceLanguage) &&
				Equals(f.TargetLanguage, targetLanguage) &&
				f.DataType == datatype);

			if (ret == null)
			{
				ret = _document.CreateFile(original, sourceLanguage, targetLanguage, datatype);
				Add(ret);
			}
			return ret;
		}

		/// <summary> Gets the enumerator. </summary>
		///
		/// <returns> The enumerator. </returns>
		public IEnumerator<XliffFile> GetEnumerator()
		{
			var ret = _xliffFiles.GetEnumerator();
			return ret;
		}

		/// <summary> Gets the enumerator. </summary>
		///
		/// <returns> The enumerator. </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

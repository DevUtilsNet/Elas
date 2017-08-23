using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Xml.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> Collection of xliff units. This class cannot be inherited. </summary>
	public sealed class XliffUnitCollection : IEnumerable<XliffUnit>
	{
		private readonly XliffBase _owner;
		private readonly XliffDocument _document;
		private readonly List<XliffUnit> _xliffUnits = new List<XliffUnit>();

		/// <summary> Gets the owner. </summary>
		///
		/// <value> The owner. </value>
		public XliffBase Owner
		{
			get { return _owner; }
		}

		/// <summary> Default constructor. </summary>
		///
		/// <param name="owner">	  The owner. </param>
		/// <param name="document"> . </param>
		public XliffUnitCollection(XliffBase owner, XliffDocument document)
		{
			_owner = owner;
			_document = document;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="xmlReader"> The XML reader. </param>
		/// <param name="owner">		 The owner. </param>
		/// <param name="document">  . </param>
		internal XliffUnitCollection(XmlReader xmlReader, XliffBase owner, XliffDocument document)
		{
			_owner = owner;
			_document = document;

			if (xmlReader.IsEmptyElement)
			{
				xmlReader.Read();
				return;
			}

			var depth = xmlReader.CreateDepthControl();

			xmlReader.Read();
			while (depth.Above)
			{
				if (xmlReader.IsStartElement("group", XliffDocument.Namespace))
				{
					Add(_document.CreateGroup(xmlReader));
					continue;
				}

				xmlReader.CheckElement("trans-unit", XliffDocument.Namespace);

				Add(_document.CreateTransUnit(xmlReader));
			}

			xmlReader.Read();
		}

		/// <summary> Adds xliffUnit. </summary>
		///
		/// <param name="xliffUnit"> The xliff unit to add. </param>
		public void Add(XliffUnit xliffUnit)
		{
			xliffUnit.SetParent(_owner);
			_xliffUnits.Add(xliffUnit);
			_document.IsDirty = true;
		}

		/// <summary> Gets transaction unit. </summary>
		///
		/// <param name="id">	Is used in many elements as a reference to the original corresponding code
		/// 									data or format for the given element. </param>
		///
		/// <returns> The transaction unit. </returns>
		public XliffTransUnit GetTransUnit(string id)
		{
			var ret = this.OfType<XliffTransUnit>().FirstOrDefault(f => f.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
			return ret;
		}

		/// <summary> Gets or add transaction unit. </summary>
		///
		/// <param name="id">	Is used in many elements as a reference to the original corresponding code
		/// 									data or format for the given element. </param>
		///
		/// <returns> The or add transaction unit. </returns>
		public XliffTransUnit GetOrAddTransUnit(string id)
		{
			var ret = GetTransUnit(id);

			if (ret == null)
			{
				ret = _document.CreateTransUnit(id);
				Add(ret);
			}
			return ret;
		}

		/// <summary> Gets a group. </summary>
		///
		/// <param name="id">	Is used in many elements as a reference to the original corresponding code
		/// 									data or format for the given element. </param>
		///
		/// <returns> The group. </returns>
		public XliffGroup GetGroup(string id)
		{
			var ret = this.OfType<XliffGroup>().FirstOrDefault(f => f.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
			return ret;
		}

		/// <summary> Gets or add group. </summary>
		///
		/// <param name="id">	Is used in many elements as a reference to the original corresponding code
		/// 									data or format for the given element. </param>
		///
		/// <returns> The or add group. </returns>
		public XliffGroup GetOrAddGroup(string id)
		{
			var ret = GetGroup(id);

			if (ret == null)
			{
				ret = _document.CreateGroup(id);
				Add(ret);
			}
			return ret;
		}

		/// <summary> Writes the given XML writer. </summary>
		///
		/// <param name="xmlWriter"> The XML writer to write. </param>
		internal void Write(XmlWriter xmlWriter)
		{
			foreach (var item in this)
			{
				item.Write(xmlWriter);
			}
		}

		/// <summary> Gets the enumerator. </summary>
		///
		/// <returns> The enumerator. </returns>
		public IEnumerator<XliffUnit> GetEnumerator()
		{
			var ret = _xliffUnits.GetEnumerator();
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
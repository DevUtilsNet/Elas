using System;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> An xliff unit. </summary>
	public abstract class XliffUnit : XliffBase
	{
		private string _id;
		private XliffResType? _resType;

		/// <summary>  The <see cref="Id"/> is used in many elements as a reference to the original corresponding code data or format for the given element. </summary>
		///
		/// <value> The identifier. </value>
		public string Id
		{
			get { return _id; }
			set
			{
				if (_id != value)
				{
					Dirty();
				}
				_id = value;
			}
		}

		/// <summary> Gets or sets the type of the resource. </summary>
		///
		/// <value> The type of the resource. </value>
		public XliffResType? ResType
		{
			get { return _resType; }
			set
			{
				if (_resType != value)
				{
					Dirty();
				}
				_resType = value;
			}
		}

		internal XliffUnit(string id, XliffDocument document)
			: base(document)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			Id = id;
		}

		internal XliffUnit(XmlReader xmlReader, XliffDocument document)
			: base(document)
		{
			Id = xmlReader.GetAttribute("id");

			var restype = xmlReader.GetAttribute("restype");
			if (restype != null)
			{
				ResType = restype.EnumFromStringValue<XliffResType>();
			}
		}

		internal virtual void Write(XmlWriter xmlWriter)
		{
			if (ResType.HasValue)
			{
				xmlWriter.WriteAttributeString("restype", ResType.GetStringValue());
			}
		}
	}
}
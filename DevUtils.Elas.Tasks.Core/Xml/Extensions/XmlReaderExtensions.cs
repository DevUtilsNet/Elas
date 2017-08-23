using System.Reflection;
using System.Xml;
using DevUtils.Elas.Tasks.Core.Reflection.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xml.Extensions
{
	static class XmlReaderExtensions
	{
		private static readonly MethodInfo CheckElementM;

		static XmlReaderExtensions()
		{
			CheckElementM = typeof(XmlReader).GetMethod("CheckElement", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static string QueryAttribute(this XmlReader xmlReader, string name)
		{
			var ret = xmlReader.GetAttribute(name);
			if (ret == null)
			{
				var xmlLineInfo = ((IXmlLineInfo) xmlReader);
				throw new XmlException(string.Format("Attribute \"{0}\" expected.", name), null, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
			}
			return ret;
		}

		public static void CheckAttributeValue(this XmlReader xmlReader, string name, string value)
		{
			var val = xmlReader.QueryAttribute(name);
			if (val != value)
			{
				var xmlLineInfo = ((IXmlLineInfo)xmlReader);
				throw new XmlException(string.Format("Value \"{0}\" of attribute \"{1}\" expected.", value, name), null, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
			}
		}

		public static void CheckElement(this XmlReader xmlReader, string localname, string ns)
		{
			CheckElementM.InvokePreserveStackTrace(xmlReader, localname, ns);
		}

		public static XmlReaderDepthControl CreateDepthControl(this XmlReader xmlReader)
		{
			var ret = new XmlReaderDepthControl(xmlReader);
			return ret;
		}
	}
}

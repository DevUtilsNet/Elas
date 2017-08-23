using System.Xml;

namespace DevUtils.Elas.Tasks.Core.Xml
{
	sealed class XmlReaderDepthControl
	{
		private readonly int _depth;
		private readonly XmlReader _xmlReader;

		public bool Above
		{
			get
			{
				var ret = _xmlReader.Depth > _depth;
				return ret;
			}
		}

		public XmlReaderDepthControl(XmlReader xmlReader)
		{
			_xmlReader = xmlReader;
			_depth = _xmlReader.Depth;
		}

		public override string ToString()
		{
			var ret = string.Format("{0} -> {1}", _depth, _xmlReader.Depth);
			return ret;
		}
	}
}
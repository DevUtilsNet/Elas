using System;

namespace DevUtils.Elas.Tasks.Core.Xliff
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	sealed class StringValueAttribute : Attribute
	{
		public string Value { get; private set; }

		public StringValueAttribute(string value)
		{
			Value = value;
		}
	}
}

using System;

namespace DevUtils.Elas.Tasks.Core.Loyc
{
	[Flags()]
	enum EscapeC
	{
		Minimal = 0,  // Only \r, \n, \0 and backslash are escaped.
		Default = Control | Quotes,
		Unicode = 2,  // Escape all characters with codes above 255 as \uNNNN
		NonAscii = 1, // Escape all characters with codes above 127 as \xNN
		Control = 4,  // Escape all characters with codes below 32  as \xNN, and also \t
		ABFV = 8,     // Use \a \b \f and \v (overrides \xNN)
		DoubleQuotes = 16, // Escape double quotes as \"
		SingleQuotes = 32, // Escape single quotes as \'
		Quotes = 48,
	}
}

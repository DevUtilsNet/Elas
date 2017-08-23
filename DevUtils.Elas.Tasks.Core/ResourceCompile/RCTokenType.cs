namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	enum RCTokenType
	{
		EOF, // = "<EOF>";
		Newline, // = "<Newline>";
		SpacesOrComments, // = "<SpaceOrComments>";
		Comma, // = ",";
		LBrace, // = "{";
		RBrace, // = "}";
		String, // = "<String>";
		Number, // = "<Number>";
		RString, // = "<RString>"; // "some ""text"" here"
		Keyword, // = "<Keyword>"
		RawKeyword, // = "<RawKeyword>"
		Expression,

		Dot, // = '.'
		Or, // = "||";
		Not, // = "!";
		Mul, // = "*";
		Div, // = "/";
		Mod, // = "%";
		And, // = "&&";
		Inc, // = "++";
		Dec, // = "--";
		Plus, // = "+";
		Colon, // = ':'
		Minus, // = "-";
		Assign, // = '='
		Equal, // = "==";
		OrBits, // = "|";
		LParen, // = "(";
		RParen, // = ")";
		LBrack, // = "[";
		RBrack, // = "]";
		AndBits, // = "&";
		XorBits, // = "^";
		NotBits, // = "~";
		UnEqual, // = "!=";
		LessThan, // = "<";
		Semicolon, // = ';'
		ShiftLeft, // = "<<";
		Apostrophe, // = '\''
		ShiftRight, // = ">>";
		GreaterThan, // = ">";
		LessThanEqual, // = "<=";
		ReverseSolidus, // = '\\'
		GreaterThanEqual, // = ">=";

		Preprocessor, // = "<Preprocessor>";
		//public static readonly string PreprocessorIf,
		//public static readonly string PreprocessorElse,
		//public static readonly string PreprocessorElIf,
		//public static readonly string PreprocessorOther,
		//public static readonly string PreprocessorEndIf,
		//public static readonly string PreprocessorUndef,
		//public static readonly string PreprocessorError,
		//public static readonly string PreprocessorIfDef,
		//public static readonly string PreprocessorIfnDef,
		//public static readonly string PreprocessorDefine,
		//public static readonly string PreprocessorPragma,
		//public static readonly string PreprocessorInclude,

		Language,
		StringTable,

		Menu,
		Dialog,
		Toolbar,
		DesignInfo,
		VersionInfo,
		Accelerators,

		End,
		Value,
		Block,
		Begin,

		Icon,
		LText,
		RText,
		CText,
		BEdit,
		HEdit,
		IEdit,
		State3,
		Control,
		PushBox,
		ListBox,
		CheckBox,
		GroupBox,
		EditText,
		ComboBox,
		ScrollBar,
		Auto3State,
		PushButton,
		UserButton,
		RadioButton,
		AutoCheckBox,
		DefPushButton,
		AutoRadioButton,

		Pure,
		Fixed,
		Impure,
		Shared,
		Preload,
		Moveable,
		NonShared,
		LoadOnCall,
		Discardable,

		Style,
		Version,
		ExStyle,
		Characteristics,

		Class,
		Popup,
		Caption,

		Font,
		Separator,
	}
}
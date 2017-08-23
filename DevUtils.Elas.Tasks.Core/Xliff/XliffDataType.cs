namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> The datatype attribute specifies the kind of text contained in the <see cref="XliffFile"/>.
	/// 					Depending on that type, you may apply different processes to the data. </summary>
	public enum XliffDataType
	{
		/// <summary> Indicates Active Server Page data. </summary>
		[StringValue("asp")]
		Asp,
		/// <summary> Indicates C source file data. </summary>
		[StringValue("c")]
		C,
		/// <summary> Indicates Channel Definition Format (CDF) data. </summary>
		[StringValue("cdf")]
		Cdf,
		/// <summary> Indicates ColdFusion data. </summary>
		[StringValue("cfm")]
		Cfm,
		/// <summary> Indicates C++ source file data. </summary>
		[StringValue("cpp")]
		Cpp,
		/// <summary> Indicates C-Sharp data. </summary>
		[StringValue("csharp")]
		CSharp,
		/// <summary> Indicates strings from C, ASM, and driver files data. </summary>
		[StringValue("cstring")]
		CString,
		/// <summary> Indicates comma-separated values data. </summary>
		[StringValue("csv")]
		Csv,
		/// <summary> Indicates database data. </summary>
		[StringValue("database")]
		Database,
		/// <summary> Indicates portions of document that follows data and contains metadata. </summary>
		[StringValue("documentfooter")]
		DocumentFooter,
		/// <summary> Indicates portions of document that precedes data and contains metadata. </summary>
		[StringValue("documentheader")]
		DocumentHeader,
		/// <summary> Indicates data from standard UI file operations dialogs (e.g., Open, Save, Save As, Export, Import). </summary>
		[StringValue("filedialog")]
		FileDialog,
		/// <summary> Indicates standard user input screen data. </summary>
		[StringValue("form")]
		Form,
		/// <summary> Indicates HyperText Markup Language (HTML) data - document instance. </summary>
		[StringValue("html")]
		Html,
		/// <summary> Indicates content within an HTML document's <body/> element. </summary>
		[StringValue("htmlbody")]
		HtmlBody,
		/// <summary> Indicates Windows INI file data. </summary>
		[StringValue("ini")]
		Ini,
		/// <summary> Indicates Interleaf data. </summary>
		[StringValue("interleaf")]
		InterLeaf,
		/// <summary> Indicates Java source file data (extension '.java'). </summary>
		[StringValue("javaclass")]
		JavaClass,
		/// <summary> Indicates Java property resource bundle data. </summary>
		[StringValue("javapropertyresourcebundle")]
		JavaPropertyResourceBundle,
		/// <summary> Indicates Java list resource bundle data. </summary>
		[StringValue("javalistresourcebundle")]
		JavaListResourceBundle,
		/// <summary> Indicates JavaScript source file data. </summary>
		[StringValue("javascript")]
		JavaScript,
		/// <summary> Indicates JScript source file data. </summary>
		[StringValue("jscript")]
		JScript,
		/// <summary> Indicates information relating to formatting. </summary>
		[StringValue("layout")]
		Layout,
		/// <summary> Indicates LISP source file data. </summary>
		[StringValue("lisp")]
		Lisp,
		/// <summary> Indicates information relating to margin formats. </summary>
		[StringValue("margin")]
		Margin,
		/// <summary> Indicates a file containing menu. </summary>
		[StringValue("menufile")]
		MenuFile,
		/// <summary> Indicates numerically identified string table. </summary>
		[StringValue("messagefile")]
		MessageFile,
		/// <summary> Indicates Maker Interchange Format (MIF) data. </summary>
		[StringValue("mif")]
		Mif,
		/// <summary> Indicates that the datatype attribute value is a MIME Type value and is defined in the mime-type attribute. </summary>
		[StringValue("mimetype")]
		MimeType,
		/// <summary> Indicates GNU Machine Object data. </summary>
		[StringValue("mo")]
		Mo,
		/// <summary> Indicates Message Librarian strings created by Novell's Message Librarian Tool. </summary>
		[StringValue("msglib")]
		MsgLib,
		/// <summary> Indicates information to be displayed at the bottom of each page of a document. </summary>
		[StringValue("pagefooter")]
		PageFooter,
		/// <summary> Indicates information to be displayed at the top of each page of a document. </summary>
		[StringValue("pageheader")]
		PageHeader,
		/// <summary> Indicates a list of property values (e.g., settings within INI files or preferences dialog). </summary>
		[StringValue("parameters")]
		Parameters,
		/// <summary> Indicates Pascal source file data. </summary>
		[StringValue("pascal")]
		Pascal,
		/// <summary> Indicates Hypertext Preprocessor data. </summary>
		[StringValue("php")]
		Php,
		/// <summary> Indicates plain text file (no formatting other than, possibly, wrapping). </summary>
		[StringValue("plaintext")]
		Plaintext,
		/// <summary> Indicates GNU Portable Object file. </summary>
		[StringValue("po")]
		Po,
		/// <summary> Indicates dynamically generated user defined document. e.g. Oracle Report, Crystal Report, etc. </summary>
		[StringValue("report")]
		Report,
		/// <summary> Indicates Windows .NET binary resources. </summary>
		[StringValue("resources")]
		Resources,
		/// <summary> Indicates Windows .NET Resources. </summary>
		[StringValue("resx")]
		Resx,
		/// <summary> Indicates Rich Text Format (RTF) data. </summary>
		[StringValue("rtf")]
		Rtf,
		/// <summary> Indicates Standard Generalized Markup Language (SGML) data - document instance. </summary>
		[StringValue("sgml")]
		Sgml,
		/// <summary> Indicates Standard Generalized Markup Language (SGML) data - Document Type Definition (DTD). </summary>
		[StringValue("sgmldtd")]
		SgmlDtd,
		/// <summary> Indicates Scalable Vector Graphic (SVG) data. </summary>
		[StringValue("svg")]
		Svg,
		/// <summary> Indicates VisualBasic Script source file. </summary>
		[StringValue("vbscript")]
		VbScript,
		/// <summary> Indicates warning message. </summary>
		[StringValue("warning")]
		Warning,
		/// <summary> Indicates Windows (Win32) resources (i.e. resources extracted from an RC script, a message file, or a compiled file). </summary>
		[StringValue("winres")]
		Winres,
		/// <summary> Indicates Extensible HyperText Markup Language (XHTML) data - document instance. </summary>
		[StringValue("xhtml")]
		Xhtml,
		/// <summary> Indicates Extensible Markup Language (XML) data - document instance. </summary>
		[StringValue("xml")]
		Xml,
		/// <summary> Indicates Extensible Markup Language (XML) data - Document Type Definition (DTD). </summary>
		[StringValue("xmldtd")]
		XmlDtd,
		/// <summary> Indicates Extensible Stylesheet Language (XSL) data. </summary>
		[StringValue("xsl")]
		Xsl,
		/// <summary> Indicates XUL elements. </summary>
		[StringValue("xul")]
		Xul
	}
}

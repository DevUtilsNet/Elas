namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> Resource name - Resource name or identifier of a item. 
	/// 					For example: the key in the key/value pair in a Java properties file, 
	/// 					the ID of a string in a Windows string table, the index value of an entry in a database table, etc. </summary>
	public enum XliffResType
	{
		/// <summary> Indicates a Windows RC AUTO3STATE control. </summary>
		[StringValue("auto3state")] Auto3State,

		/// <summary> Indicates a Windows RC AUTOCHECKBOX control. </summary>
		[StringValue("autocheckbox")] AutoCheckBox,

		/// <summary> Indicates a Windows RC AUTORADIOBUTTON control. </summary>
		[StringValue("autoradiobutton")] AutoRadioButton,

		/// <summary> Indicates a Windows RC BEDIT control. </summary>
		[StringValue("bedit")] BEdit,

		/// <summary> Indicates a bitmap, for example a BITMAP resource in Windows. </summary>
		[StringValue("bitmap")] Bitmap,

		/// <summary> Indicates a button object, for example a BUTTON control Windows. </summary>
		[StringValue("button")] Button,

		/// <summary> Indicates a caption, such as the caption of a dialog box. </summary>
		[StringValue("caption")] Caption,

		/// <summary> Indicates the cell in a table, for example the content of the &lt;td&gt; element in HTML. </summary>
		[StringValue("cell")] Cell,

		/// <summary> Indicates check box object, for example a CHECKBOX control in Windows. </summary>
		[StringValue("checkbox")] CheckBox,

		/// <summary> Indicates a menu item with an associated checkbox. </summary>
		[StringValue("checkboxmenuitem")] CheckBoxMenuItem,

		/// <summary> Indicates a list box, but with a check-box for each item. </summary>
		[StringValue("checkedlistbox")] CheckedListBox,

		/// <summary> Indicates a color selection dialog. </summary>
		[StringValue("colorchooser")] ColorChooser,

		/// <summary> Indicates a combination of edit box and listbox object, for example a COMBOBOX control in Windows. </summary>
		[StringValue("combobox")] ComboBox,

		/// <summary> Indicates an initialization entry of an extended combobox DLGINIT resource block. (code 0x1234). </summary>
		[StringValue("comboboxexitem")] ComboBoxExItem,

		/// <summary> Indicates an initialization entry of a combobox DLGINIT resource block (code 0x0403). </summary>
		[StringValue("comboboxitem")] ComboBoxItem,

		/// <summary> Indicates a UI base class element that cannot be represented by any other element. </summary>
		[StringValue("component")] Component,

		/// <summary> Indicates a context menu. </summary>
		[StringValue("contextmenu")] ContextMenu,

		/// <summary> Indicates a Windows RC CTEXT control. </summary>
		[StringValue("ctext")] CText,

		/// <summary> Indicates a cursor, for example a CURSOR resource in Windows. </summary>
		[StringValue("cursor")] Cursor,

		/// <summary> Indicates a date/time picker. </summary>
		[StringValue("datetimepicker")] DateTimePicker,

		/// <summary> Indicates a Windows RC DEFPUSHBUTTON control. </summary>
		[StringValue("defpushbutton")] DefPushButton,

		/// <summary> Indicates a dialog box. </summary>
		[StringValue("dialog")] Dialog,

		/// <summary> Indicates a Windows RC DLGINIT resource block. </summary>
		[StringValue("dlginit")] DlgInit,

		/// <summary> Indicates an edit box object, for example an EDIT control in Windows. </summary>
		[StringValue("edit")] Edit,

		/// <summary> Indicates a filename. </summary>
		[StringValue("file")] File,

		/// <summary> Indicates a file dialog. </summary>
		[StringValue("filechooser")] FileChooser,

		/// <summary> Indicates a footnote. </summary>
		[StringValue("fn")] Fn,

		/// <summary> Indicates a font name. </summary>
		[StringValue("font")] Font,

		/// <summary> Indicates a footer. </summary>
		[StringValue("footer")] Footer,

		/// <summary> Indicates a frame object. </summary>
		[StringValue("frame")] Frame,

		/// <summary> Indicates a XUL grid element. </summary>
		[StringValue("grid")] Grid,

		/// <summary> Indicates a groupbox object, for example a GROUPBOX control in Windows. </summary>
		[StringValue("groupbox")] GroupBox,

		/// <summary> Indicates a header item. </summary>
		[StringValue("header")] Header,

		/// <summary> Indicates a heading, such has the content of &lt;h1&gt;, &lt;h2&gt;, etc. in HTML. </summary>
		[StringValue("heading")] Heading,

		/// <summary> Indicates a Windows RC HEDIT control. </summary>
		[StringValue("hedit")] HEdit,

		/// <summary> Indicates a horizontal scrollbar. </summary>
		[StringValue("hscrollbar")] HscrollBar,

		/// <summary> Indicates an icon, for example an ICON resource in Windows. </summary>
		[StringValue("icon")] Icon,

		/// <summary> Indicates a Windows RC IEDIT control. </summary>
		[StringValue("iedit")] IEdit,

		/// <summary> Indicates keyword list, such as the content of the Keywords meta-data in HTML, or a K footnote in WinHelp RTF. </summary>
		[StringValue("keywords")] Keywords,

		/// <summary> Indicates a label object. </summary>
		[StringValue("label")] Label,

		/// <summary> Indicates a label that is also a HTML link (not necessarily a URL). </summary>
		[StringValue("linklabel")] LinkLabel,

		/// <summary> Indicates a list (a group of list-items, for example an &lt;ol&gt; or &lt;ul&gt; element in HTML). </summary>
		[StringValue("list")] List,

		/// <summary> Indicates a listbox object, for example an LISTBOX control in Windows. </summary>
		[StringValue("listbox")] ListBox,

		/// <summary> Indicates an list item (an entry in a list). </summary>
		[StringValue("listitem")] ListItem,

		/// <summary> Indicates a Windows RC LTEXT control. </summary>
		[StringValue("ltext")] LText,

		/// <summary> Indicates a menu (a group of menu-items). </summary>
		[StringValue("menu")] Menu,

		/// <summary> Indicates a toolbar containing one or more top level menus. </summary>
		[StringValue("menubar")] MenuBar,

		/// <summary> Indicates a menu item (an entry in a menu). </summary>
		[StringValue("menuitem")] MenuItem,

		/// <summary> Indicates a XUL menuseparator element. </summary>
		[StringValue("menuseparator")] MenuSeparator,

		/// <summary> Indicates a message, for example an entry in a MESSAGETABLE resource in Windows. </summary>
		[StringValue("message")] Message,

		/// <summary> Indicates a calendar control. </summary>
		[StringValue("monthcalendar")] MonthCalendar,

		/// <summary> Indicates an edit box beside a spin control. </summary>
		[StringValue("numericupdown")] NumericUpDown,

		/// <summary> Indicates a catch all for rectangular areas. </summary>
		[StringValue("panel")] Panel,

		/// <summary> Indicates a standalone menu not necessarily associated with a menubar. </summary>
		[StringValue("popupmenu")] PopupMenu,

		/// <summary> Indicates a pushbox object, for example a PUSHBOX control in Windows. </summary>
		[StringValue("pushbox")] PushBox,

		/// <summary> Indicates a Windows RC PUSHBUTTON control.	</summary>
		[StringValue("pushbutton")] PushButton,

		/// <summary> Indicates a radio button object. </summary>
		[StringValue("radio")] Radio,

		/// <summary> Indicates a menuitem with associated radio button. </summary>
		[StringValue("radiobuttonmenuitem")] RadioButtonMenuItem,

		/// <summary> Indicates raw data resources for an application. </summary>
		[StringValue("rcdata")] RCdata,

		/// <summary> Indicates a row in a table. </summary>
		[StringValue("row")] Row,

		/// <summary> Indicates a Windows RC RTEXT control. </summary>
		[StringValue("rtext")] RText,

		/// <summary> Indicates a user navigable container used to show a portion of a document. </summary>
		[StringValue("scrollpane")] ScrollPane,

		/// <summary> Indicates a generic divider object (e.g. menu group separator). </summary>
		[StringValue("separator")] Separator,

		/// <summary> Windows accelerators, shortcuts in resource or property files. </summary>
		[StringValue("shortcut")] Shortcut,

		/// <summary> Indicates a UI control to indicate process activity but not progress. </summary>
		[StringValue("spinner")] Spinner,

		/// <summary> Indicates a splitter bar. </summary>
		[StringValue("splitter")] Splitter,

		/// <summary> Indicates a Windows RC STATE3 control. </summary>
		[StringValue("state3")] State3,

		/// <summary> Indicates a window for providing feedback to the users, like 'read-only', etc. </summary>
		[StringValue("statusbar")] StatusBar,

		/// <summary> Indicates a string, for example an entry in a STRINGTABLE resource in Windows. </summary>
		[StringValue("string")] String,

		/// <summary> Indicates a layers of controls with a tab to select layers. </summary>
		[StringValue("tabcontrol")] TabControl,

		/// <summary> Indicates a display and edits regular two-dimensional tables of cells. </summary>
		[StringValue("table")] Table,

		/// <summary> Indicates a XUL textbox element. </summary>
		[StringValue("textbox")] TextBox,

		/// <summary> Indicates a UI button that can be toggled to on or off state. </summary>
		[StringValue("togglebutton")] ToggleButton,

		/// <summary> Indicates an array of controls, usually buttons. </summary>
		[StringValue("toolbar")] Toolbar,

		/// <summary> Indicates a pop up tool tip text. </summary>
		[StringValue("tooltip")] Tooltip,

		/// <summary> Indicates a bar with a pointer indicating a position within a certain range. </summary>
		[StringValue("trackbar")] Trackbar,

		/// <summary> Indicates a control that displays a set of hierarchical data. </summary>
		[StringValue("tree")] Tree,

		/// <summary> Indicates a URI (URN or URL). </summary>
		[StringValue("uri")] Uri,

		/// <summary> Indicates a Windows RC USERBUTTON control. </summary>
		[StringValue("userbutton")] UserButton,

		/// <summary> Indicates a user-defined control like CONTROL control in Windows. </summary>
		[StringValue("usercontrol")] UserControl,

		/// <summary> Indicates the text of a variable. </summary>
		[StringValue("var")] Var,

		/// <summary> Indicates version information about a resource like VERSIONINFO in Windows. </summary>
		[StringValue("versioninfo")] VersionInfo,

		/// <summary> Indicates a vertical scrollbar. </summary>
		[StringValue("vscrollbar")] VScrollbar,

		/// <summary> Indicates a graphical window. </summary>
		[StringValue("window")] Window
	}
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Loyc;
using DevUtils.Elas.Tasks.Core.Loyc.Extensions;
using DevUtils.Elas.Tasks.Core.Loyc.IO;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	sealed class RCExporterToIntermediateDocument : RCImportExportParser
	{
		class RCReadInfo
		{
			public XliffUnitCollection UnitCollection { get; set; }

			public XliffFile XliffFile
			{
				get
				{
					var ret = Enumerable.Repeat(UnitCollection.Owner, 1)
															.Concat(UnitCollection.Owner.SelectParent())
															.OfType<XliffFile>()
															.First();

					return ret;
				}
			}

			public RCReadInfo(XliffFile xliffFile)
			{
				UnitCollection = xliffFile.Units;
			}
		}

		private RCReadInfo[] _readInfos;

		public RCExporterToIntermediateDocument()
			: this(new RCLexer())
		{
		}

		private RCExporterToIntermediateDocument(RCLexer lexer)
			: base(lexer)
		{
		}

		public void Export(IEnumerable<XliffFile> files)
		{
			foreach (var item in files.GroupBy(g => g.Original.ToLower()))
			{
				using (var file = File.OpenRead(item.Key))
				{
					_readInfos = item.Select(s => new RCReadInfo(s)).ToArray();
					ValidSourceCultures = _readInfos.Select(s => s.XliffFile.SourceLanguage).Distinct().ToArray();
					Lexer.CharSource = new StreamCharSource(file);

					Parse();
				}
			}
		}

		#region Overrides of RCParser

		protected override void OnStringTableEntry(Token<RCTokenType> tokenId, Token<RCTokenType> tokenText)
		{
			if (!IsValidCurrentCulture)
			{
				return;
			}

			var id = Lexer.CharSource.Substring(tokenId);
			var text = ExtractResourceString(tokenText);
			foreach (var tu in _readInfos.Select(s => s.UnitCollection.UpdateOrCreateTransUnitByCompositeId(id, text)))
			{
				tu.ResType = XliffResType.String;
			}
		}

		protected override Token<RCTokenType> OnMenuResource(Token<RCTokenType> tokenMenuId)
		{
			if (!IsValidCurrentCulture)
			{
				var ret = base.OnMenuResource(tokenMenuId);
				return ret;
			}
			else
			{
				var menuId = Lexer.CharSource.Substring(tokenMenuId);

				var index = 0;
				var prefUC = new XliffUnitCollection[_readInfos.Length];

				foreach (var item in _readInfos)
				{
					prefUC[index++] = item.UnitCollection;
					var group = item.UnitCollection.GetOrAddGroupByCompositeId(menuId);
					group.ResType = XliffResType.Menu;
					item.UnitCollection = group.Units;
				}

				var ret = base.OnMenuResource(tokenMenuId);

				index = 0;

				foreach (var item in _readInfos)
				{
					item.UnitCollection = prefUC[index++];
				}

				return ret;
			}
		}

		protected override void OnPopupEntryBody(Token<RCTokenType> tokenResourceString)
		{
			if (!IsValidCurrentCulture)
			{
				base.OnPopupEntryBody(tokenResourceString);
				return;
			}

			var popupResourceString = ExtractResourceString(tokenResourceString);
			var id = StringToId(popupResourceString);

			var index = 0;
			var prefUC = new XliffUnitCollection[_readInfos.Length];

			var popupRefId = id + ".$PopupRef";

			foreach (var item in _readInfos)
			{
				prefUC[index++] = item.UnitCollection;
				var group = item.UnitCollection.GetOrAddGroup(id);
				item.UnitCollection = group.Units;
				group.ResType = XliffResType.Menu;

				var tu = item.UnitCollection.UpdateOrCreateTransUnitById(popupRefId, popupResourceString);
				tu.ResType = XliffResType.PopupMenu;
			}

			base.OnPopupEntryBody(tokenResourceString);

			index = 0;

			foreach (var item in _readInfos)
			{
				item.UnitCollection = prefUC[index++];
			}
		}

		protected override void OnMenuItem(Token<RCTokenType> tokenId, Token<RCTokenType> tokenText)
		{
			if (!IsValidCurrentCulture)
			{
				return;
			}

			var id = Lexer.CharSource.Substring(tokenId);
			var text = ExtractResourceString(tokenText);

			foreach (var item in _readInfos.Select(s => s.UnitCollection.UpdateOrCreateTransUnitById(id, text)))
			{
				item.ResType = XliffResType.MenuItem;
			}
		}

		protected override Token<RCTokenType> OnDialogResource(Token<RCTokenType> tokenDialogId)
		{
			var dialogId = Lexer.CharSource.Substring(tokenDialogId);

			var index = 0;
			var prefUC = new XliffUnitCollection[_readInfos.Length];

			foreach (var item in _readInfos)
			{
				prefUC[index++] = item.UnitCollection;
				var group = item.UnitCollection.GetOrAddGroupByCompositeId(dialogId);
				group.ResType = XliffResType.Dialog;
				item.UnitCollection = group.Units;
			}

			var ret = base.OnDialogResource(tokenDialogId);

			index = 0;

			foreach (var item in _readInfos)
			{
				item.UnitCollection = prefUC[index++];
			}

			return ret;
		}

		protected override void OnDialogControl(Token<RCTokenType> tokenControlType, Token<RCTokenType> tokenContent, Token<RCTokenType> tokenId)
		{
			if (tokenContent.Type != RCTokenType.String && tokenContent.Type != RCTokenType.RString)
			{
				return;
			}

			if (!IsValidCurrentCulture)
			{
				return;
			}

			var id = Lexer.CharSource.Substring(tokenId);
			var content = ExtractResourceString(tokenContent);

			id = GetDialogControlId(id, tokenControlType.Type, content);

			foreach (var item in _readInfos.Select(s => s.UnitCollection.UpdateOrCreateTransUnitById(id, content)))
			{
				switch (tokenControlType.Type)
				{
					case RCTokenType.LText:
					{
						item.ResType = XliffResType.LText;
						break;
					}
					case RCTokenType.RText:
					{
						item.ResType = XliffResType.RText;
						break;
					}
					case RCTokenType.CText:
					{
						item.ResType = XliffResType.CText;
						break;
					}
					case RCTokenType.State3:
					{
						item.ResType = XliffResType.State3;
						break;
					}
					case RCTokenType.Control:
					{
						item.ResType = XliffResType.UserControl;
						break;
					}
					case RCTokenType.PushBox:
					{
						item.ResType = XliffResType.PushBox;
						break;
					}
					case RCTokenType.CheckBox:
					{
						item.ResType = XliffResType.CheckBox;
						break;
					}
					case RCTokenType.GroupBox:
					{
						item.ResType = XliffResType.GroupBox;
						break;
					}
					case RCTokenType.ComboBox:
					{
						item.ResType = XliffResType.ComboBox;
						break;
					}
					case RCTokenType.Auto3State:
					{
						item.ResType = XliffResType.Auto3State;
						break;
					}
					case RCTokenType.PushButton:
					{
						item.ResType = XliffResType.PushButton;
						break;
					}
					case RCTokenType.UserButton:
					{
						item.ResType = XliffResType.UserButton;
						break;
					}
					case RCTokenType.RadioButton:
					{
						item.ResType = XliffResType.Radio;
						break;
					}
					case RCTokenType.AutoCheckBox:
					{
						item.ResType = XliffResType.AutoCheckBox;
						break;
					}
					case RCTokenType.DefPushButton:
					{
						item.ResType = XliffResType.DefPushButton;
						break;
					}
					case RCTokenType.AutoRadioButton:
					{
						item.ResType = XliffResType.AutoRadioButton;
						break;
					}
					case RCTokenType.Caption:
					{
						item.ResType = XliffResType.Caption;
						break;
					}
				}
			}
		}

		protected override void OnPreprocessor(Token<RCTokenType> tokenId)
		{
		}

		#endregion
	}
}
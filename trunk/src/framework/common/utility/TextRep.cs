using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace DL.Framework.Common
{
	/// <summary>
	/// TextRepAttribute Exceptions.
	/// </summary>
	public class TextRepException : ApplicationException
	{
		public TextRepException(string message) : base(message) {}
	}

	/// <summary>
	/// TextRepAttribute Type.
	/// </summary>
	public enum ETextRepType
	{
		Db,
		Display,
		Web
	}

	/// <summary>
	/// TextRepAttribute Modifiers.
	/// </summary>
	public enum ETextRepModifier
	{
		DbNull			= 0x00000001,
		CaseSensitive	= 0x00000002,
		TrimWhitespace	= 0x00000004
	}

	/// <summary>
	/// TextRepAttribute used to decorate enumeration members.
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple=true)]
	public class TextRepAttribute : Attribute
	{
		#region Public read-only members

		/// <summary>The text representation type (display or database). <see cref="ETextRepType"/><summary>
		public readonly ETextRepType Type;

		/// <summary>The text representation.</summary>
		public readonly string Text;

		/// <summary>The text representation modifiers. <see cref="ETextRepModifier"/></summary>
		public readonly ETextRepModifier Modifiers;

		#endregion

		#region Public construction

		/// <summary>
		/// Creates a simple TextRep attribute.
		/// </summary>
		/// <param name="type">The text type.</param>
		/// <param name="text">The text.</param>
		public TextRepAttribute( ETextRepType type, string text )
			: this( type, text, 0 )
		{
		}

		/// <summary>
		/// Creates a TextRep attribute with modifiers.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="modifiers">The text attribute modifiers such as dbNull or case sensitive.</param>
		public TextRepAttribute( ETextRepType type, string text, ETextRepModifier modifiers )
		{
			Type = type;
			Text = text;
			Modifiers = modifiers;
		}

		#endregion

		#region Modifiers

		/// <summary>
		/// Indicates whether the modifiers are set.
		/// </summary>
		public bool HasModifiers(ETextRepModifier modifiers)
		{
			return ((int)Modifiers & (int)modifiers) != 0;
		}

		#endregion
	}

	/// <summary>
	/// Represents text rep attributes for enum type and values.
	/// </summary>
	public class TextRepInfo
	{
		#region Private members

		private TextRepAttribute m_typeAttr = null;				// attribute of enum type
		private Hashtable m_valueAttrByValue = new Hashtable();	// attributes of enum values

		#endregion

		#region Public properties

		/// <summary>
		/// Returns the attribute for the type itself.
		/// </summary>
		public TextRepAttribute TypeAttribute
		{
			get { return m_typeAttr; }
			set { m_typeAttr = value; }
		}

		/// <summary>
		/// Returns the hash of attributes against enum values.
		/// </summary>
		public IDictionary ValueAttributes
		{
			get { return m_valueAttrByValue; }
		}

		#endregion
	}

	/// <summary>
	/// Provides translation between enumerated values and their associated text representation.
	/// </summary>
	/// <remarks>
	/// The text representation is defined by using the TextRepAttribute on the enum type
	/// and the individual enum values.
	/// <p>
	/// note: The individual TextRep attributes determine whether this class supports
	/// database null semantics and case-sensitive comparisons or whitespace trimming.
	/// </p>
	/// </remarks>
	public class TextRepTx
	{
 		#region Public static operations

		/// <summary>
		/// Indicates whether the enum value represents a database null.
		/// </summary>
		/// <param name="enumValue">The enum value to test.</param>
		/// <param name="textType">The text type.</param>
        public static bool IsNull<T>(T enumValue, ETextRepType textType) where T : struct, IConvertible
		{
			bool isNull = false;

			// Get TextRepInfo array from internal hashtable
			var attrs = GetTextRepInfo<T>(textType).ValueAttributes;
			if (attrs.Contains(enumValue))
			{
				var attr = attrs[enumValue] as TextRepAttribute;
				isNull = attr.HasModifiers(ETextRepModifier.DbNull);
			}

			return isNull;
		}

		/// <summary>
		/// Returns the user defined text representation of an enum value.
		/// </summary>
		/// <param name="enumValue">The enum value to translate.</param>
		/// <param name="textType">The text type.</param>
		/// <returns>The text representation.</returns>
		/// <remarks>
		/// The alternative text representation of the enum value is declared by 
		/// applying the <see cref="TextRepAttribute"/> to the value.
		/// </remarks>
		public static string ToText<T>(T enumValue, ETextRepType textType) where T : struct, IConvertible
		{
			string ret = string.Empty;

			// Get TextRepInfo array from internal hashtable
			var attrs = GetTextRepInfo<T>(textType).ValueAttributes;
			if (attrs.Contains(enumValue))
			{
				var attr = attrs[enumValue] as TextRepAttribute;
				ret = attr.Text;
			}
			
			return ret;
		}

		/// <summary>
		/// Returns the user defined text representation of an enum type.
		/// </summary>
		/// <param name="textType">The text type.</param>
		/// <returns>The text representation.</returns>
		/// <remarks>
		/// The text representation is declared by applying the <see cref="TextRepAttribute"/> 
		/// to the type.
		/// </remarks>
        public static string ToText<T>(ETextRepType textType) where T : struct, IConvertible
		{
			string ret = string.Empty;

			TextRepAttribute attr = GetTextRepInfo<T>(textType).TypeAttribute;
			if (attr != null)
				ret = attr.Text;
			
			return ret;
		}

		/// <summary>
		/// Returns the user defined database value for an enum value.
		/// </summary>
		/// <param name="enumValue">The enum value to translate.</param>
		/// <param name="textType">The text type.</param>
		/// <returns>The text representation.</returns>
		/// <remarks>
		/// The alternative text representation of the enum value is declared by 
		/// applying the <see cref="TextRepAttribute"/> to the value.
		/// </remarks>
        public static object ToDbText<T>(T enumValue, ETextRepType textType) where T : struct, IConvertible
		{
			object ret = string.Empty;

			// Get TextRepInfo array from internal hashtable
			IDictionary attrs = GetTextRepInfo<T>(textType).ValueAttributes;
			if (attrs.Contains(enumValue))
			{
				var attr = attrs[enumValue] as TextRepAttribute;
				if (attr.HasModifiers(ETextRepModifier.DbNull))
					ret = DBNull.Value;
				else
					ret = attr.Text;
			}

			return ret;
		}

		/// <summary>
		/// Given a text representation, converts to the corresponding enum value.
		/// </summary>
		/// <param name="text">The text representation.</param>
		/// <param name="textType">The text type.</param>
		/// <returns>The enum value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Could not find enum for text representation.</exception>
        public static T ToEnum<T>(string text, ETextRepType textType) where T : struct, IConvertible
		{
			// Get TextRepInfo array from internal hashtable
			IDictionary attrs = GetTextRepInfo<T>(textType).ValueAttributes;

			// Look at each TextRepInfo and compare the text
			foreach (DictionaryEntry attrEntry in attrs)
			{
				var enumValue = (T)attrEntry.Key;
				var attr = attrEntry.Value as TextRepAttribute;

				// Check for trim whitespace
				if (attr.HasModifiers(ETextRepModifier.TrimWhitespace))
					text = text.Trim();

				// Compare
				CompareOptions cmpOptions = attr.HasModifiers(ETextRepModifier.CaseSensitive)? CompareOptions.None: CompareOptions.IgnoreCase;
				if (_cmpInfo.Compare(text, attr.Text, cmpOptions) == 0)
					return enumValue;
			}

			throw new ArgumentOutOfRangeException("text", text, String.Format("The value {0} does not correspond to an attributed value in enum {1}", text, typeof(T)));
		}

		/// <summary>
		/// Indicates whether a given text representation, converts to the corresponding enum value.
		/// </summary>
		/// <param name="text">The text representation.</param>
		/// <param name="textType">The text type.</param>
		/// <returns>Returns true if valid enum otherwise false.</returns>
		public static bool IsValidEnum<T>(string text, ETextRepType textType) where T : struct, IConvertible
		{
			// Get TextRepInfo array from internal hashtable
			IDictionary attrs = GetTextRepInfo<T>(textType).ValueAttributes;

			// Look at each TextRepInfo and compare the text
			foreach (DictionaryEntry attrEntry in attrs)
			{
				var enumValue = attrEntry.Key;
				var attr = attrEntry.Value as TextRepAttribute;

				// Check for trim whitespace
				if (attr.HasModifiers(ETextRepModifier.TrimWhitespace))
					text = text.Trim();

				// Compare
				CompareOptions cmpOptions = attr.HasModifiers(ETextRepModifier.CaseSensitive)? CompareOptions.None: CompareOptions.IgnoreCase;
				if (_cmpInfo.Compare(text, attr.Text, cmpOptions) == 0)
					return true;
			}
			return false;
		}


		#endregion

		#region Private static implementation

		// Hash collection of TextRepInfo by EnumType
		private static Hashtable _textRepInfoByEnumType = new Hashtable();
		private static readonly CompareInfo _cmpInfo = CultureInfo.CurrentCulture.CompareInfo;

		/// <summary>
		/// Gets the TextRepInfo hashtable from the internal hashtable of all enum types.
		/// </summary>
        private static TextRepInfo GetTextRepInfo<T>(ETextRepType textType) where T : struct, IConvertible
		{
		    var enumType = typeof(T);

			// Ensure we have the enum type
			if (!_textRepInfoByEnumType.ContainsKey(enumType))
			{
				// Lock hashtable and add TextRepInfo hashtable
				lock (_textRepInfoByEnumType.SyncRoot)
				{
					// Check if another thread has got here first
					if (!_textRepInfoByEnumType.ContainsKey(enumType))
						_textRepInfoByEnumType.Add(enumType, new Hashtable());
				}
			}
			var textRepInfoByTextType = _textRepInfoByEnumType[enumType] as Hashtable;

			if (!textRepInfoByTextType.ContainsKey(textType))
			{
				// Lock hashtable and add TextRepInfo
				lock (textRepInfoByTextType.SyncRoot)
				{
					// Check if another thread has added the TextRepInfo
					if (!textRepInfoByTextType.ContainsKey(textType))
					{
						TextRepInfo textRepInfo = new TextRepInfo();

						// First get the type attribute, if any.
						object[] attrs = enumType.GetCustomAttributes(typeof(TextRepAttribute), false);
						foreach (TextRepAttribute attr in attrs)
						{
							if (attr.Type == textType)
							{
								if (textRepInfo.TypeAttribute != null)
									throw new TextRepException("Only single TextRepAttribute allowed for each enumType and TextRepType.");
								textRepInfo.TypeAttribute = attr;
							}
						}

						// Add each enum member TextRepAttribute
						MemberInfo[] members = enumType.GetMembers();
						foreach (MemberInfo mi in members)
						{
							attrs = mi.GetCustomAttributes(typeof(TextRepAttribute), false);
							foreach (TextRepAttribute attr in attrs)
							{
								if (attr.Type == textType)
								{
									// Parse enum member value
									object enumValue = Enum.Parse(enumType, mi.Name);
									if (textRepInfo.ValueAttributes.Contains(enumValue))
										throw new TextRepException("Only single TextRepAttribute allowed for each enumType and TextRepType.");

									// Add TextRepAttribute by enum value
									textRepInfo.ValueAttributes.Add(enumValue, attr);
								}
							}
						}

						// Add populated TextRepInfo to collection
						textRepInfoByTextType.Add(textType, textRepInfo);

						// Have you forgotten to define TextRep attributes?
						Debug.Assert(textRepInfo.ValueAttributes.Count != 0, 
							"No TextRep attributes defined for enumType: " + enumType.FullName + " and textType: " + textType.ToString());
					}
				}
			}

			return textRepInfoByTextType[textType] as TextRepInfo;
		}

		#endregion

		#region Deprecated methods for database type

        public static bool IsNull<T>(T e) where T : struct, IConvertible
		{
			return IsNull(e, ETextRepType.Db);
		}

        public static string ToText<T>(T e) where T : struct, IConvertible
		{
			return ToText<T>(ETextRepType.Db);
		}

        public static object ToDbText<T>(T e) where T : struct, IConvertible
		{
			return ToDbText<T>(e, ETextRepType.Db);
		}

        public static T ToEnum<T>(string text) where T : struct, IConvertible
		{
			return ToEnum<T>(text, ETextRepType.Db);
		}

        public static bool IsValidEnum<T>(string text) where T : struct, IConvertible
		{
			return IsValidEnum<T>(text, ETextRepType.Db);
		}

		#endregion
	}
}

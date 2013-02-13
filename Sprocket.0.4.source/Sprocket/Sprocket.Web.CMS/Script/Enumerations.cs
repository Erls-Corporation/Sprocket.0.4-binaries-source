using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sprocket.Web.CMS.Script
{
	public enum TokenType
	{
		None,
		/// <summary>
		/// An opening round bracket i.e. (
		/// </summary>
		GroupStart,
		/// <summary>
		/// An closing round bracket i.e. )
		/// </summary>
		GroupEnd,
		/// <summary>
		/// A scripted string surrounded by quote marks i.e. "
		/// </summary>
		QuotedString,
		/// <summary>
		/// Normal text that appears outside script sections.
		/// </summary>
		FreeText,
		/// <summary>
		/// A colon (:), used to indicate that the following expression is a property
		/// of the preceding expression
		/// </summary>
		PropertyDesignator,
		/// <summary>
		/// A symbolic character not reserved for any specific internal use. These are typically used
		/// by specially-designed expressions and instructions, e.g. binary expressions/operators.
		/// </summary>
		OtherSymbolic,
		/// <summary>
		/// A single standalone string made of word characters (any letter or underscore followed by
		/// any number of letters, numbers or underscores), but not surrounded by quote marks.
		/// </summary>
		Word,
		/// <summary>
		/// A set of characters that can be read as a number. e.g. 0.5, 1, .8, 1.0, 999.987
		/// </summary>
		Number,
		/// <summary>
		/// A comment block
		/// </summary>
		Comment
	}
}

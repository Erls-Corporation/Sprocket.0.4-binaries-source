using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Sprocket.Web
{
	public interface IJSONEncoder
	{
		void WriteJSON(StringWriter writer);
	}

	public interface IJSONReader
	{
		void LoadJSON(object json);
	}

	public class JSONGenericObject : IJSONEncoder
	{
		protected string json;

		public JSONGenericObject(string json)
		{
			this.json = json;
		}

		public void WriteJSON(StringWriter writer)
		{
			writer.Write(json);
		}
	}

	public class JSON
	{
		public static object Parse(string jsonString)
		{
			return new Parser(ref jsonString).Parse();
		}

		public static string Encode(object source)
		{
			StringWriter writer = new StringWriter();
			Encode(writer, source);
			return writer.ToString();
		}

		public static void Encode(StringWriter writer, object source)
		{
			if (source is IJSONEncoder)
				((IJSONEncoder)source).WriteJSON(writer);

			else if (source == null)
				writer.Write("null");

			else if (source is bool)
				writer.Write(source.ToString().ToLower());

			else if (source is ValueType)
			{
				if (source is DateTime)
					EncodeString(writer, ((DateTime)source).ToUniversalTime().ToString("r"));
				else
					try { writer.Write(Convert.ToDecimal(source).ToString()); }
					catch { EncodeString(writer, source); }
			}

			else if (source is string)
				EncodeString(writer, source);

			else if (source is IDictionary)
			{
				writer.Write("{");
				bool isFirst = true;
				foreach (DictionaryEntry kvp in (IDictionary)source)
				{
					if (isFirst)
						isFirst = false;
					else
						writer.Write(",");
					EncodeNameValuePair(writer, kvp.Key.ToString(), kvp.Value);
				}
				writer.Write("}");
			}

			else if (source is IEnumerable)
				EncodeEnumerated(writer, (IEnumerable)source);

			else
				EncodeString(writer, source.ToString());
		}

		public static void EncodeString(StringWriter writer, object str)
		{
			writer.Write("\"" + str.ToString()
				.Replace("\\", "\\\\")
				.Replace("\a", "\\a")
				.Replace("\b", "\\b")
				.Replace("\n", "\\n")
				.Replace("\f", "\\f")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t")
				.Replace("\v", "\\v")
				.Replace("\'", "\\'")
				.Replace("\"", "\\\"") + "\"");
		}

		public static void EncodeNameValuePair(StringWriter writer, string name, object value)
		{
			EncodeString(writer, name);
			writer.Write(":");
			Encode(writer, value);
		}

		public static void EncodeCustomObject(StringWriter writer, params KeyValuePair<string, object>[] nameValuePairs)
		{
			if (nameValuePairs.Length == 0)
				writer.Write("{}");
			else
			{
				writer.Write("{");
				EncodeNameValuePair(writer, nameValuePairs[0].Key, nameValuePairs[0].Value);
				for (int i = 1; i < nameValuePairs.Length; i++)
				{
					writer.Write(",");
					EncodeNameValuePair(writer, nameValuePairs[i].Key, nameValuePairs[i].Value);
				}
				writer.Write("}");
			}
		}

		public static void EncodeCustomObject(StringWriter writer, params object[] namesAndValues)
		{
			if (namesAndValues.Length % 2 == 1)
				throw new Exception("Invalid number of parameters passed to JSON.EncodeCustomObject. Must be even (names + values).");
			if (namesAndValues.Length == 0)
				writer.Write("{}");
			else
			{
				writer.Write("{");
				for (int i = 0; i < namesAndValues.Length; i += 2)
				{
					if(i > 0) writer.Write(",");
					EncodeNameValuePair(writer, namesAndValues[i].ToString(), namesAndValues[i + 1]);
				}
				writer.Write("}");
			}
		}

		public static void EncodeEnumerated(StringWriter writer, IEnumerable e)
		{
			writer.Write("[");
			int c = 0;
			foreach (object o in e)
			{
				if(c++ > 0) writer.Write(",");
				Encode(writer, o);
			}
			writer.Write("]");
		}

		#region JSON String Parser
		private class Parser
		{
			private string str;
			private int pos = 0;

			public Parser(ref string jsonString)
			{
				str = jsonString.Trim();
			}

			public object Parse()
			{
				SkipNextWhiteSpace();
				if (pos < str.Length)
				{
					object parsed = ReadChunk();
					return parsed;
				}
				return null;
			}

			private object ReadChunk()
			{
				AssertNotEndOfString();

				switch (str[pos])
				{
					case '[': return ReadArray();
					case '{': return ReadComplexObject();
					default: return ReadSimpleValue();
				}
			}

			private void SkipNextWhiteSpace()
			{
				while (pos < str.Length)
				{
					if (!char.IsWhiteSpace(str[pos]))
						return;
					pos++;
				}
			}

			private List<object> ReadArray()
			{
				pos++; // skip the opening bracket
				List<object> list = new List<object>();
				bool wasComma = false; // used when a comma was read in
				SkipNextWhiteSpace(); // skip to the first non-whitespace character in the array
				while (true)
				{
					AssertNotEndOfString(); // end of string = bad
					if ((str[pos] == ']' || str[pos] == ',') && wasComma) // make sure any comma is followed by an element
						ThrowException("Invalid character following element division in JSON array.");
					if (str[pos] == ']') // if this is the end of the array
					{
						pos++; // move beyond the closing bracket
						SkipNextWhiteSpace(); // play nicely by advancing to the next position for the parser
						return list;
					}
					if (list.Count > 0 && !wasComma) // if this is not the first item, yet no separator (comma) exists
						ThrowException("An element separator or closing bracket was expected.");

					list.Add(ReadChunk()); // load the value into the array
					SkipNextWhiteSpace(); // go to the next token
					if (str[pos] == ',') // look for an element separator (a comma)
					{
						wasComma = true; // we now expect that another element should follow
						pos++; // advance beyond the comma
						SkipNextWhiteSpace(); // go to the next token
					}
					else
						wasComma = false;
				}
			}

			private Dictionary<string, object> ReadComplexObject()
			{
				pos++; // skip past the opening brace
				Dictionary<string, object> values = new Dictionary<string, object>();
				bool wasComma = false; // used when a comma was read in
				SkipNextWhiteSpace(); // and any trailing whitespace

				while (true)
				{
					if ((str[pos] == '}' || str[pos] == ',') && wasComma) // make sure any comma is followed by an element
						ThrowException("Invalid character following item division in JSON object.");

					if (str[pos] == '}') // if the end of the object has been reached
					{
						pos++; // advance the cursor past the closing brace
						SkipNextWhiteSpace();
						return values; // and return the parsed object
					}

					AssertNotEndOfString();
					if (!wasComma && values.Count > 0) // oops, whats this? no separator comma?
						ThrowException("A separator comma or a closing brace was expected but not found.");

					string name = ReadString(false); // get the item's name
					SkipNextWhiteSpace(); // then skip any whitespace
					AssertNotEndOfString();
					if (str[pos++] != ':') // and the item/value divider colon
						ThrowException("Object identifier must be followed by a colon.");
					SkipNextWhiteSpace(); // and any further whitespace
					AssertNotEndOfString();

					object val = ReadChunk(); // then read the value for this item
					values.Add(name, val); // add the name and value to the dictionary

					AssertNotEndOfString();
					if (str[pos] == ',') // look for an item separator (a comma)
					{
						wasComma = true; // a comma means that we now expect another key/value pair to follow
						pos++; // advance beyond the comma
						SkipNextWhiteSpace(); // go to the next token
					}
					else
						wasComma = false;
				}
			}

			private object ReadSimpleValue()
			{
				if (char.IsDigit(str[pos]) || (str[pos] == '-' && char.IsDigit(str[pos + 1])))
					return ReadNumber();

				if (str.Length >= pos + 4) // if the string has enough characters left, do a quick check for null, true or false
				{
					object val;
					switch(str.Substring(pos, 4))
					{
						case "null": val = null; pos += 4; break; // null is ok
						case "true": val = true; pos += 4; break; // true is ok
						default:
							if(str.Length >= pos + 5) // false is ok
								if(str.Substring(pos, 5) == "false")
								{
									val = false;
									pos += 5;
									break;
								}
							return ReadString(true); // otherwise it's gotta be a quote-encased string
					}
					AssertWordBoundaryReached(); // make sure the value, e.g. null, wasn't actually something like "nullify"
					SkipNextWhiteSpace();
					return val;
				}
				return ReadString(true);
			}

			private char[] wordBoundaries = { '[', ']', '{', '}', ':', ',' };
			public void AssertWordBoundaryReached()
			{
				if (pos < str.Length) // if this isn't the end of the string
					if (!char.IsWhiteSpace(str[pos])) // and it's not whitespace
						if (Array.IndexOf<char>(wordBoundaries, str[pos]) == -1) // and not a special character that we know about
							ThrowException("Unexpected value. Literal strings must be encased in quotes.");
			}

			private string ReadString(bool requireQuotes)
			{
				bool isQuotedString = str[pos] == '"' || str[pos] == '\'';
				char quoteChar = '"';
				if (requireQuotes && !isQuotedString)
					ThrowException("String must be encased in quotes.");
				if (isQuotedString)
					quoteChar = str[pos++];
				StringBuilder sb = new StringBuilder();
				while (true)
				{
					if (isQuotedString)
					{
						AssertNotEndOfString();
						if (str[pos] == quoteChar) // end of quoted string
						{
							pos++; // skip past quote
							SkipNextWhiteSpace();
							return sb.ToString();
						}

						if (str[pos] == '\\') // ensure that the escape character is handled properly
						{
							pos++;
							AssertNotEndOfString();
							switch (str[pos])
							{
								case 'a': sb.Append('\a'); break;
								case 'b': sb.Append('\b'); break;
								case 'f': sb.Append('\f'); break;
								case 'n': sb.Append('\n'); break;
								case 'r': sb.Append('\r'); break;
								case 't': sb.Append('\t'); break;
								case 'v': sb.Append('\v'); break;
								case '\'': sb.Append('\''); break;
								case '"': sb.Append('"'); break;
								case '\\': sb.Append('\\'); break;
								default: ThrowException("Unrecognised escape sequence."); break;
							}
						}
						else // just another character in the string, add it to the output string
							sb.Append(str[pos]);
						pos++;
					}
					else // string is in regex format: [a-zA-Z_][a-zA-Z0-9_]*
					{
						if ((str[pos] >= 'a' && str[pos] <= 'z')
							|| (str[pos] >= 'A' && str[pos] <= 'Z')
							|| str[pos] == '_'
							|| (sb.Length > 0 && str[pos] >= '0' && str[pos] <= '9'))
						{
							sb.Append(str[pos++]);
							continue;
						}
						else if (char.IsWhiteSpace(str[pos])) // end of this string
						{
							SkipNextWhiteSpace();
							return sb.ToString();
						}
						else if (sb.Length == 0) // some other character found at start of string
							ThrowException("An invalid character was found at the start of an unquoted string.");
						else
							return sb.ToString();
					}
				}
			}

			private object ReadNumber()
			{
				int pos1 = pos;
				if (str[pos] == '-') // can start with a negative (-) sign
					pos++;
				while (pos < str.Length) // must start with at least one digit
					if(str[pos] >= '0' && str[pos] <= '9')
						pos++;
					else
						break;
				if(pos < str.Length)
					if (str[pos] == '.') // followed by an optional period
					{
						pos++;
						AssertNotEndOfString();
						if(!(str[pos] >= '0' && str[pos] <= '9')) // a digit is required after the period
							ThrowException("Unexpected character found in number.");
						pos++;
						while (pos < str.Length) // then any trailing digits
							if(str[pos] >= '0' && str[pos] <= '9') pos++;
							else break;
						if(str[pos] == 'e' || str[pos] == 'E') // power-of-ten multiplier is optional
						{
							pos++;
							AssertNotEndOfString();
							if (str[pos] != '+' && str[pos] != '-') // but must be followed by a + or -
								ThrowException("Unexpected character found in number.");
							pos++;
							AssertNotEndOfString();
							if (!(str[pos] >= '0' && str[pos] <= '9')) // and then at least one digit
								ThrowException("Unexpected character found in number.");
							pos++;
							AssertNotEndOfString();
							while (pos < str.Length) // then any trailing digits
								if(str[pos] >= '0' && str[pos] <= '9') pos++;
								else break;
						}
					}
				string strNum = str.Substring(pos1, pos - pos1);
				try
				{
					int intVal = 0;
					if (int.TryParse(strNum, out intVal))
						return intVal;
					long longVal = 0;
					if (long.TryParse(strNum, out longVal))
						return longVal;
					double dblVal = 0;
					if (double.TryParse(strNum, out dblVal))
						return dblVal;
					decimal decVal = 0;
					if (decimal.TryParse(strNum, out decVal))
						return decVal;
					ThrowException("Unable to parse number. Too many characters?");
				}
				finally
				{
					SkipNextWhiteSpace();
				}
				return null;
			}

			private void AssertNotEndOfString()
			{
				if (pos >= str.Length)
					ThrowException("Unexpected end of string found.");
			}

			private void ThrowException(string message)
			{
				throw new SprocketException("Cannot parse JSON string. "
					+ message + Environment.NewLine
					+ str.Substring(0, pos+1 > str.Length ? str.Length : pos+1)
					+ "<-- Error from position " + pos);
			}
		}
		#endregion
	}
}

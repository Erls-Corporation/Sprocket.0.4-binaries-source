using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Utility
{
	public static partial class StringUtilities
	{
		public static bool MatchesAny(object source, params object[] comparisons)
		{
			foreach (object o in comparisons)
				if (object.Equals(source, o))
					return true;
			return false;
		}

		public static bool MatchesAny(string source, string[] comparisons)
		{
			foreach (string o in comparisons)
				if (source == o)
					return true;
			return false;
		}

		public static bool BoolFromString(string value)
		{
			return MatchesAny(value.ToLower(), "yes", "true", "on", "1");
		}

		public static string GenerateRandomString(int chars)
		{
			Random r = new Random();
			string s = "";
			for(int i=0; i<chars; i++)
			{
				int x = r.Next(2);
				switch (x)
				{
					case 0: s += (char)(r.Next(10) + 48); break; // 0-9
					case 1: s += (char)(r.Next(26) + 65); break; // A-Z
					case 2: s += (char)(r.Next(26) + 97); break; // a-z
				}
			}
			return s;
		}

		public static string CommaJoin(List<string> values)
		{
			StringBuilder sb = new StringBuilder();
			for (int i=0; i<values.Count; i++)
			{
				if (i > 0)
					if (i < values.Count - 1)
						sb.Append(", ");
					else
						sb.Append(" and ");
				sb.Append(values[i]);
			}
			return sb.ToString();
		}

		public static string ApproxHowLongAgo(DateTime now, DateTime occurranceDate)
		{
			TimeSpan ts = now.Subtract(occurranceDate);
			if (ts.TotalSeconds > 3)
			{
				if (ts.TotalSeconds > 55)
				{
					if (ts.TotalSeconds >= 120)
					{
						if (ts.TotalHours >= 1 && ts.TotalHours < 2 && ts.Minutes < 55)
						{
							if (ts.Minutes < 5)
								return "About an hour ago";
							else
								return "Over an hour ago";
						}
						else if (ts.TotalHours < 2 && ts.Minutes >= 55)
							return "Nearly 2 hours ago";
						else if (ts.TotalHours > 1 && ts.Minutes < 55)
						{
							if (ts.Minutes < 5)
								return "About " + ts.TotalHours.ToString("#") + " hours ago";
							else
							{
								if (ts.Days > 1)
									return ts.Days.ToString() + " days ago";
								return "Over " + ts.TotalHours.ToString("#") + " hours ago";
							}
						}
						else if (ts.TotalHours > 1 && ts.Minutes >= 55)
							return "Nearly " + (ts.TotalHours + 1).ToString("#") + " hours ago";
						else
							return "About " + ts.TotalMinutes.ToString("#") + " minutes ago";
					}
					else
						return "About a minute ago";
				}
				else
					return ts.Seconds.ToString() + " seconds ago";
			}
			else
				return "Just then";
		}
	}
}

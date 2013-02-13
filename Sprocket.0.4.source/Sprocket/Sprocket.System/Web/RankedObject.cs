using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web
{
	public interface IRankable
	{
		int Rank { get; set; }
	}

	public class RankedObject
	{
		public static int SortByRank(IRankable a, IRankable b)
		{
			if (a == null)
				return b == null ? 0 : -1;
			if (b == null)
				return 1;

			if (a.Rank > b.Rank)
				return 1;
			else if (a.Rank < b.Rank)
				return -1;
			else
				return 0;
		}
	}
}

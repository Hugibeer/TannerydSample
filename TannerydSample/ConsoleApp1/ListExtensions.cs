using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
	public static class ListExtensions
	{
		public static IEnumerable<List<T>> BulkTake<T>(this IEnumerable<T> source, int bulkSize)
		{
			if (bulkSize <= 0)
			{
				throw new ArgumentException("Invalid bulk size.", "bulkSize");
			}

			if (source == null || source.Count() == 0)
			{
				yield break;
			}

			var list = new List<T>(bulkSize);
			foreach (var item in source)
			{
				if (list.Count >= bulkSize)
				{
					yield return list;
					list.Clear();
				}
				list.Add(item);
			}

			/// Return the rest of the bulk list if there are any elements left over from the foreach.
			if (list.Any())
			{
				yield return list;
			}
		}
	}

}

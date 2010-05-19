using System;
using System.Collections.Generic;

namespace DIExample.Services
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}
	}
}
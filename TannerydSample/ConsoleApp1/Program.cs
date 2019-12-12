using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tanneryd.BulkOperations.EF6;

namespace ConsoleApp1
{
	class Program
	{
		private const int NUMBER_OF_TEST_ITEMS = 1000000;
		private const int CHUNK_SIZE = 4000;

		static void Main(string[] args)
		{
			var data = GenerateTestData();

			DeleteOldEntries();

			var measurements = new List<double>();
			var stopwatch = new Stopwatch();

			using (var context = new Context())
			{
				stopwatch.Start();
				foreach (var bulk in data.BulkTake(CHUNK_SIZE))
				{
					var chunkWatch = new Stopwatch();

					chunkWatch.Start();
					context.BulkInsertAll(bulk.ToList());
					chunkWatch.Stop();

					Console.WriteLine($"Elapsed {chunkWatch.Elapsed.TotalSeconds}");

					measurements.Add(chunkWatch.Elapsed.TotalSeconds);
				}

				stopwatch.Stop();
			}
			var average = measurements.Average();
			Console.WriteLine($"Total elapsed time {stopwatch.Elapsed.TotalSeconds}, average seconds per {CHUNK_SIZE} entries {average}");
		}

		private static void DeleteOldEntries()
		{
			using (var context = new Context())
			{
				context.Database.ExecuteSqlCommand("DELETE FROM Test");
			}
		}

		private static List<Test> GenerateTestData()
		{
			return Enumerable.Range(1, NUMBER_OF_TEST_ITEMS)
				.Select(x => new Test
				{
					Name = "NećuPolitikuUSvojuButiku " + x,
					DateOfBirth = DateTime.UtcNow
				}).ToList();
		}
	}
}

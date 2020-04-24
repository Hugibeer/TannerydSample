using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tanneryd.BulkOperations.EF6;

namespace ConsoleApp1
{
	class Program
	{
		private const int NUMBER_OF_TEST_ITEMS = 1000000;
		private const int CHUNK_SIZE = 4000;

		static void Main(string[] args)
		{
			DeleteOldEntries();
			if (args.Length != 2)
			{
				Console.WriteLine("Use this tool with two parameters:");
				Console.WriteLine("\trange should be integer for number of threads to execute");
				Console.WriteLine("\tproc/bulk use proc to execute sql procedure, use bulk to execute tannyrd bulk insert all operation");
				Console.WriteLine("Sample: consoleapp1.exe 3 proc");
				return;
			}

			var range = int.Parse(args[0]);
			var useSql = args[1].ToLower() == "proc";

			Task[] taskArray = new Task[range];
			for (int i = 0; i < taskArray.Length; i++)
			{
				taskArray[i] = Task.Factory.StartNew(() => Measure(useSql));
			}
			Task.WaitAll(taskArray);
		}

		private static Double DoComputation(Double start)
		{
			var data = GenerateTestData();
			Double sum = 0;
			for (var value = start; value <= start + 10; value += .1)
				sum += value;

			return sum;
		}

		private static void Measure(bool useProcedure)
		{
			//Console.WriteLine($"Started at {DateTime.Now}");
			var data = GenerateTestData();
			var measurements = new List<double>();
			var stopwatch = new Stopwatch();

			using (var context = new Context())
			{
				stopwatch.Start();
				foreach (var bulk in data.BulkTake(CHUNK_SIZE))
				{
					var chunkWatch = new Stopwatch();
					chunkWatch.Start();

					if (useProcedure)
					{
						var parameter = GenerateDataTable("parameterName", bulk);
						var rowsAffected = context.Database.ExecuteSqlCommand("exec dbo.InsertTest @parameterName", parameter);
					}
					else
					{
						context.BulkInsertAll(bulk.ToList());
					}

					chunkWatch.Stop();

					//Console.WriteLine($"Elapsed {chunkWatch.Elapsed.TotalSeconds}");

					measurements.Add(chunkWatch.Elapsed.TotalSeconds);
				}

				stopwatch.Stop();
			}
			var average = measurements.Average();
			var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
			Console.WriteLine($"Total elapsed time {elapsedSeconds}, average seconds per {CHUNK_SIZE} entries {average}");

			//Console.WriteLine($"Ended at {DateTime.Now}");
		}

		private static SqlParameter GenerateDataTable(string parameterName, List<Test> bulk)
		{
			var dt = new DataTable();
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("DateOfBirth", typeof(DateTime));

			foreach (var value in bulk)
			{
				var row = dt.NewRow();
				row[0] = value.Name;
				row[1] = value.DateOfBirth;
				dt.Rows.Add(row);
			}
			//return dt;

			var sqlParameter = new SqlParameter(parameterName, SqlDbType.Structured)
			{
				TypeName = "dbo.TestInsertType",
				Value = dt
			};

			return sqlParameter;
		}

		private static void DeleteOldEntries()
		{
			using (var context = new Context())
			{
				context.Database.ExecuteSqlCommand("truncate table Test");
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

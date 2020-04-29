using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tanneryd.BulkOperations.EF6;

namespace ConsoleApp1
{
	class Program
	{
		enum Operation
		{
			Procedure, BulkExtensions, SqlBulkCopy
		}

		private const int NUMBER_OF_TEST_ITEMS = 1000000;
		private const int CHUNK_SIZE = 4000;

		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Use this tool with two parameters:");
				Console.WriteLine("\tnumber/analyze as first parameter");
				Console.WriteLine("\t\tuse integer number to indicate number of parallel thread executions");
				Console.WriteLine("\t\tuse analyze to analyze the resultset");
				Console.WriteLine("\tproc/bulk/bulkcopy/filename use proc to execute sql procedure, use bulk to execute tannyrd bulk insert operation");
				Console.WriteLine("\t\tuse proc to execute sql procedure");
				Console.WriteLine("\t\ttuse bulk to execute tannyrd bulk insert operation");
				Console.WriteLine("\t\tuse bulkcopy to execute SqlBulkCopy operation using DataTable aproach");
				Console.WriteLine("\t\tfilename of the file which should be analyzed");
				Console.WriteLine("Sample: consoleapp1.exe 3 proc");
				return;
			}

			var firstArgument = args[0];
			if (firstArgument.ToLower() == "analyze")
			{
				Analyze(args[1]);
				return;
			}
			DeleteOldEntries();
			var range = int.Parse(firstArgument);
			var operationString = args[1].ToLower();
			Operation operation = Operation.Procedure;
			if (operationString == "proc" )
			{
				operation = Operation.Procedure;
			}
			else if (operationString == "bulk")
			{
				operation = Operation.BulkExtensions;
			}
			else if (operationString == "bulkcopy")
			{
				operation = Operation.SqlBulkCopy;
			}

			Task[] taskArray = new Task[range];
			for (int i = 0; i < taskArray.Length; i++)
			{
				taskArray[i] = Task.Factory.StartNew(() => Measure(operation));
			}
			Task.WaitAll(taskArray);
		}

		private static void Analyze(string filename)
		{
			if (!File.Exists(filename))
			{
				throw new ArgumentException("Nonexistent file name", nameof(filename));
			}
			var lines = File.ReadAllLines(filename)
				.Select(line =>
				{
					var splitLine = line.Split(' ');
					var elapsedThreadTime = splitLine[3];
					elapsedThreadTime = ReplaceLastComma(elapsedThreadTime);

					return decimal.Parse(elapsedThreadTime);
				});
			if (!lines.Any())
			{
				Console.WriteLine("No entries");
			}

			var average = lines.Average();
			var min = lines.Min();
			var max = lines.Max();
			var tenPercentOfAverage = average * 0.2m;
			var totalItems = (double)lines.Count();
			var moreThanTenPercentDeviating = lines
				.Where(x => x - average > tenPercentOfAverage)
				.Count() / totalItems * 100.0;

			Console.WriteLine($"Average time per execution {average}, max {max}, min {min}, more than 20 percent deviation {moreThanTenPercentDeviating} percent");
		}

		private static string ReplaceLastComma(string elapsedThreadTime)
		{
			if (elapsedThreadTime.LastIndexOf(',') == elapsedThreadTime.Length - 1)
			{
				elapsedThreadTime = elapsedThreadTime.Substring(0, elapsedThreadTime.Length - 1);
			}

			return elapsedThreadTime;
		}

		private static void Measure(Operation operation)
		{
			var data = GenerateTestData();
			var measurements = new List<double>();
			var stopwatch = new Stopwatch();

			using (var context = new Context())
			{
				stopwatch.Start();
				for (int i = 0; i * CHUNK_SIZE < NUMBER_OF_TEST_ITEMS; i++)
				{
					var bulk = data
						.Skip(i * CHUNK_SIZE)
						.Take(CHUNK_SIZE)
						.ToList();

					var chunkWatch = new Stopwatch();
					chunkWatch.Start();
					switch (operation)
					{
						case Operation.Procedure:
							ExecuteProcedure(context, bulk);
							break;
						case Operation.BulkExtensions:
							TannerydInsert(context, bulk);
							break;
						case Operation.SqlBulkCopy:
							SqlBulkInsert(context, bulk);
							break;
						default:
							break;
					}

					chunkWatch.Stop();

					measurements.Add(chunkWatch.Elapsed.TotalSeconds);
				}

				stopwatch.Stop();
			}
			var average = measurements.Average();
			var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
			Console.WriteLine($"Total elapsed time {elapsedSeconds} average seconds per {CHUNK_SIZE} entries {average}");
		}

		private static void SqlBulkInsert(Context context, List<Test> bulk)
		{
			using (SqlBulkCopy bcp = new SqlBulkCopy(context.Database.Connection.ConnectionString))
			{
				bcp.DestinationTableName = "[Test]";

				bcp.ColumnMappings.Add("Name", "Name");
				bcp.ColumnMappings.Add("DateOfBirth", "DateOfBirth");

				var dt = GenerateDataTable(bulk);

				bcp.WriteToServer(dt);
			}
		}

		private static void TannerydInsert(Context context, List<Test> bulk)
		{
			context.BulkInsertAll(bulk.ToList());
		}

		private static void ExecuteProcedure(Context context, List<Test> bulk)
		{
			var parameter = GenerateParameter("parameterName", bulk);
			context.Database.ExecuteSqlCommand("exec dbo.InsertTest @parameterName", parameter);
		}

		private static SqlParameter GenerateParameter(string parameterName, List<Test> bulk)
		{
			var dt = GenerateDataTable(bulk);

			var sqlParameter = new SqlParameter(parameterName, SqlDbType.Structured)
			{
				TypeName = "dbo.TestInsertType",
				Value = dt
			};

			return sqlParameter;
		}

		private static DataTable GenerateDataTable(List<Test> bulk)
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

			return dt;
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

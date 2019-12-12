using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
	public class Test
	{
		[Key]
		public int Id { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}

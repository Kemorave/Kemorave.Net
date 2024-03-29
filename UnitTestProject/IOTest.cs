﻿using System;
using System.Linq;

using Kemorave.IO;
using Kemorave.IO.Zip;

using static UnitTestProject.Writing;

namespace UnitTestProject
{
	public class IOTest
	{
		[STAThread]
		public static void Main(string[] args)
		{
			if (args?.Length > 0)
			{


				foreach (string item in args)
				{
					Write(item);
				}

				Console.ReadKey();
				return;
			}
			string file = System.IO.Directory.EnumerateFiles(Environment.CurrentDirectory).FirstOrDefault();
			Write("=> " + file);
			Write("GetFileExtension => " + Path.GetFileExtension(file));
			Write("GetParentPath => " + Path.GetParentPath(file));
			Write("GetParentPathName => " + Path.GetParentPathName(file));
			Write("GetPathRoot => " + Path.GetPathRoot(file));
			Write("GetFileName => " + Path.GetFileName(file));
			Write("GetFileNameWithoutExtension => " + Path.GetFileNameWithoutExtension(file));
			TryPath();
			TryZipper();

			Console.ReadKey();
		}

		private static void TryZipper()
		{
			Zipper zipper = null;
			string lastWrite = string.Empty, lastFile = string.Empty;
			zipper = new Zipper((e) =>
			{
				if (lastFile != zipper.CurrentFile)
				{
					lastFile = zipper.CurrentFile;
					Write($"\n\nCompression of {zipper.CurrentFile}\n", ConsoleColor.Yellow);
				}
				string newWrite = $" {e} ";
				if (!newWrite.Equals(lastWrite, StringComparison.Ordinal))
				{
					lastWrite = newWrite;
					WriteOnLine("=");
				}
			})
			{
				DestinationFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Tonikaku Kawaii"),
				CompressionLevel = System.IO.Compression.CompressionLevel.Optimal,
				OverrideExistingFiles = true
			};
			zipper.CompressDirectory(@"F:\Anime k\GMV");
		}

		private static void TryPath()
		{
			throw new NotImplementedException();
		}


	}
	public class Writing
	{
		public static void WriteOnLine<T>(T obj)
		{
			Console.Write(obj);
		}

		public static void Write<T>(T obj)
		{
			Console.WriteLine(obj);
		}

		public static readonly ConsoleColor ConsoleDefaultColor = Console.ForegroundColor;
		public static void Write<T>(T obj, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(obj);
			Console.ForegroundColor = ConsoleDefaultColor;

		}
		public static void WriteOnLine<T>(T obj, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(obj);
			Console.ForegroundColor = ConsoleDefaultColor;

		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kemorave.IO;
using System.Threading.Tasks;

namespace UnitTestProject
{
  public  class IOTest
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
        { Kemorave.IO.Zipper zipper = null;
            String lastWrite = string.Empty,lastFile=string.Empty;
            zipper = new Zipper((e)=> { 
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
            });
            zipper.DestinationFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Tonikaku Kawaii");
            zipper.CompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            zipper.OverrideExistingFiles = true;
            zipper.CompressDirectory(@"F:\Anime k\GMV"); 
          }

        private static void TryPath()
        {
            throw new NotImplementedException();
        }

        static void WriteOnLine<T>(T obj) => Console.Write(obj);
        static void Write<T>(T obj) => Console.WriteLine(obj);
        static ConsoleColor ConsoleDefaultColor = Console.ForegroundColor;
        static void Write<T>(T obj, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(obj);
            Console.ForegroundColor = ConsoleDefaultColor;

        }
        static void WriteOnLine<T>(T obj, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(obj);
            Console.ForegroundColor = ConsoleDefaultColor;

        }
    }
}

using System;
using static Kemorave.IO.SystemInfo;
namespace Kemorave.IO
{
    public static class File
    {
        public enum FileType
        {
            None
         , File
        , Directory
         , Drive
         , Device
        }
        public enum Category
        {
            None
          , Audio
          , Photo
          , Archive
          , Database
          , Email
          , Internet
          , Video
          , Document
          , Executable
          , Other
        }

  
        
        public static void RenameFile(string file, string NewName, string NewExtension=null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (NewName == null)
            {
                throw new ArgumentNullException(nameof(NewName));
            }
            NewName = System.IO.Path.Combine(Path.GetParentPath(file), NewName);
            if (!string.IsNullOrEmpty(NewExtension))
            {
                NewName = System.IO.Path.ChangeExtension(NewName, NewExtension);
            }
            System.IO.File.Move(file, NewName);
        }
        public static void RenameDirectory(string dir, string NewName)
        {
            if (dir == null)
            {
                throw new ArgumentNullException(nameof(dir));
            }
            if (NewName == null)
            {
                throw new ArgumentNullException(nameof(NewName));
            }
            NewName = System.IO.Path.Combine(Path.GetParentPath(dir), NewName);
            System.IO.Directory.Move(dir, NewName);
        }

        public static bool FileEntryExist(string path)
        {
            return (System.IO.File.Exists(path) || System.IO.Directory.Exists(path));
        }

        /// <summary>
        /// Gets file category based on stored data
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Category GetFileCategory(string extension)
        {
            const System.StringComparison comparison = System.StringComparison.Ordinal;
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            } 
            if (extension[0] != '.')
            {
                extension = "." + extension;
            }
            foreach (string item in AudioFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Audio;
                }
            }

            foreach (string item in VideoFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Video;
                }
            }

            foreach (string item in ArchiveFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Archive;
                }
            }
            foreach (string item in PhotosFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Photo;
                }
            }

            foreach (string item in DocumentsFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Document;
                }
            }
            foreach (string item in ApplicationsFilesExtensions)
            {
                if (item.Equals(extension,comparison ))
                {
                    return Category.Executable;
                }
            }
            foreach (string item in InternetFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Internet;
                }
            }
            foreach (string item in DatabaseFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Database;
                }
            }
            foreach (string item in EmailFilesExtensions)
            {
                if (item.Equals(extension, comparison))
                {
                    return Category.Email;
                }
            }
            return Category.Other;
        }
       
    }
}
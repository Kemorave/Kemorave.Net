using System;
using System.Linq;

namespace Kemorave.IO
{
    public static class Path
    {
        public static char PathSeparator = System.IO.Path.DirectorySeparatorChar;
        public static char[] InavildNameChars { get; private set; } = System.IO.Path.GetInvalidFileNameChars();
        public static char[] InvalidPathChars { get; private set; } = System.IO.Path.GetInvalidPathChars();

        public static string RemoveInvaildPathCharacters(string path, char replacment = '_')
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            foreach (char item in InvalidPathChars)
            {
                if (path.Contains(item))
                {
                    path = path.Replace(item, replacment);
                }
            }
            return path;
        }
        public static string RemoveInvaildNameCharacters(string name, char replacment = '_')
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            foreach (char item in InavildNameChars)
            {
                if (name.Contains(name))
                {
                    name = name.Replace(item, replacment);
                }
            }
            return name;
        }

        public static string GetParentPathName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return GetFileName(GetParentPath(path));
        }
        public static string GetParentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == PathSeparator)
                {
                    return path.Substring(0, i);
                }
            }
            return path;
        }
        public static string GetPathRoot(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == PathSeparator)
                {
                    i++;
                    return path.Substring(0, i);
                }
            }

            return path;
        }
        public static string GetFileName(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == PathSeparator)
                {
                    i++;
                    return path.Substring(i, path.Length - i);
                }
            }

            return null;
        }
        public static string GetFileNameWithoutExtension(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                string ext = GetFileExtension(path), name = GetFileName(path);

                return name.Substring(0, name.Length - ext.Length);
            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// Gets file extension if none found returns <see langword="null"/> value 
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns></returns>
        public static string GetFileExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.')
                {
                    return path.Substring(i, path.Length - i);
                }
            }
            return null;
        }
        #region Path check

        public static bool IsParentDirectory(string destinationFolder, string folder)
        {
            if (string.IsNullOrEmpty(destinationFolder) || string.IsNullOrEmpty(folder))
            {
                return false;
            }
            //  Debug.WriteLine(destinationFolder + "\n" + folder);
            destinationFolder = RemoveStartingBackslash(destinationFolder);
            folder = RemoveStartingBackslash(folder);
            // Debug.WriteLine(destinationFolder + "==");
            string[] destinationarray = destinationFolder.Split(PathSeparator);
            string[] folderarray = folder.Split(PathSeparator);
            string path = "";
            for (int i = 0; i < destinationarray.Length; i++)
            {
                for (int x = 0; x < folderarray.Length; x++)
                {
                    if (destinationarray[i] == folderarray[x])
                    {
                        path += PathSeparator + destinationarray[i];
                        path = RemoveStartingBackslash(path);
                        // Debug.WriteLine(path + "==");
                        break;
                    }

                }
                if (path == folder || path == destinationFolder)
                {
                    // Debug.WriteLine(path + "=-=");
                    return true;
                }
            }
            return false;
        }
        public static string RemoveStartingBackslash(string path)
        {
            if (path[0] == '\\')
            {
                string vaild = path.Remove(0, 1);
                if (vaild.Length > 0)
                {
                    if (vaild[0] == '\\')
                    {
                        return RemoveStartingBackslash(vaild);
                    }
                    else
                    {
                        return vaild;
                    }
                }
                return vaild;
            }

            return path;
        }


        #endregion

    }
}

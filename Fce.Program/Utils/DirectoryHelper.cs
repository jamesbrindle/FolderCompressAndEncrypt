using System.Collections.Generic;
using System.IO;
using System;

namespace Fce.Utils
{
    /// <summary>
    /// Window directory helper methods
    /// </summary>
    internal static class DirectoryHelper
    {
        /// <summary>
        /// Returns recursed list of directories in a traversal order.
        /// </summary>
        /// <param name="targetDirectory">Parent directory</param>
        /// <returns>List of directories (string)</string></returns>
        public static List<string> GetAllDirectoriesTraversive(this string targetDirectory, bool ignoreSystem = true)
        {
            targetDirectory = targetDirectory.LongPathSafe();
            List<string> directories = new List<string>();
            TraverseDirectories(targetDirectory, directories, ignoreSystem);

            for (int i = 0; i < directories.Count; i++)
                directories[i] = directories[i].NormalPath();

            return directories;
        }

        /// <summary>
        /// Returns recursed list of files in a traversal order.
        /// </summary>
        /// <param name="targetDirectory">Parent directory</param>
        /// <returns>List of files (string)</string></returns>
        public static List<string> GetAllFilesTraversive(string targetDirectory, bool ignoreSystem = true)
        {
            targetDirectory = targetDirectory.LongPathSafe();

            List<string> directories = new List<string>();
            List<string> files = new List<string>();

            TraverseDirectories(targetDirectory, directories, ignoreSystem);

            for (int i = 0; i < directories.Count; i++)
            {
                try
                {
                    if (ignoreSystem)
                    {
                        if (!directories[i].Contains("$"))
                        {
                            foreach (var file in Directory.GetFiles(directories[i]))
                            {
                                if (ignoreSystem)
                                {
                                    if (!file.Contains("$") &&
                                        Path.GetFileName(file) != "bootmgr" &&
                                        Path.GetFileName(file) != "SYSTAG.BIN" &&
                                        Path.GetFileName(file) != "AMTAG.BIN" &&
                                        Path.GetFileName(file) != "hiberfil.sys" &&
                                        Path.GetFileName(file) != "pagefile.sys" &&
                                        Path.GetFileName(file) != "swapfile.sys")
                                        files.Add(file);
                                }
                                else
                                {
                                    files.Add(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in Directory.GetFiles(directories[i]))
                        {
                            if (ignoreSystem)
                            {
                                if (!file.Contains("$") &&
                                    Path.GetFileName(file) != "bootmgr" &&
                                    Path.GetFileName(file) != "SYSTAG.BIN" &&
                                    Path.GetFileName(file) != "AMTAG.BIN" &&
                                    Path.GetFileName(file) != "hiberfil.sys" &&
                                    Path.GetFileName(file) != "pagefile.sys" &&
                                    Path.GetFileName(file) != "swapfile.sys")
                                    files.Add(file);
                            }
                            else
                            {
                                files.Add(file);
                            }
                        }
                    }
                }
                catch { }
            }

            for (int i = 0; i < files.Count; i++)
                files[i] = files[i].NormalPath();

            return files;
        }

        /// <summary>
        /// Returns recursed list of 'FileInfo' objects in a traversal order.
        /// </summary>
        /// <param name="targetDirectory">Parent directory</param>
        /// <returns>List of FileInfo objects</string></returns>
        public static List<FileInfo> GetAllFileInfoTraversive(string targetDirectory, bool ignoreSystem = true)
        {
            targetDirectory = targetDirectory.LongPathSafe();

            List<string> directories = new List<string>();
            List<FileInfo> files = new List<FileInfo>();
            TraverseDirectories(targetDirectory, directories, ignoreSystem);

            for (int i = 0; i < directories.Count; i++)
            {
                try
                {
                    if (ignoreSystem)
                    {
                        if (!directories[i].Contains("$"))
                        {
                            foreach (var file in Directory.GetFiles(directories[i]))
                            {
                                if (ignoreSystem)
                                {
                                    if (!file.Contains("$") &&
                                        Path.GetFileName(file) != "bootmgr" &&
                                        Path.GetFileName(file) != "SYSTAG.BIN" &&
                                        Path.GetFileName(file) != "AMTAG.BIN" &&
                                        Path.GetFileName(file) != "hiberfil.sys" &&
                                        Path.GetFileName(file) != "pagefile.sys" &&
                                        Path.GetFileName(file) != "swapfile.sys")
                                        files.Add(new FileInfo(file));
                                }
                                else
                                {
                                    files.Add(new FileInfo(file));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in Directory.GetFiles(directories[i]))
                        {
                            if (ignoreSystem)
                            {
                                if (!file.Contains("$") && Path.GetFileName(file) != "bootmgr" && Path.GetFileName(file) != "SYSTAG.BIN" && Path.GetFileName(file) != "AMTAG.BIN")
                                    files.Add(new FileInfo(file));
                            }
                            else
                            {
                                files.Add(new FileInfo(file));
                            }
                        }
                    }
                }
                catch { }
            }

            for (int i = 0; i < files.Count; i++)
                files[i] = new FileInfo(files[i].FullName.NormalPath());

            return files;
        }

        /// <summary>
        /// Recursively traverse a directory building up a list
        /// </summary>
        /// <param name="targetDirectory">Root directory to start from</param>
        /// <param name="directories">Building list of directories (string)</param>
        /// <param name="ignoreSystem">Ignore certain system files that can't be read anyway</param>
        private static void TraverseDirectories(string targetDirectory, List<string> directories, bool ignoreSystem = true)
        {
            directories.Add(targetDirectory);

            try
            {
                if (ignoreSystem)
                {
                    if (!targetDirectory.Contains("$"))
                    {
                        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                        foreach (string subdirectory in subdirectoryEntries)
                        {
                            if (!subdirectory.Contains("$"))
                                TraverseDirectories(subdirectory, directories, ignoreSystem);
                        }
                    }
                }
                else
                {
                    string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        TraverseDirectories(subdirectory, directories, ignoreSystem);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Checks whether a file is locked - Useful if we want to write to a file
        /// </summary>
        /// <param name="path">Path of file to check if locked</param>
        /// <returns>True is locked, false otherwise</returns>
        public static bool IsFileLocked(string path)
        {
            path = path.LongPathSafe();
            // it must exist for it to be locked
            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                FileStream stream = null;

                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                    return true;
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }

            //file is not locked
            return false;
        }
    }
}

using FolderCompressAndEncrypt.Utils;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FolderCompressAndEncrypt
{
    internal class Runner
    {
        private static SevenZipCompressor _compressor = null;
        private static List<string> _files = null;

        /// <summary>
        /// Run the main compress and encrypt or decompress operation
        /// </summary>
        public static void Run()
        {
            Program.Logger.Log(Logger.LogType.Header, "Main Operation Started");
            CleanupTemporaryFiles(false);

            if (!Program.OptionValues.Decompressing)
            {
                SetCompressionOptions();
                CompressFilesList();
                if (Program.OptionValues.Clean)
                    DeleteArchivesWithoutSource();
            }
            else
            {

            }

            CleanupTemporaryFiles(true);

            Program.Logger.Log(Logger.LogType.Header, "ALL OPERATIONS COMPLETE");
            Program.Logger.Space();
        }

        /// <summary>
        /// Sub-method to Setup 7z compression object with options
        /// </summary>
        private static void SetCompressionOptions()
        {
            Program.Logger.Log(Logger.LogType.Header, "Setting Compression Options");

            SevenZipBase.SetLibraryPath(
                EmbeddedResourceHelper.GetEmbeddedResourcePath(
                        TargetAssemblyType.Executing,
                        "7z.dll", "Embed"));

            _compressor = new SevenZipCompressor(Program.OptionValues.TempPath);
            _compressor.EncryptHeaders = true;
            _compressor.ZipEncryptionMethod = ZipEncryptionMethod.Aes128;
            _compressor.CompressionLevel = (CompressionLevel)Enum.Parse(typeof(CompressionLevel), Program.OptionValues.CompressionLevel.ToString());

            switch (Program.OptionValues.CompressionLevel)
            {
                case Models.Enums.CompressionLevel.None:
                    _compressor.CompressionMethod = CompressionMethod.Copy;
                    break;
                case Models.Enums.CompressionLevel.Low:
                    _compressor.CompressionMethod = CompressionMethod.Default;
                    break;
                case Models.Enums.CompressionLevel.Normal:
                    _compressor.CompressionMethod = CompressionMethod.Deflate;
                    break;
                case Models.Enums.CompressionLevel.High:
                    _compressor.CompressionMethod = CompressionMethod.Lzma2;
                    break;
                case Models.Enums.CompressionLevel.Ultra:
                    _compressor.CompressionMethod = CompressionMethod.Lzma2;
                    break;
                default:
                    break;
            }

            Program.Logger.Log(Logger.LogType.Info, $"  - Compression level: {Program.OptionValues.CompressionLevel}");
            Program.Logger.Log(Logger.LogType.Info, $"  - Compression method: {_compressor.CompressionMethod}");
            Program.Logger.Log(Logger.LogType.Info, $"  - Encryption Method: {ZipEncryptionMethod.Aes128}");
            Program.Logger.Log(Logger.LogType.Info, $"  - Encrypt headers: {true}");
            Program.Logger.Log(Logger.LogType.Info, $"  - Encrypt directory and filenames: {Program.OptionValues.EncryptFilenames}");
            Program.Logger.Log(Logger.LogType.Info, $"  - Temporary path: {Program.OptionValues.TempPath}");

            Program.Logger.Log(Logger.LogType.Info, "Setting compression options complete");
            Program.Logger.Space();
        }

        /// <summary>
        /// Gathers files list to compress and calls the method to compress them
        /// </summary>
        private static void CompressFilesList()
        {
            ConsoleEx.WriteColouredLine("Compressing...", ConsoleColor.Yellow);

            Program.Logger.Log(Logger.LogType.Header, "Folder Scan Started");
            Program.Logger.Log(Logger.LogType.Info, "Gathering list of files to compress...");

            if (Program.OptionValues.Recursive)
                _files = DirectoryHelper.GetAllFilesTraversive(Program.OptionValues.InputFolder, true);
            else
                _files = Directory.GetFiles(Program.OptionValues.InputFolder, "*.*", SearchOption.TopDirectoryOnly).ToList();

            if (_files != null && _files.Count > 0)
            {
                Program.Logger.Log(Logger.LogType.Info, $"Found {_files.Count} files(s)...");
                Program.Logger.Space();
                Program.Logger.Log(Logger.LogType.Header, $"Compress and / or Encrypt Processing Started");
            }
            else
                Program.Logger.Log(Logger.LogType.Info, $"No files found...");

            foreach (var file in _files)
            {
                ConsoleEx.WriteColoured($"\n[{_files.IndexOf(file) + 1} of {_files.Count}]", ConsoleColor.Cyan);
                ConsoleEx.WriteColoured($" {Path.GetFileName(file)}", ConsoleColor.White);

                Program.Logger.Log(Logger.LogType.Info, $"({_files.IndexOf(file) + 1} of {_files.Count}) {Path.GetFileName(file)}");

                try
                {
                    string outputDirectory = string.Empty;
                    string tailDirectory = Path.GetDirectoryName(new DirectoryInfo(Program.OptionValues.InputFolder).Parent == null
                                ? new DriveInfo(Program.OptionValues.InputFolder).Name.TrimEnd('\\').TrimEnd(':') + "\\" +
                                    file.Replace(new DirectoryInfo(Program.OptionValues.InputFolder).FullName, "").TrimStart('\\')
                                : file.Replace(new DirectoryInfo(Program.OptionValues.InputFolder).Parent.FullName, "").TrimStart('\\'));

                    var tailPathDirs = tailDirectory.Split(Path.DirectorySeparatorChar);

                    string tailPathStart = tailPathDirs[0];
                    string tailPathEnd = string.Empty;

                    // Avoid recompressing what we've already compressed if the input path contains the output path in its directory a subdirectory
                    if (tailPathDirs.Length > 1)
                    {
                        if (Program.OptionValues.EncryptFilenames)
                        {
                            try
                            {
                                tailPathEnd = StringEncrypt.Decrypt(tailPathDirs[tailPathDirs.Length - 1]);
                            }
                            catch
                            {
                                tailPathEnd = tailPathDirs[tailPathDirs.Length - 1];
                            }
                        }
                        else
                            tailPathEnd = tailPathDirs[tailPathDirs.Length - 1];
                    }

                    if (!(tailPathDirs.Length > 1 || tailPathStart == tailPathEnd))
                    {
                        if (Program.OptionValues.EncryptFilenames)
                        {
                            string[] directories = tailDirectory.Split(Path.DirectorySeparatorChar);
                            string encryptedDirectory = "";
                            for (int i = 0; i < directories.Length; i++)
                                encryptedDirectory += StringEncrypt.Encrypt(directories[i]) + "\\";

                            outputDirectory = Path.Combine(Program.OptionValues.OutputFolder, encryptedDirectory);
                        }
                        else
                        {
                            outputDirectory = Path.Combine(Program.OptionValues.OutputFolder, tailDirectory);
                        }

                        if (!Directory.Exists(outputDirectory))
                            Directory.CreateDirectory(outputDirectory);

                        string outputFilename;
                        if (Program.OptionValues.EncryptFilenames)
                            outputFilename = $"{StringEncrypt.Encrypt(Path.GetFileName(file))}.7z";
                        else
                            outputFilename = $"{Path.GetFileName(file)}.7z";

                        if (File.Exists(Path.Combine(outputDirectory, outputFilename)))
                        {
                            if (Program.OptionValues.ForceOverwrite)
                            {
                                Program.Logger.Log(Logger.LogType.Info, "  - Already exists at destination but force overwrite enabled. Continuing...");
                                File.Delete(Path.Combine(outputDirectory, outputFilename));
                                CompressFile(file, Path.Combine(outputDirectory, outputFilename));
                            }
                            else
                            {
                                Program.Logger.Log(Logger.LogType.Info, $"  - Already exists at destination - Skipping");
                                ConsoleEx.WriteColoured($" Already exists: Skipping", ConsoleColor.Green);
                            }
                        }
                        else
                        {
                            CompressFile(file, Path.Combine(outputDirectory, outputFilename));
                        }
                    }
                    else
                    {
                        Program.Logger.Log(Logger.LogType.Info, $"  - Input path contains output path - Skipping");
                        ConsoleEx.WriteColoured($" Input path contains output path: Skipping", ConsoleColor.Green);
                    }
                }
                catch (Exception e)
                {
                    Program.Logger.Log(e, $"  - Unable to compress file: {Path.GetFileName(file)}");
                    ConsoleEx.WriteColoured($" Error: Skipping: {e.Message}", ConsoleColor.Red);
                }
            }

            Program.Logger.Space();
        }

        /// <summary>
        /// Compress an individual file 
        /// </summary>
        /// <param name="inputFile">File path to compress</param>
        /// <param name="outputFile">Output path of 7z compressed archive</param>
        private static void CompressFile(string inputFile, string outputFile)
        {
            if (!string.IsNullOrEmpty(Program.OptionValues.Password))
                _compressor.CompressFilesEncrypted(outputFile, Program.OptionValues.Password, inputFile);
            else
                _compressor.CompressFiles(outputFile, inputFile);
        }

        private void DecompressFiles()
        {

        }

        /// <summary>
        /// For source file deletion monitoring (a sort of 'sync') - delete the remaining archive if the original source file not present
        /// </summary>
        private static void DeleteArchivesWithoutSource()
        {
            Program.Logger.Log(Logger.LogType.Header, "Output Folder Cleanup Started");

            ConsoleEx.WriteColouredLine("\n\nDeleting archives where there is no longer a source file...", ConsoleColor.Yellow);

            Program.Logger.Log(Logger.LogType.Info, "Gathering list of files to delete...");
            var filesListInOutputFolder = DirectoryHelper.GetAllFilesTraversive(Program.OptionValues.OutputFolder, true);
            var archiveList = new List<string>();

            foreach (var file in filesListInOutputFolder)
            {
                if (Path.GetExtension(file).ToLower() == ".7z")
                    archiveList.Add(file);
            }

            bool found = false;
            foreach (var archiveFile in archiveList)
            {
                string sourceFileName = string.Empty;

                if (Program.OptionValues.EncryptFilenames)
                {
                    string unencryptedPath = string.Empty;
                    string[] directories = Path.GetDirectoryName(archiveFile).Split(Path.DirectorySeparatorChar);
                    for (int i = 0; i < directories.Length; i++)
                    {
                        if (directories[i].StartsWith("Š"))
                            unencryptedPath += StringEncrypt.Decrypt(directories[i]) + "\\";
                        else
                            unencryptedPath += directories[i] + "\\";
                    }

                    sourceFileName = Path.Combine(
                        Path.GetDirectoryName(Program.OptionValues.InputFolder),
                        Path.GetDirectoryName(unencryptedPath).Replace(Program.OptionValues.OutputFolder, "").TrimStart('\\'),
                        StringEncrypt.Decrypt(Path.GetFileNameWithoutExtension(archiveFile)));
                }
                else
                {
                    sourceFileName = Path.Combine(
                        Path.GetDirectoryName(Program.OptionValues.InputFolder),
                        Path.GetDirectoryName(archiveFile).Replace(Program.OptionValues.OutputFolder, "").TrimStart('\\'),
                        Path.GetFileNameWithoutExtension(archiveFile));
                }

                if (!File.Exists(sourceFileName))
                {
                    found = true;

                    try
                    {
                        Program.Logger.Log(Logger.LogType.Info, $"Deleting file: {Path.GetFileName(archiveFile)}");
                        ConsoleEx.WriteColouredLine($"Deleting file: {Path.GetFileName(archiveFile)} ({Path.GetFileName(sourceFileName)})", ConsoleColor.White);

                        File.Delete(archiveFile);
                    }
                    catch (Exception e)
                    {
                        Program.Logger.Log(Logger.LogType.Warning, $"  - Unable to delete file: {Path.GetFileName(archiveFile)}: Warning: {e.Message}");
                        ConsoleEx.WriteColoured($" Error: Skipping: {e.Message}", ConsoleColor.Red);
                    }
                }
            }

            if (!found)
            {
                Program.Logger.Log(Logger.LogType.Info, $"No files to delete found");
                ConsoleEx.WriteColoured($"None found", ConsoleColor.Green);
            }

            DeleteEmptyFoldersFromOutputPath();

            Program.Logger.Log(Logger.LogType.Info, "Output folder cleanup complete");
            Program.Logger.Space();
        }

        /// <summary>
        /// Delete any empty folders in output directory recursively
        /// </summary>
        public static void DeleteEmptyFoldersFromOutputPath()
        {
            Program.Logger.Log(Logger.LogType.Info, $"Deleting any empty folder(s) in the destination...");

            Thread.Sleep(100);

            var allFolders = DirectoryHelper.GetAllDirectoriesTraversive(Program.OptionValues.OutputFolder, false);
            foreach (var directory in allFolders)
            {
                if (directory != Program.OptionValues.OutputFolder) // don't delete original
                {
                    var files = DirectoryHelper.GetAllFilesTraversive(directory, false);
                    if (files == null || files.Count == 0)
                    {
                        try
                        {
                            Directory.Delete(directory);
                        }
                        catch { }
                    }
                }
            }

            Program.Logger.Log(Logger.LogType.Info, $"Empty folder deletion complete ");
        }

        /// <summary>
        /// Delete any temporary files created by this utility and 7z
        /// </summary>
        /// <param name="last">Cleanup order (just displays a different message)</param>
        private static void CleanupTemporaryFiles(bool last)
        {
            Program.Logger.Log(Logger.LogType.Info, $"{(last ? "Re-" : "")}Cleaning up any temporary files...");

            ConsoleEx.WriteColouredLine($"{(last ? "\n\nRe-" : "")}Cleaning up any temporary files... ", ConsoleColor.Yellow);
            ConsoleSpinner.Show();

            if (Directory.Exists(Program.OptionValues.TempPath))
            {
                foreach (var file in Directory.GetFiles(Program.OptionValues.TempPath, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (!(Path.GetDirectoryName(file).Contains(Program.OptionValues.LogPath) ||
                            Path.GetDirectoryName(file).Contains(Path.Combine(Path.GetTempPath(), "FCE-Temp", "Logs"))))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception e)
                    {
                        Program.Logger.Log(Logger.LogType.Warning, $"Could not delete temporary file: {file}: Warning: " + e.Message);
                    }
                }
            }
            else
                Directory.CreateDirectory(Program.OptionValues.TempPath);

            ConsoleSpinner.Hide();
            Console.WriteLine();

            Program.Logger.Log(Logger.LogType.Info, "Temporary files clean complete");
            Program.Logger.Space();
        }
    }
}

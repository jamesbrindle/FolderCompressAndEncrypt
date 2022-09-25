using Fce.Models;
using Fce.Utils;
using System;
using System.IO;
using System.Reflection;

namespace Fce
{
    internal class Program
    {
        /// <summary>
        /// Options as defined by the input parameters / arugments
        /// </summary>
        internal static OptionValues OptionValues = new OptionValues();

        /// <summary>
        /// Operation file logger
        /// </summary>
        internal static Logger Logger = null;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

            ConsoleEx.WriteColouredLine("-----------------------------------------", ConsoleColor.Cyan);
            ConsoleEx.WriteColouredLine("-- Folder Compress And Encrypt Utility --", ConsoleColor.Cyan);
            ConsoleEx.WriteColouredLine("-----------------------------------------\n", ConsoleColor.Cyan);

            /*
             * DEBUG
            */
#if DEBUG
            // Compress
            args = new string[] {
                "-i", @"c:\temp",
                "-o", @"c:\temp",
                "-m", "none",
                "-r",
                "-l", // default log path
                "-e",
                "-c",
                "-p", "SomePassword" };

            // Extract
            //args = new string[] {
            //    "-i", @"c:\temp",
            //    "-o", @"c:\temp",
            //    "-m", "none",
            //    "-r",
            //    "-d",
            //    "-l", // default log path
            //    "-e",
            //    "-c",
            //    "-p", "SomePassword" };

            // Help
            //args = new string[] {
            //    "-h" };
#endif
            /*
             * DEBUG END
            */

            if (args == null || args.Length == 0)
                ConsoleEx.WriteColouredLine("Command line arguments expected! Use fla -h for options.", ConsoleColor.Yellow);

            try
            {
                var p = new OptionSet {
                { "i|input=",
                  OptionValues.InputFolder.Description<OptionValues>("InputFolder"),
                  (v) => OptionValues.InputFolder = v },

                { "o|output=",
                  OptionValues.OutputFolder.Description<OptionValues>("OutputFolder"),
                  (v) => OptionValues.OutputFolder = v },

                { "t|tempdir=",
                  OptionValues.TempPath.Description<OptionValues>("TempPath"),
                  (v) => OptionValues.TempPath = v == null ? OptionValues.TempPath : v },

                { "l|log:",
                  OptionValues.LogPath.Description<OptionValues>("LogPath"),
                  (v) => { OptionValues.LoggingEnabled = true; OptionValues.LogPath = v == null ? OptionValues.LogPath : v; } },

                { "d|decompress",
                  OptionValues.Decompressing.Description<OptionValues>("Decompressing"),
                  (v) => OptionValues.Decompressing = v != null },

                { "r|recursive",
                  OptionValues.Recursive.Description<OptionValues>("Recursive"),
                  (v) => OptionValues.Recursive = v != null },

                { "e|encrypt",
                  OptionValues.EncryptFilenames.Description<OptionValues>("EncryptFilenames"),
                  (v) => OptionValues.EncryptFilenames = v != null },

                { "p|password=",
                  OptionValues.Password.Description<OptionValues>("Password"),
                  (v) => OptionValues.Password = v },

                { "f|overwrite",
                  OptionValues.ForceOverwrite.Description<OptionValues>("ForceOverwrite"),
                  (v) => OptionValues.ForceOverwrite = v != null },

                { "m|mode=",
                  OptionValues.CompressionLevel.Description<OptionValues>("CompressionLevel"),
                  (v) => OptionValues.CompressionLevel = v == null || v.ToString().ToLower() == "medium"
                            ? Models.Enums.CompressionLevel.Normal
                            : (Models.Enums.CompressionLevel) Enum.Parse(typeof(Models.Enums.CompressionLevel), v, true ) },

                { "c|clean",
                  OptionValues.Clean.Description<OptionValues>("Clean"),
                  (v) => OptionValues.Clean = v != null },

                { "v|version",
                  OptionValues.ShowVersion.Description<OptionValues>("ShowVersion"),
                  (v) => OptionValues.ShowVersion = v != null  },

                { "h|?|help",
                  OptionValues.ShowHelp.Description<OptionValues>("ShowHelp"),
                  (v) => OptionValues.ShowHelp =  v != null  },
                };

                p.Parse(args).ToArray();

                if (OptionValues.ShowHelp)
                {
                    ConsoleEx.WriteColouredLine($"Version: {GetVersion()}, By James Brindle\n", ConsoleColor.Green);

                    Console.Write(
                         "This utility will compress each file in a folder structure (7z) into its own compressed archived (1 file per archive).\n" +
                         "Directories will be created in the output folder maintaining the folder structure of the input folder if the '-r' - recursive option is used.\n\n" +
                         "You have the option to encrypt the archive filename and password protect each archive. You will need to provide the password when decrypting.\n\n" +
                         "When you uncompress and decrypt, the filenames and folder structure will be restored.\n\n" +

                         "The intention of this application is for secure backups, so you can run again and again on the same input folder and output folder and only " +
                         "the changes will written, however, you can use options '-f' and '-n' to alter this behaviour.\n\n" +

                         "Remember to wrap paths that contains spaces with double quotes, i.e: ");

                    ConsoleEx.WriteColoured("\"c:\\Some Path\\Some Subpath\".\n\n", ConsoleColor.Yellow);
                    Console.WriteLine("The available options / arguments are as follows:\n\n");

                    p.WriteOptionDescriptions(Console.Out);
                    return;
                }
                else if (OptionValues.ShowVersion)
                {
                    Console.WriteLine($"Folder Compress And Encrypt Utility: Version: {GetVersion()}, By {"James Brindle"}");
                }
                else
                {
                    // Validation

                    Logger = new Logger(OptionValues.LogPath, OptionValues.LoggingEnabled);
                    Logger.Log(Logger.LogType.Header, "Arguments Validation Started");
                    Logger.Log(Logger.LogType.Info, "Checking given input path...");

                    if (string.IsNullOrWhiteSpace(OptionValues.InputFolder) || !Directory.Exists(OptionValues.InputFolder))
                    {
                        string message = $"Input path not valid: {(string.IsNullOrWhiteSpace(OptionValues.InputFolder) ? "NULL path" : OptionValues.InputFolder)}. Exiting!";
                        Logger.Log(Logger.LogType.Error, message);

                        throw new ApplicationException(message);
                    }

                    Logger.Log(Logger.LogType.Info, "Input path valid");
                    Logger.Log(Logger.LogType.Info, "Checking given output path...");

                    if (string.IsNullOrWhiteSpace(OptionValues.OutputFolder))
                    {
                        string message = "Output path not valid: NULL path. Exiting!";
                        Logger.Log(Logger.LogType.Error, message);

                        throw new ApplicationException(message);
                    }

                    try
                    {
                        string path = Path.GetFullPath(OptionValues.OutputFolder);
                    }
                    catch
                    {
                        string message = $"Output path is not valid: {OptionValues.OutputFolder}. Exiting!";
                        Logger.Log(Logger.LogType.Error, message);

                        throw new ApplicationException(message);
                    }

                    Logger.Log(Logger.LogType.Info, "Output path valid");
                    Logger.Log(Logger.LogType.Info, "Checking temp path...");

                    try
                    {
                        string path = Path.GetFullPath(OptionValues.TempPath);
                    }
                    catch
                    {
                        string message = $"Temp path is not valid: {OptionValues.TempPath}. Exiting!";
                        Logger.Log(Logger.LogType.Error, message);

                        throw new ApplicationException(message);
                    }

                    Logger.Log(Logger.LogType.Info, "Temp path valid");
                    Logger.Log(Logger.LogType.Info, "Argument validation complete: Settings:");
                    Logger.Log(Logger.LogType.Info, $"  - Operation mode: {(OptionValues.Decompressing ? "Decompress" : "Compress" + (OptionValues.EncryptFilenames ? " & Encrypt" : ""))}");
                    Logger.Log(Logger.LogType.Info, $"  - Input path: {OptionValues.InputFolder}");
                    Logger.Log(Logger.LogType.Info, $"  - Output path: {OptionValues.OutputFolder}");
                    Logger.Log(Logger.LogType.Info, $"  - Temp path: {OptionValues.TempPath}");
                    Logger.Log(Logger.LogType.Info, $"  - Recursive: {OptionValues.Recursive}");
                    Logger.Log(Logger.LogType.Info, $"  - Encrypt filenames: {OptionValues.EncryptFilenames}");
                    Logger.Log(Logger.LogType.Info, $"  - Password protected: {(string.IsNullOrEmpty(OptionValues.Password) ? "False" : "True")}");
                    Logger.Log(Logger.LogType.Info, $"  - Force overwrite: {OptionValues.ForceOverwrite}");
                    Logger.Log(Logger.LogType.Info, $"  - Compression level: {OptionValues.CompressionLevel}");
                    Logger.Log(Logger.LogType.Info, $"  - Monitor for deletions: {OptionValues.Clean}");

                    Logger.Space();
                }
            }
            catch (Exception e)
            {
                if (Logger != null)
                    Logger.Log(e, "Unable to start main operation - Check input parameters. Application will quit!");

                Console.Error.WriteLine($"Oops! Something is not quite right, check your argument flags: {e.Message}");
                return;
            }

            try
            {
                Runner.Run();
            }
            catch (Exception e)
            {
                if (Logger != null)
                    Logger.Log(e, "An exception occurred while running the main operation");

                Console.Error.WriteLine($"A exception occurred while processing the file, application will now exit: {e.Message}");
            }
        }

        /// <summary>
        /// Output the current version of the application
        /// </summary>
        private static string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Dependencies are embedded in this dependency, we may need to extract them
        /// </summary>
        /// <returns>Loaded assembly</returns>
        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("SevenZipSharp"))
                return Assembly.LoadFile(
                    EmbeddedResourceHelper.GetEmbeddedResourcePath(
                        TargetAssemblyType.Executing,
                        "SevenZipSharp.dll", "Embed"));
            else
                return null;
        }
    }
}

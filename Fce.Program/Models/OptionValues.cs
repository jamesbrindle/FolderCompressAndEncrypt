using Fce.Models.Enums;
using System.ComponentModel;
using System.IO;

namespace Fce.Models
{
    /// <summary>
    /// Model store for program options and set from command line arguments (or defaulted if not set and not required)
    /// </summary>
    public class OptionValues
    {
        /// <summary>
        /// Input folder to compress.
        /// </summary>
        [Description("Input folder to compress.")]
        public string InputFolder { get; set; } = null;

        /// <summary>
        /// Output folder of archives. The folder structure of input folder will be maintained. 
        /// Don't worry if the input folder contains the output folder; 
        /// on subsequant passes the output folder will be skipped.
        /// </summary>
        [Description("Output folder of archives. The folder structure of input folder will be maintained. " +
            "Don't worry if the input folder contains the output folder; " +
            "on subsequant passes the output folder will be skipped.")]
        public string OutputFolder { get; set; } = null;

        /// <summary>
        /// You can optionally set a directory for the tempory files the tool uses when compressing files.
        /// </summary>
        [Description("You can optionally set a directory for the tempory files the tool uses when compressing files.")]
        public string TempPath { get; set; } = Path.Combine(Path.GetTempPath(), "FCE-Temp");


        /// <summary>
        /// If true, a log file will be created an appended for the process. You optional set the log path, or it
        /// will default to: %LocalAppData%\\Temp\\FCE-Temp\\Logs
        /// </summary>
        [Description("If true, a log file will be created an appended for the process. You optional set the log path, " +
                     "or it will default to: %LocalAppData%\\Temp\\FCE-Temp\\Logs")]
        public bool LoggingEnabled { get; set; } = false;

        /// <summary>
        /// Enable creating a log and optionally set a log path. If you use the '-l' flag 
        /// but don't set a path the log file will be placed here: %LocalAppData%\\Temp\\FCE-Temp\\Logs
        /// </summary>
        [Description("Enable creating a log and optionally set a log path. If you use the '-l' flag " +
                     "but don't set a path the log file will be placed in location: %LocalAppData%\\Temp\\FCE-Temp\\Logs")]
        public string LogPath { get; set; } = Path.Combine(Path.GetTempPath(), "FCE-Temp", "Logs");

        /// <summary>
        /// Decompress all the archives in a folder. If you password protected the archives you will need to provide this 
        /// (-p=[value]).
        /// </summary>
        [Description("Decompress all the archives in a folder. If you password protected the archives you will need to " +
                     "provide this (-p=<your password>).")]
        public bool Decompressing { get; set; } = false;

        /// <summary>
        /// If 'recursive' flag is used, then all files in all subdirectories will be included, otherwise only the top level 
        /// files in the output folder will be included.
        /// </summary>
        [Description("If 'recursive' flag is used, then all files in all subdirectories will be included, otherwise only " +
                     "the top level files in the output folder will be included.")]
        public bool Recursive { get; set; } = false;

        /// <summary>
        /// Encrypt directory and filenames. Upon 'decompression' they will be restored.
        /// </summary>
        [Description("Encrypt directory and filenames. Upon 'decompression' they will be restored.")]
        public bool EncryptFilenames { get; set; } = false;

        /// <summary>
        /// Password to open archive (wrap in quotes if it contains spaces or special characters).
        /// </summary>
        [Description("Password to open archive (wrap in quotes if it contains spaces or special characters).")]
        public string Password { get; set; } = null;

        /// <summary>
        /// If the resulting file already exists it will be skipped. Use this option to force overwrite regardless.
        /// </summary>
        [Description("If the resulting file already exists it will be skipped. Use this option to force overwrite regardless.")]
        public bool ForceOverwrite { get; set; } = false;

        /// <summary>
        /// File compression mode to use If no value, then 'normal' will be used. In 'low' and 'normal' compression modes, the 
        /// 'Deflate' algorithm will be used, in 'high' and 'ultra', the 'Lzma2' algorithm will be used. Higher compression takes 
        /// longer.
        /// </summary>
        [Description("File compression mode to use. " +
                     "If no value, then 'normal' will be used. In 'low' and 'normal' compression modes, the 'Deflate' algorithm " +
                     "will be used, in 'high' and 'ultra', the 'Lzma2' algorithm will be used. Higher compression takes longer.")]
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Normal;

        /// <summary>
        /// Remove archives if the resulting files exist in the output folder but not the input folder (i.e. monitor source deletions).
        /// </summary>
        [Description("Remove archives if the resulting files exist in the output folder but not the input folder " +
                    "(i.e. monitor source deletions).")]
        public bool Clean { get; set; } = false;

        /// <summary>
        /// Show help and available arguments.
        /// </summary>
        [Description("Show help and available arguments.")]
        public bool ShowHelp { get; set; } = false;

        /// <summary>
        /// Shows the version number.
        /// </summary>
        [Description("Shows the version number.")]
        public bool ShowVersion { get; set; } = false;

        /// <summary>
        /// (REQUIRES ELEVATED PERMISSION - 'Run As Admin') - Set the Windows registry to enable long path support as typically it's 
        /// limited to 260 characters. If elevated process not detected it will be skipped. This flag can be used on its own or 
        /// together with a compress /extract operation.
        /// </summary>
        [Description("Requires elevated permissions (Run as Admin) - Sets the Windows registry to enable long path support as typically it's " + 
                     "limited to 260 characters. If elevated process not detected it will be skipped. This flag can be used on its own or " +
                     "together with a compress /extract operation.")]
        public bool EnableWindowsLongPathSupport { get; set; } = false;
    }
}

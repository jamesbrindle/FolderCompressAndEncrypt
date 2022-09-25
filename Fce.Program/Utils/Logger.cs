using System;
using System.IO;

namespace Fce.Utils
{
    /// <summary>
    /// Simple file logger
    /// </summary>
    internal class Logger
    {
        internal enum LogType
        {
            Info,
            Warning,
            Error,
            Header
        }

        internal string OutputDirectory { get; }
        internal string LogFilePath { get; }
        internal bool LoggingEnable { get; }

        private string _hf = "*******";

        /// <summary>
        /// Simple file logger
        /// </summary>
        /// <param name="fileOrOutputDirectory">Full</param>
        /// <param name="loggingEnabled"></param>
        internal Logger(string fileOrOutputDirectory, bool loggingEnabled)
        {
            string ext = Path.GetExtension(fileOrOutputDirectory);

            if (string.IsNullOrWhiteSpace(ext))
            {
                OutputDirectory = fileOrOutputDirectory;
                LogFilePath = Path.Combine(
                    OutputDirectory,
                    $"FCE-Log-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff")}.log");
            }
            else
            {
                OutputDirectory = Path.GetDirectoryName(fileOrOutputDirectory);
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    OutputDirectory = Path.Combine(Path.GetTempPath(), "FCE-Temp", "Logs");
                    LogFilePath = Path.Combine(
                       OutputDirectory,
                       $"{Path.GetFileNameWithoutExtension(fileOrOutputDirectory)}-" +
                       $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff")}{ext}");
                }
                else
                {
                    LogFilePath = Path.Combine(
                       Path.GetDirectoryName(fileOrOutputDirectory),
                       $"{Path.GetFileNameWithoutExtension(fileOrOutputDirectory)}-" +
                       $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff")}{ext}");
                }
            }

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            LoggingEnable = loggingEnabled;

            if (loggingEnabled)
                CheckLogFileExists();
        }

        /// <summary>
        /// If a log file doesnt exist, set one up
        /// </summary>
        private void CheckLogFileExists()
        {
            try
            {
                if (!File.Exists(LogFilePath))
                {
                    try
                    {
                        File.WriteAllLines(
                            LogFilePath,
                            new string[] {
                        $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}: " +
                        $"[{LogType.Header}]: " +
                        $"{_hf}FOLDER COMPRESS AND ENCRYPT UTILITY LOG CREATED{_hf}\n"
                            });
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Log a message of a given log type [INFO|WARNING|ERROR|HEADER] - Header appends '*******' at the start and end of message;
        /// </summary>
        /// <param name="logType">INFO|WARNING|ERROR|HEADER</param>
        /// <param name="message">Message to log</param>
        internal void Log(LogType logType, string message)
        {
            if (LoggingEnable)
            {
                try
                {
                    CheckLogFileExists();
                    File.AppendAllLines(
                            LogFilePath,
                            new string[] {
                        $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}: " +
                        $"[{logType}]: {(logType == LogType.Header ? _hf : "")}" +
                        $"{message}{(logType == LogType.Header ? _hf : "")}"
                            });
                }
                catch { }
            }
        }

        /// <summary>
        /// Log a message passing in an exception object. The exception message will be written after any message if there is one.
        /// </summary>
        /// <param name="e">Exception to log</param>
        /// <param name="message">An optional message to give an explanation of the exception</param>
        internal void Log(Exception e, string message = null)
        {
            if (LoggingEnable)
            {
                try
                {
                    CheckLogFileExists();
                    File.AppendAllLines(
                            LogFilePath,
                            new string[] {
                        $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}: " +
                        $"[{LogType.Error}]: " +
                        (!string.IsNullOrEmpty(message) ? $"{message}: " : "") +
                        $"Exception: {e.Message}"
                            });
                }
                catch { }
            }
        }

        /// <summary>
        /// Appends an empty line to space out sections of the log
        /// </summary>
        internal void Space()
        {
            if (LoggingEnable)
            {
                try
                {
                    CheckLogFileExists();
                    File.AppendAllLines(LogFilePath, new string[] { string.Empty });
                }
                catch { }
            }
        }
    }
}

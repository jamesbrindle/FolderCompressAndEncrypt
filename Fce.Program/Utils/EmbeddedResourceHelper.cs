using System;
using System.IO;
using System.Reflection;

namespace Fce.Utils
{
    internal enum TargetAssemblyType
    {
        Calling,
        Executing
    }

    /// <summary>
    /// Methods for extracting and utilising resources embedded in the DLL (including assemblies and command line utilities)
    /// </summary>
    internal static class EmbeddedResourceHelper
    {
        /// <summary>
        /// Returns the embedded resource if it's present in the working folder or if it's been extracted. If it's not present it will extract the embedded resource
        /// to the users temp folder and return the full path to it.
        /// </summary>
        /// <remarks>
        /// You can also compress your resource within a zip file to make your assemblies smaller. This method will extract it.
        /// </remarks>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssemblyType">I.e. Calling or Executing</param>
        /// <returns>Full path to present or extracted directory</returns>
        internal static string GetEmbeddedResourcePath(
            TargetAssemblyType targetAssemblyType,
            string fileName,
            string resourcePath)
        {
            return GetEmbeddedResourcePath(
                GetTargetAssembly(targetAssemblyType),
                fileName,
                resourcePath);
        }

        /// <summary>
        /// Returns the embedded resource if it's present in the working folder or if it's been extracted. If it's not present it will extract the embedded resource
        /// to the users temp folder and return the full path to it.
        /// </summary>
        /// <remarks>
        /// You can also compress your resource within a zip file to make your assemblies smaller. This method will extract it.
        /// </remarks>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        /// <returns>Full path to present or extracted directory</returns>
        internal static string GetEmbeddedResourcePath(
            Assembly targetAssembly,
            string fileName,
            string resourcePath)
        {
            string executingFolder = Path.GetDirectoryName(targetAssembly.Location);
            string filePath = Path.Combine(executingFolder, fileName);

            if (File.Exists(filePath))
                return filePath;

            filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (File.Exists(filePath))
                return filePath;
            else
                ExtractEmbeddedResource(targetAssembly, fileName, resourcePath, Path.GetTempPath());

            if (File.Exists(filePath))
                return filePath;

            throw new Exception("Cannot find embedded resource '" +
                targetAssembly.GetName().Name.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Extracts an embedded resource to a given location.
        /// </summary>
        /// <remarks>
        /// You can also compress your resource within a zip file to make your assemblies smaller. This method will extract it.
        /// </remarks>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="outputDirectory">The parent directory to outoput to</param>
        /// <param name="targetAssemblyType">I.e Calling or executing</param>
        internal static void ExtractEmbeddedResource(
            TargetAssemblyType targetAssemblyType,
            string fileName,
            string resourcePath,
            string outputDirectory)
        {
            ExtractEmbeddedResource(
                GetTargetAssembly(targetAssemblyType),
                fileName,
                resourcePath,
                outputDirectory);
        }

        /// <summary>
        /// Extracts an embedded resource to a given location.
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="outputDirectory">The parent directory to outoput to</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        internal static void ExtractEmbeddedResource(
            Assembly targetAssembly,
            string fileName,
            string resourcePath,
            string outputDirectory)
        {
            bool resourceExists = true;
            using (Stream s = targetAssembly.GetManifestResourceStream(
                targetAssembly.GetName().Name.Replace("-", "_") + "." + resourcePath + "." + fileName))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);

                    using (BinaryWriter sw = new BinaryWriter(
                        File.Open(
                            Path.Combine(outputDirectory, string.IsNullOrEmpty(fileName) ? fileName : fileName),
                            FileMode.Create)))
                    {
                        sw.Write(buffer);
                    }

                    return;
                }
                else
                    resourceExists = false;
            }

            if (!resourceExists)
            {
                // Perhaps we've zipped it?

                string zippedFilename = Path.ChangeExtension(fileName, "zip");
                using (Stream z = targetAssembly.GetManifestResourceStream(
                    targetAssembly.GetName().Name.Replace("-", "_") + "." + resourcePath + "." + zippedFilename))
                {
                    if (z == null)
                        throw new Exception("Cannot find embedded resource '" + resourcePath + "'");

                    var tempFilename = Path.Combine(Path.GetTempPath(), zippedFilename);
                    if (!File.Exists(tempFilename))
                    {
                        // First extract the zip file from the asembly
                        ExtractEmbeddedResource(targetAssembly, zippedFilename, resourcePath, outputDirectory);
                    }

                    if (!File.Exists(Path.Combine(outputDirectory, fileName)))
                    {
                        // Then extract the contents of the zip file itself
                        System.IO.Compression.ZipFile.ExtractToDirectory(tempFilename, outputDirectory);
                    }

                    return;
                }
            }

            throw new Exception("Cannot find embedded resource '" +
                targetAssembly.GetName().Name.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Get assembly - Calling or executing by TargetAssemblyType enum
        /// </summary>
        internal static Assembly GetTargetAssembly(TargetAssemblyType targetAssemblyType)
        {
            return targetAssemblyType == TargetAssemblyType.Calling
                ? Assembly.GetCallingAssembly()
                : Assembly.GetExecutingAssembly();
        }
    }
}

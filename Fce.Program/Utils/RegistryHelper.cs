using Microsoft.Win32;
using System;

namespace Fce.Utils
{
    internal class RegistryHelper
    {
        internal static void EnableLongPathSupport()
        {
            const string REGISTRY_KEY = @"SYSTEM\CurrentControlSet\Control\FileSystem";
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                using (var subKey = baseKey.OpenSubKey(REGISTRY_KEY, true))
                {
                    subKey.SetValue("LongPathsEnabled", 1);
                    subKey.Close();
                }
                baseKey.Close();
            }

            if (Environment.Is64BitProcess)
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var subKey = baseKey.OpenSubKey(REGISTRY_KEY, true))
                    {
                        subKey.SetValue("LongPathsEnabled", 1);
                        subKey.Close();
                    }
                    baseKey.Close();
                }
            }
        }
    }
}

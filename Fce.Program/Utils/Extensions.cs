using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Language custom extension methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Get attribute 'Description' of an enum (when attribute 'tag' added)
        /// </summary>
        internal static string Description<T>(this object _, string fieldName)
        {
            try
            {
                PropertyInfo propertyInfo = typeof(T).GetProperties()?.Where(p => p.Name == fieldName).FirstOrDefault();
                if (propertyInfo != null)
                {
                    var attributes = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes.FirstOrDefault()).Description;
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Does the input string contain one of the string in the given set of strings (string[] array)
        /// </summary>
        /// <param name="inputString">String to check</param>
        /// <param name="containsList">Array of strings to compare against</param>
        /// <returns>True if a string is present, false otherwise</returns>
        /// <exception cref="ArgumentNullException">contains string[] array can't be null</exception>
        internal static bool StringContainsIn(this string inputString, params string[] containsList)
        {
            if (containsList == null)
                throw new ArgumentNullException("items");

            for (int i = 0; i < containsList.Length; i++)
                if (inputString.Contains(containsList[i]))
                    return true;

            return false;
        }
    }
}

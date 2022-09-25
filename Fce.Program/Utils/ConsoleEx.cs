namespace System
{
    /// <summary>
    /// Console extension methods, particularly writing out text and lines easily in a particular colour and setting foreground text.
    /// </summary>
    public static class ConsoleEx
    {
        /// <summary>
        /// Append text to console - in-line
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="colour">Colour of text</param>
        public static void WriteColoured(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Append tesxt line to console
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="colour">Colour of text</param>
        public static void WriteColouredLine(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
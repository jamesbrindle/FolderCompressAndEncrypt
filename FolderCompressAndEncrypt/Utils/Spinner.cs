using System;
using System.Threading;

namespace FolderCompressAndEncrypt.Utils
{
    /// <summary>
    /// Console wait or progress spinner and methods for manipulating the spinner
    /// </summary>
    public class ConsoleSpinner
    {
        private int _currentAnimationFrame;

        public ConsoleSpinner()
        {
            SpinnerAnimationFrames = new[]
                                     {
                                         '|',
                                         '/',
                                         '-',
                                         '\\'
                                     };
        }

        private char[] SpinnerAnimationFrames { get; set; }
        private static ConsoleSpinner Spinner { get; set; } = new ConsoleSpinner();
        private static Thread SpinnerThread { get; set; } = new Thread(RunSpinnerThread);

        /// <summary>
        /// Show / start the spinner
        /// </summary>
        public static void Show()
        {
            try
            {
                if (!SpinnerThread.IsAlive)
                    SpinnerThread = new Thread(RunSpinnerThread);
            }
            catch
            {
                try
                {
                    SpinnerThread.Abort();
                }
                catch { }

                try
                {
                    SpinnerThread = new Thread(RunSpinnerThread);
                }
                catch { }

            }

            try
            {
                SpinnerThread.IsBackground = true;
                SpinnerThread.Start();
            }
            catch { }
        }

        /// <summary>
        /// Stop / hide the spinner
        /// </summary>
        public static void Hide()
        {
            try
            {
                if (SpinnerThread.IsAlive)
                    SpinnerThread.Abort();
            }
            catch { }
            finally
            {
                try
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(Console.CursorLeft > 0 ? Console.CursorLeft - 1 : 0, Console.CursorTop);
                    Console.Write("  ");
                }
                catch { }
            }
        }

        /// <summary>
        /// Performs the actual writing to console.
        /// </summary>
        private void UpdateProgress()
        {
            // Store the current position of the cursor
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;
            Console.CursorVisible = false;

            // Write the next frame (character) in the spinner animation
            Console.Write(SpinnerAnimationFrames[_currentAnimationFrame]);

            // Keep looping around all the animation frames
            _currentAnimationFrame++;
            if (_currentAnimationFrame == SpinnerAnimationFrames.Length)
                _currentAnimationFrame = 0;

            // Restore cursor to original position
            Console.SetCursorPosition(originalX, originalY);
        }

        /// <summary>
        /// Run in seperate thread to avoid any blocking
        /// </summary>
        private static void RunSpinnerThread()
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    Thread.Sleep(70);
                    Spinner.UpdateProgress();
                }
            }
            catch { }
        }
    }
}

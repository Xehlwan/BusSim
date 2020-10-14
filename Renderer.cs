using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BusSim
{
    internal static class Renderer
    {
        private static readonly StringBuilder sb = new StringBuilder();
        private static int screenHeight = Console.BufferHeight;
        private static int screenWidth = Console.BufferWidth;

        public static void ClearArea(int left, int top, int width, int height)
        {
            var chArray = new char[width];
            Array.Fill(chArray, '\uFEFF');

            for (var y = 0; y < height; y++)
            {
                MoveCursor(left, top + y);
                Console.Write(chArray);
            }
        }

        public static void ClearLines(int top, int lines)
        {
            MoveCursor(0, top);
            var chars = new char[lines * screenWidth];
            Array.Fill(chars, '\uFEFF');
            Console.Write(chars);
        }

        /// <summary>
        /// Draws colored symbols to chosen area.
        /// </summary>
        /// <param name="left">The distance to the left side of the screen.</param>
        /// <param name="top">The distance to the top of the screen.</param>
        /// <param name="width">The width of the area to draw.</param>
        /// <param name="height">The height of the area to draw.</param>
        /// <param name="symbols">The characters and colors to draw.</param>
        public static void DrawArea(int left, int top, int width, int height, (char ch, Color color)[] symbols)
        {
            if (width == 0 || height == 0 || symbols.Length == 0) return;

            // Clamp dimensions to screen.
            int realLeft = left >= 0 ? left : 0;
            int realTop = top >= 0 ? top : 0;
            int xLimit = realLeft + width < Console.BufferWidth ? width : Console.BufferWidth - realLeft;
            int yLimit = realTop + height < Console.BufferHeight ? height : Console.BufferHeight - realTop;

            Color prevColor = symbols[0].color;
            Console.Write(VtColor(prevColor));

            for (int y = realTop - top; y < yLimit; y++)
            {
                sb.Clear();
                var test = new List<int>();

                for (int x = realLeft - left; x < xLimit; x++)
                {
                    int index = y * width + x;

                    // Short-circuit if we are going outside array.
                    if (index >= symbols.Length) break;

                    (char ch, Color color) = symbols[index];
                    if (char.IsWhiteSpace(ch) || ch == '\u0000')
                    {
                        // (Ab)use of unicode BOM to force draw empty space.
                        sb.Append('\uFEFF');

                        continue;
                    }

                    if (color != prevColor) sb.Append(VtColor(color));
                    sb.Append(ch);
                }

                MoveCursor(realLeft, realTop + y);
                Console.Write(sb);
            }
        }

        public static void DrawChar(int left, int top, char ch)
        {
            MoveCursor(left, top);
            Console.Write(ch);
        }

        public static void DrawEmpty(int left, int top)
        {
            DrawChar(left, top, '\uFEFF');
        }

        public static Color GetRainbow(int phaseDegree)
        {
            throw new NotImplementedException();
        }

        public static void PrintMessage(int left, int top, string message, Color color)
        {
            MoveCursor(left, top);
            SetColor(color);
            Console.Write(message);
        }

        /// <summary>
        /// Prepare screen for drawing.
        /// </summary>
        /// <param name="width">The</param>
        /// <param name="height"></param>
        public static void ScreenInit(int width, int height)
        {
            // clamp dimensions.
            if (width > Console.LargestWindowWidth) width = Console.LargestWindowWidth;
            if (height > Console.LargestWindowHeight) height = Console.LargestWindowHeight;

            // Set console flags.
            Interop.ActivateVT();
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            // Set screen size.
            Console.WindowWidth = width < Console.BufferWidth ? width : Console.BufferWidth;
            Console.WindowHeight = height < Console.BufferHeight ? height : Console.BufferHeight;
            Console.SetBufferSize(width, height);
            Console.SetWindowSize(width, height);

            screenWidth = width;
            screenHeight = height;
        }

        public static void SetColor(Color color)
        {
            Console.Write(VtColor(color));
        }

        private static void MoveCursor(int left, int top)
        {
            if (left != Console.CursorLeft || top != Console.CursorTop) Console.SetCursorPosition(left, top);
        }

        /// <summary>
        /// Generate a VT string to set the text-color to the given rgb-value.
        /// </summary>
        /// <param name="color">The color for the text.</param>
        /// <returns>A <see cref="string" /> containing the VT command to change the text-color.</returns>
        private static string VtColor(Color color)
        {
            return $"\x1b[38;2;{color.R};{color.G};{color.B}m";
        }
    }
}
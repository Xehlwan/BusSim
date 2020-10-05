using System;
using System.Drawing;
using System.Text;

namespace BusSim
{
    internal static class Renderer
    {
        private static readonly StringBuilder sb = new StringBuilder();
        private static int screenHeight = Console.BufferHeight;
        private static int screenWidth = Console.BufferWidth;

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
            // Clamp dimensions to screen.
            if (left + width >= screenWidth) width = screenWidth - left;
            if (top + height >= screenHeight) height = screenHeight - height;

            Color prevColor = symbols[0].color;
            sb.Append(VtColor(prevColor));

            for (var y = 0; y < height; y++)
            {
                sb.Clear();

                for (var x = 0; x < width; x++)
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

                MoveCursor(left, top + y);
                Console.Write(sb);
            }
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

        public static Color GetRainbow(int phaseDegree)
        {
            throw new NotImplementedException();
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
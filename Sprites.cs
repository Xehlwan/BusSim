using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BusSim
{
    public static class Sprites
    {
        // https://www.fileformat.info/info/unicode/font/consolas/grid.htm

        public static ((char, Color)[] pixels, int width, int height) Bus()
        {
            const int width = 7;
            const int height = 3;

            var busColor = Color.IndianRed;
            var windowColor = Color.CornflowerBlue;

            var busPixels = new (char, Color)[width * height];
            busPixels[1] = ('\u0332', busColor);
            busPixels[2] = ('\u0332', busColor);
            busPixels[3] = ('\u0332', busColor);
            busPixels[4] = ('\u0332', busColor);
            busPixels[5] = ('\u0332', busColor);

            busPixels[7] = ('\u2320', busColor);
            busPixels[8] = ('\u25A1', windowColor);
            busPixels[9] = ('\u25A1', windowColor);
            busPixels[10] = ('\u25A1', windowColor);
            busPixels[11] = ('\u2565', busColor);
            busPixels[12] = ('\u25A0', windowColor);
            busPixels[13] = ('\u2502', busColor);

            busPixels[14] = ('\u2514', busColor);
            busPixels[15] = ('\u20DD', busColor);
            busPixels[16] = ('\u2500', busColor);
            busPixels[17] = ('\u2500', busColor);
            busPixels[18] = ('\u2568', busColor);
            busPixels[19] = ('\u20DD', busColor);
            busPixels[20] = ('\u2518', busColor);

            return (busPixels, width, height);
        }
    }
}

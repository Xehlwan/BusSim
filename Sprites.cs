using System.Collections.ObjectModel;
using System.Drawing;
using BusSim.Simulation;

namespace BusSim
{
    public static class Sprites
    {
        // https://www.fileformat.info/info/unicode/font/consolas/grid.htm

        public static ((char, Color)[] pixels, int width, int height) Bus()
        {
            const int width = 7;
            const int height = 3;

            Color busColor = Color.IndianRed;
            Color windowColor = Color.CornflowerBlue;

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

        public static ((char, Color)[] pixels, int width, int height) Waiting(ReadOnlyCollection<Passenger> waiting)
        {
            var width = 4;
            var height = 20;
            var pixels = new (char ch, Color color)[width * height];
            for (var i = 0; i < waiting.Count; i++)
            {
                pixels[i].ch = '\u263b';
                pixels[i].color = waiting[i].Mood switch
                {
                    Mood.Happy => Color.LimeGreen,
                    Mood.Neutral => Color.LemonChiffon,
                    Mood.Annoyed => Color.DarkOrange,
                    Mood.Angry => Color.Red,
                    _ => default
                };
            }

            for (int i = waiting.Count; i < width * height; i++) pixels[i].ch = '\uFEFF';

            return (pixels, width, height);
        }
    }
}
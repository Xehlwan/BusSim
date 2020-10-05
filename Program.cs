using System;
using System.Drawing;
using System.Threading.Channels;
using BusSim.Simulation;

namespace BusSim
{
    internal class Program
    {
        private static readonly int capacity = 30;
        private static City city;
        private static readonly int distance = 10;
        private static readonly Random rng = new Random();
        private static readonly double spawnChance = 0.5;
        private static string[] stopNames;
        private static readonly int stops = 10;

        private static void Main(string[] args)
        {
            Renderer.ScreenInit(100, 40);
            var busSprite = Sprites.Bus();

            Renderer.DrawArea(25, 10, busSprite.width, busSprite.height, busSprite.pixels);

            Console.ReadKey();

        }

        private static void DrawStopSigns()
        {
        }

        private static string[] GenerateNames(int number)
        {
            const int A = 65;
            const int Z = 90;
            const int a = 97;
            const int z = 122;
            number = number < Z - A ? number : Z - A;
            var names = new string[number];
            for (var i = 0; i < names.Length; i++)
            {
                var chars = new char[3];
                chars[0] = (char) (i + A);
                chars[1] = (char) (rng.Next(z - a) + a);
                chars[2] = (char) (rng.Next(z - a) + a);
            }

            return names;
        }

        private static void MakeCity()
        {
            stopNames = GenerateNames(stops);
            city = new City(capacity, distance, spawnChance, stopNames);
        }
    }
}
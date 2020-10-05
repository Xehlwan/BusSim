using System;
using BusSim.Simulation;

namespace BusSim
{
    class Program
    {
        private static City city;
        private static int stops = 10;
        private static int capacity = 30;
        private static int distance = 10;
        private static double spawnChance = 0.5;
        private static string[] stopNames;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static string[] GenerateNames(int number)
        {
            var rng = new Random();
            const int A = 65;
            const int Z = 90;
            const int a = 97;
            const int z = 122;
            number = number < Z - A ? number : Z - A;
            var names = new string[number];
            for (int i = 0; i < names.Length; i++)
            {
                var chars = new char[3];
                chars[0] = (char) (i + A);
                chars[1] = (char) (rng.Next(z - a) + a);
                chars[2] = (char) (rng.Next(z - a) + a);
            }

            return names;
        }

        private static void Start()
        {
            stopNames = GenerateNames(stops);
            city = new City(capacity, distance, spawnChance, stopNames);
        }
    }
}

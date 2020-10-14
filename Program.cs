using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using BusSim.Simulation;

namespace BusSim
{
    internal class Program
    {
        private const int busPosition = 16;
        private const int embarkDelay = 25;
        private const int frameWait = 100;
        private const int height = 40;
        private const int roadPostion = 18;
        private const int stopPosition = 20;
        private const int width = 150;

        private static readonly int capacity = 30;
        private static readonly int distance = 10;
        private static readonly Stopwatch frameTimer = new Stopwatch();
        private static readonly Random rng = new Random();
        private static readonly StringBuilder sb = new StringBuilder();
        private static readonly double spawnChance = 0.5;
        private static readonly int stops = 10;
        private static City city;
        private static bool pause;
        private static bool running;
        private static int spacing;
        private static string[] stopNames;

        private static void DrawBus()
        {
            ((char, Color)[] pixels, int width, int height) busSprite = Sprites.Bus();
            int pixelDist = spacing + 4;
            double progress = (city.StopDistance - city.DistanceToNext) / (double) city.StopDistance;
            int xPos = (int) (pixelDist * progress) + city.NextStopIndex * pixelDist - 4;
            Renderer.DrawArea(xPos, busPosition, busSprite.width, busSprite.height, busSprite.pixels);
        }

        private static void DrawDisembark(IEnumerable<Passenger> disembarked, int stop)
        {
            var timer = new Stopwatch();
            int bottom = busPosition - 1;
            int left = GetStopPosition(stop);
            Console.SetCursorPosition(left, bottom);
            var col = 0;
            var row = 0;
            Renderer.SetColor(Color.LemonChiffon);
            foreach (Passenger passenger in disembarked)
            {
                timer.Restart();
                Renderer.DrawChar(left + col, bottom + row, '\u263b');
                if (col < 4)
                {
                    col++;
                }
                else
                {
                    col = 0;
                    row--;
                }

                WaitForFrames(1);
            }
        }

        private static void DrawEmbarked(IEnumerable<Passenger> embarked, int stop)
        {
            var timer = new Stopwatch();
            int top = stopPosition + 1;
            int left = GetStopPosition(stop);
            Console.SetCursorPosition(left, top);
            var col = 0;
            var row = 0;
            foreach (Passenger passenger in embarked)
            {
                timer.Restart();
                Renderer.DrawEmpty(left + col, top + row);
                if (col < 4)
                {
                    col++;
                }
                else
                {
                    col = 0;
                    row++;
                }

                WaitForFrames(1);
            }
        }

        private static void DrawHotkeys()
        {
            Renderer.ClearLines(0, 1);
            Renderer.PrintMessage(0, 0, "ESC: exit, SPACE: pause, 1: low traffic, 2: normal traffic, 3: high traffic",
                                  Color.LightSlateGray);
        }

        private static void DrawRoad()
        {
            Renderer.SetColor(Color.DarkSlateGray);
            Renderer.ClearLines(busPosition, 3);
            Console.SetCursorPosition(0, roadPostion);
            var road = new char[width];
            Array.Fill(road, '\u0333');
            Console.Write(road);
        }

        private static void DrawStopSigns()
        {
            Renderer.SetColor(Color.Aquamarine);

            foreach (string stopName in stopNames)
            {
                Console.CursorLeft += spacing;
                Console.Write($"[{stopName[0]}{stopName[1]}]");
            }
        }

        private static void DrawWaiting()
        {
            for (var i = 0; i < city.StopCount; i++)
            {
                ReadOnlyCollection<Passenger> passengers = city.GetPassengersAt(i);
                int xPos = GetStopPosition(i);

                if (passengers.Count == 0)
                {
                    // TODO: clear passengers.
                }
                else
                {
                    ((char, Color)[] pixels, int width, int height) sprite = Sprites.Waiting(passengers);
                    Renderer.DrawArea(xPos, stopPosition + 1, sprite.width, sprite.height, sprite.pixels);
                }
            }
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
                sb.Clear();
                sb.Append((char) (i + A));
                sb.Append((char) (rng.Next(z - a) + a));
                sb.Append((char) (rng.Next(z - a) + a));

                names[i] = sb.ToString();
            }

            return names;
        }

        private static int GetStopPosition(int i)
        {
            return spacing + (spacing + 4) * i;
        }

        private static void HandleInput()
        {
            if (Console.KeyAvailable)
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        Renderer.SetColor(Color.Azure);
                        Console.Clear();
                        Console.WriteLine("End of simulation.");
                        running = false;

                        break;
                    case ConsoleKey.Spacebar:
                        Console.SetCursorPosition(Console.BufferWidth / 2 - 5, 0);
                        if (!pause)
                        {
                            Renderer.SetColor(Color.DarkOrange);
                            Console.Write("[ PAUSED ]");
                        }
                        else
                        {
                            DrawHotkeys();
                        }

                        pause = !pause;

                        break;
                    case ConsoleKey.D3:
                        city.SetHighTraffic();
                        Renderer.ClearLines(1, 1);
                        Renderer.PrintMessage(0, 1, "Traffic: ", Color.White);
                        Renderer.PrintMessage(9, 1, "High", Color.OrangeRed);

                        break;
                    case ConsoleKey.D2:
                        city.SetNormalTraffic();
                        Renderer.ClearLines(1, 1);
                        Renderer.PrintMessage(0, 1, "Traffic: Normal", Color.White);

                        break;
                    case ConsoleKey.D1:
                        city.SetLowTraffic();
                        Renderer.ClearLines(1, 1);
                        Renderer.PrintMessage(0, 1, "Traffic: ", Color.White);
                        Renderer.PrintMessage(9, 1, "Low", Color.DodgerBlue);

                        break;
                }
        }

        private static void Main(string[] args)
        {
            // Initizalize
            Renderer.ScreenInit(width, height);
            MakeCity();

            // Set fields.
            running = true;
            spacing = (width - city.StopCount * 4) / (city.StopCount + 1);

            // Draw stop signs
            Console.SetCursorPosition(0, stopPosition);
            DrawStopSigns();
            DrawHotkeys();
            Renderer.PrintMessage(0, 1, "Traffic: Normal", Color.White);

            // Main Loop
            while (running)
            {
                frameTimer.Restart();
                city.Tick();
                PrintStatusMsg();

                DrawRoad();
                DrawBus();
                DrawWaiting();

                if (city.DistanceToNext == 0)
                {
                    IEnumerable<Passenger> disembarked = city.Disembark();
                    DrawDisembark(disembarked, city.NextStopIndex);
                    IEnumerable<Passenger> embarked = city.Embark();
                    DrawEmbarked(embarked, city.NextStopIndex);
                    Renderer.ClearArea(GetStopPosition(city.NextStopIndex), 3, 5, busPosition - 3);
                    Renderer.ClearArea(GetStopPosition(city.NextStopIndex), stopPosition + 1, 4,
                                       height - stopPosition - 1);

                    Renderer.ClearLines(2, 1);
                    Renderer.PrintMessage(0, 2, $"Passengers on bus: {city.PassengersOnBus}/{capacity}", Color.White);
                }

                do
                {
                    WaitForFrames(1);
                } while (pause);
            }
        }

        private static void MakeCity()
        {
            stopNames = GenerateNames(stops);
            city = new City(capacity, distance, spawnChance, stopNames);
        }

        private static void PrintStatusMsg()
        {
            if (city.LeftInAnger > 0)
                Renderer.PrintMessage(0, 3, $"- {city.LeftInAnger} waiting passengers left in anger.", Color.OrangeRed);
        }

        private static void WaitForFrames(int frames)
        {
            frameTimer.Restart();
            long waitTicks = Stopwatch.Frequency * frameWait * frames / 1000;
            do
            {
                HandleInput();
            } while (frameTimer.ElapsedTicks < waitTicks);
        }
    }
}
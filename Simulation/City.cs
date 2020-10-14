using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BusSim.Simulation
{
    internal class City
    {
        private static readonly Random rng = new Random();
        private static int passengerPatience;
        private readonly Bus bus;
        private readonly List<BusStop> stops = new List<BusStop>();

        /// <summary>
        /// Create a new city.
        /// </summary>
        /// <param name="busCapacity">The passenger capacity of the bus. Must be larger than 0.</param>
        /// <param name="stopDistance">The distance between stops. Must be larger than 0.</param>
        /// <param name="baseSpawnRate">How likely passengers are to spawn each tick. Must be between 0 and 1.</param>
        /// <param name="busStops">A number of names for bus stops. A minimum of two stops must be provided.</param>
        public City(int busCapacity, int stopDistance, double baseSpawnRate, params string[] busStops)
        {
            if (busCapacity <= 0) throw new ArgumentException("Must be larger than 0.", nameof(busCapacity));
            if (stopDistance <= 0) throw new ArgumentException("Must be larger than 0.", nameof(stopDistance));
            if (baseSpawnRate < 0.0 || baseSpawnRate > 1.0)
                throw new ArgumentException("Must be between 0 and 1.", nameof(baseSpawnRate));

            if (busStops.Length < 2) throw new ArgumentException("City must have at least 2 stops.", nameof(busStops));

            bus = new Bus(busCapacity);
            BaseSpawnRate = baseSpawnRate;
            SpawnRate = baseSpawnRate;
            StopDistance = stopDistance;
            passengerPatience = StopDistance * 5;
            foreach (string stop in busStops) stops.Add(new BusStop(stop));

            NextStopIndex = 1;
            DistanceToNext = StopDistance;
        }

        /// <summary>
        /// The normal chance for a new passenger to spawn each tick.
        /// </summary>
        public double BaseSpawnRate { get; }

        /// <summary>
        /// The distance remaining to next bus stop.
        /// </summary>
        public int DistanceToNext { get; private set; }

        public int LeftInAnger { get; private set; }

        /// <summary>
        /// The index of the next bus stop.
        /// </summary>
        public int NextStopIndex { get; private set; }

        /// <summary>
        /// How many passengers are currently riding the bus.
        /// </summary>
        public int PassengersOnBus => bus.Count;

        /// <summary>
        /// The actual chance for a new passenger to spawn each tick.
        /// </summary>
        public double SpawnRate { get; private set; }

        public int StopCount => stops.Count;

        /// <summary>
        /// The distance between each bus stop.
        /// </summary>
        public int StopDistance { get; }

        public IEnumerable<Passenger> Disembark()
        {
            IEnumerable<Passenger> disembarked = bus.Disembark(stops[NextStopIndex]);

            return disembarked;
        }

        public IEnumerable<Passenger> Embark()
        {
            IEnumerable<Passenger> embarked = bus.Embark(stops[NextStopIndex]);

            return embarked;
        }

        public ReadOnlyCollection<Passenger> GetPassengersAt(int busStopIndex)
        {
            return stops[busStopIndex].GetPassengers();
        }

        public IEnumerable<string> GetStopNames()
        {
            foreach (BusStop stop in stops) yield return stop.Name;
        }

        /// <summary>
        /// Sets spawn rate to rush hour. The rate caps at 99%.
        /// </summary>
        public void SetHighTraffic()
        {
            SpawnRate = BaseSpawnRate * 2.0 < 0.99 ? BaseSpawnRate * 2.0 : 0.99;
        }

        /// <summary>
        /// Sets spawn rate to low traffic.
        /// </summary>
        public void SetLowTraffic()
        {
            SpawnRate = BaseSpawnRate / 2.0;
        }

        /// <summary>
        /// Resets spawn rate to the base value.
        /// </summary>
        public void SetNormalTraffic()
        {
            SpawnRate = BaseSpawnRate;
        }

        public void Tick()
        {
            MoveBus();
            LeftInAnger = UpdatePassengers();
            if (CheckSpawn()) SpawnPassenger();
        }

        /// <summary>
        /// Check if a passenger should spawn.
        /// </summary>
        /// <returns><see langword="true" /> if passenger should spawn, otherwise <see langword="false" /></returns>
        private bool CheckSpawn()
        {
            return 1.0 - rng.NextDouble() <= SpawnRate;
        }

        private void MoveBus()
        {
            if (DistanceToNext > 0)
            {
                DistanceToNext--;

                return;
            }

            DistanceToNext = StopDistance;
            NextStopIndex++;
            if (NextStopIndex == stops.Count) NextStopIndex = 0;
        }

        private void SpawnPassenger()
        {
            int destinationIndex = rng.Next(stops.Count);
            BusStop destination = stops[destinationIndex];
            int spawnIndex;

            // Roll until spawn is different from destination.
            while ((spawnIndex = rng.Next(stops.Count)) == destinationIndex)
            {
            }

            var passenger = new Passenger(destination, passengerPatience);
            stops[spawnIndex].Add(passenger);
        }

        private int UpdatePassengers()
        {
            var leaving = 0;
            foreach (BusStop stop in stops) leaving += stop.UpdateMood();

            return leaving;
        }
    }
}
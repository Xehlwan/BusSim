using System;
using System.Collections.Generic;

namespace BusSim.Simulation
{
    class City
    {
        private const int passengerPatience = 20;
        private static Random rng = new Random();
        private List<BusStop> stops = new List<BusStop>();
        private Bus bus;
        /// <summary>
        /// The normal chance for a new passenger to spawn each tick.
        /// </summary>
        public double BaseSpawnRate { get; }
        /// <summary>
        /// The actual chance for a new passenger to spawn each tick.
        /// </summary>
        public double SpawnRate { get; private set; }
        /// <summary>
        /// The distance between each bus stop.
        /// </summary>
        public int StopDistance { get; }
        /// <summary>
        /// The distance remaining to next bus stop.
        /// </summary>
        public int DistanceToNext { get; private set; }
        /// <summary>
        /// The index of the next bus stop.
        /// </summary>
        public int NextStopIndex { get; private set; }
        /// <summary>
        /// How many passengers are currently riding the bus.
        /// </summary>
        public int PassengersOnBus => bus.Count;


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
            if (baseSpawnRate < 0.0 || baseSpawnRate > 1.0) throw new ArgumentException("Must be between 0 and 1.", nameof(baseSpawnRate));
            if (busStops.Length < 2) throw new ArgumentException("City must have at least 2 stops.", nameof(busStops));

            bus = new Bus(busCapacity);
            BaseSpawnRate = baseSpawnRate;
            SpawnRate = baseSpawnRate;
            StopDistance = stopDistance;
            foreach (string stop in busStops)
            {
                stops.Add(new BusStop(stop));
            }

            NextStopIndex = 1;
            DistanceToNext = StopDistance;
        }

        /// <summary>
        /// Sets spawn rate to rush hour. The rate caps at 99%.
        /// </summary>
        public void SetRushHour() => SpawnRate = BaseSpawnRate * 2 < 0.99 ? BaseSpawnRate * 2 : 0.99;
        /// <summary>
        /// Sets spawn rate to low traffic.
        /// </summary>
        public void SetLowTraffic() => SpawnRate = BaseSpawnRate / 2;
        /// <summary>
        /// Resets spawn rate to the base value.
        /// </summary>
        public void SetNormalTraffic() => SpawnRate = BaseSpawnRate;
        /// <summary>
        /// Check if a passenger should spawn.
        /// </summary>
        /// <returns><see langword="true"/> if passenger should spawn, otherwise <see langword="false"/></returns>
        private bool CheckSpawn() => 1 - rng.NextDouble() <= SpawnRate;

        public void Tick()
        {
            if (CheckSpawn()) SpawnPassenger();
            MoveBus();
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

        public IEnumerable<Passenger> Disembark() => bus.Disembark(stops[NextStopIndex]);

        public IEnumerable<Passenger> Embark() => bus.Embark(stops[NextStopIndex]);

        public IEnumerable<Passenger> GetPassengersAt(int busStopIndex) => stops[busStopIndex].GetPassengers();
    }
}

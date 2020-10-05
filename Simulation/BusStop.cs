using System.Collections.Generic;
using System.Linq;

namespace BusSim.Simulation
{
    internal class BusStop
    {
        private readonly List<Passenger> passengers = new List<Passenger>();
        public int Count => passengers.Count;
        public string Name { get; }

        public BusStop(string name)
        {
            Name = name;
        }

        public void Add(Passenger passenger) => passengers.Add(passenger);

        public Passenger TakeFirst()
        {
            var passenger = passengers.First();
            passengers.Remove(passenger);

            return passenger;
        }

        public IEnumerable<Passenger> GetPassengers() => passengers.AsEnumerable();
    }
}
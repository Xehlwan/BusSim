using System;
using System.Collections.Generic;
using System.Text;

namespace BusSim.Simulation
{
    class Bus
    {
        public int Capacity { get; }
        public int Count => passengers.Count;

        private readonly List<Passenger> passengers = new List<Passenger>();

        public IEnumerable<Passenger> Embark(BusStop stop)
        {
            while (stop.Count > 0 && Count < Capacity)
            {
                var passenger = stop.TakeFirst();
                passengers.Add(passenger);

                yield return passenger;
            }
        }

        public IEnumerable<Passenger> Disembark(BusStop stop)
        {
            int index = 0;
            while (index < passengers.Count)
            {
                if (passengers[index].Destination == stop)
                {
                    var disembarking = passengers[index];
                    passengers.Remove(disembarking);

                    yield return disembarking;
                }
                else
                    index++;
            }
        }

        public Bus(int capacity)
        {
            Capacity = capacity;
        }
    }
}

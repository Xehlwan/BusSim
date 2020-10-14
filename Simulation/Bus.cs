using System.Collections.Generic;

namespace BusSim.Simulation
{
    public class Bus
    {
        private readonly List<Passenger> passengers = new List<Passenger>();

        public Bus(int capacity)
        {
            Capacity = capacity;
        }

        public int Capacity { get; }
        public int Count => passengers.Count;

        public IEnumerable<Passenger> Disembark(BusStop stop)
        {
            var index = 0;
            while (index < passengers.Count)
                if (passengers[index].Destination == stop)
                {
                    Passenger disembarking = passengers[index];
                    passengers.Remove(disembarking);

                    yield return disembarking;
                }
                else
                {
                    index++;
                }
        }

        public IEnumerable<Passenger> Embark(BusStop stop)
        {
            while (stop.Count > 0 && Count < Capacity)
            {
                Passenger passenger = stop.TakeFirst();
                passengers.Add(passenger);

                yield return passenger;
            }
        }
    }
}
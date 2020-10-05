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

        public void MakeStop(BusStop stop)
        {
            Disembark(stop);
            Board(stop);
        }

        private void Board(BusStop stop)
        {
            while (stop.Count > 0 && Count < Capacity)
            {
                var passenger = stop.TakeFirst();
                passengers.Add(passenger);
            }
        }

        private void Disembark(BusStop stop)
        {
            int index = 0;
            while (index < passengers.Count)
            {
                if (passengers[index].Destination == stop)
                    passengers.RemoveAt(index);
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

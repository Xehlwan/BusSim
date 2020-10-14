using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BusSim.Simulation
{
    public class BusStop
    {
        private readonly List<Passenger> passengers = new List<Passenger>();

        public BusStop(string name)
        {
            Name = name;
        }

        public int Count => passengers.Count;
        public string Name { get; }

        public void Add(Passenger passenger)
        {
            passengers.Add(passenger);
        }

        public ReadOnlyCollection<Passenger> GetPassengers()
        {
            return passengers.AsReadOnly();
        }

        public Passenger TakeFirst()
        {
            Passenger passenger = passengers.First();
            passengers.Remove(passenger);

            return passenger;
        }

        public int UpdateMood()
        {
            var leaving = 0;
            var index = 0;
            while (index < passengers.Count)
            {
                passengers[index].TickTime();
                if (passengers[index].Mood == Mood.Furious)
                {
                    leaving++;
                    passengers.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            return leaving;
        }
    }
}
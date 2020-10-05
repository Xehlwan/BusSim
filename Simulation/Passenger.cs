using System;
using System.Collections.Generic;
using System.Text;

namespace BusSim.Simulation
{
    class Passenger
    {
        private readonly int patience;
        public int WaitTime { get; private set; }
        public Mood Mood { get; private set; }
        public BusStop Destination { get; set; }

        public Passenger(BusStop destination, int patience)
        {
            Destination = destination;
            this.patience = patience;
            Mood = Mood.Happy;
        }

        public void TickTime()
        {
            WaitTime++;
            Mood = WaitTime switch
            {
                _ when WaitTime > patience * 3 => Mood.Angry,
                _ when WaitTime > patience * 2 => Mood.Annoyed,
                _ when WaitTime > patience => Mood.Neutral,
                _ => Mood.Happy
            };
        }

        public void ResetWait() => WaitTime = 0;

        public void ResetMood() => Mood = Mood.Happy;
    }
}

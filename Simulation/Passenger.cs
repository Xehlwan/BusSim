namespace BusSim.Simulation
{
    public class Passenger
    {
        private readonly int patience;

        public Passenger(BusStop destination, int patience)
        {
            Destination = destination;
            this.patience = patience;
            Mood = Mood.Happy;
        }

        public BusStop Destination { get; set; }
        public Mood Mood { get; private set; }
        public int WaitTime { get; private set; }

        public void ResetMood()
        {
            Mood = Mood.Happy;
        }

        public void ResetWait()
        {
            WaitTime = 0;
        }

        public void TickTime()
        {
            WaitTime++;
            Mood = WaitTime switch
            {
                _ when WaitTime > patience * 4 => Mood.Furious,
                _ when WaitTime > patience * 3 => Mood.Angry,
                _ when WaitTime > patience * 2 => Mood.Annoyed,
                _ when WaitTime > patience => Mood.Neutral,
                _ => Mood.Happy
            };
        }
    }
}
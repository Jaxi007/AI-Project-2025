using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Components.Environments;
using System;
using System.Linq;

namespace TrafficSimulation
{
    public class Car : IAgent
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Position Position { get; set; }
        public int Heading { get; set; } = 0;
        public double Speed { get; set; } = 0;

        [PropertyDescription]
        public double MaxBrake { get; set; } = 1;

        [PropertyDescription]
        public double MaxAccel { get; set; } = 1;

        [PropertyDescription]
        public double SpeedLimit { get; set; } = 3;

        [Link]
        public Grid2DLayer<Car> Environment { get; set; }

        public void Init(object model = null) { }

        public void Tick()
        {
            AdjustSpeed();

            for (int i = 0; i < (int)Speed; i++)
            {
                var next = Environment.MoveTowards(Position, Heading);

                if (!Environment.IsValidPosition(next))
                {
                    Environment.RemoveAgent(this);
                    return;
                }

                var collisions = Environment.GetAgentsAt<Car>(next);
                var accidentLayer = Environment.GetLayer<Accident>();
                var accidents = accidentLayer?.GetAgentsAt<Accident>(next);

                if (collisions.Any() || (accidents?.Any() ?? false))
                {
                    var acc = accidents?.FirstOrDefault();
                    if (acc != null)
                        acc.ClearIn = 5;

                    Environment.RemoveAgent(this);
                    return;
                }

                Position = next;
            }
        }

        private void AdjustSpeed()
        {
            double minSpeed = Math.Max(Speed - MaxBrake, 0);
            double maxSpeed = Math.Min(Speed + MaxAccel, SpeedLimit);
            double targetSpeed = maxSpeed;

            var lookahead = Environment.RayTrace(Position, Heading, (int)maxSpeed);

            if (lookahead.Any(p => Environment.IsOccupied(p)))
            {
                int spaceAhead = lookahead.TakeWhile(p => !Environment.IsOccupied(p)).Count();
                while (BreakingDistance(targetSpeed) > spaceAhead && targetSpeed > minSpeed)
                    targetSpeed--;
            }

            Speed = targetSpeed;
        }

        private double BreakingDistance(double currentSpeed)
        {
            double nextSpeed = Math.Max(currentSpeed - MaxBrake, 0);
            return currentSpeed + nextSpeed;
        }
    }

    public class TrafficLight : IAgent
    {
        public enum LightColor { Green, Yellow, Red }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Position Position { get; set; }
        public LightColor Color { get; set; } = LightColor.Red;
        public int TicksAtLastChange { get; set; }

        [PropertyDescription]
        public int GreenLength { get; set; } = 10;

        [PropertyDescription]
        public int YellowLength { get; set; } = 3;

        [PropertyDescription]
        public bool Auto { get; set; } = true;

        [Link]
        public Grid2DLayer<TrafficLight> Environment { get; set; }

        public void Init(object model = null) { }

        public void Tick()
        {
            int ticks = Environment.TimeStep;

            if (!Auto) return;

            if (Color == LightColor.Green && ticks - TicksAtLastChange > GreenLength)
            {
                Color = LightColor.Yellow;
                TicksAtLastChange = ticks;
            }
            else if (Color == LightColor.Yellow && ticks - TicksAtLastChange > YellowLength)
            {
                Color = LightColor.Red;
                TicksAtLastChange = ticks;

                foreach (var light in Environment.GetAllAgents().Where(l => l.ID != this.ID))
                {
                    light.Color = LightColor.Green;
                }
            }
        }
    }

    public class Accident : IAgent
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Position Position { get; set; }
        public int ClearIn { get; set; } = 5;

        [Link]
        public Grid2DLayer<Accident> Environment { get; set; }

        public void Init(object model = null) { }

        public void Tick()
        {
            ClearIn--;
            if (ClearIn <= 0)
            {
                Environment.RemoveAgent(this);
            }
        }
    }
}

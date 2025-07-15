using Mars.Common.Core.Logging;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Core.Data.Wrapper;
using Mars.Core.Simulation;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TrafficSimulation
{
    //Defines a car agent
    public class Car : IAgent<Car>
    {
        public Guid ID { get; set; } = Guid.NewGuid(); //Every agent is assigned a unique ID when it is created.

        public Position Position { get; set; } //Car's position 
        public int Heading { get; set; } = 0; //Direction the car is facing
        public double Speed { get; set; } //Current speed of the car 
        public bool IsAlive { get; set; } = true; //Whether the car is active or removed

        [PropertyDescription]
        public double MaxBrake { get; set; } = 1; //Max speed reduction per tick 

        [PropertyDescription]
        public double MaxAccel { get; set; } = 1; //Max speed increade per tick

        [PropertyDescription]
        public double SpeedLimit { get; set; } = 3; //Max speed allowed

        public Init(Car other) { }

        public bool Tick()
        {
            AdjustSpeed(); //Adjust speed based on surroundings 

            for (int i = 0; i < (int)Speed; i++)
            {
                var next = Environment.CalculateNextPosition(Position, Heading); //Where to go next
                if (!Environment.IsPositionValid(next)) //If next position is invalid (off the map)
                {
                    IsAlive = false;
                    return false; //Remove car 
                }

                var collisions = Environment.GetAgentsAt<Car>(next); //check for other
                var accidents = Environment.GetAgentsAt<Accident>(next); //check for accident
                if (collisions.Any() || accidents.Any())
                {
                    if (accidents.FirstOrDefault() is Accident accident)
                        accident.ClearIn = 5; //Reset accident clear time if hit

                    IsAlive = false;
                    return false; //Remove Car 
                }
                Position = next; //Move to the next valid position 
            }

            return true; //Still active
        }

        private void AdjustSpeed() //Calculate and update the target speed
        {
            double minSpeed = Math.Max(Speed - MaxBrake, 0); //Minimum speed limit
            double maxSpeed = Math.Min(Speed + MaxAccel, SpeedLimit); //Maximum speed limit
            double targetSpeed = maxSpeed; // Start Fast

            var next = Environment.LookAhead(Position, Heading, (int)maxSpeed); //Check ahead
            if (next.Any(p => Environment.IsBlocked(p))) //Something is ahead
            {
                int spaceAhead = next.TakeWhile(p => !Environment.IsBlocked(p)).Count(); //Free Sapce
                while (BreakingDistance(targetSpeed) > spaceAhead && targetSpeed > minSpeed)
                    targetSpeed--; //Reduce speed if unsafe
            }
            Speed = targetSpeed; //Result
        }

        private double BreakingDistance(double currentSpeed) //Estimate stop distance
        {
            double nextSpeed = Math.Max(currentSpeed - MaxBrake, 0); //Speed next tick
            return currentSpeed + nextSpeed; // Total travel if braking
        }

        [Link]
        public IEnvironment Environment { get; set; } //Link Environment
    }

    //Defines a traffic light agent

    public class TrafficLight : IAgent<TrafficLight>
    {
        public enum LightColor { Green, Yellow, Red } //Light states

        public Guid ID { get; set; } = Guid.NewGuid(); //Unique ID

        public Position Position { get; set; } //Light Location
        public LightColor Color { get; set; } = LightColor.Red; //Initial Color 
        public int TicksAtLastChange { get; set; } //Last time it changed

        [PropertyDescription]
        public int GreenLength { get; set; } = 10; //How long the green light lasts 

        [PropertyDescription]
        public int YellowLength { get; set; } = 3; //How long the yellow light lasts.

        [PropertyDescription]
        public bool Auto { get; set; } = true; // Automatic toogle

        public void Init(TrafficLight other) { } //Interface methd

        public bool Tick()
        {
            int ticks = Environment.CurrentTick; //Get global time 

            if (Auto)
            {
                if (Color == LightColor.Green && ticks - TicksAtLastChange > GreenLength)
                {
                    Color = LightColor.Yellow; //Turn yellow
                    TicksAtLastChange = ticks;
                }
                else if (Color == LightColor.Yellow && ticks - TicksAtLastChange > YellowLength)
                {
                    Color = LightColor.Red; //Turn red 
                    TicksAtLastChange = ticks;

                    foreach (var light in Environment > GetAgents<TrafficLight>())
                    {
                        if (light.ID != this.ID)
                            light.Color = LightColor.Green;
                    }
                }

            }
            return true; //Remains active

        }

        [Link]
        public Environment Environment { get; set; } // Link to environment
    }

    public class Accident : IAgent<Accident>
    {
        public Guid ID { get; set; } = Guid.NewGuid(); //Unique ID
        public Position Position { get; set; } // The position of the accident

        public int CleaIn { get; set; } = 5; // Time until the accident gets cleared

        public void Init(Accident other) { } // Interface method 

        public bool Tick()
        {
            ClearIn--; //Decrease countdown
            return CleaIn > 0; //Remove if time is up
        }

        [Link]
        public IEnvironment Environment { get; set; } //Link to environment 


    }
    
}
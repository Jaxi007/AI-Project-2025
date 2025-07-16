using Mars.Core.Simulation;
using Mars.Interfaces.Model;
using System;

namespace TrafficSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the model from your model.json configuration
            var description = SimulationDescription.Load("model.json");

            // Create a runner and run the simulation
            var runner = SimulationStarter.Start(description);

            //Run ticks
            runner.Run(100); // Run 100 ticks
        }
    }
}
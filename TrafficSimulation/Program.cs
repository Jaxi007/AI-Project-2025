using Mars.Components.Starter;
using Mars.Interfaces.Model;
using System;

namespace TrafficSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            var description = SimulationDescription.Load("model.json");
            var starter = SimulationStarter.Start(description);
            var result = starter.Run();
            Console.WriteLine($"Simulation finished after {result.Tick} ticks.");
        }
    }
}

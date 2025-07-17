# AI-Project-2025

# Intro

This project as been implented by Cosmin Musteata. 

# Traffic Intersection Simulation 

This project is an **agent-based traffic intersection simulation** originally developed in **NetLogo** and re-implemented in **C#** using the **MARS.NET** framework (Multi-Agent Research and Simulation).

The goal is to model realistic vehicle and traffic light interactions at an intersection, maintaining comparable output behavior to the NetLogo version.

To view the original NetLogo simulation, visit:
(https://www.netlogoweb.org/launch#https://www.netlogoweb.org/assets/modelslib/Sample%20Models/Social%20Science/Unverified/Traffic%20Intersection.nlogo)

## System Overview 

The simulation consists of:

- **Cars**: Move across a grid, respond to traffic lights and accidents.
- **Traffic Lights**: Change color (Green → Yellow → Red) based on timers.
- **Accidents**: Temporary obstacles that block movement.

All entities operate within a **2D discrete grid environment**.

**Note:** The MARS Framework integrates into TrafficIntersection via a [nuget](https://www.nuget.org/) package. If the MARS dependencies of TrafficIntersection are not resolved properly, use the nuget package manager of your IDE to search for and install the nuget package `Mars.Life.Simulations`.

## Visualization

It is not required when implementing in VS, but the code has to be inplemented in a way that could give an output that can be compared with the NetLogo visualization. They have to be similar. 

## How to use

1. Build the project by going in the terminal by typing "dotnet build"
2. After step 1, write "dotnet run" (Program.cs)
3. Run the Program.cs.
4. See output.

The output should include a list of agents with ID numbers, traffic lights (changing in different scenarios), and the list of accidents. 


## Game Setup

##  Setup & Usage

###  Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Visual Studio](https://visualstudio.microsoft.com/) or VS Code with C# support
- **MARS.NET v4.2.0** NuGet packages:
  - `Mars.Core`
  - `Mars.Components`
  - `Mars.Interfaces`

You can install them via the NuGet Package Manager or the command line: 
dotnet add package Mars.Core
dotnet add package Mars.Components
dotnet add package Mars.Interfaces '

## Contact 

Cosmin Musteata, Borgfelder Str.16, 20537, Hamburg, Germany. 
eMail: [wmi150@haw-hamburg.de]
GitRepository: [www.github.com/Jaxi007/AI-Project-2025.git] (https://github.com/Jaxi007/AI-Project-2025.git)






# Introduction
The EINP Model is a simple MARS model that incorporates georeferenced raster and vector data. The model's environment represents the Elk Island National Park in Alberta, Canada. This area is modelled as a grid layer with georeferenced cells.

We simulate the behaviour of Bison, Moose and Elk inside the park, using agents.

Our goal is to provide an initial impression of how the animal populations evolves over time to make an estimate of to total carrying capacity of Elk Island National Park.

> Note: In its current state the model does not represent real animal behaviour accurately, and we hope that more features can be added in the future.

## Quickstart
To use the model clone the repository by running:
```bash
git clone https://github.com/Red-Sigma/einp-model.git
```

Then go into the newly downloaded project and run the simulation:
```bash
cd einp-model/GeoRasterBlueprint/
dotnet run -sm config.json
```

If you want more details and options on how to run the simulation please have a look at [Running & Output](./running.md)

## Project Structure

After running `git clone` you should have a folder that looks similar to the one below.

```bash
einp-model/
├── docs/
├── GeoRasterBlueprint/
│   ├── Model/
│   ├── Resources/
│   ├── config.json
│   ├── GeoRasterBlueprint.csproj
│   ├── Program.cs
│   └── run.sh
├── GeoRasterBlueprint.sln
├── LICENSE
└── README.md
```

- `docs/`: the source of [red-sigma.github.io/einp-model](https://red-sigma.github.io/einp-model/)
- `GeoRasterBlueprint/`: contains most of the model source
    - `Model/`: source code for layers and agents
    - `Resources/`: assets like the spawn locations of the animals
    - `config.json`: the main configuration file that parametrizes the model
    - `GeoRasterBlueprint.csproj`: a configuration file of the .NET framework
    - `Program.cs`: the main entry point where the simulation starts
    - `run.sh`: a convenience [bash](https://en.wikipedia.org/wiki/Bash_(Unix_shell)) script to run the model from a terminal
- `GeoRasterBlueprint.sln`: a .NET solution file for use with Visual Studio
- `LICENSE`: a copy of the MIT-license under which this project is licensed
- `README.md`: a file describing the project


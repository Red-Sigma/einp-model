# Model Structure and Resources

The following components and resources make up the model:

- `Bison`: This agent type models a bison that moves around randomly in the Elk Island National Park. When its Energy is below a certain threshold, it queries the WaterLayer (see below) for a water source to navigate to. Bison agents are parameterized with the **bisons.csv** file in the `Resources` folder.

- `Moose`: This agent type models a moose that moves around randomly in the Elk Island National Park. When its Energy is below a certain threshold, it queries the WaterLayer (see below) for a water source to navigate to. Moose agents are parameterized with the **moose.csv** file in the `Resources` folder.

- `Elk`: This agent type models an elk that moves around randomly in the Elk Island National Park. When its Energy is below a certain threshold, it queries the WaterLayer (see below) for a water source to navigate to. Bison agents are parameterized with the **elks.csv** file in the `Resources` folder.

- `Perimeter`: This vector layer holds the perimeter of the Elk Island National Park. The georeferenced data are provided by the `einp_perimeter.geojson` file in the `Resources/` folder.

- `WaterLayer`: The raster layer holds a set of pixels that represent water sources in the Elk Island National Park. The georeferenced data of the water sources are provided by the `einp_water_spots.geojson` file in the `Resources/` folder.

- `LandscapeLayer`: This layer holds a grid of cells. Each cell is referenced by geo-coordinates. The georeferenced data contained in the `Model` folder are used to determine which cell is accessible (i.e., within the perimeter) and inaccessible (i.e., outside the perimeter).

- `VegetationLayer`: This raster layer consists of a collection of pixels representing the vegetation on the surface of the park, based on the calculated NDVI. This high-resolution vegetation map was created using QGIS processing and then exported to a raster file. The georeferenced data enable agents in the simulation to effectively determine which areas in the park are suitable for food production. The associated data are located in the Resources/ folder.

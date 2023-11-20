using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using Mars.Interfaces.Environments;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

/// <summary>
///     This raster layer provides information about biomass of animals etc.
/// </summary>
/// 
public class VegetationLayer : RasterLayer {
    public bool IsPointInside(Position coordinate) {
        return Extent.Contains(coordinate.X, coordinate.Y) && Math.Abs(GetValue(coordinate)) > 0.0;
    }
}
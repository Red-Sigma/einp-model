using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace GeoRasterBlueprint.Model;

/// <summary>
///     A vector layer that ingests a .geojson vector file with the walkable area by the agents.
/// </summary>
public class Perimeter : VectorLayer {
    /// <summary>
    ///     Checks for the coordinate whether this point is inside the perimeter (the defined polygon).
    /// </summary>
    /// <param name="coordinate">The coordinate to check</param>
    /// <returns>
    ///     Returns true if the coordinate is inside the perimeter.
    /// </returns>
    public bool IsPointInside(Position coordinate) {
        return Extent.Contains(coordinate.X, coordinate.Y);
    }
}
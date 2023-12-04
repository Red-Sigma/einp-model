using Mars.Components.Layers;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

/// <summary>
///     Represents water spots in the Elk Island National Park.
/// </summary>
public class WaterLayer : VectorLayer {
    public bool IsPointInside(Position coordinate) {
        foreach (var waterSpot in Features) {
            if (waterSpot.VectorStructured.BoundingBox.Contains(coordinate.X, coordinate.Y)) {
                return true;
            }
        }
        return false;
    }
    
    public bool IsIntersectsAny(Position source, Position target) {
        foreach (var waterSpot in Features) {
            if (waterSpot.VectorStructured.BoundingBox.Intersects(
                    new Coordinate(source.X, source.Y), 
                    new Coordinate(target.X, target.Y))) {
                return true;
            }
        }
        return false;
    }
    
}
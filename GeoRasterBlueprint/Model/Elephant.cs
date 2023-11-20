using System;
using System.Linq;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

/// <summary>
///     The Elephant agent is situated in the Addo Elephant National Park and moves between water holes to meet its
///     energy needs.
/// </summary>
public class Elephant : IAgent<LandscapeLayer>, IPositionable {
    #region Properties and Fields

    /// <summary>
    ///     The latitude of the current geo-referenced position of the agent
    /// </summary>
    [PropertyDescription(Name = "Latitude")]
    public double Latitude { get; set; }

    /// <summary>
    ///     The longitude of the current geo-referenced position of the agent
    /// </summary>
    [PropertyDescription(Name = "Longitude")]
    public double Longitude { get; set; }

    /// <summary>
    ///     The current energy level of the agent
    /// </summary>
    [PropertyDescription(Name = "Energy")]
    public double Energy { get; set; }

    /// <summary>
    ///     The bearing of the agent (0-360 degrees)
    /// </summary>
    private double _bearing = 222.0;

    private readonly Random _random = new();

    /// <summary>
    ///     The distance in meters the elephant should move per tick
    /// </summary>
    private const double Distance = 10.0;

    /// <summary>
    ///     The current position of the agent
    /// </summary>
    public Position Position { get; set; }
    
    /// <summary>
    ///     The current target of the agent
    /// </summary>
    private Position Target { get; set; }

    /// <summary>
    ///     The layer on which these agents live
    /// </summary>
    private LandscapeLayer Layer { get; set; }

    /// <summary>
    ///     The layer through which the agent can access water spots
    /// </summary>
    [PropertyDescription(Name = "WaterLayer")]
    public WaterLayer WaterLayer { get; set; }

    /// <summary>
    ///     The perimeter of the simulation environment
    /// </summary>
    [PropertyDescription(Name = "Perimeter")]
    public Perimeter Perimeter { get; set; }

    /// <summary>
    ///     The unique identifier of the agent
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    ///     The layer through which the agent can find food
    /// </summary>
    [PropertyDescription(Name = "VegetationLayer")]
    public VegetationLayer VegetationLayer { get; set; }
    
    

    #endregion
    
    #region constants

    /// <summary>
    ///     The frequency in which the animal looks for food
    /// </summary>
    private int tickSearchForFood = 10;

    #endregion

    #region Initialization

    public void Init(LandscapeLayer layer) {
        // Store the given layer in agent property for later access
        Layer = layer;

        // Make sure elephants' initial position is inside the perimeter
        if (!Perimeter.IsPointInside(new Position(Longitude, Latitude))) {
            throw new Exception("Start point is not inside perimeter.");
        }

        // Position of elephant is created using coordinates specified in input CSV file
        Position = Position.CreateGeoPosition(Longitude, Latitude);
    }

    #endregion

    #region Tick

    public void Tick() {
        Energy -= 1;

        // Create target position with current bearing and distance
        Target = Position.CalculateRelativePosition(_bearing, Distance);

        if (Layer.GetCurrentTick() % tickSearchForFood == 0) {
            if (VegetationLayer.IsPointInside(Position)) {
                var all = VegetationLayer.Explore(Position, double.MaxValue, 4);
                var res = all.OrderBy(a => a.Node.Value).Last();
                if (res.Node?.NodePosition != null) {
                    
                    var targetX = res.Node.NodePosition.X;
                    var targetY = res.Node.NodePosition.Y;
                    
                    var targetLon = VegetationLayer.LowerLeft.X +
                                    targetX * VegetationLayer.CellWidth;
                    var targetLat = VegetationLayer.LowerLeft.Y +
                                    targetY * VegetationLayer.CellHeight;
                    
                    Target = new Position(targetLon, targetLat);

                    if (Perimeter.IsPointInside(Target)) {
                        _bearing = Position.GetBearing(Target);
                        return;
                    }
                }
            }
        }

        if (false) {
            // Energy is low, so look for water
            var waterSources = WaterLayer.Explore(Position.PositionArray, 10000).ToList();
            if (waterSources.Any()) {
                // Get coordinates of the nearest water source...
                var nearestWaterSource = waterSources.First();
                var waterSourceLocation = (Point)nearestWaterSource.VectorStructured.Geometry;
                Target = new Position(waterSourceLocation.X, waterSourceLocation.Y);

                // ... and change the agent's bearing such that it looks in the direction of the water source
                _bearing = Position.GetBearing(Target);

                // If the agent in close the the water source, increase its energy and change bearing
                if (Target.DistanceInMTo(Position) < 20) {
                    Energy += 5000;
                    _bearing = (_bearing + 45) % 360;
                }
            }
            else {
                //Console.WriteLine("No water in area");
            }
        }
        else {
            // Every 123 ticks, change the bearing randomly to generate random multi-directional movement
            if (Layer.Context.CurrentTick % 123 == 0) {
                _bearing = _random.Next(0, 360);
            }
        }

        // Make sure the calculated target is still inside our perimeter
        if (Perimeter.IsPointInside(Target)) {
            // Target is inside perimeter, so move there
            Position = Layer.Environment.MoveTowards(this, _bearing, Distance);
        }
        else {
            // Target is outside of perimeter, so don't move. Instead, change bearing
            _bearing = (_bearing + 45) % 360;
        }
    }
    
    #endregion
    
}
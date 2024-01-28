using System;
using System.Collections.Generic;
using System.Linq;
using GeoRasterBlueprint.Util;
using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using NetTopologySuite.Utilities;
using Position = Mars.Interfaces.Environments.Position;
using Mars.Interfaces.Annotations;

namespace GeoRasterBlueprint.Model;

public abstract class AbstractAnimal : IPositionable, IAgent<LandscapeLayer> {

    [ActiveConstructor]
    public AbstractAnimal() {
    }
    
    [ActiveConstructor]
    public AbstractAnimal(
        LandscapeLayer landscapeLayer, 
        Perimeter perimeter,
        VegetationLayer vegetationLayer,
        WaterLayer waterLayer,
        Guid id,
        AnimalType animalType,
        bool isLeading,
        int herdId,
        double latitude, 
        double longitude) { 
        Position = Position.CreateGeoPosition(longitude, latitude);
        _landscapeLayer = landscapeLayer;
        _perimeter = perimeter;
        _vegetationLayer = vegetationLayer;
        _waterLayer = waterLayer;
        _animalType = animalType;
        ID = id;
        //_isLeading = isLeading;
        //_herdId = herdId;
    }
    
    public Guid ID { get; set; }
    public abstract Position Position { get; set; }
    public abstract Position Target { get; set; }
    public double Bearing = 222.0;
    public const double Distance = 5000.0;
    public LandscapeLayer _landscapeLayer { get; set; }
    public abstract double Latitude { get; set; }
    public abstract double Longitude { get; set; }
    public Perimeter _perimeter { get; set; }
    public abstract double Hydration { get; set; }
    public abstract double Satiety { get; set; }
    public VectorWaterLayer VectorWaterLayer { get; set; }
    public RasterWaterLayer RasterWaterLayer { get; set; }
    public VegetationLayer _vegetationLayer { get; set; }
    
    public int _hoursLived;
    public AnimalType _animalType;
    public readonly int[] _reproductionYears = {2, 15};
    public bool _pregnant;
    public int _pregnancyDuration;
    public int _chanceOfDeath;
    public int Age { get; set; }
    public AnimalLifePeriod _LifePeriod;
    public MattersOfDeath MatterOfDeath { get; private set; }
    public bool IsAlive { get; set; } = true;

    protected bool isLeading { get; }
    protected int herdId { get; } 
    
    public static Random _random = new ();
    private const int RandomWalkMaxDistanceInM = 500;
    private const int RandomWalkMinDistanceInM = 10;
    public const double MaxHydration = 100.0;
    public const double MaxSatiety = 100.0; 
    public const int MaxAge = 25;

    
    public void Init(LandscapeLayer layer) {
        _landscapeLayer = layer;

        var spawnPosition = new Position(Longitude, Latitude);
        _landscapeLayer = layer;
        
        if (_perimeter.IsPointInside(spawnPosition) && !RasterWaterLayer.IsPointInside(spawnPosition)) {
            Position = Position.CreateGeoPosition(Longitude, Latitude);
        } else {
            throw new Exception($"Start point is not valid. Lon: {Longitude}, Lat: {Latitude}");
        }
    }
    
    public abstract void Tick();
    
    protected void DoRandomWalk(int numOfAttempts) {
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
        
        while (numOfAttempts > 0) {
            var randomDistance = _random.Next(RandomWalkMinDistanceInM, RandomWalkMaxDistanceInM);
            var randomDirection = _random.Next(0, 360);
            
            Target = Position.GetRelativePosition(randomDirection, randomDistance);
            
            if (Perimeter.IsPointInside(Target) && !RasterWaterLayer.IsPointInside(Target)) {
                Position = Target;
                break;
            }
            numOfAttempts--;
        }
        
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
    }
    
    protected void LookForWaterAndDrink() {
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
        const int radius = 2000;
        var nearWaterSpots = VectorWaterLayer.Explore(Position.PositionArray, radius)
            .ToList();

        if (!nearWaterSpots.Any()) return;
        
        var nearestWaterSpot = VectorWaterLayer
            .Nearest(new []{Position.X, Position.Y})
            .VectorStructured
            .Geometry
            .Coordinates
            .Where(coordinate => Perimeter.IsPointInside(new Position(coordinate.X, coordinate.Y)))
            .OrderBy(coordinate => Position.DistanceInMTo(coordinate.X, coordinate.Y))
            .ToList();

        foreach (var point in nearestWaterSpot) {
            Target =  new Position(point.X, point.Y);
            
            if (Perimeter.IsPointInside(Target) && !RasterWaterLayer.IsPointInside(Target)) {
                Position = Target;
                Hydration += 20;
                break;
            }
        }
        
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
    }

    protected void LookForFoodAndEat() {
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
        if (VegetationLayer.IsPointInside(Position)) {
            var nearVegetationSpots = VegetationLayer.Explore(Position, 20)
                .OrderByDescending(node => node.Node.Value)
                .ToList();

            foreach (var spot in nearVegetationSpots) {
                
                var targetX = spot.Node.NodePosition.X;
                var targetY = spot.Node.NodePosition.Y;

                var targetLon = _vegetationLayer.LowerLeft.X +
                                targetX * _vegetationLayer.CellWidth;
                var targetLat = _vegetationLayer.LowerLeft.Y +
                                targetY * _vegetationLayer.CellHeight;

                Target = new Position(targetLon, targetLat);

                if (Perimeter.IsPointInside(Target) && !RasterWaterLayer.IsPointInside(Target)) {
                    Position = Target;
                    Satiety += 12;
                    break;
                }
            }
        }
        Assert.IsTrue(Perimeter.IsPointInside(Position) && !RasterWaterLayer.IsPointInside(Position));
    }
    
    //every animals has different ways to consume food or hydration
    protected abstract void UpdateState();

    protected void BurnSatiety(double rate)
    {
        if (Satiety > 0) {
            if (Satiety > rate) {
                Satiety -= rate;
            } else {
                Satiety = 0;
            }
        }
    }

    protected void Dehydrate(double rate)
    {
        if (Hydration > 0) {
            if (Hydration > rate) {
                Hydration -= rate;
            } else {
                Hydration = 0;
            }
        }
    }
    public abstract void YearlyRoutine();

    public abstract AnimalLifePeriod GetAnimalLifePeriodFromAge(int age);
    
    public void Die(MattersOfDeath mannerOfDeath)
    {
        MatterOfDeath = mannerOfDeath;
        IsAlive = false;
        //add removal of animal
    }
    
}

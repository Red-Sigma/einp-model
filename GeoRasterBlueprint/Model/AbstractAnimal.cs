using System;
using System.Linq;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using NetTopologySuite.Utilities;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

public abstract class AbstractAnimal : IPositionable, IAgent<LandscapeLayer> {
    
    public Guid ID { get; set; }
    public abstract Position Position { get; set; }
    public abstract Position Target { get; set; }
    public double Bearing = 222.0;
    public const double Distance = 5000.0;
    public LandscapeLayer LandscapeLayer { get; set; }
    public abstract double Latitude { get; set; }
    public abstract double Longitude { get; set; }
    public Perimeter Perimeter { get; set; }
    public abstract double Hydration { get; set; }
    public abstract double Satiety { get; set; }
    public VectorWaterLayer VectorWaterLayer { get; set; }
    public RasterWaterLayer RasterWaterLayer { get; set; }
    public VegetationLayer VegetationLayer { get; set; }
    
    public int _hoursLived;
    public AnimalType _animalType;
    public readonly int[] _reproductionYears = {2, 15};
    public bool _pregnant;
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
        LandscapeLayer = layer;

        var spawnPosition = new Position(Longitude, Latitude);
        
        if (Perimeter.IsPointInside(spawnPosition) && !RasterWaterLayer.IsPointInside(spawnPosition)) {
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

                var targetLon = VegetationLayer.LowerLeft.X +
                                targetX * VegetationLayer.CellWidth;
                var targetLat = VegetationLayer.LowerLeft.Y +
                                targetY * VegetationLayer.CellHeight;

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

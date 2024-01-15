using System;
using System.Collections.Generic;
using System.Linq;
using GeoRasterBlueprint.Util;
using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

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
    public WaterLayer WaterLayer { get; set; }
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
    
    public static Random _random = new ();
    private const int RandomWalkMaxDistanceInM = 500;
    private const int RandomWalkMinDistanceInM = 10;
    public const double MaxHydration = 100.0;
    public const double MaxSatiety = 100.0;
    public const double DehydrationRate = 6.0;
    public const double StarvationRate = 4.0;
    public const int MaxAge = 25;

    public void Init(LandscapeLayer layer) {
        LandscapeLayer = layer;
        
        if (LandscapeLayer.Fence.IsPointInside(new Position(Longitude, Latitude))) {
            Position = Position.CreateGeoPosition(Longitude, Latitude);
        } else {
            throw new Exception($"Start point is not inside perimeter. Lon: {Longitude}, Lat: {Latitude}");
        }
    }
    
    public abstract void Tick();

    protected Position Move(AbstractAnimal animal, double bearing, double distance) {
        return LandscapeLayer.Environment.MoveTowards(animal, bearing, distance);
    }
    
    protected void DoRandomWalk(int numOfAttempts) {
        bool walkedSuccessfully = false;
        
        while (numOfAttempts > 0 && !walkedSuccessfully) {
            var randomDistance = _random.Next(RandomWalkMinDistanceInM, RandomWalkMaxDistanceInM);
            var randomDirection = _random.Next(0, 360);
            
            var targetPosition = Position.GetRelativePosition(randomDirection, randomDistance);

            // removed for being bugged && !WaterLayer.IsIntersectsAny(Position, targetPosition)
            if (Perimeter.IsPointInside(targetPosition)) {
                var newCurrentPosition = Move(this, randomDirection, randomDistance);
                if (newCurrentPosition != null) {
                    walkedSuccessfully = true;
                }
            }

            numOfAttempts--;
        }
    }
    
    protected void MoveToWaterSource() {
        int radius = 2000;
        List<VectorFeature> nearWaterSpots = WaterLayer.Explore(Position.PositionArray, radius).ToList();
        if (nearWaterSpots.Any()) {
            var nearest = WaterLayer.Nearest(new []{Position.X, Position.Y});

            var nearestPoints = nearest.VectorStructured.Geometry.Coordinates.ToList();
            
            nearestPoints.Sort(new DistanceComparer(Position.X, Position.Y));
            
            Target =  new Position(nearestPoints.First().X, nearestPoints.First().Y);
            
            Bearing = Position.GetBearing(Target);

            if (Target.DistanceInMTo(Position) < 50) {
                Hydration += 20;
                Bearing = (Bearing + 45) % 360;
            }

            var distanceToTarget = Target.DistanceInMTo(Position);
            
            Position = distanceToTarget > Distance
                ? Move(this, Bearing, Distance)
                : Move(this, Bearing, distanceToTarget - 10);
        } else {
            Console.WriteLine("No water found in {0}m radius.", radius);
        }
    }

    protected void SearchForFood() {
        if (VegetationLayer.IsPointInside(Position)) {
            var all = VegetationLayer.Explore(Position, 20, 4);
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
                    Satiety += 12;
                    var oldPos = Position;
                    var distanceToTarget = Target.DistanceInMTo(Position);
                    Bearing = Position.GetBearing(Target);
                    Position = distanceToTarget > Distance
                        ? Move(this, Bearing, Distance)
                        : Move(this, Bearing, distanceToTarget);
                }
            }
        }
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

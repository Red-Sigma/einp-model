using System;
using System.Collections.Generic;
using System.Linq;
using GeoRasterBlueprint.Util;
using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using NetTopologySuite.Geometries;
using ServiceStack.Text;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

public class Bison : IAgent<LandscapeLayer>, IPositionable, IAnimalAgent {

    #region Properties and Fields

    [PropertyDescription(Name = "Latitude")]
    public double Latitude { get; set; }

    [PropertyDescription(Name = "Longitude")]
    public double Longitude { get; set; }
    
    private double Hydration { get; set; } = MaxHydration;
    private double Satiety { get; set; } = MaxSatiety;
    public MattersOfDeath MatterOfDeath { get; private set; }
    private double _bearing = 222.0;
    private const double Distance = 5000.0;
    private int _hoursLived;
    private BisonType _bisonType;
    private readonly int[] _reproductionYears = {2, 15};
    private bool _pregnant;
    private int _chanceOfDeath;
    public Position Position { get; set; }
    private Position Target { get; set; }
    private LandscapeLayer LandscapeLayer { get; set; }

    [PropertyDescription(Name = "WaterLayer")]
    public WaterLayer WaterLayer { get; set; }

    [PropertyDescription(Name = "Perimeter")]
    public Perimeter Perimeter { get; set; }

    [PropertyDescription(Name = "VegetationLayer")]
    public VegetationLayer VegetationLayer { get; set; }

    [PropertyDescription(Name = "TemperatureLayer")]
    public TemperatureLayer TemperatureLayer { get; set; }

    [PropertyDescription(Name = "AltitudeLayer")]
    public AltitudeLayer AltitudeLayer { get; set; }

    public Guid ID { get; set; }
    private int HoursWithoutWater { get; set; }
    private int HoursWithoutFood { get; set; }

    private bool IsAlive { get; set; } = true;
    public int Age { get; set; }
    public int DailyEatingHours = 8;
    public string Gender { get; set; }
    public float Health { get; set; }
    public int UniqueId { get; set; }
    public AnimalLifePeriod _LifePeriod;

    #endregion

    #region Constants

    public static Random _random = new Random();
    private const int RandomWalkMaxDistanceInM = 5000;
    private const int RandomWalkMinDistanceInM = 100;
    public const double MaxHydration = 100.0;
    public const double MaxSatiety = 100.0;
    public const double DehydrationRate = 20.0;
    public const double StarvationRate = 1.5;
    private int TickSearchForFood = 10;
    public const double HoursToDeathWithoutWater = MaxHydration / DehydrationRate;
    public const double HoursToDeathWithoutFood = MaxSatiety / StarvationRate;
    public const int MaxAge = 25;

    #endregion

    #region Initialization

    public void Init(LandscapeLayer landscapeLayer) {
        LandscapeLayer = landscapeLayer;

        if (Perimeter.IsPointInside(new Position(Longitude, Latitude))) {
            Position = Position.CreateGeoPosition(Longitude, Latitude);
        }
        else {
            throw new Exception("Start point is not inside perimeter.");
        }
    }

    #endregion

    #region Tick

    public void Tick() {
        // still alive, increase hour counter
        _hoursLived++;
        
        //yearly routine
        //get older, die eventually
        if (_hoursLived == 300)
        {
            var alive = YearlyRoutine();
            if (!alive) return;
        }
        
        if (Hydration < 40) {
            MoveToWaterSource();
        }
        else if (Satiety < 40) {
            if (LandscapeLayer.GetCurrentTick() % TickSearchForFood == 0) {
                SearchForFood();
            }
        }
        
        else {
            DoRandomWalk(10);
        }

        // if (Age >= MaxAge) {
        //     DieOfOldAge();
        // } else {
        //     if (Hydration <= 0) {
        //         MoveToWaterSource();
        //     } else if (Satiety <= 0) {
        //         MoveTowardsGrazingArea();
        //     }else {
        //         if (CanSeeVegetation()) {
        //             Graze();
        //         } else {
        //             DoRandomWalk();
        //         }
        //     }
        // }
        UpdateState();
        // CheckSurvival();
    }

    #endregion

    private void UpdateState() {
        Hydration -= DehydrationRate;
        Satiety -= StarvationRate;
        HoursWithoutWater++;
        HoursWithoutFood++;
    }

    private void MoveToWaterSource() {
        int radius = 2000;
        List<VectorFeature> nearWaterSpots = WaterLayer.Explore(Position.PositionArray, radius).ToList();
        if (nearWaterSpots.Any()) {
            var nearest = WaterLayer.Nearest(new[] { Position.X, Position.Y });

            var nearestPoints = nearest.VectorStructured.Geometry.Coordinates.ToList();

            nearestPoints.Sort(new DistanceComparer(Position.X, Position.Y));

            Target = new Position(nearestPoints.First().X, nearestPoints.First().Y);

            _bearing = Position.GetBearing(Target);

            if (Target.DistanceInMTo(Position) < 50) {
                Hydration += 10;
                _bearing = (_bearing + 45) % 360;
            }

            var distanceToTarget = Target.DistanceInMTo(Position);
            Position = distanceToTarget > Distance
                ? LandscapeLayer.Environment.MoveTowards(this, _bearing, Distance)
                : LandscapeLayer.Environment.MoveTowards(this, _bearing, distanceToTarget - 10);
        }
        else {
            Console.WriteLine("No water found in {0}m radius.", radius);
        }
    }

    private void SearchForFood() {
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
                }
            }
        }
    }

    public void DieOfOldAge() {
        throw new NotImplementedException();
    }

    public void GiveBirth() {
        throw new NotImplementedException();
    }

    public Bison Mate(Bison partner) {
        throw new NotImplementedException();
    }

    private bool CanSeeVegetation() {
        throw new NotImplementedException();
    }

    private void MoveTowardsGrazingArea() {
        throw new NotImplementedException();
    }

    private void Graze() {
        throw new NotImplementedException();
    }

    private void DoRandomWalk(int numOfAttempts) {
        bool walkedSuccessfully = false;

        while (numOfAttempts > 0 && !walkedSuccessfully) {
            var randomDistance = _random.Next(RandomWalkMinDistanceInM, RandomWalkMaxDistanceInM);
            var randomDirection = _random.Next(0, 360);

            var targetPosition = Position.GetRelativePosition(randomDirection, randomDistance);

            if (Perimeter.IsPointInside(targetPosition) && !WaterLayer.IsIntersectsAny(Position, targetPosition)) {
                var newCurrentPosition = LandscapeLayer.Environment.MoveTowards(this, randomDirection, randomDistance);
                if (newCurrentPosition != null) {
                    walkedSuccessfully = true;
                }
            }

            numOfAttempts--;
        }
    }

    private void CheckSurvival() {
        if (Hydration <= 0 || Satiety <= 0) {
            MarkAsDead();
        }
    }

    private void MarkAsDead() {
        throw new NotImplementedException();
    }

    public void UpdateDailyEatingHours() {
        if (IsSummer()) {
            DailyEatingHours = 12; // Example value TODO
        }
        else if (IsWinter()) {
            DailyEatingHours = 8; // Example value TODO
        }
    }

    private bool IsSummer() {
        throw new NotImplementedException();
    }

    private bool IsWinter() {
        throw new NotImplementedException();
    }

    public void Move(Point destination) {
        throw new NotImplementedException();
    }

    public void Reproduce(IAnimalAgent partner) {
        throw new NotImplementedException();
    }

    public void Interact(IAnimalAgent agent) {
        throw new NotImplementedException();
    }

    public void Eat() {
        throw new NotImplementedException();
    }

    public void Die(MattersOfDeath mannerOfDeath)
    {
        MatterOfDeath = mannerOfDeath;
        IsAlive = false;
        LandscapeLayer.Bisons.Remove(this);
    }

    public void Drink() {
        throw new NotImplementedException();
    }

    private bool YearlyRoutine() {
        _hoursLived = 0;
        Age++;

        //decide sex once the bison reaches the adult stage
        var newLifePeriod = GetBisonLifePeriodFromAge(Age);
        if (newLifePeriod != _LifePeriod) {
            if (newLifePeriod == AnimalLifePeriod.Adult) {
                //50:50 chance of being male or female
                if (_random.Next(2) == 0)
                    _bisonType = BisonType.BisonBull;
                else
                    _bisonType = BisonType.BisonCow;
            }
            Console.WriteLine("Es ist ein " + _bisonType);

            _LifePeriod = newLifePeriod;
            
            Console.WriteLine("New LifePeriod: " + _LifePeriod);
            
        }
        
        //died of age?
        //max age 25
        if (Age > 15)
        {
            _chanceOfDeath = (Age - 15) * 10;
            if (_random.Next(0, 100) >= _chanceOfDeath) return true;

            Console.WriteLine("Bison ist gestorben mit Alter: " + Age);
            //Die not implemented yet
            Die(MattersOfDeath.Age);
            return false;
        }

        //check for possible reproduction
        if (!_reproductionYears.Contains(Age)) return true;

        if (!_bisonType.Equals(BisonType.BisonCow)) return true;

        _pregnant = true;
        Console.WriteLine("Bison ist nun schwanger mit Alter: " + Age);

        return true;
    }
    
    private static AnimalLifePeriod GetBisonLifePeriodFromAge(int age)
    {
        if (age < 1) return AnimalLifePeriod.Calf;

        return age <= 2 ? AnimalLifePeriod.Adolescent : AnimalLifePeriod.Adult;
    }

}

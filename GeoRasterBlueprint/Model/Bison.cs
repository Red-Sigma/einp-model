using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace GeoRasterBlueprint.Model;

public class Bison : AbstractAnimal {
    
    #region Properties and Fields
    
    public override double Hydration { get; set; } = MaxHydration;
    public override double Satiety { get; set; } = MaxSatiety;
    public override Position Position { get; set; }
    public override Position Target { get; set; }
    [PropertyDescription(Name = "Latitude")]
    public override double Latitude { get; set; }
    [PropertyDescription(Name = "Longitude")]
    public override double Longitude { get; set; }

    #endregion
    #region Constants
    private readonly Dictionary<AnimalLifePeriod, double> _satietyIntakeHourly = new()
    {
        //food per day (kg) / 16 (hours)
        { AnimalLifePeriod.Calf, 0.56 }, //9kg per day
        { AnimalLifePeriod.Adolescent, 1.81 }, //20-29 kg per day
        { AnimalLifePeriod.Adult, 3.75 } //60 kg per day
    };
    
    private readonly Dictionary<AnimalLifePeriod, double> _dehydrationRate =
        new()
        {
            { AnimalLifePeriod.Calf, 0.7 }, // daily water consumption 17.0 = 9 / 60 *  113, divided by 24
            { AnimalLifePeriod.Adolescent, 2.29 }, //daily water consumption 55.0 =  29 / 60 * 113, all divided by 24
            { AnimalLifePeriod.Adult, 4.7} //daily water consumption 113, divided by 24
        };
    
    [PropertyDescription]
    public static double DailyFoodAdult { get; set; }
    [PropertyDescription]
    public static double DailyFoodCalf { get; set; } 
    [PropertyDescription]
    public static double DailyFoodAdolescent { get; set; }
    
    //total need of water per day in liters   
    [PropertyDescription]
    public static double DailyWaterAdult { get; set; }
    [PropertyDescription]
    public static double DailyWaterCalf { get; set; }
    [PropertyDescription]
    public static double DailyWaterAdolescent { get; set; }
    #endregion
    
    public override void Tick() { 
        
        if (!IsAlive) return;
        _hoursLived++;
        if (_hoursLived == 300)
        {
            YearlyRoutine();
        }
        if (!IsAlive) return;
       
        if (Satiety < 40) {
            SearchForFood();
        }
        else if (Hydration < 40) {
            MoveToWaterSource();
            // currently buggy because we walk into water
            Hydration += 20;
        }
        else {
            DoRandomWalk(10);
        }
        UpdateState();
    }
    protected override void UpdateState()
    {
        int currentHour;
        if (LandscapeLayer.Context.CurrentTimePoint != null)
            currentHour = LandscapeLayer.Context.CurrentTimePoint.Value.Hour;
        else
            throw new NullReferenceException();
        

        if (currentHour is >= 21 and <= 23 || currentHour is >= 0 and <= 4 ) {   
            BurnSatiety(_satietyIntakeHourly[_LifePeriod] / 4); //less food is consumed while sleeping
            Dehydrate(_dehydrationRate[_LifePeriod]/2);           //less water is consumed at night
        }
        else
        {
            BurnSatiety(_satietyIntakeHourly[_LifePeriod]);
            Dehydrate(_dehydrationRate[_LifePeriod]);
        }
    }
    
    public override void YearlyRoutine() {
        _hoursLived = 0;
        Age++;

        //decide sex once the bison reaches the adult stage
        var newLifePeriod = GetAnimalLifePeriodFromAge(Age);
        if (newLifePeriod != _LifePeriod) {
            if (newLifePeriod == AnimalLifePeriod.Adult) {
                //50:50 chance of being male or female
                if (_random.Next(2) == 0)
                    _animalType = AnimalType.BisonBull;
                else
                    _animalType = AnimalType.BisonCow;
            }
            _LifePeriod = newLifePeriod;
        }
        
        //max age 25
        if (Age > 15)
        {
            _chanceOfDeath = (Age - 15) * 10;
            var rnd = _random.Next(0, 100);
            if (rnd >= _chanceOfDeath) return;
            Die(MattersOfDeath.Age);
            return;
        }

        //check for possible reproduction
        if (!_reproductionYears.Contains(Age)) return;

        if (!_animalType.Equals(AnimalType.BisonBull)) return;

        _pregnant = true;
    }
    
    public override AnimalLifePeriod GetAnimalLifePeriodFromAge(int age)
    {
        if (age < 1) return AnimalLifePeriod.Calf;
        return age <= 2 ? AnimalLifePeriod.Adolescent : AnimalLifePeriod.Adult;
    }
    
}

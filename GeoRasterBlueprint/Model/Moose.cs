using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace GeoRasterBlueprint.Model; 

public class Moose : AbstractAnimal {
    
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
    //TODO use real moose data
    private readonly Dictionary<AnimalLifePeriod, double> _satietyIntakeHourly = new()
    {
        //food per day (kg) / 16 (hours)
        { AnimalLifePeriod.Calf, 0.25 },        //4 kg per day
        { AnimalLifePeriod.Adolescent, 0.81 },  //13 kg per day
        { AnimalLifePeriod.Adult, 1.68 }        //27 kg per day
    };
    
    private readonly Dictionary<AnimalLifePeriod, double> _dehydrationRate =
        new()
        {   
            //total daily water consumption / 24
            { AnimalLifePeriod.Calf, 3.29},         // daily water consumption 79  
            { AnimalLifePeriod.Adolescent, 10.75 },  // daily water consumption 258
            { AnimalLifePeriod.Adult, 22.0}          // daily water consumption 529, 
        };
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
    
    //TODO change to simulate moose water and food consumption
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
                    _animalType = AnimalType.MooseBull;
                else
                    _animalType = AnimalType.MooseCow;
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

        if (!_animalType.Equals(AnimalType.MooseBull)) return;

        _pregnant = true;
    }
    
    public override AnimalLifePeriod GetAnimalLifePeriodFromAge(int age)
    {
        if (age < 1) return AnimalLifePeriod.Calf;
        return age <= 2 ? AnimalLifePeriod.Adolescent : AnimalLifePeriod.Adult;
    }
    
}

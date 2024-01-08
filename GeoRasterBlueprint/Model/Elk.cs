using System;
using System.Linq;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace GeoRasterBlueprint.Model; 

public class Elk : AbstractAnimal {
    
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
    
    public override void YearlyRoutine() {
        _hoursLived = 0;
        Age++;

        //decide sex once the bison reaches the adult stage
        var newLifePeriod = GetAnimalLifePeriodFromAge(Age);
        if (newLifePeriod != _LifePeriod) {
            if (newLifePeriod == AnimalLifePeriod.Adult) {
                //50:50 chance of being male or female
                if (_random.Next(2) == 0)
                    _animalType = AnimalType.ElkBull;
                else
                    _animalType = AnimalType.ElkCow;
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

        if (!_animalType.Equals(AnimalType.ElkBull)) return;

        _pregnant = true;
    }
    
    public override AnimalLifePeriod GetAnimalLifePeriodFromAge(int age)
    {
        if (age < 1) return AnimalLifePeriod.Calf;
        return age <= 2 ? AnimalLifePeriod.Adolescent : AnimalLifePeriod.Adult;
    }
    
}

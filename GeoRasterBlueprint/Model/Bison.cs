using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoRasterBlueprint.Model;

public class BisonAgent: IAgent<LandscapeLayer>, IPositionable, IAnimalAgent 
{
    // Constants
    public const double MaxHydration = 100.0;
    public const double MaxSatiety = 100.0;
    public const double DehydrationRate = 2.0;
    public const double StarvationRate = 1.5;
    public const double HoursToDeathWithoutWater = MaxHydration / DehydrationRate;
    public const double HoursToDeathWithoutFood = MaxSatiety / StarvationRate;
    public const int MaxAge = 175200; // Maximum age in hours (20 years)
    

    
    // Agent properties
    private double hydration;
    private double satiety;
    private int hoursWithoutWater;
    private int hoursWithoutFood;
    private int age;
    private bool isAlive;
    private int dailyEatingHours;


    // Initialize the Bison agent
    
    public void Init(LandscapeLayer layer)
    {
        hydration = MaxHydration;
        satiety = MaxSatiety;
        hoursWithoutWater = 0;
        hoursWithoutFood = 0;
        age = 0;
        isAlive = true;
        dailyEatingHours = 8;
    }
    
    
    
    // Update the state of the Bison agent
    public void UpdateState()
    {
        hydration -= DehydrationRate;
        satiety -= StarvationRate;
        hoursWithoutWater++;
        hoursWithoutFood++;
        age++;
    }
    
    public void MoveToWatersource()
    {
        // Implement the logic to move towards the nearest water source
    }

    public void DieOfOldAge()
    {
        // Handle Bison's death due to old age in the simulation
    }

    public void GiveBirth()
    {
        // Implement logic for Bison giving birth
    }

    public BisonAgent Mate(BisonAgent partner)
    {
        // Implement logic for Bison mating with a partner
        return new BisonAgent(); // Return a new Bison as a result of mating
    }
    

    private bool CanSeeVegetation()
    {
        // Implement the logic to check if Bison can see vegetation
        return false;
    }

    
    private void MoveTowardsGrazingArea()
    {
        // Implement the logic to move towards the nearest grazing area
    }

    private void Graze()
    {
        // Implement the logic for Bison grazing
    }
    private void DoRandomWalk()
    {
        // Simulate random movement within the simulation area
    }

    private void CheckSurvival()
    {
        if (hydration <= 0 || satiety <= 0)
        {
            MarkAsDead();
        }
    }
    private void MarkAsDead()
    {
        // Handle Bison's death due to dehydration or starvation in the simulation
    }
    
    // Method to update daily eating hours based on conditions
    public void UpdateDailyEatingHours()
    {
        // Adjust daily eating hours based on season
      
        if (IsSummer())
        {
            dailyEatingHours = 12; // Example value
        }
        else if (IsWinter())
        {
            dailyEatingHours = 8; // Example value
        }
      
    }

    // Check if it's summer
    private bool IsSummer()
    {
        // Implement logic to check the season
        //TODO
        return true; 
    }

    // Check if it's winter
    private bool IsWinter()
    {
        //TODO
        return false; 
    }

    
    
    public void Tick()
    {
        
        // Update agent state
        hydration -= DehydrationRate;
        satiety -= StarvationRate;
        hoursWithoutWater++;
        hoursWithoutFood++;
        age++;

        // Decide and perform actions
        if (age >= MaxAge)
        {
            DieOfOldAge();
        }
        else
        {
            if (hydration <= 0)
            {
                MoveToWatersource();
            }
            else if (satiety <= 0)
            {
                MoveTowardsGrazingArea();
            }
            else
            {
                if (CanSeeVegetation())
                {
                    Graze();
                }
                else
                {
                    DoRandomWalk();
                }
            }
        }

        // Check if the Bison has survived
        CheckSurvival();
    }
    
    

    public Guid ID { get; set; }
    public Position Position { get; set; }
    public int UniqueId { get; set; }
    public int Age { get;set; }
    public Point Location { get; set; }
    public float Health { get;  set;}
    public string Gender { get; set; }
    
    
    public void Move(Point destination)
    {
        // Implement logic for Bison migration to a specified destination
        throw new NotImplementedException();
    }

    public void Reproduce(IAnimalAgent partner)
    {
        throw new NotImplementedException();
    }

    public void Interact(IAnimalAgent agent)
    {
        throw new NotImplementedException();
    }

    public void Eat()
    {
        throw new NotImplementedException();
    }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public void Drink()
    {
        throw new NotImplementedException();
    }
}
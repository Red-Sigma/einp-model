using Mars.Components.Layers;
using System.IO;
using System;
using ServiceStack;

namespace GeoRasterBlueprint.Model;

public class TemperatureLayer: AbstractLayer
{
    private string[] temps;
    public TemperatureLayer()
    {
        String path = Directory.GetCurrentDirectory() + "\\Resources\\open-meteo-53.60N112.93W736m.csv";
        try
        {
            temps = File.ReadAllLines(path);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading temperature file: "+ e.Message);
        }
    }

    public double GetTemperature(long tick)
    {
        // + 4 because first 4 lines are discarded
        string[] parsed = temps[tick + 4].Split(',');
        return parsed[1].ToDouble();
    }
}
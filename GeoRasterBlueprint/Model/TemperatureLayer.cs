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
        Console.WriteLine(Directory.GetCurrentDirectory());
        String path = Directory.GetCurrentDirectory()+ "\\Resources\\open-meteo-53.60N112.93W736m.csv";
        temps = File.ReadAllLines(path);

    }

    public double GetTemperature(long tick)
    {
        // + 4 because first 4 lines are discarded
        string[] parsed = temps[tick + 4].Split(',');
        return parsed[1].ToDouble();
    }
}
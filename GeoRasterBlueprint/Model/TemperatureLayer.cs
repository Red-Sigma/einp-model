using Mars.Components.Layers;
using System.IO;
using System;
using NetTopologySuite.Geometries;
using ServiceStack;
using Mars.Interfaces.Environments;
namespace GeoRasterBlueprint.Model;

public class TemperatureLayer: RasterLayer
{
    private string[] _temps;
    
    /*
    #region Constructor
    public TemperatureLayer()
    {
        String path = Directory.GetCurrentDirectory() + "/Resources/open-meteo-53.60N112.93W736m.csv";
        try
        {
            _temps = File.ReadAllLines(path);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading temperature file: "+ e.Message);
        }
    }
    #endregion
    */
    
    public double GetTemperature(Mars.Interfaces.Environments.Position position)
    {
        return GetValue(position);
    } 
}
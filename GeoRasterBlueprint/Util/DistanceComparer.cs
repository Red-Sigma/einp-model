using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace GeoRasterBlueprint.Util;

public class DistanceComparer : IComparer<Coordinate> {
    
    private double CurrentX { get; }
    private double CurrentY { get; }

    public DistanceComparer(double currentX, double currentY)
    {
        CurrentX = currentX;
        CurrentY = currentY;
    }

    public int Compare(Coordinate coordinate1, Coordinate coordinate2)
    {
        double distance1 = CalculateDistance(coordinate1);
        double distance2 = CalculateDistance(coordinate2);

        return distance1.CompareTo(distance2);
    }

    private double CalculateDistance(Coordinate coordinate)
    {
        double deltaX = coordinate.X - CurrentX;
        double deltaY = coordinate.Y - CurrentY;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    
}

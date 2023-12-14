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
        DoRandomWalk(10);
        UpdateState();
    }
    
}

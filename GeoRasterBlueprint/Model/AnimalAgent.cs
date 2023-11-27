using NetTopologySuite.Geometries;

namespace GeoRasterBlueprint.Model;

public interface IAnimalAgent {
    int UniqueId { get; set; }
    int Age { get; set; }
    float Health { get; set; }
    string Gender { get; set; }
    void Move(Point destination);
    void Reproduce(IAnimalAgent partner);
    void Interact(IAnimalAgent agent);
    void Eat();
    void Die();
    void Drink();
}
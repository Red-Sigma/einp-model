using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace GeoRasterBlueprint.Model;

public class LandscapeLayer : AbstractLayer {
    
    #region Properties and Fields

    private List<Bison> Bisons { get; set; }

    [PropertyDescription(Name = "Perimeter")]
    public Perimeter Fence { get; set; }

    public GeoHashEnvironment<Bison> Environment { get; set; }

    #endregion
    
    /// <summary>
    /// The LandscapeLayer registers the animals in the runtime system. In this way, the tick methods
    /// of the agents can be executed later. Then the expansion of the simulation area is calculated using
    /// the raster layers described in config.json. An environment is created with this bounding box.
    /// </summary>
    /// <param name="layerInitData"></param>
    /// <param name="registerAgentHandle"></param>
    /// <param name="unregisterAgentHandle"></param>
    /// <returns>true if the agents where registered</returns>
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle) {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        // Calculate and expand extent
        var baseExtent = new Envelope(Fence.Extent.ToEnvelope());
        Console.WriteLine(new BoundingBox(baseExtent));

        // Create GeoHashEnvironment with the calculated extent
        Environment = GeoHashEnvironment<Bison>.BuildByBBox(new BoundingBox(baseExtent), 1000);

        var agentManager = layerInitData.Container.Resolve<IAgentManager>();
        Bisons = agentManager.Spawn<Bison, LandscapeLayer>().ToList();

        return Bisons.Count > 0;
    }
    
}

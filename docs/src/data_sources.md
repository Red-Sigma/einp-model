# Data sources

## Park
General Infos and official data [(click)](https://open.canada.ca/en)

## Animals
### Bison

- Number of Bison in the park
    - Sources:
        - [EINP Canada](https://parks.canada.ca/pn-np/ab/elkisland/nature/eep-sar/faq_bison)
        - [Parks Canada](https://open.canada.ca/data/en/organization/pc?q=Elk+Island+National+Park&keywords=Alberta&portal_type=dataset&collection=primary&sort=)
- Bison Behaviour
    - Sources: 
        - [EINP Canada](https://parks.canada.ca/pn-np/ab/elkisland/securite-safety/bison)
        - [National Geographic](https://kids.nationalgeographic.com/animals/mammals/facts/american-bison)
        - [US doi](https://www.doi.gov/blog/15-facts-about-our-national-mammal-american-bison)
        - [WorldWildLife](https://www.worldwildlife.org/stories/meet-the-bison-facts-about-america-s-iconic-species)
        - [National park trust](https://parktrust.org/blog/10-facts-about-bison/)

### Moose
- Number of Moose in the Park
    - Sources: 
        - [National Park](https://national-parks.org/canada/elk-island#:~:text=The%20estimates%20for%20these%20include,Exceeding%20over%20500%20deer)
        - [Parks Canada](https://open.canada.ca/data/en/organization/pc?q=Elk+Island+National+Park&keywords=Alberta&portal_type=dataset&collection=primary&sort=)
- Moose Behaviour
    - Sources: 
        - [National Geographic](https://www.nationalgeographic.com/animals/mammals/facts/moose)
        - [OneKindPlanet](https://www.onekindplanet.org/animal/moose/)
        - [NWF org](https://www.nwf.org/Educational-Resources/Wildlife-Guide/Mammals/Moose)
        - [Northern Ontario](https://northernontario.travel/sunset-country/interesting-facts-about-moose)
        - [a-z-Animals](https://a-z-animals.com/blog/incredible-moose-facts/)

### Elk
- Number of Elk in the Park
    - Sources: 
        - [National Park](https://national-parks.org/canada/elk-island#:~:text=The%20estimates%20for%20these%20include,Exceeding%20over%20500%20deer)
        - [Parks Canada](https://open.canada.ca/data/en/organization/pc?q=Elk+Island+National+Park&keywords=Alberta&portal_type=dataset&collection=primary&sort=)
- Elk Behaviour
    - Sources: 
        - [National Geographic](https://www.nationalgeographic.com/animals/mammals/facts/elk-1)
        - [Altina Wildlife](https://www.altinawildlife.com/wapiti/)
        - [Nature Canada](https://naturecanada.ca/news/blog/north-american-elk-wapiti/)
        - [BioKids](http://www.biokids.umich.edu/critters/Cervus_elaphus/)
        - [a-z-Animals](https://a-z-animals.com/animals/elk/)


### Vegetation layer

- NDVI
    - Satellite pictures: [LANDSAT 8](https://landsat.gsfc.nasa.gov/satellites/landsat-8/ )


We have used Normalized Difference Vegetation Index (NDVI) for Vegetation layer, since it is a simple indicator that can be used to analyse remote sensing measurements and assess whether the target area contains live green vegetation or not. It is calculated from the visible and near-infrared light reflected by vegetation. NDVI values range from -1 to 1, with higher values indicating healthier, more dense vegetation.

![Image](https://eos.com/wp-content/uploads/2020/07/plants.jpg.webp)

The formula for NDVI is:

    NDVI = (NIR - RED) \ (NIR + RED)

Where:

NIR represents the amount of near-infrared light reflected.
RED represents the amount of red light reflected.

All calculations and file format conversions are made in QGIS.

## Map/Geo features
Layers for our model (water layer, wood, roads etc.) were extracted from OpenStreetMap and converted in format supported by MARS. More info on feature extraction and how to query them on [OpenStreet Map Wiki](https://wiki.openstreetmap.org/wiki/Map_features#Street_parking_tagged_on_the_main_roadway_(see_Street_parking)).

## How to get additional raster data
To create a georeferenced raster layer of your simulation area, please see the [MARS](https://www.mars-group.org/docs/tutorial/intro#what-is-mars) documentation.

Raster layers can be queried, extracted and converted with [QGIS](https://qgis.org/en/site/) or at [Geojson](https://geojson.io/) website.

## Reference projects and papers

[MARS-Group-HAW/blueprint-georaster](https://github.com/MARS-Group-HAW/blueprint-georaster)
[MARS-Group-HAW/model-knp-elephant](https://github.com/MARS-Group-HAW/model-knp-elephant)

Jennifer M., Boyce Mark S. (2022). Bison and elk spatiotemporal interactions in Elk Island National Park. [(link)](https://www.frontiersin.org/articles/10.3389/fcosc.2022.937203/full)

Thomas Clemen, Ulfia A. Lenfers, Janus Dybulla, Sam M. Ferreira, Greg A. Kiker, Carola Martens, Simon Scheiter. (2021).
A cross-scale modeling framework for decision support on elephant management in Kruger National Park, South Africa.
Ecological Informatics. [(link)](https://www.sciencedirect.com/science/article/pii/S1574954121000571)

Bunting, Erin & Fullman, Timothy & Kiker, G. & Southworth, Jane. (2016). Utilization of the SAVANNA model to analyze future patterns of vegetation cover in Kruger National Park under changing climate. [(link)](https://geog.ufl.edu/2016/10/17/utilization-of-the-savanna-model-to-analyze-future-patterns-of-vegetation-cover-in-kruger-national-park-under-changing-climate/)


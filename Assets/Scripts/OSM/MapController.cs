using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapSettings))]
public class MapController : MonoBehaviour
{
    /*
     * handle "latitude" as X
     * handle "longitude" as Y
     * Of course this is not MercatorProjection, but will  look atural in a small range. <= I want to use as a game service.
     */
    BuildingMaker buildingMaker;
    RoadMaker roadMaker;


    MapSettings set;

    // OSM map data
    [SerializeField, TooltipAttribute("OSM map data (${MapName}.osm) located in Assets/OSM/Data folder")]
    public string OSMfileName="MapName.osm";




    void Awake(){
        buildingMaker=GetComponent<BuildingMaker>();
        roadMaker=GetComponent<RoadMaker>();
        set=GetComponent<MapSettings>();
    }

    void Start()
    {   
        //load .oms map and make buildings, roads
        MapReader map=new MapReader(OSMfileName); //map data is in mapReader.mapData
        Debug.Log("map " + OSMfileName + " loaded");

        //Debug.Log("Make Buildings and Roads");
        StartCoroutine(buildingMaker.Make(map,set));
        StartCoroutine(roadMaker.Make(map,set));
        
        //ShowMapData
        Debug.Log("\nLongitude: " + map.mapData.bounds.MinLon + " ~ " + map.mapData.bounds.MaxLon + " , Latitude: " + map.mapData.bounds.MinLat + " ~ " + map.mapData.bounds.MaxLat);
        Debug.Log("Centre: " + map.mapData.bounds.Centre);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

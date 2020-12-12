using System;
using System.Xml;
using UnityEngine;

public class OSMbounds: OSM_Func
{
    /*
     * Latitude
     * Longitude
    */
    public float MinLat{get;set;} 
    public float MaxLat{get;set;} 
    public float MinLon{get;set;}
    public float MaxLon{get;set;}
    public Vector3 Centre{get;set;}//Not "Center" but "Centre"!! Because .osm file is using "centre".

    // bounds: minlat="53.2770000" minlon="-9.0703000" maxlat="53.2893000" maxlon="-9.0567000"
    public OSMbounds(XmlNode node)
    {
        this.MinLat = GetAttribute<float>("minlat", node.Attributes);
        this.MaxLat = GetAttribute<float>("maxlat", node.Attributes);
        this.MinLon = GetAttribute<float>("minlon", node.Attributes);
        this.MaxLon = GetAttribute<float>("maxlon", node.Attributes); 
        this.Centre=new Vector3(MaxLon-MinLon,0,MaxLat-MinLat);
    }
}

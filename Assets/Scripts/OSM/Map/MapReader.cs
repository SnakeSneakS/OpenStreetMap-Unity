using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapReader : MonoBehaviour
{

    /*[HideInInspector]
    public Dictionary<ulong, OSMnode> nodes;

    [HideInInspector]
    public List<OSMway> ways;

    [HideInInspector]
    public OSMbounds bounds;*/

    public class MapData{//OMS MAP DATA
        public OSMbounds bounds{get;set;}
        public Dictionary<ulong, OSMnode> nodes{get;set;}
        public List<OSMway> ways{get;set;}
    }

    //this is a map data - 
    public MapData mapData;
    
    //this check if map data is ready 
    //public bool IsReady {get; private set; } //comment out



    public MapReader(string OSMfileName)//Constructor - perhaps heavy
    {   
        //MapData mapData=new MapData();
        this.mapData=new MapData();
        //Debug.Log("load file Assets/OSM/Data/"+OSMfileName);
        XmlDocument doc = new XmlDocument();
        doc.Load(new XmlTextReader("Assets/OSM/Data/" + OSMfileName));
        this.mapData.bounds = SetBounds(doc.SelectSingleNode("/osm/bounds"));
        this.mapData.nodes = SetNodes(doc.SelectNodes("/osm/node"));
        this.mapData.ways = SetWays(doc.SelectNodes("/osm/way"));
        
        //return mapData;
    }

    OSMbounds SetBounds(XmlNode xmlNode) 
    {
        //Debug.Log("SetBounds");
        OSMbounds bound;
        bound=new OSMbounds(xmlNode);
        return bound;
    }

    Dictionary<ulong, OSMnode> SetNodes(XmlNodeList xmlNodeList)
    {
        //Debug.Log("SetNodes");
        Dictionary<ulong, OSMnode> nodes=new Dictionary<ulong, OSMnode>();
        foreach (XmlNode n in xmlNodeList)
        {
            OSMnode node = new OSMnode(n);
            nodes[node.ID] = node;
        }
        return nodes;
    }

    List<OSMway> SetWays(XmlNodeList xmlNodeList)
    {
        //Debug.Log("SetWays");
        List<OSMway> ways=new List<OSMway>();

        foreach (XmlNode node in xmlNodeList)
        {
            OSMway way = new OSMway(node);
            ways.Add(way);
        }
        return ways;

    }

    

    
}
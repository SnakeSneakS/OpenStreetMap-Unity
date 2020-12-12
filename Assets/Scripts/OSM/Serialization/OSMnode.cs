using System;
using System.Xml;

public class OSMnode: OSM_Func
{
    public ulong ID{get;set;}
    public float Latitude{get;set;}
    public float Longitude{get;set;}
    public bool Visible{get;set;}

    public OSMnode(XmlNode node)
    {
        this.ID = GetAttribute<ulong>("id", node.Attributes);
        this.Latitude = GetAttribute<float>("lat", node.Attributes);
        this.Longitude = GetAttribute<float>("lon", node.Attributes);
        this.Visible = GetAttribute<bool>("visible", node.Attributes);
    }

}


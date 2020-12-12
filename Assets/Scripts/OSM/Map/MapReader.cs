using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    /*
     * handle "latitude" as X
     * handle "longitude" as Y
     * Of course this is not MercatorProjection, but will  look atural in a small range. <= I want to use as a game service.
     */


    //nodesとwaysに保存する。
    [HideInInspector]
    public Dictionary<ulong, OSMnode> nodes;

    [HideInInspector]
    public List<OSMway> ways;

    [HideInInspector]
    public OSMbounds bounds;
    
    [SerializeField, TooltipAttribute("OSM map data (${MapName}.osm) located in Assets/OSM/Data folder")]
    public string OSMfileName="MapName.osm";

    public bool IsReady {get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        nodes = new Dictionary<ulong, OSMnode>();
        ways = new List<OSMway>();

        /*var txtAsset = Resources.Load<TextAsset>("Assets/OSM/Data/" + OSMfileName);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(txtAsset.text);*/

        XmlDocument doc = new XmlDocument();
        doc.Load(new XmlTextReader("Assets/OSM/Data/" + OSMfileName));
        SetBounds(doc.SelectSingleNode("/osm/bounds"));
        SetNodes(doc.SelectNodes("/osm/node"));
        SetWays(doc.SelectNodes("/osm/way"));

        IsReady = true;
    /*}

    void Update()
    {*/
        foreach ( OSMway w in ways)
        {
            if (w.Visible)
            {
                /*
                 * Draw Line Between Node in OMSWay
                 */

                Color c = Color.cyan; // cyan for buildings
                if (!w.IsBoundary) c = Color.red; // red for roads

                for (int i =1; i < w.NodeIDs.Count; i++)
                {
                    OSMnode p1 = nodes[w.NodeIDs[i - 1 ]];
                    OSMnode p2 = nodes[w.NodeIDs[i]];

                    //Vector3 v1 = p1 - bounds.Centre;
                    Vector3 v1=new Vector3(p1.Longitude,0,p1.Latitude);
                    //Vector3 v2 = p2 - bounds.Centre;
                    Vector3 v2=new Vector3(p2.Longitude,0,p2.Latitude);

                    Debug.DrawLine(v1, v2, c);
                }

            }
        }
    }

    void SetWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            OSMway way = new OSMway(node);
            ways.Add(way);
        }

    }

    void SetNodes(XmlNodeList xmlNodeList)
    {
         foreach (XmlNode n in xmlNodeList)
        {
            OSMnode node = new OSMnode(n);
            nodes[node.ID] = node;
        }
    }

    void SetBounds(XmlNode xmlNode) 
    {
        bounds = new OSMbounds(xmlNode);
    }
}
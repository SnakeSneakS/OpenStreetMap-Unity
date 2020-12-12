using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class DeserializeOSM : MonoBehaviour
{
	/*
	 * Don't use this!!
	 * Of course this is one way!!
	 */

    private XmlDocument doc = new XmlDocument();
	private List<Transform> wayObjects = new List<Transform>();

	[SerializeField]
	private float x;
	[SerializeField]
	private float y;
	[SerializeField]
	private float boundsX = 34;
	[SerializeField]
	private float boundsY = -118;
	[SerializeField]
	private string mapName = "AroundWaseda.osm";

	public struct OSM_BasicData{

	}

    public struct OSM_Way {
	    public ulong id;
	    public List<ulong> wnodes;

		public bool isBuilding, isBoundary,isRoad;

		public float height;


	    public OSM_Way(ulong ID) {
		    id = ID;
		    wnodes = new List<ulong>();
			this.isBuilding = false;
			this.isBoundary = false;
			this.isRoad=false;
			height=0.0f;
	    }
    }

    public struct OSM_Node {
	    public ulong id;
	    public float lat, lon;
		public bool visible;

	    public OSM_Node(ulong ID, float LAT, float LON) {
		    id = ID;
		    lat = LAT;
		    lon = LON;
			visible=true;
            //Debug.Log("ID: " + id + ", LAT: " + lat + ", LON: " + lon);
	    }
    }

	enum OSM_Tag{//https://wiki.openstreetmap.org/wiki/Map_features //tagのうちどれを使うか

		//railway,
		highway,
		natural //tree
		//entrance
		//building,
		//waterway
		//shop
		//geological
	}
	


	private List<OSM_Node> nodes = new List<OSM_Node>();
	private List<OSM_Way> ways = new List<OSM_Way>();


	
	private void Start () {
		doc.Load(new XmlTextReader("Assets/OSM/Data/" + mapName));

		XmlNodeList elemList = doc.GetElementsByTagName("node");
		for (int i = 0; i < elemList.Count; i++) {
			nodes.Add(new OSM_Node(ulong.Parse(elemList[i].Attributes["id"].InnerText),
				float.Parse(elemList[i].Attributes["lat"].InnerText),
				float.Parse(elemList[i].Attributes["lon"].InnerText)
			));
		}

		XmlNodeList wayList = doc.GetElementsByTagName("way");
		int ct = 0;
		foreach (XmlNode node in wayList) {			
			XmlNodeList wayNodes = node.ChildNodes;
			ways.Add(new OSM_Way(ulong.Parse(node.Attributes["id"].InnerText)));
			foreach(XmlNode nd in wayNodes) {//node in way
				if(nd.Attributes[0].Name == "ref") {
					ways[ct].wnodes.Add(ulong.Parse(nd.Attributes["ref"].InnerText));
					//Debug.Log(ways[ct].wnodes.Count);
				}
			}
			/*foreach(XmlTag tag in wayNodes){//tag in way
				if(node.Attributes[1].Name=="k"){
					ways[ct].wnodes.Add();
				}
			}*/
			ct++;
		}

		for (int i = 0; i < ways.Count; i++) {
			wayObjects.Add(new GameObject("wayObject"+ ways[i].id).transform);
			wayObjects[i].gameObject.AddComponent<LineRenderer>();
			wayObjects[i].GetComponent<LineRenderer>().SetWidth(0.05f,0.05f);
			wayObjects[i].GetComponent<LineRenderer>().SetVertexCount(ways[i].wnodes.Count);
			for (int j = 0; j < ways[i].wnodes.Count; j++) {				
				foreach (OSM_Node nod in nodes) {
					if (nod.id == ways[i].wnodes[j]) {
						x = nod.lat;
						y = nod.lon;
					}
				}
				wayObjects[i].GetComponent<LineRenderer>().SetPosition(j, new Vector3((x-boundsX)*100,0,(y-boundsY)*100));
			}
		}	

        //Debug.Log(nodes);	
	}
}

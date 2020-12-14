using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

class RoadMaker : MonoBehaviour
{
    [System.SerializableAttribute]
    public class RoadMaterial{
        public Material MainHighway;
        public Material SubHighway;
        public Material PathHighway;
        public Material ElseHighway;
    }

    [SerializeField]
    public RoadMaterial roadMaterial=new RoadMaterial();
    

    /*
     * https://wiki.openstreetmap.org/wiki/Key:highway 
     * already   way: highway
     * remaining way: footway, sidewalk, cycleway, etc.
     */
    class RoadClassification{
        public List<string> MainHighwayValues = new List<string>(){"motorway","trunk","primary","secondary","tertiary","unclassified","residential","motorway_link","trunk_link","primary_link","secondary_link","tertiary_link"}; //Apply MainHighwayMaterial //Classified as "Roads" or "Link roads" in osm
        public List<string> SubHighwayValues = new List<string>(){"living_street","service","pedestrian","track","bus_guideway","escape","raceway","road"}; //Apply MainHighwayMaterial //Classified as "Special road types" in osm
        public List<string> PathHighwayValues = new List<string>(){"footway","bridleway","steps","corridor","path","cycleway"}; //Apply PathHighwayMaterial //Classified as "Paths" or "When cycleway is drawn as its own way" in osm //remaining: "Lifecycle" etc
    }
    RoadClassification roadClassification=new RoadClassification();

   


    public IEnumerator Make(MapReader map, MapSettings set, GameObject parentObj)
    {
        if(map.mapData==null){
            Debug.Log("No Map Data");
            yield break;
        }

        /*while (!map.IsReady)
        {
            yield return null;
        }*/

        foreach (var way in map.mapData.ways.FindAll((w) => { return w.Highway != ""; }))//foreach in way where way.Highway!="", This means some kind of Highway
        {   
            GameObject go = new GameObject();
            Vector3 localOrigin = GetCentre(map,way);
            go.transform.position = (localOrigin - map.mapData.bounds.Centre)*set.mag_h;//magnitude
            go.transform.parent=parentObj.transform;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            
            //classfy highway and assgin each material
            if(roadClassification.MainHighwayValues.Contains(way.Highway)){
                mr.material = roadMaterial.MainHighway;
            }else if(roadClassification.SubHighwayValues.Contains(way.Highway)){
                mr.material = roadMaterial.SubHighway;
            }else if(roadClassification.PathHighwayValues.Contains(way.Highway)){
                mr.material = roadMaterial.PathHighway;
            }else{
                mr.material = roadMaterial.ElseHighway;
            }
            

            List<Vector3> vectors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();

            bool isFirstRoadVector=true;
            Vector3 v1_old=Vector3.zero;
            Vector3 v2_old=Vector3.zero;
            Vector3 v3_old=Vector3.zero;
            Vector3 v4_old=Vector3.zero; 

            int wayCount=way.NodeIDs.Count;
            for (int i = 1; i < wayCount; i++)
            {
                OSMnode p1 = map.mapData.nodes[way.NodeIDs[i - 1]];
                OSMnode p2 = map.mapData.nodes[way.NodeIDs[i]];

                Vector3 s1 = new Vector3(p1.Longitude,0,p1.Latitude) - localOrigin;
                Vector3 s2 = new Vector3(p2.Longitude,0,p2.Latitude) - localOrigin;
                
                //magnitude horizontal map 
                s1.x*=set.mag_h; s1.z*=set.mag_h;
                s2.x*=set.mag_h; s2.z*=set.mag_h;
                
                Vector3 diff = (s2 - s1).normalized;
                var cross = Vector3.Cross(diff, Vector3.up) * set.road_w; // width of road

                /*
                Shape: 

                    //normal squad(v1, v2, v3, v4)
                    v3  v4

                    v1  v2

                    //here squad(v1_old, v2_old, v3_old, v4_old), v1(v2) is calculated as (v1(v2) + v3_old)/2
                    (v3)    (v4)
                    v1      v2
                    v1_old  v2_old
                    
                */

                /*

                    Here: Don't show back side of road.

                */

                Vector3 v1 = s1 - cross;
                Vector3 v2 = s1 + cross;
                Vector3 v3 = s2 - cross;
                Vector3 v4 = s2 + cross;

                if(isFirstRoadVector){//just set v?_old (? is between 1 and 4) first
                    isFirstRoadVector=false;
                    v1_old=v1;
                    v2_old=v2;
                    v3_old=v3;
                    v4_old=v4;
                    vectors.Add(v1); vectors.Add(v2);
                    normals.Add(-Vector3.up); normals.Add(-Vector3.up);
                    uvs.Add(new Vector2(0,0)); uvs.Add(new Vector2(1,0));
                    continue;
                }else{
                    v1=(v1+v3_old)/2;
                    v2=(v2+v4_old)/2;
                }

                //precise vectors are "v1_old. v2_old, v1, v4". So add them
                /*vectors.Add(v1_old); vectors.Add(v2_old);*/ vectors.Add(v1); vectors.Add(v2);

                if(i%2==0){
                    uvs.Add(new Vector2(0,1));
                    uvs.Add(new Vector2(1,1));
                }else{
                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(1,0));
                }
                /*
                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(1,0));
                uvs.Add(new Vector3(0,1));
                uvs.Add(new Vector3(1,1));
                */

                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                /*normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);*/
                 
                int idx1, idx2,idx3, idx4; // index values
                int count=vectors.Count;
                idx4 = count - 1; //v2
                idx3 = count - 2; //v1
                idx2 = count - 3; //v2_old
                idx1 = count - 4; //v1_old

               
                indices.Add(idx1); indices.Add(idx2); indices.Add(idx3);  // first triangle v1, v3, v2 //one side
                indices.Add(idx3); indices.Add(idx2); indices.Add(idx4); // second triangle v3, v4, v2 //one side
                /*indices.Add(idx1); indices.Add(idx3); indices.Add(idx2); // third triangle v2, v3, v1 //the other side 
                indices.Add(idx3); indices.Add(idx2); indices.Add(idx4); // fourth triangle v2, v4, v3 //the other side*/
                
                if(i==wayCount-1){//if last: add last triangle v1, v2, v3, v4
                    /*vectors.Add(v1_old); vectors.Add(v2_old);*/ vectors.Add(v3); vectors.Add(v4);

                    uvs.Add(new Vector2(0,1)); uvs.Add(new Vector2(1,1)); /*uvs.Add(new Vector3(0,1)); uvs.Add(new Vector3(1,1));*/

                    normals.Add(-Vector3.up); normals.Add(-Vector3.up); /*normals.Add(-Vector3.up); normals.Add(-Vector3.up);*/

                    count=vectors.Count;
                    idx4 = count - 1; //v2
                    idx3 = count - 2; //v1
                    idx2 = count - 3; //v2_old
                    idx1 = count - 4; //v1_old

                    indices.Add(idx1); indices.Add(idx2); indices.Add(idx3);  // first triangle v1, v3, v2 //one side
                    indices.Add(idx3); indices.Add(idx2); indices.Add(idx4); // second triangle v3, v4, v2 //one side
                    /*indices.Add(idx1); indices.Add(idx3); indices.Add(idx2); // third triangle v2, v3, v1 //the other side 
                    indices.Add(idx3); indices.Add(idx2); indices.Add(idx4); // fourth triangle v2, v4, v3 //the other side*/
                }


                //ready for next triangles
                v1_old=v1; v2_old=v2; v3_old=v3; v4_old=v4;

                
            }


            mf.mesh.vertices = vectors.ToArray();
            mf.mesh.normals = normals.ToArray();
            mf.mesh.triangles = indices.ToArray();
            mf.mesh.uv = uvs.ToArray();

            yield return null;
        }

    }


    private Vector3 GetCentre(MapReader map ,OSMway way)
    {
        float lat=0.0f;
        float lon=0.0f;
        foreach (var id in way.NodeIDs)
        {
            lat+=map.mapData.nodes[id].Latitude;
            lon+=map.mapData.nodes[id].Longitude;
        }

        Vector3 total = new Vector3(lon,0,lat);

        return total / way.NodeIDs.Count;
    }
         
}
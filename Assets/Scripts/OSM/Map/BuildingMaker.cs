using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingMaker : MonoBehaviour
{
    public Material building;


    public IEnumerator Make(MapReader map, MapSettings set, GameObject parentObj, Vector3 buildingPos)
    {
        if(map.mapData==null){
            Debug.Log("No Map Data");
            yield break;
        }

        GameObject buildingObj = new GameObject();
        buildingObj.transform.parent=parentObj.transform;
        buildingObj.name="Buildings";

        foreach (var way in map.mapData.ways.FindAll((w) => { return w.IsBuilding && w.NodeIDs.Count > 1; }))
        {
            GameObject go = new GameObject();
            Vector3 localOrigin = GetCentre(map,way);
            Vector3 TransformPos=localOrigin - map.mapData.bounds.Centre;
            go.transform.parent=buildingObj.transform;
            go.name=way.Name;

            //magnitude horizontal 
            TransformPos.x*=set.mag_h; TransformPos.z*=set.mag_h;

            go.transform.position = TransformPos;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            mr.material = building;

            List<Vector3> vectors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int i = 1; i < way.NodeIDs.Count; i++)
            {
                OSMnode p1 = map.mapData.nodes[way.NodeIDs[i - 1]];
                OSMnode p2 = map.mapData.nodes[way.NodeIDs[i]];

                Vector3 v1 = new Vector3(p1.Longitude,0,p1.Latitude) - localOrigin;
                Vector3 v2 = new Vector3(p2.Longitude,0,p2.Latitude) - localOrigin;

                //magnitude horizontal  
                v1.x*=set.mag_h; v1.z*=set.mag_h;
                v2.x*=set.mag_h; v2.z*=set.mag_h;

                Vector3 v3 = v1 + new Vector3(0, way.Height, 0);
                Vector3 v4 = v2 + new Vector3(0, way.Height, 0);

                //magnitude vertical 
                v3.y*=set.mag_v; 
                v4.y*=set.mag_v; 
                

                vectors.Add(v1);
                vectors.Add(v2);
                vectors.Add(v3);
                vectors.Add(v4);

                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                
                // index values
                int idx1, idx2,idx3, idx4;
                int count=vectors.Count;
                idx4 = count - 1;
                idx3 = count - 2;
                idx2 = count - 3;
                idx1 = count - 4;

                
                indices.Add(idx1); indices.Add(idx3); indices.Add(idx2); // first triangle v1, v3, v2 //one side
                indices.Add(idx2); indices.Add(idx3); indices.Add(idx1); // second triangle v2, v3, v1 //the other side  
                
                indices.Add(idx3); indices.Add(idx4); indices.Add(idx2); // third triangle v3, v4, v2 //one side
                indices.Add(idx2); indices.Add(idx4); indices.Add(idx3); // fourth triangle v2, v4, v3 //the other side
            }

            //roof
            if(/*way.IsBuilding*/true){
                int idx0=-1; //FirstRoofVector, one point of triangle
                int idx1=-1; //one point of triangle - old Vector
                int idx2=-1; //one point of triangle - new Vector
                for(int i=0;i<vectors.Count;i++){
                    if(vectors[i].y>0){ //Check if this is roof
                        if(idx0==-1){ //first point of triangle (not change)
                            idx0=i;
                        }else{
                            if(idx1==-1){ //if old vector is not assgined
                                idx1=i;
                            }else{ //both idx0 and idx1 is ready. Let's start to make triangle!
                                idx2=i;
                                indices.Add(idx1);indices.Add(idx0);indices.Add(idx2);//first triangle - one side
                                indices.Add(idx2);indices.Add(idx0);indices.Add(idx1);//first triangle - the other side
                                idx1=i;
                            }
                        }             
                    }
                }
            }
            

            mf.mesh.vertices = vectors.ToArray();
            mf.mesh.normals = normals.ToArray();
            mf.mesh.triangles = indices.ToArray();

            yield return null;
        }

        buildingObj.transform.position=buildingPos;

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

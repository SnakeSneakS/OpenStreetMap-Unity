using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[RequireComponent(typeof(MapSettings))]
class RoadMaker : InfrastructureBehaviour
{
    public Material roadMaterial;
    
    private MapSettings set;

    void Awake(){
        set=this.gameObject.GetComponent<MapSettings>();
    }

    IEnumerator Start()
    {
        while (!map.IsReady)
        {
            yield return null;
        }

        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {   
            GameObject go = new GameObject();
            Vector3 localOrigin = GetCentre(way);
            go.transform.position = (localOrigin - map.bounds.Centre)*set.mag_h;//magnitude

            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            mr.material = roadMaterial;

            List<Vector3> vectors = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();

            for (int i = 1; i < way.NodeIDs.Count; i++)
            {
                OSMnode p1 = map.nodes[way.NodeIDs[i - 1]];
                OSMnode p2 = map.nodes[way.NodeIDs[i]];

                Vector3 s1 = new Vector3(p1.Longitude,0,p1.Latitude) - localOrigin;
                Vector3 s2 = new Vector3(p2.Longitude,0,p2.Latitude) - localOrigin;
                
                //magnitude horizontal map 
                s1.x*=set.mag_h; s1.z*=set.mag_h;
                s2.x*=set.mag_h; s2.z*=set.mag_h;
                
                Vector3 diff = (s2 - s1).normalized;
                var cross = Vector3.Cross(diff, Vector3.up) * set.road_w; // 0.05 meters - width of lane

                Vector3 v1 = s1 + cross;
                Vector3 v2 = s1 - cross;
                Vector3 v3 = s2 + cross;
                Vector3 v4 = s2 - cross;

                vectors.Add(v1);
                vectors.Add(v2);
                vectors.Add(v3);
                vectors.Add(v4);

                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(1,0));
                uvs.Add(new Vector3(0,1));
                uvs.Add(new Vector3(1,1));

                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                
                // index values
                int idx1, idx2,idx3, idx4;
                idx4 = vectors.Count - 1;
                idx3 = vectors.Count - 2;
                idx2 = vectors.Count - 3;
                idx1 = vectors.Count - 4;

                // first triangle v1, v3, v2
                indices.Add(idx1);
                indices.Add(idx3);
                indices.Add(idx2);

                // second triangle v3, v4, v2
                indices.Add(idx3);
                indices.Add(idx4);
                indices.Add(idx2);

                // third triangle v2, v3, v1
                indices.Add(idx2);
                indices.Add(idx3);
                indices.Add(idx1);

                // fourth triangle v2, v4, v3
                indices.Add(idx2);
                indices.Add(idx4);
                indices.Add(idx3);
            }

            mf.mesh.vertices = vectors.ToArray();
            mf.mesh.normals = normals.ToArray();
            mf.mesh.triangles = indices.ToArray();
            mf.mesh.uv = uvs.ToArray();

            yield return null;
        }

    }
         
}
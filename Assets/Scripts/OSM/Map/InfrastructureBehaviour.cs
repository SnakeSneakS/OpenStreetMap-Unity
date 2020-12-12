using UnityEngine;

[RequireComponent(typeof(MapReader))]
public abstract class InfrastructureBehaviour : MonoBehaviour
{
    public MapReader map;

    void Awake()
    {
        map = GetComponent<MapReader>();
    }

    protected Vector3 GetCentre(OSMway way)
    {
        float lat=0.0f;
        float lon=0.0f;
        foreach (var id in way.NodeIDs)
        {
            lat+=map.nodes[id].Latitude;
            lon+=map.nodes[id].Longitude;
        }

        Vector3 total = new Vector3(lat,0,lon);

        return total / way.NodeIDs.Count;
    }
}
using UnityEngine;
using ProtoCasual.Core.Interfaces;

public class TrackGenerator : MonoBehaviour, IMapGenerator
{
    [SerializeField] private GameObject[] trackPieces;
    [SerializeField] private int length = 30;

    private GameObject mapParent;

    public void GenerateMap(MapConfig config)
    {
        ClearMap();
        
        if (config != null && config.tilePrefabs != null && config.tilePrefabs.Length > 0)
        {
            // Use config prefabs if provided
            GenerateFromConfig(config);
        }
        else
        {
            // Use procedural generation with default track pieces
            Generate();
        }
    }

    private void GenerateFromConfig(MapConfig config)
    {
        Vector3 pos = Vector3.zero;
        
        for (int i = 0; i < length; i++)
        {
            var piece = config.tilePrefabs[Random.Range(0, config.tilePrefabs.Length)];
            Instantiate(piece, pos, Quaternion.identity, transform);
            pos += Vector3.forward * 10f;
        }
    }

    public void ClearMap()
    {
        if (mapParent != null)
        {
            Destroy(mapParent);
            mapParent = null;
        }
        
        // Clear any procedurally generated pieces
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void RegenerateMap()
    {
        ClearMap();
        Generate();
    }

    public void Generate()
    {
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < length; i++)
        {
            var piece = trackPieces[Random.Range(0, trackPieces.Length)];
            Instantiate(piece, pos, Quaternion.identity, transform);
            pos += Vector3.forward * 10f;
        }
    }
}

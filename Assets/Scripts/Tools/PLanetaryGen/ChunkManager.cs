using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public int range;
    public ChunkQueue chunkQueue;
    public PlanetGeneration planet;
    private List<Vector3Int> coordsToRemove;
    public Queue<GameObject> freeObjects;



    private void Start()
    {
        freeObjects = new Queue<GameObject> ();
        coordsToRemove = new List<Vector3Int>();
        setRenderDistance();
    }
    public void setRenderDistance()
    {
        foreach(var obj in freeObjects)
        {
            Destroy(obj);
        }

        //filter out the ones in range
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    var chunkObject = new GameObject();
                    chunkObject.layer = 6;
                    chunkObject.tag = "Planet";
                    chunkObject.name = "free";
                    var meshFilter = chunkObject.AddComponent<MeshFilter>();
                    var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
                    var meshCollider = chunkObject.AddComponent<MeshCollider>();
                    meshRenderer.material = planet.chunkMaterial;
                    var ChunkMeshGen = chunkObject.AddComponent<ChunkMeshGenerator>();

                    freeObjects.Enqueue(chunkObject);
                }
            }
        }
    }
    public void OnDrawGizmos()
    {
        Vector3Int playerChunkPos = Vector3Int.FloorToInt(transform.position);
        Vector3Int chunkSize = planet.chunkSize;



        //filter out the ones in range
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    Vector3Int chunkCoord = playerChunkPos;
                    // Calculate the chunk coordinate by flooring the division of player position by chunk size
                    Vector3Int b = Vector3Int.FloorToInt(new Vector3Int(chunkCoord.x / chunkSize.x, chunkCoord.y / chunkSize.y, chunkCoord.z / chunkSize.z));
                    Vector3Int c = b * chunkSize;
                    c += new Vector3Int(x * chunkSize.x, y * chunkSize.x, z * chunkSize.x);
                    if (c.x < 0 || c.y < 0 || c.z < 0)
                    {
                        break;
                    }
                    if (PlanetData.IsChunkNearValid(c, planet.chunkSize.x, planet.terrainSurface, planet) == true)
                    {
                        Gizmos.color = Color.green;

                        Gizmos.DrawWireCube(c + chunkSize / 2, chunkSize);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(c + chunkSize / 2, chunkSize);
                    }




                }
            }
        }
    }
    private void Update()
    {
        coordsToRemove.Clear();
        Vector3Int playerChunkPos = Vector3Int.FloorToInt(transform.position);
        Vector3Int chunkSize = planet.chunkSize;
        int rangeInChunks = Mathf.CeilToInt(range / planet.chunkSize.x);


        //Get all activeChunks
        foreach (KeyValuePair<Vector3Int, GameObject> activeChunk in PlanetData.activeChunks)
        {
            coordsToRemove.Add(activeChunk.Key);
        }
        //filter out the ones in range
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    Vector3Int chunkCoord = playerChunkPos;
                    // Calculate the chunk coordinate by flooring the division of player position by chunk size
                    Vector3Int b = Vector3Int.FloorToInt(new Vector3Int(chunkCoord.x / chunkSize.x, chunkCoord.y / chunkSize.y, chunkCoord.z / chunkSize.z));
                    Vector3Int c = b * chunkSize;
                    c += new Vector3Int(x * chunkSize.x, y * chunkSize.x, z * chunkSize.x);
                    if (c.x < 0 || c.y < 0 || c.z < 0)
                    {
                        break;
                    }
                    if (PlanetData.IsChunkNearValid(c,planet.chunkSize.x,planet.terrainSurface,planet) == false)
                    {
                        if (!PlanetData.activeChunks.ContainsKey(c) && freeObjects.Count > 0)
                        {
                            Debug.Log("Nigga It Spawned from Here");
                            chunkQueue.CreateChunk(c, freeObjects.Dequeue());

                        }
                    }
                    else
                    {

                    }
                  

                    coordsToRemove.Remove(c);

                }
            }
        }

        //Destroy Chunks That Are Not InRange
        foreach (Vector3Int coord in coordsToRemove)
        {
            if (PlanetData.activeChunks.TryGetValue(coord, out GameObject chunkToDelete))
            {

                chunkQueue.DequeueChunk(chunkToDelete.GetComponent<ChunkMeshGenerator>().chunkRefrence);
                //Destroy(chunkToDelete);
                chunkToDelete.GetComponent<MeshFilter>().mesh = null;
                chunkToDelete.name = "free";
                freeObjects.Enqueue(chunkToDelete);
                PlanetData.activeChunks.Remove(coord);
            }
        }


    }


}
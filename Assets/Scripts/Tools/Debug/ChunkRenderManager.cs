using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ChunkRenderManager : MonoBehaviour
{
    public bool renderGizmo;
    public int range;
    public PlanetGeneration planet;
    public ChunkRenderer chunkRenderer;

    private List<Vector3Int> coordsToRemove;
    public Queue<GameObject> freeObjects;

    public static Dictionary<Vector3Int, GameObject> activeChunks;
    public Camera mainCam;
    public void Start()
    {
        mainCam = Camera.main;
        freeObjects = new Queue<GameObject>();
        coordsToRemove = new List<Vector3Int>();
        activeChunks = new Dictionary<Vector3Int, GameObject>();
        SetRenderDistance();
    }

    public void SetRenderDistance()
    {
        foreach (var obj in freeObjects)
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
                    chunkObject.AddComponent<ChunkHolder>();
                    chunkObject.transform.localScale = new Vector3(planet.sizeOverAll, planet.sizeOverAll, planet.sizeOverAll);
                    freeObjects.Enqueue(chunkObject);
                }
            }
        }
    }


    private void Update()
    {
        coordsToRemove.Clear();
        Vector3Int playerChunkPos = Vector3Int.FloorToInt(transform.position) / planet.sizeOverAll;
        Vector3Int chunkSize = planet.chunkSize;
   


       
        foreach (KeyValuePair<Vector3Int, GameObject> activeChunk in activeChunks)
        {
            coordsToRemove.Add(activeChunk.Key);
        }

        
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
                    bool isEmpty = true;
                    if (planet.useFrustum)
                    {
                        if (PlanetData.IsChunkInFrustum(mainCam, c*planet.sizeOverAll + chunkSize * planet.sizeOverAll / 2, chunkSize * planet.sizeOverAll, planet.chunkBuffer))
                        {
                            if (PlanetData.planetData.ContainsKey(c))
                            {
                                isEmpty = PlanetData.planetData[c].isEmpty;
                            }
                            else
                            {
                                isEmpty = PlanetData.IsChunkEmpty(c, planet.chunkSize.x, planet.terrainSurface, planet);
                            }

                            if (isEmpty == false)
                            {
                                if (!activeChunks.ContainsKey(c) && freeObjects.Count > 0)
                                {
                                    var gmObject = freeObjects.Dequeue();
                                    activeChunks.TryAdd(c, gmObject);
                                    if (PlanetData.planetData.TryGetValue(c, out Chunk j))
                                    {
                                        gmObject.GetComponent<ChunkHolder>().chunkReference = j;
                                    }
                                    
                                    bool a = false;
                                    if (PlanetData.planetData.ContainsKey(c))
                                    {
                                        if (PlanetData.planetData[c].mesh != null)
                                        {
                                            StartCoroutine(chunkRenderer.RenderChunk(c, gmObject,x =>  a = x));
                                        }
                                        else
                                        {
                                            chunkRenderer.EnqueueChunk(gmObject);
                                        }
                                    }
                                    else
                                    {
                                        chunkRenderer.EnqueueChunk(gmObject);
                                    }
                                        
                                }
                            }

                            coordsToRemove.Remove(c);
                        }
                    }
                    else
                    {
                            if (PlanetData.planetData.ContainsKey(c))
                            {
                                isEmpty = PlanetData.planetData[c].isEmpty;
                            }
                            else
                            {
                                isEmpty = PlanetData.IsChunkEmpty(c, planet.chunkSize.x, planet.terrainSurface, planet);
                            }

                            if (isEmpty == false)
                            {
                                if (!activeChunks.ContainsKey(c) && freeObjects.Count > 0)
                                {
                                var gmObject = freeObjects.Dequeue();
                                activeChunks.TryAdd(c, gmObject);
                                if (PlanetData.planetData.TryGetValue(c, out Chunk j))
                                {
                                    gmObject.GetComponent<ChunkHolder>().chunkReference = j;
                                }

                                bool a = false;
                                if (PlanetData.planetData.ContainsKey(c))
                                {
                                    if (PlanetData.planetData[c].mesh != null)
                                    {
                                        StartCoroutine(chunkRenderer.RenderChunk(c, gmObject, x => a = x));
                                    }
                                    else
                                    {
                                        chunkRenderer.EnqueueChunk(gmObject);
                                    }
                                }
                                else
                                {
                                    chunkRenderer.EnqueueChunk(gmObject);
                                }

                                }
                            }

                            coordsToRemove.Remove(c);
                        
                    }
                   

                  

                }
            }
        }

        //Destroy Chunks That Are Not InRange
        foreach (Vector3Int coord in coordsToRemove)
        {
            if (activeChunks.TryGetValue(coord, out GameObject chunkToDelete))
            {
                
                if (chunkRenderer.chunksList.Contains(chunkToDelete))
                {
                    chunkRenderer.DequeueChunk(chunkToDelete);
                }
              
                    chunkToDelete.GetComponent<MeshFilter>().mesh = null;
                

                
                    chunkToDelete.name = "freeK";
              
                freeObjects.Enqueue(chunkToDelete);
                activeChunks.Remove(coord);
            }
        }
        foreach(var c in freeObjects)
        {
            c.GetComponent<MeshFilter>().mesh = null;
        }


    }

    public void OnDrawGizmos()
    {
        if (renderGizmo == false)
        {
            return;
        }
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
                    if (PlanetData.IsChunkNearValid(c, planet.chunkSize.x, planet.terrainSurface, planet) == true )
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(c + chunkSize / 2, chunkSize.x / 2);
                       
                    }
                    else
                    {

                        Gizmos.color = Color.green;

                        Gizmos.DrawWireSphere(c + chunkSize / 2, 2f);
                    }




                }
            }
        }
    }
}

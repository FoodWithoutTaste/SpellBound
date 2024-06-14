using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;


public class ChunkMeshGenerator : MonoBehaviour
{
    public Chunk chunkRefrence;
    public Vector3Int chunkPos;
    //DefaulStuff
    private Vector3Int size;
    private float terrainSurface;
    private PlanetGeneration planet;

    //Generation Stuff
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    public float[,,] blocks;



    public void Initialization(PlanetGeneration planet)
    {
        this.planet = planet;
        size = planet.chunkSize;
        terrainSurface = planet.terrainSurface;
    }

    //Return Chunk Mesh
    public IEnumerator CreateChunkMesh(Vector3Int chunkPos, float[,,] data, System.Action<Mesh> callback)
    {
        vertices.Clear();
        triangles.Clear();
        blocks = data;
        Debug.Log("marching");
        Task t = Task.Factory.StartNew(delegate {


            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        MarchCube(new Vector3Int(x, y, z));

                    }
                }
            }


        });
        yield return new WaitUntil(() =>
        {
            return t.IsCompleted;
        });


        if (t.Exception != null)
        {
            Debug.LogError(t.Exception);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        PlanetData.meshData.TryAdd(chunkPos, mesh);
        callback(mesh);
    }


    //marchingCubesAlgorithm
    private void MarchCube(Vector3Int position)
    {
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            cube[i] = blocks[position.x + PlanetData.CornerTable[i].x, position.y + PlanetData.CornerTable[i].y, position.z + PlanetData.CornerTable[i].z];
        }

        int configIndex = GetCubeConfiguration(cube);

        if (configIndex == 0 || configIndex == 255)
            return;

        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int p = 0; p < 3; p++)
            {
                int indice = PlanetData.TriangleTable[configIndex, edgeIndex];
                if (indice == -1)
                    return;

                Vector3 vert1 = position + PlanetData.CornerTable[PlanetData.EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + PlanetData.CornerTable[PlanetData.EdgeIndexes[indice, 1]];

                Vector3 vertPosition;
                float vert1Sample = cube[PlanetData.EdgeIndexes[indice, 0]];
                float vert2Sample = cube[PlanetData.EdgeIndexes[indice, 1]];

                float difference = vert2Sample - vert1Sample;
                if (difference == 0)
                    difference = terrainSurface;
                else
                    difference = (terrainSurface - vert1Sample) / difference;

                vertPosition = vert1 + ((vert2 - vert1) * difference);
                triangles.Add(VertForIndice(vertPosition));

                edgeIndex++;
            }
        }
    }
    private int GetCubeConfiguration(float[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > terrainSurface)
            {
                configurationIndex |= 1 << i;
            }
        }
        return configurationIndex;
    }
    private int VertForIndice(Vector3 vert)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i] == vert)
                return i;
        }
        vertices.Add(vert);
        return vertices.Count - 1;
    }





}
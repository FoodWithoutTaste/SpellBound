using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class WallGenerator : MonoBehaviour
{
    [SerializeField] Vector3Int wallSize;
    public int[,,] blocks;

    private void Start()
    {
        GenerateWall();
    }

    private void GenerateWall()
    {
        // Assuming 'blocks' array is already initialized and filled with appropriate values (1 for blocks, 0 for air)

        // Create a new GameObject to hold the wall mesh
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.position = Vector3.zero;
        MeshFilter meshFilter = wallObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = wallObject.AddComponent<MeshRenderer>();
        Mesh wallMesh = new Mesh();

        // Generate vertices and triangles for the wall mesh
        Vector3[] vertices;
        int[] triangles;
        GenerateWallMesh(out vertices, out triangles);

        // Assign vertices and triangles to the wall mesh
        wallMesh.vertices = vertices;
        wallMesh.triangles = triangles;

        // Recalculate normals for lighting
        wallMesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter component
        meshFilter.mesh = wallMesh;
    }

    private void GenerateWallMesh(out Vector3[] vertices, out int[] triangles)
    {
        // Temporary lists to hold vertices and triangles
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        // Generate vertices and triangles for the wall mesh
        for (int x = 0; x < wallSize.x; x++)
        {
            for (int y = 0; y < wallSize.y; y++)
            {
                for (int z = 0; z < wallSize.z; z++)
                {
                    if (blocks[x, y, z] == 1)
                    {
                      
                    }
                }
            }
        }

        // Convert lists to arrays
        vertices = vertexList.ToArray();
        triangles = triangleList.ToArray();
    }

    
}

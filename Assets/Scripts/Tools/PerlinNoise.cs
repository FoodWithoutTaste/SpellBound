using UnityEngine;
using System.Collections.Generic;

public class PerlinNoise : MonoBehaviour
{

    [SerializeField] Vector3 size;
    [SerializeField] float noiseScale;
    [SerializeField] float threshold = 0.5f;
    public bool generate = false;
    public int[,,] blocks;

    public void Start()
    {
        // Initialize the blocks array
        blocks = new int[(int)size.x, (int)size.y, (int)size.z];
    }


    private void GenerateBlock(List<Vector3> vertices, List<int> triangles, Vector3 position)
    {
        // Define the vertices of the block
        Vector3[] blockVertices =
        {
            new Vector3(-0.5f, -0.5f, -0.5f) + position,
            new Vector3( 0.5f, -0.5f, -0.5f) + position,
            new Vector3(-0.5f,  0.5f, -0.5f) + position,
            new Vector3( 0.5f,  0.5f, -0.5f) + position,
            new Vector3(-0.5f, -0.5f,  0.5f) + position,
            new Vector3( 0.5f, -0.5f,  0.5f) + position,
            new Vector3(-0.5f,  0.5f,  0.5f) + position,
            new Vector3( 0.5f,  0.5f,  0.5f) + position
        };

        // Define the triangles of the block
        int[] blockTriangles =
        {
            0, 2, 1, // Front
            2, 3, 1,
            4, 5, 6, // Back
            6, 5, 7,
            0, 1, 4, // Bottom
            4, 1, 5,
            2, 6, 3, // Top
            3, 6, 7,
            0, 4, 2, // Left
            2, 4, 6,
            1, 3, 5, // Right
            3, 7, 5
        };

        // Determine if there are adjacent blocks
        bool leftNeighbor = position.x > 0 && blocks[(int)position.x - 1, (int)position.y, (int)position.z] == 0;
        bool rightNeighbor = position.x < size.x - 1 && blocks[(int)position.x + 1, (int)position.y, (int)position.z] == 0;
        bool bottomNeighbor = position.y > 0 && blocks[(int)position.x, (int)position.y - 1, (int)position.z] == 0;
        bool topNeighbor = position.y < size.y - 1 && blocks[(int)position.x, (int)position.y + 1, (int)position.z] == 0;
        bool backNeighbor = position.z > 0 && blocks[(int)position.x, (int)position.y, (int)position.z - 1] == 0;
        bool frontNeighbor = position.z < size.z - 1 && blocks[(int)position.x, (int)position.y, (int)position.z + 1] == 0;

        // Generate vertices and triangles for each face
        for (int i = 0; i < blockTriangles.Length; i += 3)
        {
            bool faceVisible = true;

            // Check if face should be visible based on adjacent blocks
            switch (i / 3)
            {
                case 0: // Front face
                    if (frontNeighbor) faceVisible = false;
                    break;
                case 1: // Back face
                    if (backNeighbor) faceVisible = false;
                    break;
                case 2: // Bottom face
                    if (bottomNeighbor) faceVisible = false;
                    break;
                case 3: // Top face
                    if (topNeighbor) faceVisible = false;
                    break;
                case 4: // Left face
                    if (leftNeighbor) faceVisible = false;
                    break;
                case 5: // Right face
                    if (rightNeighbor) faceVisible = false;
                    break;
            }

            // Add vertices and triangles if face is visible
            if (faceVisible)
            {
                for (int j = 0; j < 3; j++)
                {
                    vertices.Add(blockVertices[blockTriangles[i + j]]);
                    triangles.Add(vertices.Count - 1);
                }
            }
        }
    }
}

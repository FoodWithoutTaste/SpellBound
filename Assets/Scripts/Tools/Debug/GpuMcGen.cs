using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuMcGen : MonoBehaviour
{

    public ComputeShader computeShader;
    public Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    public float terrainSurface = 0.5f;

    private ComputeBuffer voxelBuffer;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer cornerTableBuffer;
    private ComputeBuffer edgeIndexesBuffer;
    private ComputeBuffer triangleTableBuffer;
    private ComputeBuffer vertexCounterBuffer;
    private ComputeBuffer triangleCounterBuffer;

 

    void Start()
    {
        // Initialize buffers
        voxelBuffer = new ComputeBuffer(chunkSize.x * chunkSize.y * chunkSize.z, sizeof(float));
        vertexBuffer = new ComputeBuffer(1000000, sizeof(float) * 3); // Adjust size as needed
        triangleBuffer = new ComputeBuffer(1000000, sizeof(int)); // Adjust size as needed
        cornerTableBuffer = new ComputeBuffer(8, sizeof(int) * 3);
        edgeIndexesBuffer = new ComputeBuffer(12, sizeof(int) * 2);
        triangleTableBuffer = new ComputeBuffer(256 * 16, sizeof(int));
        vertexCounterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        triangleCounterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        // Fill voxel data buffer with your terrain data
        float[] voxelData = new float[chunkSize.x * chunkSize.y * chunkSize.z];
       
        // Populate voxelData with your terrain generation logic

        voxelBuffer.SetData(voxelData);
        cornerTableBuffer.SetData(PlanetData.CornerTable);
        edgeIndexesBuffer.SetData(PlanetData.EdgeIndexes);
        triangleTableBuffer.SetData(PlanetData.TriangleTable);
        vertexCounterBuffer.SetData(new int[] { 0 });
        triangleCounterBuffer.SetData(new int[] { 0 });

        // Set compute shader parameters
        computeShader.SetBuffer(0, "voxelData", voxelBuffer);
        computeShader.SetBuffer(0, "vertices", vertexBuffer);
        computeShader.SetBuffer(0, "triangles", triangleBuffer);
        computeShader.SetBuffer(0, "CornerTable", cornerTableBuffer);
        computeShader.SetBuffer(0, "EdgeIndexes", edgeIndexesBuffer);
        computeShader.SetBuffer(0, "TriangleTable", triangleTableBuffer);
        computeShader.SetBuffer(0, "vertexCounter", vertexCounterBuffer);
        computeShader.SetBuffer(0, "triangleCounter", triangleCounterBuffer);
        computeShader.SetFloat("terrainSurface", terrainSurface);

        // Execute compute shader
        int threadGroupsX = Mathf.CeilToInt(chunkSize.x / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(chunkSize.y / 8.0f);
        int threadGroupsZ = Mathf.CeilToInt(chunkSize.z / 8.0f);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, threadGroupsZ);

        // Retrieve data from GPU
        int[] vertexCounter = new int[1];
        vertexCounterBuffer.GetData(vertexCounter);
        int[] triangleCounter = new int[1];
        triangleCounterBuffer.GetData(triangleCounter);

        Vector3[] vertices = new Vector3[vertexCounter[0]];
        int[] triangles = new int[triangleCounter[0] * 3];
        vertexBuffer.GetData(vertices);
        triangleBuffer.GetData(triangles);

        // Use the retrieved data to create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Apply the mesh to your GameObject
        GetComponent<MeshFilter>().mesh = mesh;

        // Release buffers
        voxelBuffer.Release();
        vertexBuffer.Release();
        triangleBuffer.Release();
        cornerTableBuffer.Release();
        edgeIndexesBuffer.Release();
        triangleTableBuffer.Release();
        vertexCounterBuffer.Release();
        triangleCounterBuffer.Release();
    }
}



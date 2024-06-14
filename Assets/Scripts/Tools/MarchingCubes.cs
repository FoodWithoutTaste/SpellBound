using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    public MarchingCubesTable table;
    public bool gizmo = false;



    public float[,,] blocks;
    public Vector3 size;


    public float threshold;
    public float noiseScale;
    void Start()
    {
        blocks = new float[(int)size.x, (int)size.y, (int)size.z];
       MakeSphere(blocks, size);
   
    }











    private void OnDrawGizmos()
    {
        if (gizmo == false)
            return;
        if (blocks == null) // Check if blocks is null before proceeding
            return;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    if(blocks[x,y,z] == 1)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(new Vector3(x, y, z), 0.25f);
                       
                    }
                    else
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireSphere(new Vector3(x, y, z), 0.25f);
                    }
                   

                }
            }
        }
    }
    public static void MakeSphere(float[,,] array , Vector3 size)
    {
        // Center and radius of the sphere
        int cx = (int)size.x / 2;
        int cy = (int)size.y / 2;
        int cz = (int)size.z / 2;
        int radius = (int)size.x / 2 - 1;

        // Iterate over the 3D array and set values based on whether they fall within the sphere
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    // Check if the current point is within the sphere
                    if ((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz) <= radius * radius)
                    {
                        array[x, y, z] = 1; // Inside the sphere
                    }
                    else
                    {
                        array[x, y, z] = 0; // Outside the sphere
                    }
                }
            }
        }
    }
    public static float PerlinNoise3d(float x, float y, float z )
    {
        float ab = Mathf.PerlinNoise(x, y);
        float ac = Mathf.PerlinNoise(x, z);
        float ba = Mathf.PerlinNoise(y, x);
        float bc = Mathf.PerlinNoise(y, z);
        float ca = Mathf.PerlinNoise(z, x);
        float cb = Mathf.PerlinNoise(z, y);
        float abc = ab + ac + ca + cb + ba + bc;
        return abc / 6f;
    }
    public  void MakeNoise(float[,,] array, Vector3 size)
    {
      

        // Iterate over the 3D array and set values based on whether they fall within the sphere
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    float noiseValue = PerlinNoise3d((x + 0.5f) * noiseScale, (y + 0.5f) * noiseScale, (z + 0.5f) * noiseScale);
                    blocks[x, y, z] = noiseValue >= threshold ? 1 : 0;
                }
            }
        }
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlanetGeneration : MonoBehaviour
{
	[Header("Planet Properties")]
	public bool smoothTerrain = false;
	public bool flatShaded = false;
	[Range(0.0f, 1.0f)]
	public float terrainSurface = 0.5f;


	public float chunkBuffer = 0.5f;
	public bool useFrustum = true;
	public Vector3 center { get { return new Vector3(worldSizeInChunks * chunkSize.x*sizeOverAll, worldSizeInChunks * sizeOverAll * chunkSize.x, worldSizeInChunks * sizeOverAll * chunkSize.x) / 2; } }

	[Header("Sphere Properties")]
	[Range(0.75f, 1.5f)]
	public float sphereScale;
	[Header("Contiunales")]
	public float continentalnessScale;
	public float continentalnessAplitude;
	public ContinentalnessRanges[] continentalRanges;
	[Header("Noise Properties")]
	public int layers;
	public float noiseScale;
	public float amplitude;
	public float upscaler;
	public float descaler;
	[Header("Chunk Properties")]
	public Material chunkMaterial;
	public int worldSizeInChunks;
	public Vector3Int chunkSize;
	public int sizeOverAll;
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		// Calculate center and radius
		float centerX = worldSizeInChunks * chunkSize.x / 2;
		float centerY = worldSizeInChunks * chunkSize.y / 2;
		float centerZ = worldSizeInChunks * chunkSize.z / 2;
		Vector3 center = new Vector3(centerX, centerY, centerZ);

		float radius = (worldSizeInChunks * chunkSize.x / 2 - 1) * sphereScale;

		// Draw the wire sphere
		Gizmos.DrawWireSphere(center, radius);
	}
    public void Start()
    {
		chunkMaterial.SetVector("_Center", center);
	}

}

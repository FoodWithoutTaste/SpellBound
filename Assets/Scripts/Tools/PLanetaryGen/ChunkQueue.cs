using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ChunkQueue : MonoBehaviour
{
	public PlanetGeneration planet;

	private LinkedList<Chunk> chunksList = new LinkedList<Chunk>();

	public void EnqueueChunk(Chunk chunk)
	{
		
			chunksList.AddLast(chunk);
	}

	public void DequeueChunk(Chunk chunk)
	{
		chunksList.Remove(chunk);

	}
	public void Start()
	{
		StartCoroutine(ProcessChunks());
	}

	private IEnumerator ProcessChunks()
	{
		while (true)
		{
			if (chunksList.Count > 0)
			{
				Chunk firstChunk = chunksList.First.Value;

				if (firstChunk != null)
				{
					bool isProcessed = false;

					// Assuming chunkMeshGenerator.Initialization() and RenderChunk() are defined elsewhere
					firstChunk.chunkMeshGenerator.Initialization(planet);
					StartCoroutine(RenderChunk(
						firstChunk.chunkPos,
						firstChunk.chunkMeshGenerator,
						firstChunk.chunkMeshGenerator.GetComponent<MeshFilter>(),
						firstChunk.chunkMeshGenerator.GetComponent<MeshCollider>(),
						result => isProcessed = result
					));

					// Wait until the chunk is processed before continuing
					yield return new WaitUntil(() => isProcessed);

					// Remove the chunk from the list after processing
					chunksList.RemoveFirst();
				}
				else
				{
					// If the first chunk is null, just remove it
					chunksList.RemoveFirst();
				}
			}

			// Wait a frame before checking the list again to avoid tight looping
			yield return null;
		}
	}


	public IEnumerator RenderChunk(Vector3Int chunkPos, ChunkMeshGenerator chunk, MeshFilter filter, MeshCollider collider, System.Action<bool> callback)
	{
		float[,,] dataToApply =null;
		if (dataToApply == null)
		{
			StartCoroutine(FillUpTheTerrain(chunkPos, x => dataToApply = x));
		}
		yield return new WaitUntil(() => dataToApply != null);


		Mesh meshToApply = PlanetData.meshData.ContainsKey(chunkPos) ? PlanetData.meshData[chunkPos] : null;

		if (meshToApply == null)
		{
			yield return StartCoroutine(chunk.CreateChunkMesh(chunkPos, dataToApply, x => meshToApply = x));
			yield return new WaitUntil(() => meshToApply != null);
		}

		if (meshToApply != null && filter != null && collider != null)
		{
			filter.mesh = meshToApply;
			collider.sharedMesh = meshToApply;
		}

		//we got mesh so now we gotta somehow use it
		callback(true);
	}


	public void CreateChunk(Vector3Int chunkPos,GameObject chunkObject)
	{
		Debug.Log("Nigga It Spawned");
		PlanetData.activeChunks.TryAdd(chunkPos, chunkObject);
		chunkObject.name = $"{chunkPos.x}_{chunkPos.y}_{chunkPos.z}";
		chunkObject.transform.position = chunkPos;
	
		var meshFilter = chunkObject.GetComponent<MeshFilter>();
		var meshRenderer = chunkObject.GetComponent<MeshRenderer>();
		var meshCollider = chunkObject.GetComponent<MeshCollider>();
		var ChunkMeshGen = chunkObject.GetComponent<ChunkMeshGenerator>();
		bool isProcessed = false;
		ChunkMeshGen.Initialization(planet);
		StartCoroutine(RenderChunk(
			chunkPos,
			ChunkMeshGen,
			meshFilter,
			meshCollider,
			result => isProcessed = result
		));

		



	}
	public IEnumerator FillUpTheTerrain(Vector3Int chunkPosition, System.Action<float[,,]> callback)
	{
		float[,,] tempdata = new float[planet.chunkSize.x + 1, planet.chunkSize.y + 1, planet.chunkSize.z + 1];

		Task t = Task.Factory.StartNew(() =>
		{
			for (int x = 0; x < tempdata.GetLength(0); x++)
			{
				for (int y = 0; y < tempdata.GetLength(1); y++)
				{
					for (int z = 0; z < tempdata.GetLength(2); z++)
					{
						tempdata[x, y, z] = PlanetData.GetTerrainValue(new Vector3(x + chunkPosition.x, y + chunkPosition.y, z + chunkPosition.z), planet);

					}
				}
			}
		});

		yield return new WaitUntil(() => t.IsCompleted);

		if (t.Exception != null)
		{
			Debug.LogError(t.Exception);
		}

		

		callback(tempdata);
	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel;

public class ChunkRenderer : MonoBehaviour
{
	public PlanetGeneration planet;
	public LinkedList<GameObject> chunksList = new LinkedList<GameObject>();
	public int chunks;

	public void EnqueueChunk(GameObject chunk)
	{

		chunksList.AddLast(chunk);
	}

	public void DequeueChunk(GameObject chunk)
	{
		chunksList.Remove(chunk);

	}
	public void Start()
	{
		StartCoroutine(ProcessChunks(chunks));
	}

	private IEnumerator ProcessChunks(int chunksToProcess)
	{
		while (true)
		{
			if (chunksList.Count >= chunksToProcess) // Ensure there are enough chunks to process
			{
				List<GameObject> chunks = new List<GameObject>();
				List<WaitUntil> waitConditions = new List<WaitUntil>();
				List<bool> isProcessedList = new List<bool>();

				// Collect the specified number of chunks
				for (int i = 0; i < chunksToProcess; i++)
				{
					GameObject chunk = chunksList.First.Value;
					chunksList.RemoveFirst();

					if (chunk != null)
					{
						bool isProcessed = false;
						isProcessedList.Add(isProcessed);

						int index = i; // Capture the current index for the closure
						StartCoroutine(RenderChunk(chunk.GetComponent<ChunkHolder>().chunkReference.chunkPos, chunk, x => isProcessedList[index] = x));

						// Add a wait condition for this chunk
						waitConditions.Add(new WaitUntil(() => isProcessedList[index]));
					}
				}

				// Wait until all chunks are processed before continuing
				foreach (var waitCondition in waitConditions)
				{
					yield return waitCondition;
				}
			}
			else if (chunksList.Count > 0) // Process remaining chunks if less than the specified number
			{
				List<WaitUntil> waitConditions = new List<WaitUntil>();
				List<bool> isProcessedList = new List<bool>();

				while (chunksList.Count > 0)
				{
					GameObject chunk = chunksList.First.Value;
					chunksList.RemoveFirst();

					if (chunk != null)
					{
						bool isProcessed = false;
						isProcessedList.Add(isProcessed);

						int index = isProcessedList.Count - 1; // Capture the current index for the closure
						StartCoroutine(RenderChunk(chunk.GetComponent<ChunkHolder>().chunkReference.chunkPos, chunk, x => isProcessedList[index] = x));

						// Add a wait condition for this chunk
						waitConditions.Add(new WaitUntil(() => isProcessedList[index]));
					}
				}

				// Wait until all remaining chunks are processed before continuing
				foreach (var waitCondition in waitConditions)
				{
					yield return waitCondition;
				}
			}

			// Wait a frame before checking the list again to avoid tight looping
			yield return null;
		}
	}




	public IEnumerator RenderChunk(Vector3Int chunkPos, GameObject chunkObject, System.Action<bool> callback)
    {
		//name
		chunkObject.name = $"{chunkPos.x}_{chunkPos.y}_{chunkPos.z}";
		chunkObject.transform.position = chunkPos * planet.sizeOverAll;
		//getters
		var meshFilter = chunkObject.GetComponent<MeshFilter>();
		var meshRenderer = chunkObject.GetComponent<MeshRenderer>();
		var meshCollider = chunkObject.GetComponent<MeshCollider>();


		float[,,] dataToApply =  null;
		Mesh meshToApply = null;


		if (PlanetData.planetData.ContainsKey(chunkPos))
		{
			if (PlanetData.planetData[chunkPos].chunkData != null)
			{
				dataToApply = PlanetData.planetData[chunkPos].chunkData;
			}
		}

		if (dataToApply == null)
		{
			
			StartCoroutine(FillUpTheTerrain(chunkPos, planet, x => dataToApply = x));
		
		}
		yield return new WaitUntil(() => dataToApply != null);

		if (PlanetData.planetData.ContainsKey(chunkPos))
		{
			if (PlanetData.planetData[chunkPos].mesh != null)
			{
				meshToApply = PlanetData.planetData[chunkPos].mesh;
			}
		}

		if (meshToApply == null)
		{
			
			StartCoroutine(MC_Generator.CreateChunkMesh(chunkPos, dataToApply, planet, x => meshToApply = x));
			
		}
		yield return new WaitUntil(() => meshToApply != null);



		if (meshToApply != null && meshFilter != null && meshCollider != null)
		{
			meshFilter.mesh = meshToApply;
			meshCollider.sharedMesh = meshToApply;
		}
		callback(true);

	}




	public static IEnumerator FillUpTheTerrain(Vector3Int chunkPosition,PlanetGeneration planet, System.Action<float[,,]> callback)
	{
		float[,,] tempdata = new float[planet.chunkSize.x + 1, planet.chunkSize.y + 1, planet.chunkSize.z + 1];
		Task t = Task.Factory.StartNew(delegate {

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
		yield return new WaitUntil(() =>
		{
			return t.IsCompleted;
		});


		if (t.Exception != null)
		{
			Debug.LogError(t.Exception);
		}

	
			if(PlanetData.planetData.TryGetValue(chunkPosition,out Chunk c))
            {
			 c.chunkData = tempdata;
            }
		callback(tempdata);
	}

	
}

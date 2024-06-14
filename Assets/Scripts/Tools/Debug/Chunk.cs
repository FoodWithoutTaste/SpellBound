using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class Chunk
{
    public Vector3Int chunkPos;
    public Mesh mesh;
    public float[,,] chunkData;
    public bool isEmpty;
    public ChunkMeshGenerator chunkMeshGenerator;
    public Chunk(Vector3Int chunkPos, float[,,] chunkData,bool isEmpty)
    {
        this.chunkPos = chunkPos;
        this.chunkData = chunkData;
        this.isEmpty = isEmpty;
    }
    public Chunk(Vector3Int chunkPos, bool isEmpty)
    {
        this.chunkPos = chunkPos;
        this.isEmpty = isEmpty;
    }
    public Chunk(Vector3Int chunkPos, float[,,] chunkData,Mesh mesh)
    {
        this.chunkPos = chunkPos;
        this.chunkData = chunkData;
        this.mesh = mesh;
    }

}

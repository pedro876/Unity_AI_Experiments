using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrainState
{
    public int inputSize;
    public int outputSize;
    public int totalSize;
    public float iterationsPerSecond;
    public int currentBufferIdx = 0;

    public float[] biases;
    public float[] passivity;
    public float[] persistence;
    public float[,] weights;
    public bool[,] adjacencies;
    public float[,] positions;
    public float[] frontBuffer;
    public float[] backBuffer;

    public BrainState(int inputSize, int outputSize, int totalSize, int iterationsPerSecond)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.totalSize = totalSize;
        this.iterationsPerSecond = iterationsPerSecond;
        CreateEmptyBuffers();
    }

    public float GetOutputAt(int outputIndex)
    {
        return frontBuffer[inputSize + outputIndex];
    }

    private void CreateEmptyBuffers()
    {
        biases = new float[totalSize];
        passivity = new float[totalSize];
        persistence = new float[totalSize];
        weights = new float[totalSize, totalSize];
        adjacencies = new bool[totalSize, totalSize];
        positions = new float[totalSize, 2];
        frontBuffer = new float[totalSize];
        backBuffer = new float[totalSize];
    }
}


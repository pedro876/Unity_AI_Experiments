using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrainState
{
    public int inputSize;
    public int outputSize;
    public int totalSize;
    public float iterationsPerSecond;

    public float[] biases;
    public float[] passivity;
    public float[] persistence;
    public float[,] weights;
    public bool[,] adjacencies;
    public float[,] positions;
    public float[] frontBuffer;
    public float[] backBuffer;

    public BrainState(int inputSize, int outputSize, int totalSize, float iterationsPerSecond)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.totalSize = totalSize;
        this.iterationsPerSecond = iterationsPerSecond;
        CreateEmptyBuffers();
    }

    public BrainState Clone(bool emptyValueBuffer = true)
    {
        BrainState clonedState = new BrainState(inputSize, outputSize, totalSize, iterationsPerSecond);
        for(int i = 0; i < totalSize; i++)
        {
            clonedState.biases[i] = biases[i];
            clonedState.passivity[i] = passivity[i];
            clonedState.persistence[i] = persistence[i];
            if(!emptyValueBuffer) clonedState.frontBuffer[i] = frontBuffer[i];
            clonedState.positions[i, 0] = positions[i, 0];
            clonedState.positions[i, 1] = positions[i, 1];

            for(int j = 0; j < totalSize; j++)
            {
                clonedState.weights[i, j] = weights[i, j];
                clonedState.adjacencies[i, j] = adjacencies[i, j];
            }
        }
        return clonedState;
    }

    public void CopyTo(BrainState state)
    {
        for (int i = 0; i < totalSize; i++)
        {
            state.biases[i] = biases[i];
            state.passivity[i] = passivity[i];
            state.persistence[i] = persistence[i];
            state.frontBuffer[i] = frontBuffer[i];
            state.positions[i, 0] = positions[i, 0];
            state.positions[i, 1] = positions[i, 1];

            for (int j = 0; j < totalSize; j++)
            {
                state.weights[i, j] = weights[i, j];
                state.adjacencies[i, j] = adjacencies[i, j];
            }
        }
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


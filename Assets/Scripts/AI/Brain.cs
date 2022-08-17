using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    int inputNeuronsAmount;
    int outputNeuronsAmount;
    int totalNeurons;

    private float[] biases;
    private float[,] weights;
    private bool[,] adjacencies;
    private float[] valuesBuffer0;
    private float[] valuesBuffer1;
    private int currentBufferIdx = 0;

    public Brain(int inputCount, int outputCount)
    {
        inputNeuronsAmount = inputCount;
        outputNeuronsAmount = outputCount;
        totalNeurons = inputCount + outputCount;
        biases = new float[totalNeurons];
        weights = new float[totalNeurons, totalNeurons];
        adjacencies = new bool[totalNeurons, totalNeurons];
        valuesBuffer0 = new float[totalNeurons];
        valuesBuffer1 = new float[totalNeurons];

        // TEST
        weights[0, 1] = 0.6f;
        adjacencies[0, 1] = true;
    }

    public void Propagate(float[] input, float deltaTime)
    {
        float[] frontBuffer = currentBufferIdx == 0 ? valuesBuffer0 : valuesBuffer1;
        float[] backBuffer = currentBufferIdx == 0 ? valuesBuffer1 : valuesBuffer0;

        // FILL INPUT
        for (int i = 0; i < input.Length; i++)
        {
            frontBuffer[i] = input[i];
        }

        // PROPAGATE VALUES ACROSS NETWORK
        for(int i = 0; i < totalNeurons; i++)
        {
            backBuffer[i] = biases[i];
            // For each neuron we must add the sum of values * weight + bias
            for (int j = 0; j < totalNeurons; j++)
            {
                if(adjacencies[j,i])
                {
                    backBuffer[i] += weights[j, i] * frontBuffer[j];
                }
            }
            // RELU
            backBuffer[i] = Mathf.Max(0, backBuffer[i]);
        }

        SwapBuffers();
    }

    public float GetOutputAt(int outputIndex)
    {
        float[] frontBuffer = currentBufferIdx == 0 ? valuesBuffer0 : valuesBuffer1;
        return frontBuffer[inputNeuronsAmount + outputIndex];
    }

    private void SwapBuffers()
    {
        currentBufferIdx = (currentBufferIdx + 1) % 2;
    }
}

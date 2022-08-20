using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    public BrainState state;
    private float iterationReminder;

    public Brain(BrainState state)
    {
        this.state = state;
        iterationReminder = 0f;
    }

    public float GetOutputAt(int index) => state.GetOutputAt(index);

    public void Propagate(float[] input, float deltaTime)
    {
        // FILL INPUT
        for (int i = 0; i < input.Length; i++)
        {
            state.frontBuffer[i] = input[i];
        }

        // CALCULATE ITERATIONS
        int iterations = CalculateIterations(deltaTime);

        // PERFORM ITERATIONS
        for (int i = 0; i < iterations; i++)
        {
            IterateNetwork();
        }
    }

    private int CalculateIterations(float deltaTime)
    {
        float time = deltaTime + iterationReminder;
        float iterationsRaw = time * state.iterationsPerSecond;
        float decimalPart = iterationsRaw - (int)iterationsRaw;
        int iterations = Mathf.RoundToInt(iterationsRaw);

        float secondsPerIteration = 1f / state.iterationsPerSecond;
        if (decimalPart >= 0.5f)
            iterationReminder = (1f - decimalPart) * -secondsPerIteration;
        else
            iterationReminder = decimalPart * secondsPerIteration;

        return iterations;
    }

    private void IterateNetwork()
    {
        //PERSISTENCE VALUES
        for (int i = state.inputSize; i < state.totalSize; i++)
        {
            state.frontBuffer[i] = state.frontBuffer[i] * state.persistence[i];
        }

        // PROPAGATE VALUES ACROSS NETWORK
        for (int i = state.inputSize; i < state.totalSize; i++)
        {
            // For each neuron we must add the sum of values * weight + bias
            state.backBuffer[i] = state.biases[i];
            for (int j = 0; j < state.totalSize; j++)
            {
                if (state.adjacencies[j, i])
                {
                    state.backBuffer[i] += state.weights[j, i] * state.frontBuffer[j];
                }
            }
            // RELU
            state.backBuffer[i] = Mathf.Max(0, state.backBuffer[i]);
        }

        //PROCESS FINAL INPUT VALUE INTO EACH NEURON
        for(int i = state.inputSize; i < state.totalSize; i++)
        {
            state.frontBuffer[i] += state.backBuffer[i] * state.passivity[i];
        }
    }
}

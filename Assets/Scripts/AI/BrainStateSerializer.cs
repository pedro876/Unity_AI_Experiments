using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class BrainStateSerializer
{
    public static string Serialize(BrainState state, bool withPropertyNames = false)
    {

        /*
         * 1 -> input
         * 1 -> output
         * 2 -> total
         * 30 -> iterations per second
         * 0 -> current buffer idx
         * 0 0 -> biases
         * 0 0.6 -> weights
         * 0 0
         * 0 1 -> adjacencies
         * 0 0
         * 0 0 -> values buffer 0
         */
        StringBuilder result = new StringBuilder();
        if(withPropertyNames) result.Append($"Input count:\n");
        result.Append($"{state.inputSize}\n");
        if (withPropertyNames) result.Append($"Output count:\n");
        result.Append($"{state.outputSize}\n");
        if (withPropertyNames) result.Append($"Total Neuron count:\n");
        result.Append($"{state.totalSize}\n");
        if (withPropertyNames) result.Append("Iterations per second:\n");
        result.Append($"{state.iterationsPerSecond}\n");
        if (withPropertyNames) result.Append("Current buffer:\n");
        result.Append($"{state.currentBufferIdx}\n");

        //BIASES
        if (withPropertyNames) result.Append("Biases:\n");
        Write1DArray(result, state.biases);
        result.Append("\n");

        //PASSIVITY
        if (withPropertyNames) result.Append("Passivity:\n");
        Write1DArray(result, state.passivity);
        result.Append("\n");

        //PERSISTENCE
        if (withPropertyNames) result.Append("Persistence:\n");
        Write1DArray(result, state.persistence);
        result.Append("\n");

        //WEIGHTS
        if (withPropertyNames) result.Append("Weights:\n");
        Write2DArray(result, state.weights);

        //ADJACENCIES
        if (withPropertyNames) result.Append("Adjacencies:\n");
        for (int i = 0; i < state.totalSize; i++)
        {
            for (int j = 0; j < state.totalSize; j++)
                result.Append($"{(state.adjacencies[i, j] ? 1 : 0)} ");
            result.Append("\n");
        }

        //POSITIONS
        if (withPropertyNames) result.Append("Positions:\n");
        Write2DArray(result, state.positions);

        //VALUES BUFFER 0
        if (withPropertyNames) result.Append("Buffer 0:\n");
        Write1DArray(result, state.frontBuffer);
        result.Append("\n");

        return result.ToString();
    }

    public static BrainState Deserialize(string text)
    {
        string[] lines = text.Split("\n");

        int currentLine = 0;
        int inputSize = int.Parse(lines[currentLine++]);
        int outputSize = int.Parse(lines[currentLine++]);
        int totalSize = int.Parse(lines[currentLine++]);
        int iterationsPerSecond = int.Parse(lines[currentLine++]);
        int currentBufferIdx = int.Parse(lines[currentLine++]);

        BrainState state = new BrainState(inputSize, outputSize, totalSize, iterationsPerSecond);
        state.iterationsPerSecond = iterationsPerSecond;
        state.currentBufferIdx = currentBufferIdx;

        Read1DArray(lines[currentLine++], state.biases);
        Read1DArray(lines[currentLine++], state.passivity);
        Read1DArray(lines[currentLine++], state.persistence);
        Read2DArray(lines, currentLine, state.weights);
        currentLine += totalSize;
        //ADJACENCIES
        int length = totalSize;
        for (int i = 0; i < length; i++)
        {
            string line = lines[currentLine + i].Trim();
            string[] strValues = line.Split(" ");
            for (int j = 0; j < length; j++)
            {
                state.adjacencies[i, j] = int.Parse(strValues[j]) == 1;
            }
        }
        currentLine += totalSize;


        Read1DArray(lines[currentLine++], state.frontBuffer);

        return state;
    }

    private static void Write1DArray(StringBuilder builder, float[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            builder.Append($"{arr[i]} ");
    }

    private static void Write2DArray(StringBuilder builder, float[,] arr)
    {
        int length = arr.GetLength(0);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
                builder.Append($"{arr[i, j]} ");
            builder.Append("\n");
        }
    }

    private static void Read1DArray(string line, float[] arr)
    {
        line = line.Trim();
        string[] strValues = line.Split(" ");
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = float.Parse(strValues[i]);
        }
    }

    private static void Read2DArray(string[] lines, int lineIdx, float[,] arr)
    {
        int length = arr.GetLength(0);
        for(int i = 0; i < length; i++)
        {
            string line = lines[lineIdx + i].Trim();
            string[] strValues = line.Split(" ");
            for (int j = 0; j < length; j++)
            {
                arr[i,j] = float.Parse(strValues[j]);
            }
        }
    }
}


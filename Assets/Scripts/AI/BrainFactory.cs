using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BrainFactory
{
    // NEURON PROPERTIES
    public static float bias;
    public static float passivity;
    public static float persistence;
    public static float weight;
    public static float iterationsPerSecond;

    // MUTATION PROPERTIES
    public static float minBias;
    public static float maxBias;
    public static float minPassivity;
    public static float maxPassivity;
    public static float minPersistence;
    public static float maxPersistence;
    public static float minWeight;
    public static float maxWeight;
    public static float minDistance;
    public static float maxDistance;

    // RANDOM RANGES
    public static int seed;
    public static float randomMultiplier;
    public static float maxWeightChange;
    public static float maxBiasChange;
    public static float maxPassivityChange;
    public static float maxPersistenceChange;

    // PROBABILITES
    public static float chanceOfNeuronAddition;
    public static float chanceOfNeuronRemoval;
    public static float chanceOfLinkAddition;
    public static float chanceOfLinkRemoval;
    public static int maxNeuronAdditionTries;

    // STRUCTURE PROPERTIES
    public static bool allowSelfLinks;
    public static bool allowBidirectionalLinks;

    static Random rnd;

    static BrainFactory()
    {
        RestoreDefaultConfig();
    }

    public static void RestoreDefaultConfig()
    {

        bias = 0f;
        passivity = 1f;
        persistence = 0f;
        weight = 0.5f;
        iterationsPerSecond = 60;

        minBias = -1;
        maxBias = 1;
        minPassivity = 0.001f;
        maxPassivity = 1f;
        minPersistence = 0;
        maxPersistence = 1;
        minWeight = -1;
        maxWeight = 1;
        minDistance = 0.75f;
        maxDistance = 1.5f;

        seed = 0;
        rnd = new Random(seed);
        randomMultiplier = 1f;
        maxWeightChange = 0.05f;
        maxBiasChange = 0.05f;
        maxPassivityChange = 0.05f;
        maxPersistenceChange = 0.05f;

        chanceOfNeuronAddition = 0.15f;
        chanceOfNeuronRemoval = 0.01f;
        chanceOfLinkAddition = 0.15f;
        chanceOfLinkRemoval = 0.05f;
        //chanceOfNeuronAddition = 2f;
        maxNeuronAdditionTries = 5;

        allowSelfLinks = false;
        allowBidirectionalLinks = false;
    }

    public static Brain CreateBrain(int inputSize, int outputSize, bool fullyConnected = false)
    {
        int totalSize = inputSize + outputSize;
        BrainState state = new BrainState(inputSize, outputSize, totalSize, iterationsPerSecond);

        float inputWidth = inputSize-1;
        for(int i = 0; i < inputSize; i++)
        {
            state.positions[i, 0] = -inputWidth / 2f + i;
            state.positions[i, 1] = 0.5f;
        }

        float outputWidth = outputSize - 1;
        for(int j = inputSize; j < totalSize; j++)
        {
            int i = j - inputSize;
            state.positions[j, 0] = -outputWidth / 2f + i;
            state.positions[j, 1] = -0.5f;

            if (fullyConnected)
            {
                for(int w = 0; w < inputSize; w++)
                {
                    state.adjacencies[w, j] = true;
                    state.weights[w, j] = weight;
                }
            }
            else if(i < inputSize)
            {
                state.adjacencies[i, j] = true;
                state.weights[i, j] = weight;
            }
        }

        for(int i = 0; i < totalSize; i++)
        {
            if(i >= inputSize)
            {
                state.biases[i] = bias;
            }
            state.passivity[i] = passivity;
            state.persistence[i] = persistence;
        }

        Brain brain = new Brain(state);
        return brain;
    }

    public static Brain CreateMutation(Brain brain)
    {
        BrainState mutation = brain.state.Clone();
        mutation = MutateShape(mutation);
        MutateValues(mutation);
        ClampStateValues(mutation);
        return new Brain(mutation);
    }

    private static BrainState CreateEmptyState(int inputSize, int outputSize, int totalSize)
    {
        BrainState state = new BrainState(inputSize, outputSize, totalSize, iterationsPerSecond);
        return state;
    }

    private struct NeuronCreationInfo
    {
        public float x;
        public float y;
        public int srcNeuron;

        public NeuronCreationInfo(float x, float y, int srcNeuron)
        {
            this.x = x;
            this.y = y;
            this.srcNeuron = srcNeuron;
        }
    }

    /*
     * Will mutate adjacencies and positions of current neurons
     * It will also try to add new neurons or even remove current
     */
    private static BrainState MutateShape(BrainState state)
    {
        //REMOVE NEURONS
        /*for(int i = state.inputSize + state.outputSize; i < state.totalSize; i++)
        {
            if()
        }*/
        //MOVE NEURONS


        //CREATE NEURONS
        List<NeuronCreationInfo> newNeurons = new List<NeuronCreationInfo>();
        for(int i = 0; i < state.totalSize; i++)
        {
            bool added = false;
            for(int u = 0; u < maxNeuronAdditionTries && !added; u++)
            {
                if (Chance(chanceOfNeuronAddition))
                {
                    double angle = RandomFloat() * 2f * Math.PI;
                    double dist = RandomFloat(minDistance, maxDistance);
                    float x = (float)(dist * Math.Cos(angle)) + state.positions[i,0];
                    float y = (float)(dist * Math.Sin(angle)) + state.positions[i,1];
                    if (IsPositionFeasible(state, x, y, i, newNeurons))
                    {
                        newNeurons.Add(new NeuronCreationInfo(x, y, i));
                        added = true;
                    }
                }
            }
        }

        state = AddNeurons(state, newNeurons);
        //CREATE CONNECTIONS
        for(int i = 0; i < state.totalSize; i++)
        {
            for(int j = state.inputSize; j < state.totalSize; j++)
            {
                if (i == j && !allowSelfLinks) continue; //ignore self connections

                if (state.adjacencies[i, j]) //may remove connection
                {
                    if (!Chance(chanceOfLinkRemoval)) continue;
                    bool isLastConnection = CountConnectedTo(state, j, 2) < 2;
                    if(!isLastConnection)
                        state.adjacencies[i, j] = false;
                }
                else //may add connection
                {
                    float distance = Distance(state.positions[i, 0], state.positions[i, 1], state.positions[j, 0], state.positions[j, 1]);
                    if (!(allowBidirectionalLinks || !state.adjacencies[j, i])) continue;
                    if (!Chance(chanceOfLinkAddition)) continue;
                    if (distance > maxDistance || distance < minDistance) continue;
                    state.adjacencies[i, j] = true;
                    state.weights[i, j] = RandomFloat(minWeight, maxWeight);
                }
            }
        }

        return state;
    }

    private static BrainState AddNeurons(BrainState state, List<NeuronCreationInfo> temp)
    {
        if (temp.Count == 0) return state;
        int totalSize = state.totalSize + temp.Count;
        BrainState newState = CreateEmptyState(state.inputSize, state.outputSize, totalSize);
        state.CopyTo(newState);
        for(int i = 0; i < temp.Count; i++)
        {
            int trueI = state.totalSize + i;
            newState.positions[trueI, 0] = temp[i].x;
            newState.positions[trueI, 1] = temp[i].y;
            newState.adjacencies[temp[i].srcNeuron, trueI] = true;
            newState.weights[temp[i].srcNeuron, trueI] = RandomFloat(minWeight, maxWeight);
            newState.biases[trueI] = RandomFloat(minBias, maxBias);
            newState.passivity[trueI] = RandomFloat(minPassivity, maxPassivity);
            newState.persistence[trueI] = RandomFloat(minPersistence, maxPersistence);
        }
        return newState;
    }

    /*
     * Will mutate weights, biases, passivity and persistence values of all neurons
     */
    private static void MutateValues(BrainState state)
    {
        for(int i = 0; i < state.totalSize; i++)
        {
            state.biases[i] += RandomFloat(maxBiasChange);
            state.passivity[i] += RandomFloat(maxPassivityChange);
            state.persistence[i] += RandomFloat(maxPersistenceChange);

            for (int j = 0; j < state.totalSize; j++)
            {
                if (state.adjacencies[i, j])
                {
                    state.weights[i, j] += RandomFloat(maxWeightChange);
                }
            }
        }
    }

    private static void ClampStateValues(BrainState state)
    {
        for (int i = 0; i < state.totalSize; i++)
        {
            state.biases[i] = Math.Clamp(state.biases[i], minBias, maxBias);
            state.passivity[i] = Math.Clamp(state.passivity[i], minPassivity, maxPassivity);
            state.persistence[i] = Math.Clamp(state.persistence[i], minPersistence, maxPersistence);
            state.persistence[i] = Math.Clamp(state.persistence[i], minPersistence, maxPersistence);
            for (int j = 0; j < state.totalSize; j++)
            {
                state.weights[i, j] = Math.Clamp(state.weights[i, j], minWeight, maxWeight);
            }
        }
    }

    private static float RandomFloat() => (float)rnd.NextDouble();
    private static float RandomFloat(float range) => (RandomFloat() - 0.5f) * 2f * range * randomMultiplier;
    private static float RandomFloat(float min, float max) => RandomFloat() * (max - min) + min;
    private static bool Chance(float chance) => RandomFloat() < chance * randomMultiplier;

    private static bool IsPositionFeasible(BrainState state, float x, float y, int neuronToIgnore = -1, List<NeuronCreationInfo> temp = null)
    {
        for (int i = 0; i < state.totalSize; i++)
        {
            if (i == neuronToIgnore) continue;
            float dist = Distance(state.positions[i, 0], state.positions[i, 1], x, y);
            if (dist < minDistance)
                return false;
        }

        for (int i = 0; i < temp.Count; i++)
        {
            float dist = Distance(temp[i].x, temp[i].y, x, y);
            if (dist < minDistance)
                return false;
        }
        return true;
    }

    private static float Distance(float srcX, float srcY, float dstX, float dstY)
    {
        float dirX = srcX - dstX;
        float dirY = srcY - dstY;
        float dist = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
        return dist;
    }

    private static int CountConnectedTo(BrainState state, int dst, int max)
    {
        int count = 0;
        for(int i = 0; i < state.totalSize; i++)
        {
            if (state.adjacencies[i, dst])
                count++;
            if (count >= max)
                return count;
        }
        return count;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAgentInputBrain : TileAgentInputBase
{
    [HideInInspector] public Brain brain;
    TileRaycast perception;
    float[] perceptionValues;
    [SerializeField] int iterationsPerSecond = 60;
    [SerializeField] int initialMutations = 5;

    private void Awake()
    {
        BrainFactory.iterationsPerSecond = iterationsPerSecond;
        brain = BrainFactory.CreateBrain(1, 1, false);
        for(int i = 0; i < initialMutations; i++)
            brain = BrainFactory.CreateMutation(brain);
        perception = GameObject.FindGameObjectWithTag("TilePerception").GetComponent<TileRaycast>();
        perceptionValues = new float[1];

        // TEST
        /*brain.state.weights[0, 1] = 0.6f;
        brain.state.adjacencies[0, 1] = true;
        brain.state.passivity[0] = 1f;
        brain.state.passivity[1] = 1f;*/


        /*Debug.Log("BEFORE DESERIALIZE");
        string serialized = BrainStateSerializer.Serialize(brain.state);
        Debug.Log(serialized);
        Debug.Log("AFTER DESERIALIZE");
        BrainState deserializedState = BrainStateSerializer.Deserialize(serialized);
        brain.state = deserializedState;
        serialized = BrainStateSerializer.Serialize(brain.state);
        Debug.Log(serialized);*/
    }

    void Update()
    {
        perceptionValues[0] = perception.isWhite ? 1f : 0f;
        brain.Propagate(perceptionValues, Time.deltaTime);
        isWhite = brain.GetOutputAt(0) >= 0.5f;
    }
}
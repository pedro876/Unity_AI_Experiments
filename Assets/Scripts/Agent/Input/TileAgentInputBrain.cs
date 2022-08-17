using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAgentInputBrain : TileAgentInputBase
{
    Brain brain;
    TileRaycast perception;
    float[] perceptionValues;

    private void Awake()
    {
        brain = new Brain(1, 1);
        perception = GameObject.FindGameObjectWithTag("TilePerception").GetComponent<TileRaycast>();
        perceptionValues = new float[1];
    }

    // Update is called once per frame
    void Update()
    {
        perceptionValues[0] = perception.isWhite ? 1f : 0f;
        brain.Propagate(perceptionValues, Time.deltaTime);
        isWhite = brain.GetOutputAt(0) > 0.5f;
    }
}
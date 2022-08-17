using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAgentInputPerfect : TileAgentInputBase
{
    TileRaycast tileRaycast;

    private void Awake()
    {
        tileRaycast = GameObject.FindGameObjectWithTag("TileRaycast").GetComponent<TileRaycast>();
    }

    // Update is called once per frame
    void Update()
    {
        isWhite = tileRaycast.isWhite;
    }
}

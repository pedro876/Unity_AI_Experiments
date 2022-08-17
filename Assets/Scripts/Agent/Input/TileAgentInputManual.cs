using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAgentInputManual : TileAgentInputBase
{

    // Update is called once per frame
    void Update()
    {
        isWhite = Input.GetKey(KeyCode.Space);
    }
}

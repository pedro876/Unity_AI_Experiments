using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    static readonly Color whiteCol = Color.white;
    static readonly Color blackCol = Color.black;
    [HideInInspector] public bool isWhite;
    SpriteRenderer sprite;

    public void RandomizeColor()
    {
        isWhite = Random.Range(0, 2) > 0;
        sprite.color = isWhite ? whiteCol : blackCol;
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
}

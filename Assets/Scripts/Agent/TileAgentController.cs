using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileAgentController : MonoBehaviour
{
    private bool isWhite = false;
    [SerializeField] float maxLifeSeconds = 10;
    [SerializeField] float currentLife = 0;
    TileRaycast tileRaycast;
    SpriteRenderer sprite;
    TileAgentInputBase input;

    public event Action onDead;
    static readonly Color deadColor = Color.red;
    static readonly Color whiteColor = Color.white;
    static readonly Color blackColor = Color.black;

    public void ReplenishHealth()
    {
        currentLife = maxLifeSeconds;
        
    }

    private void SetIsWhite(bool isWhite)
    {
        this.isWhite = isWhite;
        if (currentLife > 0f)
        {
            sprite.color = isWhite ? whiteColor : blackColor;
        }
    }

    private void Awake()
    {
        tileRaycast = GameObject.FindGameObjectWithTag("TileRaycast").GetComponent<TileRaycast>();
        currentLife = maxLifeSeconds;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = blackColor;
        input = GetComponent<TileAgentInputBase>();
    }

    private void LateUpdate()
    {
        SetIsWhite(input.isWhite);

        if (currentLife > 0f)
        {
            if (tileRaycast.isWhite != isWhite)
            {
                currentLife -= Time.deltaTime;
                if (currentLife < 0f)
                {
                    onDead?.Invoke();
                    sprite.color = deadColor;
                }
            }
        }
    }
}

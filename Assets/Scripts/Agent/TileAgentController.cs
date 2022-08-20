using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TileAgentController : MonoBehaviour
{
    [SerializeField] float maxLifeSeconds = 10;
    [SerializeField] float currentLife = 0;
    [SerializeField]TextMeshProUGUI hpText;

    private bool isWhite = false;
    private TileRaycast tileRaycast;
    private SpriteRenderer sprite;
    private TileAgentInputBase input;

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
            if (tileRaycast.detectsSomething && tileRaycast.isWhite != isWhite)
            {
                currentLife -= Time.deltaTime;
                if (currentLife < 0f)
                {
                    onDead?.Invoke();
                    sprite.color = deadColor;
                }
            }

            hpText.text = "HP:"+(((int)(currentLife * 10f)) / 10f).ToString();
        }
    }
}

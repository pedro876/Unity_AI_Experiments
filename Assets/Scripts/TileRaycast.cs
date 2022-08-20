using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRaycast : MonoBehaviour
{
    public bool isWhite = false;
    public bool detectsSomething = false;
    [SerializeField] Color lineCol = Color.red;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f);
        if (hit.collider != null)
        {
            Tile tile = hit.transform.GetComponent<Tile>();
            if (tile != null)
            {
                detectsSomething = true;
                isWhite = tile.isWhite;
            }
            else
                detectsSomething = false;
        }
        else
            detectsSomething = false;
        
        //Debug.Log(isWhiteDown);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = lineCol;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
    }
}

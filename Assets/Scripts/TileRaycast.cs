using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRaycast : MonoBehaviour
{
    public bool isWhite = false;
    [SerializeField] Color lineCol = Color.red;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f);
        Tile tile = hit.transform.GetComponent<Tile>();
        if (tile)
        {
            isWhite = tile.isWhite;
        }
        //Debug.Log(isWhiteDown);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = lineCol;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCarrusel : MonoBehaviour
{

    [SerializeField] GameObject tileProto;
    [SerializeField] int numTiles = 16;
    [SerializeField] float speed = 1f;
    Tile[] tiles;

    private float limitLeft;
    private Vector3 displacementRight;

    void Start()
    {
        tiles = new Tile[numTiles];
        for(int i = 0; i < numTiles; i++)
        {
            var obj = Instantiate(tileProto, transform);
            tiles[i] = obj.GetComponent<Tile>();
            obj.name = "Tile_" + i;
            tiles[i].RandomizeColor();
            obj.transform.localPosition = Vector3.right * i;
        }

        limitLeft = transform.position.x - 0.5f;
        displacementRight = Vector3.right * numTiles;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < numTiles; i++)
        {
            tiles[i].transform.position += Vector3.left * Time.deltaTime * speed;
            if(tiles[i].transform.position.x <= limitLeft)
            {
                tiles[i].transform.position += displacementRight;
                tiles[i].RandomizeColor();
            }
        }
    }
}

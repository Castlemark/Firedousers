using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBehaviour : MonoBehaviour, IBehaviour
{

    /*
	 * INTACT = 0
	 * BURNED = 1
	 */
    private int state;
    public Sprite[] small_sprites;
    public Sprite[] large_top_sprites;
    public Sprite[] large_bottom_sprites;
    public Sprite sprite;

    public void Initialize(int state)
    {
        this.state = 0;
    }

    public void ExecuteBehaviour()
    {

    }

    public bool CanPass()
    {
        return state == 1;
    }

    public bool IsFlammable()
    {
        return true;
    }

    public void SetSprite(int room_tileset = 0)
    {
        Tile parent = this.transform.parent.GetComponent<Tile>();
        Tile[] tiles = parent.GetAdjoiningTiles();

        if (tiles[2] != null && tiles[2].name == "furniture")
        {
            int index = Random.Range(0, large_top_sprites.Length);
            this.GetComponent<SpriteRenderer>().sprite = large_top_sprites[index];
            tiles[2].containedObject.GetComponent<SpriteRenderer>().sprite = large_bottom_sprites[index];

            Tile south_tile = tiles[2].GetAdjoiningTiles()[2];
            if (south_tile != null && south_tile.name == "furniture") south_tile.containedObject.GetComponent<SpriteRenderer>().sprite = small_sprites[Random.Range(0, small_sprites.Length)];
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = small_sprites[Random.Range(0, small_sprites.Length)];
        }
    }
}

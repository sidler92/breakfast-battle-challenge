using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{
    public Sprite emptySprite;
    public Sprite dropSprite;
    public Sprite heartSprite;
    public Sprite ringSprite;
    public Sprite starSprite;
    public Sprite circleSprite;
    public Sprite crossSprite;


    /*


    tiles get value "time Before next y" -> the position of the gameobject is y + this value
    this should get updated on tile changeD? (but thats alot -> maybe decouple and do it in on update of tilespritecontroller) 



    */


    Dictionary<Tile, GameObject> tileGameObjectMap;    

    Board board { get { return BoardController.Instance.board; } }

    // Start is called before the first frame update
    void Start()
    {
        createTileGameObjectMap();
        // Register our callback so that our gameobject gets updated whenever
        // the tile's type changes.
        RegisterCallbackFunctions();
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(0, board.RowOffset);
    }

    void RegisterCallbackFunctions()
    {
        board.RegisterTileChanged(OnTileChanged);
        board.RegisterTilesToBeClearedChanged(OnTilesToBeClearedChanged);
    }

    void createTileGameObjectMap()
    {
        Debug.Log("create tilegameobjectmap");
        //  instantiate our dict that tracks which gameobject is rendering which tile data
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // create a gameobject for each of our tiles on the board, so they show visually
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                // get tile data
                Tile tile_data = board.GetTileAt(x, y);

                // create and add go to our scene
                GameObject tile_go = new GameObject();

                // add tile/GO pair to dict
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.SetParent(this.transform, true);
                tile_go.transform.localPosition = new Vector3(tile_data.X, tile_data.Y, 0);

                // set the right sprite depending on type
                SetSprite(tile_data, tile_go);



                OnTileChanged(tile_data); //callback
            }
        }
    }

    void OnTilesToBeClearedChanged(HashSet<Tile> tilesToBeCleared)
    {
        foreach (Tile tile in tilesToBeCleared)
        {
            // maybe start an animation or sth?
            SpriteRenderer sr = tileGameObjectMap[tile].GetComponent<SpriteRenderer>();
            sr.color = new Color(0.2f, 0.2f, 0.2f, 1);            
        }
    }

    void OnTileChanged(Tile tile_data)
    {

        if (tileGameObjectMap.ContainsKey(tile_data) == false)
        {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data-- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("tileGameObjectMap returned Gameobject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        SetSprite(tile_data, tile_go);
    }


    void SetSprite(Tile tile_data, GameObject tile_go)
    {
        if (tile_go.GetComponent<SpriteRenderer>() == null)
            tile_go.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = tile_go.GetComponent<SpriteRenderer>();
        tile_go.transform.localScale = new Vector3(1, 1, 1);
        sr.sortingLayerName = "Tiles";

        // FIXME
        switch (tile_data.Type)
        {
            case TileType.Empty:
                sr.sprite = null;
                break;
            case TileType.Drop:
                sr.sprite = dropSprite;
                sr.color = new Color(1, 1, 1, 1);
                break;
            case TileType.Star:
                sr.color = new Color(1, 1, 1, 1);
                sr.sprite = starSprite;
                break;
            case TileType.Ring:
                sr.color = new Color(1, 1, 1, 1);
                sr.sprite = ringSprite;
                break;
            case TileType.Heart:
                sr.color = new Color(1, 1, 1, 1);
                sr.sprite = heartSprite;
                break;
            case TileType.Circle:
                sr.sprite = circleSprite;
                sr.color = new Color(1, 1, 1, 1);
                break;
            case TileType.Cross:
                sr.sprite = crossSprite;
                sr.color = new Color(1, 1, 1, 1);
                break;
            default:
                Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
                break;
        }
    }



}

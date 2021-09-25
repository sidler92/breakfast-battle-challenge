using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Sprite cursorSprite;
    public GameObject goTilesAnchor;
    public float cursorOffsetY;

    Cursor cursor;
    GameObject cursor_go;
    Board board { get { return BoardController.Instance.board; } }
    // Start is called before the first frame update
    void Start()
    {
        cursor = board.cursor;

        // create game object cursor
        cursor_go = new GameObject();

        cursor_go.name = "Cursor";
        cursor_go.transform.SetParent(goTilesAnchor.transform, true);
        SetCursorTransform(cursor_go);
        cursor_go.transform.localScale = new Vector3(1f, 1f, 0);

        // Add a sprite renderer, set default to empty
        SpriteRenderer sr = cursor_go.AddComponent<SpriteRenderer>();
        sr.sprite = cursorSprite;
        sr.sortingLayerName = "Cursor";


        // Register our callback so that our gameobject gets updated whenever the cursor's position changes
        cursor.RegisterCursorChanged(OnCursorChanged);
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
    }

    private void GetMovementInput()
    {
        if(Input.GetKeyDown("up"))
        {
            cursor.MoveUp();
        }
        if(Input.GetKeyDown("down"))
        {
            cursor.MoveDown();
        }
        if(Input.GetKeyDown("left"))
        {
            cursor.MoveLeft();
        }
        if(Input.GetKeyDown("right"))
        {
            cursor.MoveRight();
        }
        if(Input.GetKeyDown("space"))
        {
            cursor.SwapTileData();
        }

    }

    // this funciton should be called automatically whenever the cursors position/data gets changed
    void OnCursorChanged(Cursor cursorData)
    {
        if (cursor_go == null)
        {
            Debug.LogError("cursor game object returned null -- did you forget to specify the go? or maybe forget to unregister callback???");
            return;
        }
        SetCursorTransform(cursor_go);
    }

    void SetCursorTransform(GameObject cursor_go)
    {
        cursor_go.transform.localPosition = new Vector3(cursor.X + 0.5f, cursor.Y - cursorOffsetY, 0);
    }
}

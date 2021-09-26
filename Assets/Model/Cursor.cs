using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Cursor
{
    public Board board { get; protected set; }

    private int _x;
    private int _y;
    public int X
    {
        get { return _x; }
        set
        {
            int oldX = _x;
            _x = value;

            // call the callback and let things know we've changed
            if (oldX != _x)
                OnCursorChanged(this);
        }
    }
    public int Y
    {
        get { return _y; }
        set
        {
            int oldY = _y;
            _y = value;

            // call the callback and let things know we've changed
            if (oldY != _y)
                OnCursorChanged(this);
        }
    }

    Tile LeftTile;
    Tile RightTile;


    Action<Cursor> cbCursorChanged;

    public Cursor(Board board)
    {
        this.board = board;
        X = 3;
        Y = 5;
        SetTiles();
    }

    public void RegisterCursorChanged(Action<Cursor> callbackfunc)
    {
        cbCursorChanged += callbackfunc;
    }
    public void UnregisterCursorChanged(Action<Cursor> callbackfunc)
    {
        cbCursorChanged -= callbackfunc;
    }

    void OnCursorChanged(Cursor c)
    {
        // do all the stuff that need to be done when tile changed
        SetTiles();

        // now check if we have registered callback functions
        if (cbCursorChanged == null)
            return;

        // calls all registered "callback functions"
        cbCursorChanged(c);        
    }

    void SetTiles()
    {
        LeftTile = board.GetTileAt(X, Y);
        RightTile = board.GetTileAt(X + 1, Y);
    }

    public void SwapTileData()
    {
        if (board.TilesToBeCleared.Contains(LeftTile) || board.TilesToBeCleared.Contains(RightTile))
            return;
        board.SwapTileData(LeftTile, RightTile);
    }

    public void MoveLeft()
    {
        if (X > 0)
        {
            X--;
        }
    }
    public void MoveRight()
    {
        if (X < board.Width-2)
        {
            X++;
        }
    }
    public void MoveUp()
    {
        if (Y < board.Height-1)
        {
            Y++;
        }
    }
    public void MoveDown()
    {
        if (Y > 0)
        {
            Y--;
        }
    }

}

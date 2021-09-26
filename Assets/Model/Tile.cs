using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType { Empty, Star, Drop, Ring, Heart, Circle, Cross }

public class Tile
{
    Action<Tile> cbTileChanged;

    private TileType _type = TileType.Empty;
    public TileType Type {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;
            // call the callback and let things know we've changed.

            if (oldType != _type)
                OnTileChanged(this);
        } 
    }

    public Board board { get; protected set; }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    // Aciton<Tile> cbTileChanged; //callback action?! maybe we need

    public Tile(Board board, int x, int y)
    {
        this.board = board;
        X = x;
        Y = y;

        // FIXME dont use random here, board should manage how th tiles should look
        // Type = (TileType)(UnityEngine.Random.Range(0, 6) + 1);
    }
    public Tile(Board board, int x, int y, TileType type)
    {
        this.board = board;
        X = x;
        Y = y;

        Type = type;
    }

    // return swapped tiles in has set for further manip
    public HashSet<Tile> SwapData(Tile tile)
    {
        if (Type != tile.Type)
        {
            TileType oldType = Type;
            Type = tile.Type;
            tile.Type = oldType;
        }
        HashSet<Tile> tiles = new HashSet<Tile>();
        tiles.Add(this);
        tiles.Add(tile);
        return tiles;
    }

    public void ClearType()
    {
        this.Type = TileType.Empty;
    }

    public void RegisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged += callbackfunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged -= callbackfunc;
    }

    void OnTileChanged(Tile t)
    {
        // now check if we have registered callback functions
        if (cbTileChanged == null)
            return;

        // calls all registered "callback functions"
        cbTileChanged(t);        
    }


}

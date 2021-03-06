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
    public Row row { get; protected set; }
    public LinkedListNode<Row> rowNode { get; protected set; }

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
    public Tile(Board board, Row row, int x, int y)
    {
        this.board = board;
        this.row = row;
        this.rowNode = rowNode;
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

    public void SetRowNode(LinkedListNode<Row> rowNode)
    {
        this.rowNode = rowNode;
    }

    public void IncrementY()
    {
        Y++;
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

    public Tile GetUpNeighbor()
    {
        if (rowNode.Next != null)
            return rowNode.Next.Value.GetTileAt(this.X);
        return null;
    }
    public Tile GetDownNeighbor()
    {
        if (rowNode.Previous != null)
            return rowNode.Previous.Value.GetTileAt(this.X);
        return null;
    }
    public Tile GetLeftNeighbor()
    {
        if (X > 0)
            return row.GetTileAt(X - 1);
        return null;
    }
    public Tile GetRightNeighbor()
    {
        if (X < row.Width - 1)
            return row.GetTileAt(X + 1);
        return null;
    }
}

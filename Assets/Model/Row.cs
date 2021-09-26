using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Row
{
    public Tile[] tiles { get; protected set; }
    public Board board { get; protected set; }
    public LinkedListNode<Row> rowNode { get; protected set; }
    public int Width { get; protected set; }
    public int Y { get; protected set; }

    Action<Tile> cbTileChanged;

    public Row(int width, int y, Board board)
    {
        Y = y;
        Width = width;
        this.board = board;

        InstantiateRow();
    }

    public void SetRowNode(LinkedListNode<Row> rowNode)
    {
        Debug.Log("Set Rownode to " + rowNode);
        this.rowNode = rowNode;
        // also set rownode to each tile
        foreach (Tile tile in tiles)
        {
            tile.setRowNode(this.rowNode);
        }
    }
    void InstantiateRow()
    {
        tiles = new Tile[Width];
        for (int x = 0; x < Width; x++)
        {
            tiles[x] = new Tile(this.board, this, x, Y); ;
            tiles[x].RegisterTileChanged(OnTileChanged);
        }
    }

    public Tile GetTileAt(int x)
    {
        if (x >= 0 && x < Width)
            return tiles[x];
        return null;
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
        // even do some other stuff when tile change?

        // if we have registered callback funcitons do stuff
        if (cbTileChanged == null)
            return;

        // stuff
        cbTileChanged(t);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is int objAsY))
        {
            return false;
        }

        return objAsY == Y; ;
    }

    public override int GetHashCode()
    {
        return Y;
    }
    /*public override bool Equals(object obj)
{
   if(!(obj is int objAsY))
   {
       return false;
   }

   return objAsY == Y;
}

public override int GetHashCode()
{
   //eturn base.GetHashCode();
   unchecked
   {
       return this.Y;
   }

}*/
}

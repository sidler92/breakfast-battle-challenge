using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row
{
    public Tile[] tiles { get; protected set; }
    public Board board { get; protected set; }
    public int Width { get; protected set; }
    public int Y { get; protected set; }

    public Row(int width, int y, Board board)
    {
        Y = y;
        Width = width;
        this.board = board;

        InstantiateRow();


    }

    void InstantiateRow()
    {
        tiles = new Tile[Width];
        for (int x = 0; x < Width; x++)
        {
            tiles[x] = new Tile(this.board, x, Y);
        }
    }
}

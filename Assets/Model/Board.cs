using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Board
{
    public Tile[,] tiles { get; protected set; }

    public LinkedList<Row> newTiles { get; protected set; }

    public int CurrentClearPoints { get; protected set; }

    public Cursor cursor { get; protected set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public HashSet<Tile> TilesToBeCleared { get; protected set; }

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------
    /*
    
    linked list of "rows" -> a row is just an array of size width
    TODO: update GetTileAt (should not be expensive with get previous -> next of "row"

    each time step we increased a value -> this tells us position on the board

    is this value dependent on current y?
     -> this value goes from 0 to 1 -> if it reaches 1 it should increased y by 1 (and reset back to 0)
    ( this means that each y will not change position too much ? probably?)

    --> this value gets updated with deltatime from the board controller -> you can press button to speed that up

    --> add first new row -> update dict
    --> remove last empty row -> update dict

    if rows.count > Height+1? we at top -> check if last row is empty -> remove -> if not -> we lose
    we can probably check this every time we reach 1 on the (position value)





    */
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------------------------------------------------



    Action<Tile> cbTileChanged;
    Action cbClearStarted;
    Action<int> cbClearFinished;
    Action<HashSet<Tile>> cbTilesToBeClearedChanged;

    public Board(int width, int height)
    {
        CreateBoard(width, height);
    }

    public Board()
    {
        CreateBoard(6, 12);
    }

    public Board(int width, int height, TileType[,] types)
    {
        CreateBoard(width, height);
    }
    /*void CreateBoard(int width, int height)
    {
        CurrentClearPoints = 0;

        Width = width;
        Height = height;

        TilesToBeCleared = new HashSet<Tile>();

        InstantiateTiles(Width, Height);

        Debug.Log("World created with " + (Width * Height) + " tiles.");

        ChangeTilesWithoutClears(6);

        cursor = new Cursor(this);
    }*/

    void CreateBoard(int width, int height)
    {
        CurrentClearPoints = 0;
        Width = width;
        Height = height;

        TilesToBeCleared = new HashSet<Tile>();

        InstantiateTiles(Width, Height);
        Debug.Log("World created with " + (Width * Height) + " tiles");
        Debug.Log("It has " + (newTiles.Count) + " rows");
        Debug.Log("and the rows have " + (newTiles.First.Value.tiles.Length) + "tiles each");

        ChangeTilesWithoutClears(6);

        cursor = new Cursor(this);
    }

    void InstantiateTiles(int width, int height)
    {
        newTiles = new LinkedList<Row>();

        for (int y = 0; y < Height; y++)
        {
            Row currentRow = new Row(Width, y, this);
            LinkedListNode<Row> currentRowNode = newTiles.AddLast(currentRow);
            currentRow.SetRowNode(currentRowNode);            
            currentRow.RegisterTileChanged(OnTileChanged);
        }
        Debug.Log("New Tiles Instantiated");
    }

    /*void InstantiateTiles(int width, int height)
    {
        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileChanged(OnTileChanged); //register callback ty
            }
        }
        Debug.Log("Tiles Instantiated");
    }*/

    /*void ChangeTilesWithoutClears(int height)
    {
        // hash set of all valid types (without empty)
        HashSet<TileType> validTypes = new HashSet<TileType>(Enum.GetValues(typeof(TileType)).Cast<TileType>().ToList());
        validTypes.Remove(TileType.Empty);
        // hash set of all adjacent types
        HashSet<TileType> adjacentTypes = new HashSet<TileType>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // get all adjacent types
                adjacentTypes.Clear();
                adjacentTypes.UnionWith(AdjacentTypes(tiles[x, y]));

                // set type of current tile to one that is in valid types but not in adjacent types -> never have 2 of the same next to eachother
                HashSet<TileType> noDupeTypes = new HashSet<TileType>();
                noDupeTypes.UnionWith(validTypes);
                noDupeTypes.ExceptWith(adjacentTypes);
                tiles[x, y].Type = noDupeTypes.ElementAt(UnityEngine.Random.Range(0, noDupeTypes.Count));
            }
        }
    }*/

    void ChangeTilesWithoutClears(int height)
    {
        // hash set of all valid types (without empty)
        HashSet<TileType> validTypes = new HashSet<TileType>(Enum.GetValues(typeof(TileType)).Cast<TileType>().ToList());
        validTypes.Remove(TileType.Empty);
        // hash set of all adjacent types
        HashSet<TileType> adjacentTypes = new HashSet<TileType>();

        foreach (Row row in newTiles)
        {
            for (int x = 0; x < Width; x++)
            {
                // get all adjacent types
                adjacentTypes.Clear();
                adjacentTypes.UnionWith(AdjacentTypes(row.GetTileAt(x)));

                // set type of current tile to one that is in valid types but not in adjacent types -> never have 2 of the same next to eachother
                HashSet<TileType> noDupeTypes = new HashSet<TileType>();
                noDupeTypes.UnionWith(validTypes);
                noDupeTypes.ExceptWith(adjacentTypes);
                row.GetTileAt(x).Type = noDupeTypes.ElementAt(UnityEngine.Random.Range(0, noDupeTypes.Count));
            }
        }
    }

    HashSet<TileType> AdjacentTypes(Tile tile)
    {
        HashSet<TileType> adjacentTypes = new HashSet<TileType>();
        int x = tile.X;
        int y = tile.Y;

        // TODO: maybe acces over tile -> go row up/down
        if (x > 0)
            adjacentTypes.Add(tile.GetLeftNeighbor().Type);
        if (x < Width - 1)
            adjacentTypes.Add(tile.GetRightNeighbor().Type);
        if (y > 0)
            adjacentTypes.Add(tile.GetDownNeighbor().Type);
        if (y < Height - 1)
            adjacentTypes.Add(tile.GetUpNeighbor().Type);

        return adjacentTypes;
    }

    /*HashSet<TileType> AdjacentTypes(Tile tile)
    {
        HashSet<TileType> adjacentTypes = new HashSet<TileType>();
        int x = tile.X;
        int y = tile.Y;

        if (x > 0)
            adjacentTypes.Add(GetTileAt(x - 1, y).Type);
        if (x < Width - 1) 
            adjacentTypes.Add(GetTileAt(x + 1, y).Type);
        if (y > 0)
            adjacentTypes.Add(GetTileAt(x, y - 1).Type);
        if (y < Height - 1) 
            adjacentTypes.Add(GetTileAt(x, y + 1).Type);

        return adjacentTypes;
    }*/

    /*public void ChangeTiles(TileType[,] types)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y].Type = types[x, y];
            }
        }
    }*/

    public void ChangeTiles(TileType[,] types)
    {
        int y = 0;
        foreach (Row row in newTiles)
        {
            for (int x = 0; x < Width; x++)
            {
                row.GetTileAt(x).Type = types[x, y];
            }
            y++;
        }
    }

    public void RegisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged += callbackfunc;
    }

    public void UnregisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged -= callbackfunc;
    }

    public void RegisterClearStarted(Action callbackfunc)
    {
        cbClearStarted += callbackfunc;
    }
    public void UnregisterClearStarted(Action callbackfunc)
    {
        cbClearStarted += callbackfunc;
    }
    public void RegisterClearFinished(Action<int> callbackfunc)
    {
        cbClearFinished += callbackfunc;
    }
    public void UnregisterClearFinished(Action<int> callbackfunc)
    {
        cbClearFinished += callbackfunc;
    }
     public void RegisterTilesToBeClearedChanged(Action<HashSet<Tile>> callbackfunc)
    {
        cbTilesToBeClearedChanged += callbackfunc;
    }
    public void UnregisterTilesToBeClearedChanged(Action<HashSet<Tile>> callbackfunc)
    {
        cbTilesToBeClearedChanged += callbackfunc;
    }

    // gets called whenever ANY tile changes
    void OnTileChanged(Tile t)
    {
        // even do some other stuff when tile change?

        // if we have registered callback funcitons do stuff
        if (cbTileChanged == null)
            return;

        // stuff
        cbTileChanged(t);
    }

    void OnClearStarted()
    {
        if (cbClearStarted == null)
            return;

        cbClearStarted();
    }
    void OnClearFinished()
    {
        if (cbClearFinished == null)
            return;

        cbClearFinished(CurrentClearPoints);
        CurrentClearPoints = 0;
    }
    void OnTilesToBeClearedChanged()
    {
        if (cbTilesToBeClearedChanged == null)
            return;

        cbTilesToBeClearedChanged(TilesToBeCleared);
    }

    // ---------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------

    /*public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            return null;
        }

        return tiles[x, y];
    }*/

    public Tile GetTileAt(int x, int y)
    {
        newTiles.Find(y);
    }


    public void SwapTileData(Tile leftTile, Tile rightTile)
    {
        HashSet<Tile> swappedTiles = new HashSet<Tile>();

        swappedTiles = leftTile.SwapData(rightTile);

        //TODO: handle filling elsewhere
        //FillColumn(leftTile);
        //FillColumn(rightTile);

        UpdateColumns();
        CheckAndClearTiles(swappedTiles);
    }

    // Checks for each Tile in the set if it has "clears" -> creates a super set of Tiles that can be cleared
    public void CheckAndClearTiles(HashSet<Tile> tiles)
    {
        HashSet<Tile> currentTilesToBeCleared = new HashSet<Tile>();
        // we save all tiles to be cleared in public "super set"
        foreach (Tile currentTile in tiles)
        {
            // GetClearTiles should only return tiles if it actually is a valid clear
            // if not it should be empty
            currentTilesToBeCleared.UnionWith(GetClearTiles(currentTile));
            // AddTilesToBeCleared(GetClearTiles(currentTile));
        }

        AddTilesToBeCleared(currentTilesToBeCleared);
        //now we have our super clear set -> clear all of them?!
        // this will maybe trigger a filling action of the column for every tile in the super set

        // only start clear if tiles to be cleared is not empty
        if(currentTilesToBeCleared.Count > 0)
            OnClearStarted();
    }

    public void UpdateColumns()
    {
        HashSet<Tile> changedTiles = new HashSet<Tile>();
        // TODO: maybe only update columns that have changed lately (bool[])
        for (int x = 0; x < Width ; x++)
        {
            changedTiles.UnionWith(UpdateColumn(x));
        }

        // check and clear all newly generated "clears"
        if(changedTiles.Count > 0)
        {
            CheckAndClearTiles(changedTiles);
        }
    }

    // update column of given x coord ie let tiles "fall" 
    public HashSet<Tile> UpdateColumn(int x)
    {
        List<Tile> fillTiles = new List<Tile>();

        HashSet<Tile> changedTiles = new HashSet<Tile>();

        // save all non empty tiles from bottom to top in fill Tiles list (order matters)
        for (int y = 0; y < Height; y++)
        {
            Tile currentTile = GetTileAt(x, y);
            if (currentTile.Type != TileType.Empty)
                fillTiles.Add(currentTile);
        }

        bool hasChanged = false;
        // set the column to not include empty tiles -> like removing empty tiles
        for (int y = 0; y < Height; y++)
        {
            // ass long there are more tiles in fillTiles
            if (y < fillTiles.Count)
            {
                if (tiles[x, y].Type != fillTiles[y].Type)
                    hasChanged = true;
                tiles[x, y].Type = fillTiles[y].Type;
            }
            else
            {
                tiles[x, y].Type = TileType.Empty;
            }


            // add the current tile  to changed tiles if it has changed and if it is not empty
            if (hasChanged && tiles[x, y].Type != TileType.Empty)
            {
                changedTiles.Add(tiles[x, y]);
            }
        }
        return changedTiles;
    }

    public HashSet<Tile> GetClearTiles(Tile tile)
    {
        TileType clearType = tile.Type;


        HashSet<Tile> clearTilesHorizontal = new HashSet<Tile>();
        HashSet<Tile> clearTilesVertical = new HashSet<Tile>();
        HashSet<Tile> clearTiles = new HashSet<Tile>();

        // return out of this funciton if the clear type is empty - no need to
        if (clearType == TileType.Empty)
            return clearTiles;

        clearTilesHorizontal.UnionWith(getClearTilesLeft(tile, clearType));
        clearTilesHorizontal.UnionWith(getClearTilesRight(tile, clearType));

        if (clearTilesHorizontal.Count > 1)
            clearTiles.UnionWith(clearTilesHorizontal);

        clearTilesVertical.UnionWith(getClearTilesDown(tile, clearType));
        clearTilesVertical.UnionWith(getClearTilesUp(tile, clearType));

        if (clearTilesVertical.Count > 1)
            clearTiles.UnionWith(clearTilesVertical);

        if (clearTilesHorizontal.Count > 1 || clearTilesVertical.Count > 1)
            clearTiles.Add(tile);

        AddPoints(clearTiles.Count);

        return clearTiles;
    }

    public void AddPoints(int numberOfTiles)
    {
        switch (numberOfTiles)
        {
            case 3:
                CurrentClearPoints += 3;
                break;
            case 4:
                CurrentClearPoints += 5;
                break;
            case 5:
                CurrentClearPoints += 8;
                break;
            case 6:
                CurrentClearPoints += 13;
                break;
            case 7:
                CurrentClearPoints += 21;
                break;
            default:
                break;
        }
    }

    void AddTilesToBeCleared(HashSet<Tile> tilesToBeCleared)
    {
        TilesToBeCleared.UnionWith(tilesToBeCleared);
        // callback
        if(TilesToBeCleared.Count>0)
            OnTilesToBeClearedChanged();
    }

    void RemoveTilesToBeCleared()
    {
        TilesToBeCleared = new HashSet<Tile>();
        //callback
        OnTilesToBeClearedChanged();
    }

    // this will make every tile in the set of type empty
    public void ClearTiles()
    {
        foreach (Tile tile in TilesToBeCleared)
        {
            tile.ClearType();
        }

        // remove old cleared tiles from this
        RemoveTilesToBeCleared();
        // callback
        OnClearFinished();
        // wait with dropping them all
        // make room for combo time?
    }


    // recursive checks in different directions! they will return all elements that are the same type as the base in the specified direction
    //-----------------------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------------------
    HashSet<Tile> getClearTilesLeft(Tile tile, TileType clearType)
    {
        HashSet<Tile> clearTiles = new HashSet<Tile>();
        if (tile.X > 0)
        {
            Tile leftTile = this.GetTileAt(tile.X - 1, tile.Y);
            
            if (leftTile.Type == clearType && !TilesToBeCleared.Contains(leftTile))
            {
                clearTiles.UnionWith(getClearTilesLeft(leftTile, clearType));
                clearTiles.Add(leftTile);
            }
        }
        return clearTiles;
    }

    HashSet<Tile> getClearTilesRight(Tile tile, TileType clearType)
    {
        HashSet<Tile> clearTiles = new HashSet<Tile>();
        if (tile.X+1 < Width)
        {
            Tile rightTile = this.GetTileAt(tile.X + 1, tile.Y);

            if (rightTile.Type == clearType && !TilesToBeCleared.Contains(rightTile))
            {
                clearTiles.UnionWith(getClearTilesRight(rightTile, clearType));
                clearTiles.Add(rightTile);
            }
        }
        return clearTiles;
    }

    HashSet<Tile> getClearTilesDown(Tile tile, TileType clearType)
    {
        HashSet<Tile> clearTiles = new HashSet<Tile>();
        if (tile.Y > 0)
        {
            Tile downTile = this.GetTileAt(tile.X, tile.Y - 1);

            if (downTile.Type == clearType && !TilesToBeCleared.Contains(downTile))
            {
                clearTiles.UnionWith(getClearTilesDown(downTile, clearType));
                clearTiles.Add(downTile);
            }
        }
        return clearTiles;
    }

    HashSet<Tile> getClearTilesUp(Tile tile, TileType clearType)
    {
        HashSet<Tile> clearTiles = new HashSet<Tile>();
        if (tile.Y+1 < Height)
        {
            Tile upTile = this.GetTileAt(tile.X, tile.Y+1);

            if (upTile.Type == clearType && !TilesToBeCleared.Contains(upTile))
            {
                clearTiles.UnionWith(getClearTilesUp(upTile, clearType));
                clearTiles.Add(upTile);
            }
        }
        return clearTiles;
    }
 }

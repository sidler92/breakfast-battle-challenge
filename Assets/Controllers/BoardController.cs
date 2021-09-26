using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; protected set; }

    public Board board { get; protected set; }

    public int Points;

    public Text currentPointText;
    public Text currentMultiText;
    public Text totalPointText;
    public Text timerText;
    public float ComboTimer { get; protected set; }
    public float ComboMulti { get; protected set; }
    public bool ComboIsRunning;

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two board controllers?!");
        }
        Instance = this;

        ComboIsRunning = false;
        ComboTimer = 0;
        ComboMulti = 1;
        Points = 0;

        CreateEmptyBoard();
    }

    void Update()
    {
        currentMultiText.text = ("currentMulti: " + ComboMulti.ToString());
        totalPointText.text = ("totalPoints: " + Points.ToString());
        currentPointText.text = ("currentclearPoints: " + board.CurrentClearPoints.ToString());

        timerText.text = ("ComboTimer: " + ComboTimer.ToString());
    } 


    void CreateEmptyBoard()
    {
        // delete old board if it exists -> unregistering callback funcs
        if (board != null)
            DeleteBoard();
        // create board with default tiles
        board = new Board();
        RegisterCallbackFuncitons();
        // camera pos??
    }

    void CreateBoard(TileType[,] types)
    {
        // delete old board if it exists -> unregistering callback funcs
        if (board != null)
        {
            DeleteBoard();
        }
        board = new Board(6, 12, types);
        RegisterCallbackFuncitons();
    }

    private void RegisterCallbackFuncitons()
    {
        board.RegisterClearStarted(OnClearStarted);
        board.RegisterClearFinished(OnClearFinished);
        board.RegisterTilesToBeClearedChanged(OnTilesToBeClearedChanged);
    }

    // unregister all callback funcs tyvm
    void DeleteBoard()
    {  
        board.UnregisterClearStarted(OnClearStarted);
        board.UnregisterClearFinished(OnClearFinished);
        board.UnregisterTilesToBeClearedChanged(OnTilesToBeClearedChanged);
        board = null;
    }


    public void CheckClearsForAllTiles()
    {
        HashSet<Tile> tiles = new HashSet<Tile>();
        foreach (Tile tile in board.tiles)
        {
            tiles.Add(tile);
        }
        board.CheckAndClearTiles(tiles);
    }

    public void SaveBoardToCSV()
    {
        string path = "Assets/BoardSaves/test.txt";
        StreamWriter writer = new StreamWriter(path, false);
        

        for (int y = board.Height-1; y >= 0; y--)
        {
            for (int x = 0; x < board.Width; x++)
            {
                writer.Write("{0}\t", board.GetTileAt(x, y).Type);

            }
            writer.WriteLine();
        }
        writer.Close();
    }

    public void LoadBoardFromCSV()
    {
        
        try
        {
            char[] seperators = { '\t' };
            string[] read;

            // TODO: don't hard code it tyvm
            TileType[,] types = new TileType[6,12];

            using StreamReader sr = new StreamReader("Assets/BoardSaves/test.txt");
            string line;

            int y = 11;

            while ((line = sr.ReadLine()) != null)
            {
                int x = 0;
                read = line.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in read)
                {
                    TileType type = (TileType)Enum.Parse(typeof(TileType), word);
                    types[x, y] = type;
                    x++;
                }
                y--;
            }
            board.ChangeTiles(types);
        } 
        catch (Exception e)
        { 
            Debug.Log("The file could not be read: ");
            Debug.Log(e.Message);
        }
    }

    // everytime we call this maybe add to the combo timer?
    // every time we onclearstarted add 1 second to timer
    void OnClearStarted()
    {
        ComboTimer += 1f;
        
        if (ComboIsRunning)
        {
            ComboMulti += 0.5f;
            return;
        }            
        StartCoroutine(StartClearTilesWithDelay(5f));
    }

    void OnClearFinished(int currentClearPoints)
    {
        Points += (int)(currentClearPoints * ComboMulti);
        ComboMulti = 1;
        StartCoroutine(StartUpdateColumnsWithDelay(0.5f));
    }

    void OnTilesToBeClearedChanged(HashSet<Tile> tilesToBeCleared)
    {
    }

    IEnumerator StartClearTilesWithDelay(float t)
    {
        // start combo
        ComboIsRunning = true;
        ComboTimer = 3f;
        while(ComboTimer >= 0)
        {
            ComboTimer -= Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(t);

        // in this time it is now possible to swap tiles that should be destroyed
        // TODO: set tiles to a state that is not swappable when they are getting cleared!
        board.ClearTiles();
        ComboIsRunning = false;
    }
    IEnumerator StartUpdateColumnsWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        // it is stil possible to move tiles before them dropping down -> intended?
        board.UpdateColumns();
    }
}

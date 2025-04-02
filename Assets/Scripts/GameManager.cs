using PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum PlayerType {Human, RBAgent, RLAgent };

    public PlayerType player1 = PlayerType.Human;
    public PlayerType player2 = PlayerType.Human;

    public int stage = 0;

    public enum Algorithm { BFS, DFS, Astar, None };
    List<string> dropOptions = new List<string> { "Choose an Algorithm", "Breadth First Search", "Depth First Search", "A* Search" };

    public RBAgent rbAgent;

    //player data
    public Algorithm[] selectedAlgos = { Algorithm.None, Algorithm.None }; //player's algorithms
    public int[] tilesRemaining = { 10, 10 };
    public int[] boostsRemaining = { 3, 3 };
    public int currentTurn = 0; //0 for player 1, 1 for player two
    public bool[] playersFinishedEditing = {false, false};

    private int[] boostsScored = {0, 0}; //to keep track of how many boosts scored
    private float[] runTime = {0f, 0f};

    public int placementsPerTurn = 4;
    public int placementsRemaining = 4;


    //ui data
    public Dropdown player1DropDown;
    public Dropdown player2DropDown;

    public TileButtonHandler tileButtonHandler;
    public TileGrid tileGrid;
    public Text stageText;
    public Text player1Info;
    public Text player2Info;

    public GameObject popup;
    public GameObject continueButton;



    // Start is called before the first frame update
    void Start()
    {
        if (player1 == PlayerType.RBAgent)
        {
            player1DropDown.gameObject.SetActive(false); //hide player 1 drop down
            RBAgent rb = rbAgent;
            rb.player = 1; //mark RB as player 1

            Algorithm rbAlgo = rb.ChooseAlgorithm(selectedAlgos); //let RB select an algorithm
            UnityEngine.Debug.Log(rbAlgo);
            selectedAlgos[0] = rbAlgo;
        }
    }

    public void changePlayer1Algo()
    {
        int value_idx = player1DropDown.value;

        string value_text = player1DropDown.options[value_idx].text;

        //Debug.Log("Change player 1 algorithm to: " + value_text);

        updateAlgos(1, value_text);

        //player2DropDown.ClearOptions();
        //List<string> newOptions = new List<string>();

        //foreach (string option in dropOptions)
        //{
        //    if (option != value_text)
        //    {
        //        newOptions.Add(option);
        //    }
        //}

        //player2DropDown.AddOptions(newOptions);
    }

    public void changePlayer2Algo()
    {
        int value_idx = player2DropDown.value;

        string value_text = player2DropDown.options[value_idx].text;

        //Debug.Log("Change player 2 algorithm to: " + value_text);
        updateAlgos(2, value_text);

        //player1DropDown.ClearOptions();
        //List<string> newOptions = new List<string>();

        //foreach (string option in dropOptions)
        //{
        //    if (option != value_text)
        //    {
        //        newOptions.Add(option);
        //    }
        //}

        //player1DropDown.AddOptions(newOptions);
    }

    public void updateAlgos(int player, string algo)
    {
        if (algo == "Breadth First Search")
        {
            selectedAlgos[player - 1] = Algorithm.BFS;
        }

        if (algo == "Depth First Search")
        {
            selectedAlgos[player - 1] = Algorithm.DFS;
        }

        if (algo == "A* Search")
        {
            selectedAlgos[player - 1] = Algorithm.Astar;
        }

        if (algo == "Choose an Algorithm")
        {
            selectedAlgos[player - 1] = Algorithm.None;
        }

        UnityEngine.Debug.Log("Selected Algos" + selectedAlgos[player - 1].ToString());

        if (selectedAlgos[0] == selectedAlgos[1])
        {
            UnityEngine.Debug.Log("No duplicates allowed");
        }
    }

    public void moveToNextStage()
    {
        if (stage == 0 && selectedAlgos.Contains(Algorithm.None) || selectedAlgos[1] == selectedAlgos[0])
        {
            UnityEngine.Debug.Log("Both players must select different algorithm to continue.");
            return;
        }

        if ( stage == 1 && playersFinishedEditing.Contains(false)) //level editing stage can only change after both players are finished editing level
        {
            nextTurn();
            return;
        }

        stage++;
        UnityEngine.Debug.Log("Stage: " + stage.ToString());
        updateStage(stage);
    }

    public void updateStage(int stage)
    {
        if (stage == 1)
        {
            //level editing stage

            //unlock grid
            tileButtonHandler.InteractableGrid(true);

            //remove dropdowns
            player1DropDown.gameObject.SetActive(false);
            player2DropDown.gameObject.SetActive(false);

            player1Info.gameObject.SetActive(true);
            player2Info.gameObject.SetActive(true);

            player1Info.transform.GetChild(0).gameObject.SetActive(true);
            player2Info.transform.GetChild(0).gameObject.SetActive(true);

            //add text
            UpdateInfoText();

            //edit top text
            stageText.text = "Edit the Level!";

            //change button label
            //continueButton.GetComponentInChildren<Text>().text = "Next Turn >>";

            //display pop-up
            Text popupText = popup.GetComponentInChildren<Text>();
            popupText.text = "Player 1's Turn";
            popup.SetActive(true);
            
        }

        if (stage == 2)
        {

            currentTurn = 0; //set to player 1's turn

            //update player info
            UpdateInfoText();

            //hide toggles
            player1Info.transform.GetChild(0).gameObject.SetActive(false);
            player2Info.transform.GetChild(0).gameObject.SetActive(false);

            //edit top text
            stageText.text = "Race Your Algorithms!";

            //run p1 algorithm
            switch (selectedAlgos[0])
            {
                case Algorithm.BFS:
                    UnityEngine.Debug.Log("Run BFS");
                    tileGrid.runBFS = true;
                    break;

                case Algorithm.DFS:
                    UnityEngine.Debug.Log("Run DFS");
                    tileGrid.runDFS = true;
                    break;

                case Algorithm.Astar:
                    UnityEngine.Debug.Log("Run A*");
                    tileGrid.runAStar = true;
                    break;
            }
        }

        if (stage == 3)
        {

            currentTurn = 1 - currentTurn; //flip turn

            //change text of button
            continueButton.GetComponentInChildren<Text>().text = "Restart";

            //run p2 algorithm
            switch (selectedAlgos[1])
            {
                case Algorithm.BFS:
                    UnityEngine.Debug.Log("Run BFS");
                    tileGrid.runBFS = true;
                    break;

                case Algorithm.DFS:
                    UnityEngine.Debug.Log("Run DFS");
                    tileGrid.runDFS = true;
                    break;

                case Algorithm.Astar:
                    UnityEngine.Debug.Log("Run A*");
                    tileGrid.runAStar = true;
                    break;
            }

        }

        if (stage == 4)
        {
            //restart game
            tileGrid.StopPathCoroutine();
            ResetGame();
        }
    }

    public void nextTurn()
    {
        currentTurn = 1 - currentTurn; //flip turn

        //display pop-up
        Text popupText = popup.GetComponentInChildren<Text>();
        popupText.text = "Player " + (currentTurn + 1).ToString() + "'s Turn";
        popup.SetActive(true);

        placementsRemaining = placementsPerTurn; //reset turn placements
    }

    public void UpdateInfoText()
    {
        if (stage == 1)
        {
            player1Info.text = "Your Algorithm: " + selectedAlgos[0] + "\r\nTiles Remaining: " + tilesRemaining[0] +
                "\r\nBoosts Remaining: " + boostsRemaining[0];
            player2Info.text = "Your Algorithm: " + selectedAlgos[1] + "\r\nTiles Remaining: " + tilesRemaining[1] +
                "\r\nBoosts Remaining: " + boostsRemaining[1];
        }

        if (stage >= 2)
        {
            player1Info.text = "Your Algorithm: " + selectedAlgos[0] + "\r\nRun Time: " + runTime[0] +
                            "\r\nBoosts Reached: " + boostsScored[0];
            player2Info.text = "Your Algorithm: " + selectedAlgos[1] + "\r\nRun Time: " + runTime[1] +
                            "\r\nBoosts Reached: " + boostsScored[1];
        }
        
    }

    public void reduceTiles()
    {
        tilesRemaining[currentTurn] -= 1;
        UnityEngine.Debug.Log("Tiles Remaining: " + tilesRemaining[currentTurn]);
        UpdateInfoText();
        placementsRemaining -= 1;
    }

    public void addTiles()
    {
        tilesRemaining[currentTurn] += 1;
        UnityEngine.Debug.Log("Tiles Remaining: " + tilesRemaining[currentTurn]);
        UpdateInfoText();
        placementsRemaining += 1;
    }

    public void reduceBoosts()
    {
        boostsRemaining[currentTurn] -= 1;
        UpdateInfoText();
        placementsRemaining -= 1;
    }

    public void addBoosts()
    {
        boostsRemaining[currentTurn] += 1;
        UpdateInfoText();
        placementsRemaining += 1;
    }

    public void addBoostToScore()
    {
        boostsScored[currentTurn] += 1;
        UnityEngine.Debug.Log("Boost hit!");
        UpdateInfoText();
    }


    //sorry this is kind of ugly and inefficient, I'll change it if I have time later
    public void togglePlayer1Finished()
    {
        playersFinishedEditing[0] = !playersFinishedEditing[0];
    }
    public void togglePlayer2Finished()
    {
        playersFinishedEditing[1] = !playersFinishedEditing[1];
    }

    public void ResetGame()
    {
        //reset UI
        //show dropdowns
        player1DropDown.gameObject.SetActive(true);
        player2DropDown.gameObject.SetActive(true);

        player1DropDown.value = -1;
        player2DropDown.value = -1;

        player1Info.gameObject.SetActive(false);
        player2Info.gameObject.SetActive(false);

        player1Info.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        player2Info.transform.GetChild(0).GetComponent<Toggle>().isOn = false;

        //edit top text
        stageText.text = "Select Your Algorithms!";

        continueButton.GetComponentInChildren<Text>().text = "Continue >>";

        //reset game data
        for (int i = 0; i < 2; i++)
        {
            selectedAlgos[i] = Algorithm.None;
            tilesRemaining[i] = 10;
            boostsRemaining[i] = 3;
            playersFinishedEditing[i] = false;
            boostsScored[i] = 0;
            runTime[i] = 0f;
        }

        placementsRemaining = placementsPerTurn;
        currentTurn = 0;
        stage = 0;

        //reset tile grid

        foreach (var tile in tileGrid.Tiles)
        {
            if (tile.isBoost)
            {
                Vector3 newScale = new Vector3(0.9f, 0.9f, 0.9f);
                tile.ChangePrefab(tileGrid.TilePrefab, newScale, 0.0f);
                tile.SetText("");
                tile.SetColor(tileGrid.TileColor_Default);
            }

            tile.isBoost = false; //reset coins on grid
        }

        tileGrid.TileColor_Expensive = new UnityEngine.Color(0.19f, 0.65f, 0.43f);
        tileGrid.ClearGrid();
        tileGrid.RandomPath();
        tileGrid.RandomizedObstacle();        

        //lock grid
        tileButtonHandler.InteractableGrid(false);
    }

    public void getRunTime(float time)
    {
        runTime[currentTurn] = time;
        UpdateInfoText();
    }

    public void revealWinner()
    {
        if (currentTurn == 1)
        {
            int winner;

            if ((runTime[0] - (boostsScored[0] * 2)) > (runTime[1] - (boostsScored[1] * 2))) //2 seconds is removed for each coin scored
            {
                winner = 2;
            } else
            {
                winner = 1;
            }

            //display pop-up
            Text popupText = popup.GetComponentInChildren<Text>();
            popupText.text = "Player " + winner.ToString() + " wins!";
            popup.SetActive(true);
        }
    }
}

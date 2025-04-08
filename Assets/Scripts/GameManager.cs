using PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int stage = 0;

    public enum Algorithm { BFS, DFS, Astar, None };
    List<string> dropOptions = new List<string> { "Choose an Algorithm", "Breadth First Search", "Depth First Search", "A* Search" };

    public RBAgent rbAgent;
    public RLAgent rlAgent; // Reference to the RLAgent

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

    private bool startAutomation = true;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize the RLAgent
        rlAgent.Initialize();
        
        if (rlAgent.trainingMode) {
            if (player1DropDown != null) player1DropDown.gameObject.SetActive(false);
            if (player2DropDown != null) player2DropDown.gameObject.SetActive(false);
            if (continueButton != null) continueButton.SetActive(false);
           if (popup != null) popup.SetActive(false);
        }
    }

    private void Update() {
        // Yasitha: I needed to wait out 'start' methods of other classes, so had to do it this way.
        if (!startAutomation) return;
        startAutomation = false;
        SetRandomAlgorithms();
    }

    private void SetRandomAlgorithms() {
         // Choose two different algorithms at random.
         List<Algorithm> availableAlgos = new List<Algorithm>() 
         { 
             Algorithm.BFS, 
             Algorithm.DFS, 
             Algorithm.Astar 
         };

         int index = UnityEngine.Random.Range(0, availableAlgos.Count);
         selectedAlgos[0] = availableAlgos[index];
         availableAlgos.RemoveAt(index);

         index = UnityEngine.Random.Range(0, availableAlgos.Count);
         selectedAlgos[1] = availableAlgos[index];

         Debug.Log("Player 1 algorithm: " + selectedAlgos[0].ToString());
         Debug.Log("Player 2 algorithm: " + selectedAlgos[1].ToString());
         nonUIMoveToNextStage();
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

        Debug.Log("Selected Algos" + selectedAlgos[player - 1].ToString());

        if (selectedAlgos[0] == selectedAlgos[1])
        {
            Debug.Log("No duplicates allowed");
        }
    }

    public void moveToNextStage()
    {
        if (stage == 0 && selectedAlgos.Contains(Algorithm.None) || selectedAlgos[1] == selectedAlgos[0])
        {
            Debug.Log("Both players must select different algorithm to continue.");
            return;
        }

        if ( stage == 1 && playersFinishedEditing.Contains(false)) //level editing stage can only change after both players are finished editing level
        {
            nextTurn();
            return;
        }

        stage++;
        Debug.Log("Stage: " + stage.ToString());
        updateStage(stage);
    }

    public void nonUIMoveToNextStage() {
        stage++;
        Debug.Log("Stage: " + stage.ToString());
        nonUIUpdateStage();
    }

    public void nonUIUpdateStage() {
        switch (stage) {
            case 1:
                tileButtonHandler.InteractableGrid(true);
                rlAgent.RequestAction();                             // Making RL agent play the first move
                break;
            // Running player 1's pathfinding algorithm
            case 2:
                currentTurn = 0; 
                switch (selectedAlgos[0]) {
                    case Algorithm.BFS:
                        Debug.Log("Run BFS");
                        tileGrid.runBFS = true;
                        break;

                    case Algorithm.DFS:
                        Debug.Log("Run DFS");
                        tileGrid.runDFS = true;
                        break;

                    case Algorithm.Astar:
                        Debug.Log("Run A*");
                        tileGrid.runAStar = true;
                        break;
                }
                break;
            // Running player 2's pathfinding algorithm
            case 3:
                currentTurn = 1 - currentTurn; 
                switch (selectedAlgos[1]) {
                    case Algorithm.BFS:
                        Debug.Log("Run BFS");
                        tileGrid.runBFS = true;
                        break;

                    case Algorithm.DFS:
                        Debug.Log("Run DFS");
                        tileGrid.runDFS = true;
                        break;

                    case Algorithm.Astar:
                        Debug.Log("Run A*");
                        tileGrid.runAStar = true;
                        break;
                }
                break;
            // Resetting everything at the end of a game
            case 4:
                tileGrid.StopPathCoroutine();
                nonUIResetGame();
                rlAgent.Initialize();
                rlAgent.OnEpisodeBegin();  
                SetRandomAlgorithms();
                break;
        }
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
                    Debug.Log("Run BFS");
                    tileGrid.runBFS = true;
                    break;

                case Algorithm.DFS:
                    Debug.Log("Run DFS");
                    tileGrid.runDFS = true;
                    break;

                case Algorithm.Astar:
                    Debug.Log("Run A*");
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
                    Debug.Log("Run BFS");
                    tileGrid.runBFS = true;
                    break;

                case Algorithm.DFS:
                    Debug.Log("Run DFS");
                    tileGrid.runDFS = true;
                    break;

                case Algorithm.Astar:
                    Debug.Log("Run A*");
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

    public void nonUINextTurn() {
        currentTurn = 1 - currentTurn;
        placementsRemaining = placementsPerTurn;
        
        // if both players have placed everything, move to next stage
        if (AreBothPlayersDone()) {
            nonUIMoveToNextStage();
            return;
        }
        
        // if current player has already placed everything, just switch to the other player
        if (IsPlayerDone()) {
            nonUINextTurn();
            return;
        }
        
        switch (currentTurn) {
            // Make random agent play the turn
            case 1:
                DoRandomOpponentTurn();
                break;
            // Make RL agent play the turn
            case 0:
                rlAgent.RequestDecision();
                break;
        }
    }
    
    /// <summary>
    /// If the player with current turn has nothing more to place, return true.
    /// </summary>
    private bool IsPlayerDone() {
         return tilesRemaining[currentTurn] <= 0 && boostsRemaining[currentTurn] <= 0;
    }
    
    /// <summary>
    /// If both players are finished placing items, returns true.
    /// </summary>
    private bool AreBothPlayersDone() {
        return tilesRemaining[currentTurn] <= 0 && boostsRemaining[currentTurn] <= 0 &&
               tilesRemaining[1 - currentTurn] <= 0 && boostsRemaining[1 - currentTurn] <= 0;
    }
    
    /// <summary>
    /// Since I didn't have a RB agent to go against the RL agent, I made an agent that plays completely randomly. it
    /// will first decide how many items it wants to place (0-4 range). Then randomly picks how many walls and how many
    /// boosts. Then randomly places them in the grid. 
    /// </summary>
    private void DoRandomOpponentTurn() {
         // Decide on a random number of placements (from 1 to placementsPerTurn).
         int intendedPlacements = UnityEngine.Random.Range(1, placementsPerTurn + 1);

         // Decide randomly how many will be walls versus boosts.
         int wallCount = UnityEngine.Random.Range(0, intendedPlacements + 1);
         int boostCount = intendedPlacements - wallCount;

         // Place walls.
         for (int i = 0; i < wallCount; i++) {
             bool placed = false;
             int attempts = 0;
             while (!placed && attempts < 50) {
                 List<Tile> validTiles = new List<Tile>();
                 foreach (Tile tile in tileGrid.Tiles) {
                     if (tile.Weight == TileGrid.TileWeight_Default &&
                         !tile.isBoost &&
                         !tile.start &&
                         !tile.end) {
                         validTiles.Add(tile);
                     }
                 }
                 if (validTiles.Count == 0)
                     break;
                 Tile chosenTile = validTiles[UnityEngine.Random.Range(0, validTiles.Count)];
                 int row = chosenTile.Row;
                 int col = chosenTile.Col;
                 placed = tileButtonHandler.AttemptManualPlacement(row, col, false); // false means wall.
                 attempts++;
             }
         }

         // Place boosts.
         for (int i = 0; i < boostCount; i++) {
             bool placed = false;
             int attempts = 0;
             while (!placed && attempts < 50) {
                 List<Tile> validTiles = new List<Tile>();
                 foreach (Tile tile in tileGrid.Tiles) {
                     if (tile.Weight == TileGrid.TileWeight_Default &&
                         !tile.isBoost &&
                         !tile.start &&
                         !tile.end) {
                         validTiles.Add(tile);
                     }
                 }
                 if (validTiles.Count == 0)
                     break;
                 Tile chosenTile = validTiles[UnityEngine.Random.Range(0, validTiles.Count)];
                 int row = chosenTile.Row;
                 int col = chosenTile.Col;
                 placed = tileButtonHandler.AttemptManualPlacement(row, col, true); // true means boost.
                 attempts++;
             }
         }

         // Passing the turn to the RL agent
         nonUINextTurn();
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
        Debug.Log("Tiles Remaining: " + tilesRemaining[currentTurn]);
        UpdateInfoText();
        placementsRemaining -= 1;
    }

    public void addTiles()
    {
        tilesRemaining[currentTurn] += 1;
        Debug.Log("Tiles Remaining: " + tilesRemaining[currentTurn]);
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
        Debug.Log("Boost hit!");
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

    public void nonUIResetGame()
    {
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
    
    /// <summary>
    /// Moves the stage to the next if it's player 1's turn. At this point, player 1's pathfinding algorithm would have
    /// been run. So moving the stage forward will make player 2's pathfinding algorithm to run.
    ///
    /// If current turn belongs to player 2, it means pathfinding has been completed. Determines the winner and handles
    /// the RL agent's reward for winning/losing. Then move on to next stage (restarts the game).
    /// </summary>
    public void nonUIRevealWinner() {
        if (currentTurn == 0) nonUIMoveToNextStage();
        else {
            int winner;
            // 2 seconds is removed for each boost scored
            winner = (runTime[0] - (boostsScored[0] * 2)) > (runTime[1] - (boostsScored[1] * 2)) ? 2 : 1;
            rlAgent.HandleFinalReward(winner);
            nonUIMoveToNextStage();
        }
    }

    public void revealWinner()
    {
        // Yasitha: Just did this temporarily, should work on a more elegant solution
        if (rlAgent.trainingMode) nonUIRevealWinner();
        else {
            if (currentTurn == 1)
            {
                int winner;

                if ((runTime[0] - (boostsScored[0] * 2)) >
                    (runTime[1] - (boostsScored[1] * 2))) //2 seconds is removed for each coin scored
                {
                    winner = 2;
                }
                else
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
}

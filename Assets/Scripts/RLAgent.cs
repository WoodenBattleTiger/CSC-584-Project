using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using PathFinding;
using Random = UnityEngine.Random;

public class RLAgent : Agent {
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileGrid tileGrid;

    private bool episodeActive;
    public bool trainingMode = true;
    public int player { get; set; }

    public override void Initialize() {
        episodeActive = false;
    }

    public override void OnEpisodeBegin() {
        episodeActive = true;
    }
    
    /// <summary>
    /// Adds observations. Status of the tiles on the board, players' pathfinding algorithms, and item placements
    /// are tracked.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor) {
        if (tileGrid == null || tileGrid.Tiles == null || gameManager == null) {
            throw new Exception("TileGrid, Tiles, or Game Manager is null!");
        }
        
        // Observing the tile grid status
        foreach (Tile t in tileGrid.Tiles) {
            if (t.start) sensor.AddObservation(0f);
            else if (t.end) sensor.AddObservation(1f);
            else if (t.isBoost) sensor.AddObservation(2f);
            
            else if (t.Weight == TileGrid.TileWeight_Expensive) {
                switch (t.owner) {
                    case Tile.Owner.Player1:
                        sensor.AddObservation(3f);
                        break;
                    case Tile.Owner.Player2:
                        sensor.AddObservation(4f);
                        break;
                    default:
                        sensor.AddObservation(5f);
                        break;
                }
            }
            else sensor.AddObservation(6f);
        }
        

        // Observing resource usage and pathfinding algorithms of each player
        sensor.AddObservation((float)gameManager.tilesRemaining[player - 1]);  
        sensor.AddObservation((float)gameManager.boostsRemaining[player - 1]); 
        sensor.AddObservation((float)gameManager.placementsRemaining);
        
        sensor.AddObservation((float)gameManager.tilesRemaining[2 - player]);
        sensor.AddObservation((float)gameManager.boostsRemaining[2- player]);

        sensor.AddObservation((float)gameManager.currentTurn);

        float myAlgo = ConvertAlgoToFloat(gameManager.selectedAlgos[player - 1]);
        float oppAlgo = ConvertAlgoToFloat(gameManager.selectedAlgos[2 - player]);
        sensor.AddObservation(myAlgo);
        sensor.AddObservation(oppAlgo);
    }
    
    /// <summary>
    /// Converts the pathfinding algorithm into an int so it can be passed in as an observation.
    /// </summary>
    /// <param name="algo"> Pathfinding algorithm </param>
    private float ConvertAlgoToFloat(GameManager.Algorithm algo) {
        switch (algo) {
            case GameManager.Algorithm.BFS:   return 0f;
            case GameManager.Algorithm.DFS:   return 1f;
            case GameManager.Algorithm.Astar: return 2f;
            default:                          return 3f;
        }
    }
    
    /// <summary>
    /// Defines rewards for agent's actions.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions) {
        if (!episodeActive) return;
        if (gameManager == null || tileGrid == null || tileGrid.Tiles == null) 
            throw new Exception("TileGrid, Tiles, or Game Manager is null!");

        // Small per-step penalty to discourage wasted actions. This is necessary cos otherwise agent might learn to stall
        AddReward(-0.001f);
        
        // Shouldn't take decisions when it's not their turn. (Ideally this won't happen, but including as a safety measure)
        if (gameManager.currentTurn != player - 1) {
            AddReward(-0.001f);
            return;
        }

        // Deciding first action. I'm taking min here as a safety measure. Agent shouldn't try to place more than what
        // can be place at any given moment.
        int intendedPlacements = Mathf.Min(actions.DiscreteActions[0], gameManager.placementsRemaining);

        int wallsUsed = 0;
        int boostsUsed = 0;

        // For each placement decision (each decision uses 3 actions: row, col, type) 
        for (int i = 0; i < intendedPlacements; i++) {
            int offset = 1 + i * 3;
            // Safety check: if action vector is too short, break out.
            if (offset + 2 >= actions.DiscreteActions.Length) {
                break;
            }
            int row = actions.DiscreteActions[offset];       // Decide row index
            int col = actions.DiscreteActions[offset + 1];   // Decide column index 
            int act = actions.DiscreteActions[offset + 2];   // Decide the type: 0 for wall, 1 for boost

            // Validate row and column. If they picked something out of range, give a penalty.
            if (row < 0 || row >= 15 || col < 0 || col >= 15) {
                AddReward(-0.01f);
                continue;
            }
            
            // If agent try to place an item on an already occupied cell, give a penalty.
            Tile selectedTile = tileGrid.GetTile(row, col);
            if (selectedTile.Weight != TileGrid.TileWeight_Default || selectedTile.isBoost || selectedTile.start ||
                selectedTile.end) {
                AddReward(-0.005f);
                continue;
            }

            // Rewards based on the item they try to place.
            bool placementSuccess = false;
            if (act == 0) {
                // Attempt wall placement. If agent try to place walls more than what's available, gives a penalty.
                // For successful placement, gives a reward.
                if (gameManager.tilesRemaining[player - 1] - wallsUsed > 0) {
                    placementSuccess = gameManager.tileButtonHandler.AttemptManualPlacement(row, col, false);
                    if (placementSuccess) {
                        wallsUsed++;
                        AddReward(+0.01f);
                    }
                    else {
                        AddReward(-0.01f);
                    }
                }
                else {
                    AddReward(-0.01f);
                }
            }
            else if (act == 1) {
                // Attempt boost placement. If agent try to place boosts more than what's available, gives a penalty.
                // For successful placement, gives a reward.
                if (gameManager.boostsRemaining[player - 1] - boostsUsed > 0) {
                    placementSuccess = gameManager.tileButtonHandler.AttemptManualPlacement(row, col, true);
                    if (placementSuccess) {
                        boostsUsed++;
                        AddReward(+0.01f);
                    }
                    else {
                        AddReward(-0.01f);
                    }
                }
                else {
                    AddReward(-0.01f);
                }
            }
            else {
                AddReward(-0.005f);
            }
        }

        // After all decisions are made, pass the turn
        gameManager.nonUINextTurn();
    }
    
    /// <summary>
    /// Rewarding the RL agent if it wins. The reward here is considerably bigger than all other rewards cos we want
    /// the RL agent to learn how to win.
    /// </summary>
    /// <param name="winningPlayerIndex"> The index of the player that won the game (1 or 2) </param>
    public void HandleFinalReward(int winningPlayerIndex) {
        if (!episodeActive) return;

        if (winningPlayerIndex == player) AddReward(+1.0f);
        else AddReward(-1.0f);

        Debug.Log("The winner is player: " + winningPlayerIndex);
        EndEpisode();
        episodeActive = false;
    }
    
    /// <summary>
    /// This is for testing purposes. When running the game on Unity without a trained model, RL agent will use the logic
    /// in this method for decision making.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut) {
        var discrete = actionsOut.DiscreteActions;
        // First action: number of placements to attempt (0-4 range)
        discrete[0] = Random.Range(0, gameManager.placementsPerTurn + 1);
        // For each possible placement, pick a row, col, and type.
        for (int i = 0; i < gameManager.placementsPerTurn; i++) {
            int offset = 1 + i * 3;
            discrete[offset] = Random.Range(0, 15);      // row
            discrete[offset + 1] = Random.Range(0, 15);  // column
            discrete[offset + 2] = Random.Range(0, 2);   // type: 0=wall, 1=boost
        }
    }
}

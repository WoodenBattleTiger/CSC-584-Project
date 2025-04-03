using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static GameManager;

public class ReinforcementLearningAgent : Agent
{
    // Is the agent being trained
    public bool trainingMode;

    // Defines how many simulation steps can occur before the Agent's episode ends
    public int maxStep;

    // Chosen pathfinding method for the game
    private Algorithm pathFinding;

    // Number of tiles to place
    private int tilesPlaced;

    // Number of boosts to place
    private int boostsPlaced;

    // Reference to the GameManager
    private GameManager gameManager;

    /// <summary>
    /// Initialize the agent (overrides ml-agents class)
    /// </summary>
    public override void Initialize()
    {
        trainingMode = true;
        maxStep = 1000;
        pathFinding = Algorithm.None;
        tilesPlaced = 0;
        boostsPlaced = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// How the agent understands the game environment
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // I think this is how we get the current game state
        sensor.AddObservation(gameManager.tilesRemaining[gameManager.currentTurn]);
        sensor.AddObservation(gameManager.boostsRemaining[gameManager.currentTurn]);
        sensor.AddObservation(gameManager.boostsScored[gameManager.currentTurn]);
        sensor.AddObservation(gameManager.runTime[gameManager.currentTurn]);
    }

    /// <summary>
    /// What the agent can do
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        // Convert actions to discrete values
        int action = Mathf.FloorToInt(vectorAction[0]);

        switch (action)
        {
            case 0:
                ChoosePathFindingAlgorithm();
                break;
            case 1:
                PlaceTiles();
                break;
            case 2:
                PlaceBoosts();
                break;
        }

        // Check for win condition
        if (CheckWinCondition())
        {
            SetReward(5f);
            EndEpisode();
        }

        // Check for max steps
        if (StepCount >= maxStep)
        {
            EndEpisode();
        }
    }

    private void ChoosePathFindingAlgorithm()
    {
        // Randomly choose a pathfinding algorithm
        pathFinding = (Algorithm)Random.Range(0, System.Enum.GetValues(typeof(Algorithm)).Length);
        gameManager.SelectPathFindingAlgorithm(pathFinding);
    }

    private void PlaceTiles()
    {
        if (gameManager.tilesRemaining[gameManager.currentTurn] > 0)
        {
            // Randomly choose coordinates to place a tile
            int x = Random.Range(0, gameManager.tileGrid.Width);
            int y = Random.Range(0, gameManager.tileGrid.Height);
            gameManager.PlaceTile(x, y);
        }
    }

    private void PlaceBoosts()
    {
        if (gameManager.boostsRemaining[gameManager.currentTurn] > 0)
        {
            // Randomly choose coordinates to place a boost
            int x = Random.Range(0, gameManager.tileGrid.Width);
            int y = Random.Range(0, gameManager.tileGrid.Height);
            gameManager.PlaceBoost(x, y);
        }
    }

    private bool CheckWinCondition()
    {
        // logic for checking win condition
    }

    /// <summary>
    /// Reset the scene
    /// </summary>
    public override void OnEpisodeBegin()
    {
        pathFinding = Algorithm.None;
        tilesPlaced = 0;
        boostsPlaced = 0;
        gameManager.ResetGame();
    }
}
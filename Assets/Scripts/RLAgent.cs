using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents
using UnityEngine;

public class ReinforcementLearningAgent : Agent
{
    // Is the agent being trained
    public bool trainingMode;

    // Defines how manu simulation stesp can occur beofre the Agent's episode ends
    public int maxStep;

    // chosen path finding method for the game
    private string pathFinding;

    // number of tiles to place
    private int tilesPlaced;

    // number of boosts to place;
    private int boostsPlaced;

    /// <summary>
    /// Initialize the agent (overridees ml-agents class)
    /// </summary>
    public override void InitializAgent()
    {
        trainingMode = true;
        maxStep = 0;
        pathFinding = '';
        tilesPlaced = 0;
        boostsPlaced = 0;
    }

    /// <summary>
    /// How the agent understands the game environment
    /// </summary>
    public override void CollectObservations()
    {
        //grid
        //walls
        //starting point
        //goal
    }

    /// <summary>
    /// What hte agent can do
    /// </summary>
    public override void AgentAction()
    {
        // Functions to implement
        ChoosePathFindingAlgorithm();
        PlaceTiles();
        PlaceBoosts();

        // if (win) {
        //     AddReward(5f);
        //     Done();
        // }
    }

    private void ChoosePathFindingAlgorithm() 
    {

    }

    private void PlaceTiles()
    {

    }

    private voice PlaceBoosts()
    {

    }

    /// <summary>
    /// Reset the scene
    /// </summary>
    public override void AgentReset()
    {
        pathFinding = '';
        tilesPlaced = 0;
        boostsPlaced = 0;
    }
}

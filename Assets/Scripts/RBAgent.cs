using PathFinding;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static GameManager;

public class RBAgent : MonoBehaviour
{

    int player = 2; //is the agent player 1 or player 2

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Algorithm ChooseAlgorithm(Algorithm[] selectedAlgos)
    {
        if (player == 1)
        {
            return Algorithm.Astar; //player 1 selects algorithm first, always choose A*
        } else
        {
            if (selectedAlgos[0] != Algorithm.Astar) return Algorithm.Astar; //if player 1 does not choose A*, choose it
            else
            {
                return Algorithm.BFS; //else choose BFS
            }
        }
    }

    public void PlaceObstacle()
    {
        //place an obstacle tile
    }

    public void PlaceBoost()
    {
        //place an obstacle tile
    }
}

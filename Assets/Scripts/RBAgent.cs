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

    public (int,int) PlaceBoost(TileGrid grid, int sourceRow, int sourceCol, int goalRow, int goalCol, int boostsRemaining)
    {
        //players get 3 boosts

        //place first boost near character in the direction of the goal
        int rowDis = sourceRow - goalRow;
        int colDis = sourceCol - goalCol;

        (int, int) tileLocation = (-1, -1);

        if (boostsRemaining == 3)
        {
            //first boost placement
            if (Mathf.Abs(rowDis) >= Mathf.Abs(colDis))
            {
                //the difference in row takes priority, place boost in row closer to goal
                if (rowDis > 0)
                {
                    //goal is below source
                    tileLocation = (sourceRow - 1, sourceCol);

                }
                else
                {
                    //goal is above source
                    tileLocation = (sourceRow + 1, sourceCol);
                }

            }
            else
            {
                //col difference takes priorty, place boost in col closer to goal
                if (colDis > 0)
                {
                    //goal is to the left of source
                    tileLocation = (sourceRow, sourceCol - 1);
                }
                else
                {
                    //goal is to the right source
                    tileLocation = (sourceRow, sourceCol - 1);
                }
            }
        }
        else if (boostsRemaining == 2)
        {
            if (Mathf.Abs(rowDis) >= Mathf.Abs(colDis))
            {
                //the difference in row takes priority, place boost in row closer to character
                if (rowDis > 0)
                {
                    //goal is below source
                    tileLocation = (goalRow + 1, goalCol);

                }
                else
                {
                    //goal is above source
                    tileLocation = (goalRow - 1, goalCol);
                }

            }
            else
            {
                //col difference takes priorty, place boost in col closer to character
                if (colDis > 0)
                {
                    //goal is to the left of source
                    tileLocation = (goalRow, goalCol + 1);
                }
                else
                {
                    //goal is to the right source
                    tileLocation = (goalRow, goalCol + 1);
                }
            }
        }
        //else
        //{
        //    
        //}

        return tileLocation;
    }
}

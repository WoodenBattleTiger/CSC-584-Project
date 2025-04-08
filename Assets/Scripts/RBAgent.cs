using PathFinding;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static GameManager;

public class RBAgent : MonoBehaviour
{

    int player = 0; //is the agent player 1 or player 2
    public int currentTurn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayer(int playerNumber)
    {
        player = playerNumber;
    }

    public int getPlayer()
    {
        return player;
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

    public (int, int) PlaceObstacle(TileGrid grid, int tilesRemaining, List<Tile> opponentPath)
    {
        //place an obstacle tile
        //since the boost placement will want to place one boost for the first three turns, the RB agent can place only 3 obstacles for those turns

        //pick a random tile from the opponents path and block it
        (int, int) tileToBlock = GetTileFromPath(opponentPath);

        return tileToBlock;
    }

    public (int,int) PlaceBoost(TileGrid grid, int sourceRow, int sourceCol, int goalRow, int goalCol, int boostsRemaining, List<Tile> path)
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
                if (rowDis > 0) tileLocation = (sourceRow - 1, sourceCol); //goal is below source
                else tileLocation = (sourceRow + 1, sourceCol); //goal is above source
            }
            else
            {
                //col difference takes priorty, place boost in col closer to goal
                if (colDis > 0) tileLocation = (sourceRow, sourceCol - 1); //goal is to the left of source
                else tileLocation = (sourceRow, sourceCol - 1); //goal is to the right source
            }
        }
        else if (boostsRemaining == 2)
        {
            if (Mathf.Abs(rowDis) >= Mathf.Abs(colDis))
            {
                //the difference in row takes priority, place boost in row closer to character
                if (rowDis > 0) tileLocation = (goalRow + 1, goalCol); //goal is below source
                else tileLocation = (goalRow - 1, goalCol); //goal is above source
            }
            else
            {
                //col difference takes priorty, place boost in col closer to character
                if (colDis > 0) tileLocation = (goalRow, goalCol + 1); //goal is to the left of source
                else tileLocation = (goalRow, goalCol + 1); //goal is to the right source
            }
        }
        else {

            tileLocation = GetTileFromPath(path);
        }

        return tileLocation;
    }
    private (int, int) GetTileFromPath(List<Tile> path)
    {

        int randomIdx = Random.Range(0, path.Count);

        Tile selectedTile = path[randomIdx];

        (int, int) tileLocation = (selectedTile.Col, selectedTile.Row);

        return tileLocation;
    }
}


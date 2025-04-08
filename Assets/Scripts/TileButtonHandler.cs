using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.ComponentModel.Design;

public class TileButtonHandler : MonoBehaviour
{
    private int originX = -235;
    private int originY = 201;

    public GameObject ButtonPrefab;
    public GameObject coinPrefab;
    public GameObject tilePrefab;
    public TileGrid Grid;

    public bool gridLocked = false;

    UnityEvent placedTile = new UnityEvent();
    UnityEvent removeTile = new UnityEvent();
    UnityEvent placedCoin = new UnityEvent();
    UnityEvent removeCoin = new UnityEvent();

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                GameObject button = Instantiate(ButtonPrefab, transform);
                button.name = $"TileButton({i}, {j})";
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                TileButton buttonScript = button.GetComponent<TileButton>();
                Button buttonComponent = button.GetComponent<Button>();
                rectTransform.localPosition = new Vector2(originX + (i * 29), originY - (j * 29));
                //Debug.Log(rectTransform.localPosition);
                buttonScript.row = j;
                buttonScript.col = i;
                buttonComponent.onClick.AddListener(() => OnButtonPress(buttonScript.row, buttonScript.col));
            }

        }

        //button.position = new Vector2(100, 100);

        if (gridLocked)
        {
            InteractableGrid(false); //false locks, true unlocks
        }

        placedTile.AddListener(gameManager.reduceTiles);
        removeTile.AddListener(gameManager.addTiles);
        placedCoin.AddListener(gameManager.reduceBoosts);
        removeCoin.AddListener(gameManager.addBoosts);
    }

    public void OnButtonPress(int row, int col)
    {
        //Debug.Log("Button pressed at " + row + " " + col);
        var selectedTile = Grid.GetTile(row, col);
        Debug.Log(selectedTile.owner);
        if (selectedTile.Weight == TileGrid.TileWeight_Default && !selectedTile.start && !selectedTile.end && !selectedTile.isBoost)
        {
            if (gameManager.placementsRemaining > 0 && gameManager.tilesRemaining[gameManager.currentTurn] > 0)
            {
                Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Expensive); //add obstacle
                placedTile.Invoke();

                if (gameManager.currentTurn == 0)
                {
                    selectedTile.owner = Tile.Owner.Player1;
                    //selectedTile.SetColor(Grid.TileColor_Player1);
                } else
                {
                    selectedTile.owner = Tile.Owner.Player2;
                    //selectedTile.SetColor(Grid.TileColor_Player2);
                }
            } else if (gameManager.boostsRemaining[gameManager.currentTurn] > 0 && gameManager.placementsRemaining > 0)
            {
                //add coin
                //Debug.Log("Add coin");
                Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove weight
                selectedTile.ChangePrefab(coinPrefab, new Vector3(0.7f, 0.7f, 0.7f), 0.0f, "coin");
                selectedTile.isBoost = true;
                selectedTile.owner = (gameManager.currentTurn == 0) ? Tile.Owner.Player1 : Tile.Owner.Player2;
                placedCoin.Invoke();

            } else
            {
                Debug.Log("No placements remaining");
            }
            
        }
        else if (selectedTile.Weight == TileGrid.TileWeight_Expensive && !selectedTile.start && !selectedTile.end && selectedTile.owner != Tile.Owner.Level)
        {
            if (gameManager.placementsRemaining >= 0 && gameManager.boostsRemaining[gameManager.currentTurn] > 0)
            {
                if (gameManager.currentTurn == 0 && selectedTile.owner == Tile.Owner.Player1) //can only modify owned tiles
                {
                    //Debug.Log("Add coin");
                    Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove weight
                    selectedTile.ChangePrefab(coinPrefab, new Vector3(0.7f, 0.7f, 0.7f), 0.0f, "coin");
                    selectedTile.isBoost = true;
                    removeTile.Invoke();
                    placedCoin.Invoke();
                }

                if (gameManager.currentTurn == 1 && selectedTile.owner == Tile.Owner.Player2)
                {
                    //Debug.Log("Add coin");
                    Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove weight
                    selectedTile.ChangePrefab(coinPrefab, new Vector3(0.7f, 0.7f, 0.7f), 0.0f, "coin");
                    selectedTile.isBoost = true;
                    removeTile.Invoke();
                    placedCoin.Invoke();
                }

            } else if (gameManager.placementsRemaining < 0)
            {
                Debug.Log("No placements remaining"); //not sure if this ever gets used
            } else
            {
                if (gameManager.currentTurn == 0 && selectedTile.owner == Tile.Owner.Player1 || gameManager.currentTurn == 1 && selectedTile.owner == Tile.Owner.Player2)
                {
                    //Debug.Log("No coins remaining");
                    selectedTile.isBoost = false;
                    Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove tile
                    selectedTile.ChangePrefab(tilePrefab, new Vector3(0.9f, 0.9f, 0.9f), 0.0f);
                    selectedTile.SetColor(Grid.TileColor_Default);
                    removeTile.Invoke();
                }
            }
        }
        else if (selectedTile.isBoost && !selectedTile.start && !selectedTile.end && selectedTile.owner != Tile.Owner.Level)
        {

            if (gameManager.currentTurn == 0 && selectedTile.owner == Tile.Owner.Player1)
            {
                selectedTile.isBoost = false;
                Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove tile
                selectedTile.ChangePrefab(tilePrefab, new Vector3(0.9f, 0.9f, 0.9f), 0.0f);
                selectedTile.SetColor(Grid.TileColor_Default);
                removeCoin.Invoke();
            }

            if (gameManager.currentTurn == 1 && selectedTile.owner == Tile.Owner.Player2)
            {
                selectedTile.isBoost = false;
                Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default); //remove tile
                selectedTile.ChangePrefab(tilePrefab, new Vector3(0.9f, 0.9f, 0.9f), 0.0f);
                selectedTile.SetColor(Grid.TileColor_Default);
                removeCoin.Invoke();
            }


        }

        Grid.ResetGrid();
    }

    public void InteractableGrid(bool which)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject tile = gameObject.transform.GetChild(i).gameObject;
            Button tileButton = tile.GetComponent<Button>();
            tileButton.interactable = which;
        }
    }
    
    /// <summary>
    /// Called by "random agent" or RL code to place an item in grid (with no UI click). This method returns true if the
    /// item was successfully placed, false otherwise.
    ///
    /// Trying to place something out of bounds, or on locations that already have items is considered invalid.
    /// </summary>
    public bool AttemptManualPlacement(int row, int col, bool placeBoost) {
        if (gameManager == null || Grid == null) return false;

        Tile selectedTile = Grid.GetTile(row, col);
        if (selectedTile == null) return false;

        // If something is already there, return false
        if (selectedTile.Weight != TileGrid.TileWeight_Default ||
            selectedTile.start || selectedTile.end || selectedTile.isBoost) {
            return false;
        }
        
        // If allocated number of items are already placed, return false
        if (gameManager.placementsRemaining <= 0) return false;

        // PLacing a wall
        if (!placeBoost) {
            if (gameManager.tilesRemaining[gameManager.currentTurn] <= 0) return false;

            Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Expensive);
            selectedTile.owner = (gameManager.currentTurn == 0) ? Tile.Owner.Player1 : Tile.Owner.Player2;
            placedTile.Invoke();
        }
        // Placing a boost
        else {
            if (gameManager.boostsRemaining[gameManager.currentTurn] <= 0) return false;

            Grid.CreateExpensiveArea(row, col, 1, 1, TileGrid.TileWeight_Default);
            selectedTile.ChangePrefab(coinPrefab, new Vector3(0.7f, 0.7f, 0.7f), 0f, "coin");
            selectedTile.isBoost = true;
            selectedTile.owner = (gameManager.currentTurn == 0) ? Tile.Owner.Player1 : Tile.Owner.Player2;
            placedCoin.Invoke();
        }
        
        gameManager.placementsRemaining--;
        Grid.ResetGrid();
        return true;
    }
}

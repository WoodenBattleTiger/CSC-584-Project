using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;
using System.Drawing;
using UnityEngine.UI;
using System.Text;
using PathFinding;
using UnityEngine.Events;

namespace PathFinding
{
    
    public class TileGrid : MonoBehaviour
    {
        public const int TileWeight_Default = 1;
        public const int TileWeight_Expensive = 50;
        private const int TileWeight_Infinity = int.MaxValue;

        public TimeDisplay timeDisplay;
        public TimeDisplay timeDisplay2;

        public int Rows;
        public int Cols;
        public GameObject TilePrefab;
        public GameObject DFSPrefab;
        public GameObject AstarPrefab;
        public GameObject StartTilePrefab;
        public GameObject EndTilePrefab;
        public GameObject TileButtonPrefab;

        public Text summaryText;


        public GameObject AstarTileColor;

        // Static flag to track button press
        private static bool buttonPressed = false;
        
        public Tile start;
        public Tile end;

        //int count = 0;
        // Warning since count is not used

        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public int oldStartX;
        public int oldStartY;
        public int oldEndX;
        public int oldEndY;
        public int row;
        public int col;
        public int col2;
        public int val;

        private float totalBFSTime = 0f;
        private bool bfsPressed = false;
        private int bfsAttempts = 0;
        private float avgBfsTime = 0f;
        private int bfsRuns = 0;

        private float totalDFSTime = 0f;
        private bool dfsPressed = false;
        private int dfsAttempts = 0;
        private float avgDfsTime = 0f;
        private int dfsRuns = 0;

        private float totalASTARTime = 0f;
        private bool astarPressed = false;
        private int astarAttempts = 0;
        private float avgAstarTime = 0f;
        private int astarRuns = 0;

        public float[] playerRunTimes = { 0f, 0f };
        
        //change from unity events to csharp events for consistency
        UnityEvent coinScored = new UnityEvent();

        public delegate void SendRunTime(float runtime);
        public event SendRunTime sendRunTime;

        public delegate void RevealWinner();
        public event RevealWinner revealWinner;



        public UnityEngine.Color TileColor_Default = new UnityEngine.Color(0.7450981f, 0.7450981f, 0.7450981f);
        public UnityEngine.Color TileColor_Expensive = new UnityEngine.Color(0.19f, 0.65f, 0.43f);
        public UnityEngine.Color TileColor_Infinity = new UnityEngine.Color(0.37f, 0.37f, 0.37f);
        public UnityEngine.Color TileColor_Start = new UnityEngine.Color(0.7450981f, 0.7450981f, 0.7450981f);
        public UnityEngine.Color TileColor_End = new UnityEngine.Color(0.7450981f, 0.7450981f, 0.7450981f);
        public UnityEngine.Color TileColor_Path = new UnityEngine.Color(0.2f, 0.1f, 3.0f);
        public UnityEngine.Color TileColor_Visited = new UnityEngine.Color(0.75f, 0.55f, 0.38f);
        public UnityEngine.Color TileColor_Frontier = new UnityEngine.Color(0.4f, 0.53f, 0.8f);
        public UnityEngine.Color TileColor_Frontier1 = new UnityEngine.Color(0.66f, 0.86f, 0.86f);

        public UnityEngine.Color TileColor_Player1 = new UnityEngine.Color(0.75f, 0.0f, 0.0f);
        public UnityEngine.Color TileColor_Player2 = new UnityEngine.Color(0.0f, 0.05f, 0.75f);

        private Vector3 end_scale = new Vector3(0.7f, 0.7f, 0.7f);

        public bool runBFS = false;
        public bool runDFS = false;
        public bool runAStar = false;
       
        public Tile[] Tiles { get; private set; }

        private IEnumerator _pathRoutine;

        public GameManager gameManager;

        private void Awake()
        {

            Tiles = new Tile[Rows * Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeight_Default);
                    Vector3 newScale = new Vector3(0.9f, 0.9f, 0.9f);
                    tile.InitGameObject(transform, TilePrefab, newScale);

                    int index = GetTileIndex(r, c);
                    Tiles[index] = tile;
                }
            }

            StartUp();
        }

        public void StartUp()
        {

            ClearGrid();

            // Original values before randomization
            CreateExpensiveArea(3, 3, 9, 1, TileWeight_Expensive);
            CreateExpensiveArea(3, 11, 1, 9, TileWeight_Expensive);

            // Original value before randomization
            startX = 9;
            startY = 2;
            start = GetTile(9, 2);
            endX = 7;
            endY = 14;
            end = GetTile(7, 14);

            Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 scale1 = new Vector3(0.4f, 0.4f, 0.4f);
            start.ChangePrefab(StartTilePrefab, scale, 0.0f, "start");
            end.ChangePrefab(EndTilePrefab, end_scale, 0.0f, "end");
            end.SetText("");

            ResetGrid();

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

            sendRunTime += gameManager.getRunTime;
            revealWinner += gameManager.revealWinner;
            coinScored.AddListener(gameManager.addBoostToScore);
        }

        //Randomizes the start and end points on the grid
        public void RandomPath(){
            

            if (start != null)
            {
                Vector3 newScale = new Vector3(0.9f, 0.9f, 0.9f);
                start.ChangePrefab(TilePrefab, newScale, 0.0f);
                start.SetText("");
                start.SetColor(TileColor_Default);
            }

            if (end != null)
            {
                Vector3 newScale = new Vector3(0.9f, 0.9f, 0.9f);
                end.ChangePrefab(TilePrefab, newScale, 0.0f);
                end.SetText("");
                end.SetColor(TileColor_Default);

            }

             do {
                startX = Random.Range(0,14);
                startY = Random.Range(0,14);

            } while(GetTile(startX, startY).Weight != 1);

            do {
                endX = Random.Range(0,14);
                endY = Random.Range(0,14);

            } while(GetTile(endX, endY).Weight != 1);


            Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 scale1 = new Vector3(0.4f, 0.4f, 0.4f);
            start = GetTile(startX, startY);
            end = GetTile(endX, endY);
            start.ChangePrefab(StartTilePrefab, scale, 0.0f, "start");
            end.ChangePrefab(EndTilePrefab, end_scale, 0.0f, "end");
            end.SetText("");

            
        }
        
        //Randomizes the obstacles on the grid
        public void RandomizedObstacle(){

            Tiles = new Tile[Rows * Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeight_Default);
                    Vector3 newScale = new Vector3(0.9f, 0.9f, 0.9f);
                    tile.InitGameObject(transform, TilePrefab, newScale);

                    int index = GetTileIndex(r, c);
                    Tiles[index] = tile;

                }
            }


            Vector3 scale = new Vector3(0.9f, 0.9f, 0.9f);
            start.ChangePrefab(TilePrefab, scale, 0.0f, "start");
            start.SetColor(start.Grid.TileColor_Default);
            start.SetText("");
            end.ChangePrefab(TilePrefab, end_scale, 0.0f, "end");
            end.SetColor(end.Grid.TileColor_Default);
            end.SetText("");

            //Sets the original start and end point values if the (0,0) coordinate is produced for the points
            if(startX == 0 & startY == 0){
                start = GetTile(9, 2);
                end = GetTile(7, 14);
            } else {
                start = GetTile(startX, startY);  
                end = GetTile(endX, endY); 
            }

            row = Random.Range(0,10);
            col = Random.Range(0,10);
            col2 = Random.Range(0,10);
            val = Random.Range(6,9);

            //Makes sure the obstales dont overlap with the start and end points
            do {
                row = Random.Range(0,10);
            } while(row == startX || row == endX);

            do {
                col = Random.Range(0,10);
            } while(col == startY || col == endY);

            do {
                col2 = Random.Range(0,10);
            } while(col2 == startY || col2 == endY);

            
            CreateExpensiveArea(row, col2, val, 1, TileWeight_Expensive);
            CreateExpensiveArea(row, col, 1, val, TileWeight_Expensive);

            ResetGrid();

            Vector3 pathScale = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 scale1 = new Vector3(0.4f, 0.4f, 0.4f);
            start.ChangePrefab(StartTilePrefab, pathScale, 0.0f, "start");
            start.SetText("");
            end.ChangePrefab(EndTilePrefab, end_scale, 0.0f, "end");
            end.SetText("");

        }

        

        public void DisplaySummary()
        {
            TimeDisplay[] timers = new TimeDisplay[] { timeDisplay /* Add other timers if needed */ };

            StringBuilder summary = new StringBuilder("Pathfinding Summary\n");

            // Displays the average times for each algorithm
            summary.AppendLine($"\nAverage Times: ");
            if(bfsRuns == 1) {
                summary.AppendLine($"\nBreadth First Search: {avgBfsTime} seconds ({bfsRuns} run)");
            } else {
                summary.AppendLine($"\nBreadth First Search: {avgBfsTime} seconds ({bfsRuns} runs)");
            }

            if(astarRuns == 1) {
                summary.AppendLine($"\nA* Search: {avgAstarTime} seconds ({astarRuns} run)");
            } else {
                summary.AppendLine($"\nA* Search: {avgAstarTime} seconds ({astarRuns} runs)");
            }

            if(dfsRuns == 1) {
                summary.AppendLine($"\nDepth First Search: {avgDfsTime} seconds ({dfsRuns} run)");
            } else {
                summary.AppendLine($"\nDepth First Search: {avgDfsTime} seconds ({dfsRuns} runs)");
            }

            // Updates the UI Text
            summaryText.text = summary.ToString();
        }

        
        private void Update()
        {

        if(!buttonPressed) {

            if(runBFS) {

                TileColor_Expensive = new UnityEngine.Color(0.9215686f,0.2705882f,0.372549f);
                TileColor_Visited = new UnityEngine.Color(0.7294118f,0.8431373f,0.9137255f);
                TileColor_Frontier = new UnityEngine.Color(0.5841936f, 0.6037736f, 0.4471342f); 
                TileColor_Path = new UnityEngine.Color(0.1686275f, 0.2039216f, 0.4039216f);
                
                
                StopPathCoroutine();
    
                TimeDisplay timeDisplay = GetComponent<TimeDisplay>();
        
                if (timeDisplay == null)
                {
                    timeDisplay = gameObject.AddComponent<TimeDisplay>();
                }

                //Start Timer
                startTime();
                
                // Sets the button as pressed
                bfsPressed = true;
                
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_BFS); 
                Vector3 pathScale = new Vector3(0.5f, 0.5f, 0.5f);
                start.ChangePrefab(StartTilePrefab, pathScale, 0.0f, "start");
                start.SetText("");
                StartCoroutine(_pathRoutine);

                buttonPressed = true;
                
                // Stops timer
                timeDisplay.StopTimer();
               
                // Destroys timer
                Destroy(timeDisplay);

                //reset runBFS
                runBFS = false;

            } 
            else if (runAStar)
            {
                TileColor_Expensive = new UnityEngine.Color(0.0627451f, 0.02745098f,0.1254902f);
                TileColor_Visited = new UnityEngine.Color(1f, 0.7607843f, 0.2352941f);
                TileColor_Frontier = new UnityEngine.Color(0.9803922f,0.1843137f,0.7098039f); 
                TileColor_Path = new UnityEngine.Color(0.1921569f,0.03137255f,0.4823529f);

                TimeDisplay timeDisplay = GetComponent<TimeDisplay>();
        
                if (timeDisplay == null)
                {
                    timeDisplay = gameObject.AddComponent<TimeDisplay>();
                    
                }
                
                //Start Timer
                startTime();

                astarPressed = true;
                Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
                start.ChangePrefab(AstarPrefab, scale, 0.0f, "start");
                StopPathCoroutine();
                ChangeAllTilesSpeed(0.1f);
                
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_AStar);
                StartCoroutine(_pathRoutine);
               
                //Stop timer
                timeDisplay.StopTimer();

                //Destroys timer
                Destroy(timeDisplay);

                buttonPressed = true;

                //reset runAStar
                runAStar = false;
            }
            else if (runDFS)
            {

                
                TimeDisplay timeDisplay = GetComponent<TimeDisplay>();
        
                if (timeDisplay == null)
                {
                    timeDisplay = gameObject.AddComponent<TimeDisplay>();
    
                }
                
                //Start Timer
                startTime();

                dfsPressed = true;

                TileColor_Expensive = new UnityEngine.Color(0.1333333f,0.6509804f,0.6f);
                TileColor_Visited = new UnityEngine.Color(0.7294118f,0.7450981f,0.1333333f);
                TileColor_Frontier = new UnityEngine.Color(0.9490196f, 0.5921569f, 0.1529412f); 
                TileColor_Path = new UnityEngine.Color(0.9490196f, 0.2980392f, 0.2392157f);

                Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
                start.ChangePrefab(DFSPrefab, scale, 0.0f, "start");
                StopPathCoroutine();
                
                
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_DFS);
               
                StartCoroutine(_pathRoutine);

                //Stop timer
                timeDisplay.StopTimer();
               
                // Destorys timer
                Destroy(timeDisplay);

                buttonPressed = true;

                //reset runDFS
                runDFS = false;

                }
            else if (Quit.QuitbuttonPressed == true)
            {
                StopPathCoroutine();
                ResetGrid();
                
                Vector3 scale = new Vector3(0.9f, 0.9f, 0.9f);
                start.ChangePrefab(TilePrefab, scale, 0.0f);
                start.SetColor(start.Grid.TileColor_Frontier);
                start.SetText("");
                end.ChangePrefab(TilePrefab, scale, 0.0f);
                end.SetColor(end.Grid.TileColor_Frontier);
                end.SetText("");

                //Awake();
                StartUp();
                start.SetText("");
                end.SetText("");

                ResetGrid();

                buttonPressed = true;
                
            }

            // else if (TimeMetric.timeMetricButtonPressed == true) {
            //     DisplaySummary();
            // }

            //Randomizes obstacle pattern if button is pressed
            if (RandomObstacle.RandomObstaclebuttonPressed == true)
            {
                TileColor_Expensive = new UnityEngine.Color(0.19f, 0.65f, 0.43f);
                
                StopPathCoroutine();
                ResetGrid();
                RandomizedObstacle(); 
                    
                RandomObstacle.RandomObstaclebuttonPressed = false;
                buttonPressed = true;

                // totalBFSTime = 0;
                // totalDFSTime = 0;
                // totalASTARTime = 0;
                // avgBfsTime = 0;
                // avgDfsTime = 0;
                // avgAstarTime = 0;
                // bfsAttempts = 0;
                // dfsAttempts = 0;
                // astarAttempts = 0;
                // bfsRuns = 0;
                // dfsRuns = 0;
                // astarRuns = 0;
                
            } 

            //Randomize start and end point if button is pressed
            if (RandomizePath.RandomizePathbuttonPressed == true){

                
                TileColor_Expensive = new UnityEngine.Color(0.19f, 0.65f, 0.43f);
                StopPathCoroutine();
                ResetGrid();
                
                RandomPath();
                RandomizePath.RandomizePathbuttonPressed = false;
                buttonPressed = true;

                // totalBFSTime = 0;
                // totalDFSTime = 0;
                // totalASTARTime = 0;
                // avgBfsTime = 0;
                // avgDfsTime = 0;
                // avgAstarTime = 0;
                // bfsAttempts = 0;
                // dfsAttempts = 0;
                // astarAttempts = 0;
                // bfsRuns = 0;
                // dfsRuns = 0;
                // astarRuns = 0;

            }

            DisplaySummary();

            buttonPressed = false;
        }
        
               
        }

        // Used to start timer after button press
        public void startTime() {

            timeDisplay.StartTimer();
           
        }

        // Used to stop timer after button press
        public void stopTime() {

            timeDisplay.StopTimer();
        }


        public void StopPathCoroutine()
        {
            if (_pathRoutine != null)
            {
                StopCoroutine(_pathRoutine);
                _pathRoutine = null;
            }

            
        }

        public void ChangeAllTilesSpeed(float newSpeed)
        {
            foreach (var tile in Tiles)
            {
                tile.moveSpeed = newSpeed;
                Debug.Log($"Tile ({tile.Row}, {tile.Col}) - MoveSpeed: {tile.moveSpeed}");
            }
        }

        public void CreateExpensiveArea(int row, int col, int width, int height, int weight)
        {
            for (int r = row; r < row + height; r++)
            {
                for (int c = col; c < col + width; c++)
                {
                    Tile tile = GetTile(r, c);
                    if (tile != null)
                    {
                        tile.Weight = weight;
                        if (tile.owner == Tile.Owner.None) tile.owner = Tile.Owner.Level;
                    }
                }
            }

        }

        private void ResetGridExpensiveColor(UnityEngine.Color tileExpensiveColor){
            
            foreach (var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
                tile.SetText("");

                switch (tile.Weight)
                {
                    case TileWeight_Default:
                        tile.SetColor(TileColor_Default);
                        break;
                    case TileWeight_Expensive:
                        tile.SetColor(tileExpensiveColor);
                        break;
                    case TileWeight_Infinity:
                        tile.SetColor(TileColor_Infinity);
                        break;
                }

            }
        }

        public void ResetGrid()
        {
           
            foreach (var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
                tile.SetText("");
                //tile.isCoin = false;

                switch (tile.Weight)
                {
                    case TileWeight_Default:
                        tile.SetColor(TileColor_Default);
                        break;
                    case TileWeight_Expensive:
                        if (tile.owner == Tile.Owner.Player1)
                        {
                            tile.SetColor(TileColor_Player1);
                        } else if (tile.owner == Tile.Owner.Player2)
                        {
                            tile.SetColor(TileColor_Player2);
                        } else { tile.SetColor(TileColor_Expensive); }
                        break;
                    case TileWeight_Infinity:
                        tile.SetColor(TileColor_Infinity);
                        break;
                }

            }
        }

        public void ClearGrid()
        {
            foreach (var tile in Tiles)
            {
                CreateExpensiveArea(tile.Row, tile.Col, 1, 1, TileGrid.TileWeight_Default);
            }

            ResetGrid();
        }

        private IEnumerator FindPath(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            // Gets start time of algorithm
            float startTime = UnityEngine.Time.realtimeSinceStartup;

            List<IVisualStep> steps = new List<IVisualStep>();
            List<Tile> path = pathFindingFunc(this, start, end, steps);



            foreach (var step in steps)
            {
                try
                {
                    step.Execute();
                }
                catch (NullReferenceException)
                {
                    gameManager.skipRun();
                }

                yield return new WaitForFixedUpdate();
            }

            timeDisplay.StopTimer();

            // Gets end time of algorithm
            float endTime = UnityEngine.Time.realtimeSinceStartup;

            float temp = endTime - startTime;

            sendRunTime((float)Math.Round(temp, 2));

            foreach (Tile tile in path)
            {
                if (tile.isBoost)
                {
                    //Debug.Log("coin collected");
                    coinScored.Invoke();
                }
            }

            // Calculates average time based on algorithm pressed
            if (bfsPressed)
            {

                float roundedDecimal = (float)Math.Round(temp, 2);

                bfsAttempts++;

                totalBFSTime += roundedDecimal;
                avgBfsTime = totalBFSTime / bfsAttempts;

                bfsPressed = false;

                bfsRuns++;

            }
            else if (dfsPressed)
            {

                float roundedDecimal = (float)Math.Round(temp, 2);

                dfsAttempts++;

                totalDFSTime += roundedDecimal;
                avgDfsTime = totalDFSTime / dfsAttempts;


                dfsPressed = false;

                dfsRuns++;

            }
            else if (astarPressed)
            {

                float roundedDecimal = (float)Math.Round(temp, 2);

                astarAttempts++;

                totalASTARTime += roundedDecimal;
                avgAstarTime = totalASTARTime / astarAttempts;


                astarPressed = false;

                astarRuns++;
            }

            revealWinner();
        }


        public Tile GetTile(int row, int col)
        {
            if (!IsInBounds(row, col))
            {
                return null;
            }

            return Tiles[GetTileIndex(row, col)];
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            Tile right = GetTile(tile.Row, tile.Col + 1);
            if (right != null)
            {
                yield return right;
            }

            Tile up = GetTile(tile.Row - 1, tile.Col);
            if (up != null)
            {
                yield return up;
            }

            Tile left = GetTile(tile.Row, tile.Col - 1);
            if (left != null)
            {
                yield return left;
            }

            Tile down = GetTile(tile.Row + 1, tile.Col);
            if (down != null)
            {
                yield return down;
            }
        }

        public Tile GetNeighbor(Tile tile, int direction)
        {
            if (direction == 0)
            {
                Tile right = GetTile(tile.Row, tile.Col + 1);
                if (right != null)
                {
                    return right;
                }
            }
            else if (direction == 1)
            {
                Tile up = GetTile(tile.Row - 1, tile.Col);
                if (up != null)
                {
                    return up;
                }
            }
            else if (direction == 2)
            {
                Tile left = GetTile(tile.Row, tile.Col - 1);
                if (left != null)
                {
                    return left;
                }
            }
            else if (direction == 3)
            {
                Tile down = GetTile(tile.Row + 1, tile.Col);
                if (down != null)
                {
                    return down;
                }
            }

            return null;
        }

        private bool IsInBounds(int row, int col)
        {
            bool rowInRange = row >= 0 && row < Rows;
            bool colInRange = col >= 0 && col < Cols;
            return rowInRange && colInRange;
        }

        private int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }


    }
}

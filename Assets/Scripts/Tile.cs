using System;
using UnityEngine;
using UnityEngine.UI;

namespace PathFinding
{
    public class Tile
    {
        public GameObject GameObject { get; private set; }
        public TileGrid Grid { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Weight { get; set; }
        public int Cost { get; set; }
        public Tile PrevTile { get; set; }

        private GameObject _gameObject;
        private SpriteRenderer _spriteRenderer;
        private Text _textComponent;

        public float moveSpeed;
        public bool start = false;
        public bool end = false;
        public bool isBoost = false;

        public enum Owner { Player1, Player2, Level, None };
        public Owner owner;

        public Tile(TileGrid grid, int row, int col, int weight)
        {
            Grid = grid;
            Row = row;
            Col = col;
            Weight = weight;
            //moveSpeed = 2.0f;
            owner = Owner.None;
        }

        public Tile(int row, int col)
        {
            Row = row;
            Col = col;
            //moveSpeed = 0.0f; 
            owner = Owner.None;
            
        }


        public void InitGameObject(Transform parent, GameObject prefab, Vector3 scale )
        {
            _gameObject = GameObject.Instantiate(prefab);
            _gameObject.name = $"Tile({Row}, {Col})";
            _gameObject.transform.parent = parent;
            _gameObject.transform.localPosition = new Vector3(Col, -Row, 0.0f);
            _gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
            _textComponent = _gameObject.GetComponentInChildren<Text>();
            _gameObject.transform.localScale = scale;

            //Debug.Log("Tile");
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        public void SetText(string text)
        {
            _textComponent.text = text;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(Col, Row);
        }

        public void ChangePrefab(GameObject newPrefab, Vector3 scale, float newSpeed, String type = null )
        {
            if (_gameObject != null)
            {
                // Clear the old tile appearance
                _spriteRenderer.color = Color.white;
                _textComponent.text = "";
                // Destroy the current GameObject.
                GameObject.Destroy(_gameObject);
            }

            if (type == "start")
            {
                start = true;
            }

            if (type == "end")
            {
                end = true;
            }

            // Create a new GameObject with the specified prefab.
            InitGameObject(Grid.transform, newPrefab, scale);

            moveSpeed = newSpeed;
        }
    }
}


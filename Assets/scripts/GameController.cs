using System.Collections.Generic;
using UnityEngine;

namespace NWSTDio {
    public class GameController : MonoBehaviour {

        public static GameController Instance { get; private set; }

        [SerializeField] private TilePanelUI _tilePanel;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Transform _parentTiles;
        [SerializeField] private Sprite[] _tileSprites;
        [SerializeField] private bool _rotateTiles;
        [SerializeField] private float _collisionRadius = .1f;// радиус правильной позиции тайла

        private readonly List<Tile> _tiles = new();
        private readonly int[] _rotationMask = { 0, 90, 180, 270 };
        private int _completeTile = 0;

        public TilePanelUI TilePanel => _tilePanel;
        public float CollisionRadius => _collisionRadius;
        public bool IsRotateTiles => _rotateTiles;

        #region Unity Methods
        private void Awake() => Instance = this;
        private void Start() {
            for (int i = 0; i < _tileSprites.Length; i++)
                SpawnTile(i);

            ShuffleTiles();

            _tilePanel.UpdateTilesText(_completeTile, _tiles.Count);
            }
        #endregion
        #region Unity Action
        private void CompleteTile() {
            _completeTile++;

            if (_completeTile >= _tiles.Count)
                _tilePanel.ShowWinPanel();

            _tilePanel.UpdateTilesText(_completeTile, _tiles.Count);
            }
        #endregion

        public void MoveToEndList(Tile letter) {
            int index = _tiles.IndexOf(letter);

            if (index == -1)
                return;

            _tiles.RemoveAt(index);
            _tiles.Add(letter);

            SortingTiles();
            }

        public void RemoveTile(Tile tile) {
            tile.OnCompleteTile -= CompleteTile;
            _tiles.Remove(tile);

            SortingTiles();
            }

        private void SortingTiles() {
            for (int i = 0; i < _tiles.Count; i++)
                _tiles[i].UpdateZPosition(i * -.1f);
            }

        private void SpawnTile(int index) {
            var position = new Vector3() {
                x = -5.5f + (index % 12),
                y = 2.5f + (-index / 12),
                z = -(_tiles.Count * .1f)
                };

            Tile tile = Instantiate(_tilePrefab, position, Quaternion.identity, _parentTiles);

            tile.OnCompleteTile += CompleteTile;

            tile.Initialize(_tileSprites[index]);

            _tiles.Add(tile);
            }

        private void ShuffleTiles() {
            foreach (var tile in _tiles) {
                tile.transform.position = new Vector3() {
                    x = Random.Range(-6, 6),
                    y = -4,
                    z = tile.transform.position.z
                    };

                if (IsRotateTiles)
                    tile.Rotate(_rotationMask[Random.Range(0, _rotationMask.Length - 1)]);
                }
            }

        }
    }
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NWSTDio {
    public class TilePanelUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI _tilesText;
        [SerializeField] private Transform _winPanel;
        [SerializeField] private RectTransform _viewPort, _content, _elements;
        [SerializeField] private TileUI _tileUIPrefab;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private int _countTilesInPool = 50;

        private Camera _camera;
        private readonly List<TileUI> _tiles = new();

        #region Unity Methods
        private void Start() {
            _camera = _canvas.worldCamera;

            for (int i = 0; i < _countTilesInPool; i++)
                _tiles.Add(CreateTile());
            }
        #endregion
        #region Tiles Pool
        private TileUI CreateTile() {
            var tile = Instantiate(_tileUIPrefab, _elements);
            tile.name = $"tile_{_tiles.Count}_UI";
            tile.gameObject.SetActive(false);

            return tile;
            }
        private TileUI GetTile() {
            foreach (var item in _tiles) {
                if (item.gameObject.activeSelf == false)
                    return item;
                }

            TileUI tile = CreateTile();

            _tiles.Add(tile);

            return tile;
            }
        #endregion
        #region working with elements
        public void AddElement(Tile tile, bool rotate) {
            var tileUI = GetTile();

            tileUI.transform.SetParent(_content);
            tileUI.transform.SetSiblingIndex(GetInsertIndex());

            tileUI.gameObject.SetActive(true);
            tileUI.Initialize(this, tile, rotate);
            }
        public void RemoveElemet(TileUI tile) {
            tile.transform.SetParent(_elements);
            tile.gameObject.SetActive(false);
            }
        #endregion
        #region Update UI Elements
        public void UpdateTilesText(int completeTile, int countTile) => _tilesText.text = $"{completeTile} / {countTile}";
        public void ShowWinPanel() => _winPanel.gameObject.SetActive(true);
        #endregion

        public bool IsWithinPanel(Transform transform) {
            Vector3[] corners = new Vector3[4];

            _viewPort.GetWorldCorners(corners);

            var panelRect = new Rect(corners[0], corners[2] - corners[0]);

            return panelRect.Contains(transform.position);
            }

        private int GetInsertIndex() {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, Input.mousePosition, _camera, out Vector2 localPoint);

            localPoint += _content.anchoredPosition;

            for (int i = 0; i < _content.childCount; i++) {
                RectTransform child = _content.GetChild(i) as RectTransform;

                if (localPoint.y > child.localPosition.y)
                    return i;
                }

            return _content.childCount;
            }

        }
    }
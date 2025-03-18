using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWSTDio {
    public class TileUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        [SerializeField] private Image _image;

        private TilePanelUI _tilePanelUI;
        private Tile _tile;

        #region Init
        public void Initialize(TilePanelUI panel, Tile tile, bool rotate) {
            _tile = tile;
            _tilePanelUI = panel;
            _image.sprite = _tile.Sprite;

            name = tile.name + "_UI";

            if (rotate)
                transform.eulerAngles = new Vector3(0, 0, tile.CurrentRotation);
            }
        #endregion
        #region Drag Interface
        public void OnBeginDrag(PointerEventData eventData) => ReturnToWorld();
        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }
        #endregion

        private void ReturnToWorld() {
            _tile.ReturnToWorld();

            _tilePanelUI.RemoveElemet(this);
            }

        }
    }
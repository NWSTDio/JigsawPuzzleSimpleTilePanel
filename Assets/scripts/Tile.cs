using System;
using UnityEngine;

namespace NWSTDio {
    public class Tile : MonoBehaviour {

        public event Action OnCompleteTile;

        [SerializeField] private SpriteRenderer _renderer;

        private GameController _controller;
        private Vector3 _offset;
        private bool _isDragging, _isEventDragging;// перемещение через систему Unity, перемещение вызванное вручную
        private float _originalXPos, _originalYPos;
        private float _currentRotation;

        public Sprite Sprite => _renderer.sprite;
        public float CurrentRotation => _currentRotation;

        #region Init
        public void Initialize(Sprite sprite) {
            _controller = GameController.Instance;

            _renderer.sprite = sprite;

            _originalXPos = transform.position.x;
            _originalYPos = transform.position.y;

            gameObject.name = $"{transform.position.x}-:-{transform.position.y}_obj";
            }
        #endregion
        #region Unity Methods
        private void Update() {
            if (_isDragging == false)
                return;

            if (Input.GetMouseButtonUp(1) && _controller.IsRotateTiles)
                Rotation();

            if (_isEventDragging == false) {
                if (Input.GetMouseButtonUp(0)) {
                    EndDrag();

                    return;
                    }
                }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector3() {
                x = mousePos.x + _offset.x,
                y = mousePos.y + _offset.y,
                z = transform.position.z
                };

            SetScale(_controller.TilePanel.IsWithinPanel(transform) ? 1f : 1.25f);
            }
        #endregion
        #region UnityMethods Mouse
        private void OnMouseEnter() => SetScale(1.25f);
        private void OnMouseDown() => BeginDrag(true);
        private void OnMouseUp() => EndDrag();
        private void OnMouseExit() => SetScale(1);
        #endregion
        #region Dragging Methods
        private void BeginDrag(bool isEvent) {
            _offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            _controller.MoveToEndList(this);
            SetScale(1.25f);

            _isDragging = true;
            _isEventDragging = isEvent;
            } // isEvent указывает откуда началось событие начала перемещения, их событий Unity или после перемещения из панели
        private void EndDrag() {
            _isDragging = false;

            SetScale(1f);

            if (_controller.TilePanel.IsWithinPanel(transform)) {
                UpdateZPosition(0);

                _controller.TilePanel.AddElement(this, _controller.IsRotateTiles);

                gameObject.SetActive(false);

                return;
                }

            if (Mathf.Abs(transform.position.x - _originalXPos) <= _controller.CollisionRadius && Mathf.Abs(transform.position.y - _originalYPos) <= _controller.CollisionRadius) {
                if (_controller.IsRotateTiles && _currentRotation != 0)
                    return;

                transform.position = new Vector3() {
                    x = _originalXPos,
                    y = _originalYPos,
                    z = 0
                    };

                gameObject.name += "_completed";

                OnCompleteTile?.Invoke();

                _controller.RemoveTile(this);

                Destroy(GetComponent<BoxCollider2D>());
                Destroy(this);
                }
            }
        #endregion
        #region Rotate Functional
        public void Rotate(int angle) {
            _currentRotation = angle;

            Rotate();
            }
        private void Rotate() => transform.eulerAngles = new Vector3(0, 0, _currentRotation);
        private void Rotation() {
            _currentRotation = (_currentRotation - 90 + 360) % 360;

            Rotate();
            }
        #endregion

        public void ReturnToWorld() {
            gameObject.SetActive(true);

            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            BeginDrag(false);
            }
        public void UpdateZPosition(float z) {
            transform.position = new Vector3() {
                x = transform.position.x,
                y = transform.position.y,
                z = z
                };
            }

        private void SetScale(float scale) {
            transform.localScale = new Vector3() {
                x = scale,
                y = scale,
                z = transform.localScale.z
                };
            }

        }
    }
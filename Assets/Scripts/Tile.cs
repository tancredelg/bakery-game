using UnityEngine;

public class Tile : MonoBehaviour
{
        [SerializeField][Range(0,1)] private float AlphaChangeOnHover;
        [SerializeField][Range(0,1)] private float AlphaChangeOnEdit;
        private SpriteRenderer _spriteRenderer;
        private Collider _collider;
        private HexGrid _grid;
        private Color _colorBefore, _colorOnHover, _colorOnEdit;
        public bool Placed { get; private set; }

        public void Awake()
        {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _collider = GetComponent<Collider>();
                _grid = FindObjectOfType<HexGrid>();
                Placed = true;
        }

        public void InitAsEditable()
        {
                Placed = false;
                _collider.enabled = false;
                _colorBefore = _spriteRenderer.color;
                _colorOnEdit = new Color(_colorBefore.r, _colorBefore.g, _colorBefore.b,
                        _colorBefore.a - AlphaChangeOnEdit);
                _spriteRenderer.color = _colorOnEdit;
        }

        public void Place()
        {
                _spriteRenderer.color = _colorBefore;
                _collider.enabled = true;
                Placed = true;
        }

        private void OnMouseEnter()
        {
                _colorBefore = _spriteRenderer.color;
                _colorOnHover = new Color(_colorBefore.r, _colorBefore.g, _colorBefore.b,
                        _colorBefore.a - AlphaChangeOnHover);
                _spriteRenderer.color = _colorOnHover;
        }

        private void OnMouseExit()
        {
                _spriteRenderer.color = _colorBefore;
        }
}
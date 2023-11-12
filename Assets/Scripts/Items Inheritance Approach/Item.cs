using UnityEngine;

namespace Items_Inheritance_Approach
{
    public class Item : MonoBehaviour
    {
        protected bool Grabbed;
        protected Transform GrabTransform;
        protected Vector3 LastPosition;
    
        protected ItemGrid ItemGrid;
        protected Rigidbody2D Rb;
        protected Collider2D Coll2D;
        private SpriteRenderer _sr;
        private Color _defaultColor;
    
        protected virtual void Awake()
        {
            Grabbed = false;
            ItemGrid = FindObjectOfType<ItemGrid>();
            Rb = GetComponent<Rigidbody2D>();
            Coll2D = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            _defaultColor = _sr.color;
        }

        public void Hover()
        {
            _defaultColor = _sr.color;
            _sr.color = Color.Lerp(_sr.color, Color.black, 0.125f);
        }

        public void UnHover()
        {
            _sr.color = _defaultColor;
        }

        public virtual bool TryGrab(Transform grabTransform)
        {
            Debug.LogError("Item.TryGrab() override not implemented in child class");
            return false;
        }
    
        public virtual bool TryPlace()
        {
            Debug.LogError("Item.TryPlace() override not implemented in child class");
            return false;
        }
    }
}

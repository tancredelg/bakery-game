using UnityEngine;

namespace Items_Inheritance_Approach
{
    public class PropItem : Item
    {
        private SpriteRenderer _shadowSR;

        protected override void Awake()
        {
            base.Awake();
            _shadowSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Grabbed)
            {
                transform.position = GrabTransform.position;
            }
        }

        public override bool TryGrab(Transform grabTransform)
        {
            if (Grabbed) return false;
            if (ItemGrid.TryGetBaseItem(transform.position, out var baseItem)) baseItem.Empty();

            Grab(grabTransform);
            return true;
        }

        public override bool TryPlace()
        {
            if (!Grabbed) return false;
            if (ItemGrid.TryGetBaseItem(transform.position, out var baseItem))
            {
                if (!baseItem.TryPut(this)) return false;
                baseItem.UnHover();
            }
        
            Place();
            return true;
        }
    
        private void Grab(Transform grabberTransform)
        {
            transform.parent = null;
            GrabTransform = grabberTransform;
            _shadowSR.enabled = false;
            Grabbed = true;
        }
    
        private void Place()
        {
            UnHover();
            transform.parent = ItemGrid.transform;
            transform.position = ItemGrid.GetNearestCellWorld(transform.position);
            _shadowSR.enabled = true;
            Grabbed = false;
        }
    }
}
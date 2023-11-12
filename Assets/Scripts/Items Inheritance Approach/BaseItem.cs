using System.Collections;
using UnityEngine;

namespace Items_Inheritance_Approach
{
    public class BaseItem : Item
    {
        public bool Locked;

        private Vector3 _defaultScale;
        private PropItem _propItem;
        private bool _isMoving;

        protected override void Awake()
        {
            base.Awake();
            _defaultScale = transform.localScale;
        }

        private void Update()
        {
            if (Grabbed)
            {
                var snappedPos = ItemGrid.GetNearestCellWorld(GrabTransform.position);
                if (snappedPos != LastPosition && ItemGrid.IsEmpty(snappedPos))
                {
                    StartCoroutine(MoveToPosCR(snappedPos, 0.07f));
                }
            }
        }

        public override bool TryGrab(Transform grabTransform)
        {
            if (Grabbed || Locked) return false;
            if (!ItemGrid.TryRemoveBaseItem(transform.position)) return false;

            Grab(grabTransform);
            return true;
        }

        public override bool TryPlace()
        {
            if (!Grabbed || Locked) return false;
            if (!ItemGrid.TryAddBaseItem(transform.position, this)) return false;

            Place();
            return true;
        }
        
        public void ForcePlace()
        {
            transform.position = ItemGrid.GetNearestCellWorld(transform.position);
            
            if (!ItemGrid.TryAddBaseItem(transform.position, this)) return;

            Place();
        }

        private void Grab(Transform grabberTransform)
        {
            Coll2D.enabled = false;
            transform.parent = null;
            GrabTransform = grabberTransform;
            transform.localScale = 0.9f * _defaultScale;
            Grabbed = true;
        }
        
        private void Place()
        {
            Coll2D.enabled = true;
            UnHover();
            SnapRotation();
            transform.parent = ItemGrid.transform;
            transform.localScale = _defaultScale;
            Grabbed = false;
        }

        private void SnapRotation()
        {
            int newRotation = Mathf.RoundToInt(transform.eulerAngles.z / 60) * 60;
            transform.eulerAngles = new Vector3(0, 0, newRotation);
        }
        
        private IEnumerator MoveToPosCR(Vector3 targetPosition, float timeToMove)
        {
            if (_isMoving) yield break;
            _isMoving = true;
            
            int frames = Mathf.RoundToInt(timeToMove / Time.deltaTime);
            for (int i = 0; i < frames; i++)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (float)i / frames);
                yield return null;
            }
            
            transform.position = targetPosition;
            LastPosition = targetPosition;
            _isMoving = false;
        }

        public bool TryPut(PropItem propItem)
        {
            if (_propItem != null) return false;
            _propItem = propItem;
            return true;
        }
        
        public void Empty()
        {
            _propItem = null;
        }

        public void Lock() => Locked = true;
        public void Unlock() => Locked = false;
    }
}
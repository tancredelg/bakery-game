using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour, IMovable
    {
        public bool Locked;

        private Transform _grabTransform;
        private Vector3 _lastPosition;
        private Prop _prop;

        // State Flags
        private bool _isMoving;
        private bool _isPlaced;

        // Components references
        private BlockGrid _blockGrid;
        private Collider2D _coll2D;
        private SpriteRenderer _sr;
        
        // Component default values
        private Color _defaultColor;
        private Vector3 _defaultScale;
        
        
        private void Awake()
        {
            _blockGrid = FindObjectOfType<BlockGrid>();
            _coll2D = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();

            _defaultColor = _sr.color;
            _defaultScale = transform.localScale;
        
            _isPlaced = true;
        }

        private void Update()
        {
            if (!_isPlaced)
            {
                var snappedPos = _blockGrid.GetNearestCellWorld(_grabTransform.position);
                if (snappedPos != _lastPosition && _blockGrid.IsEmpty(snappedPos))
                {
                    StartCoroutine(MoveToPosCR(snappedPos, 0.07f));
                }
            }
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

        public bool TryGrab(Transform grabTransform)
        {
            if (!_isPlaced || Locked) return false;
            if (!_blockGrid.Remove(transform.position)) return false;

            Grab(grabTransform);
            return true;
        }

        public bool TryPlace()
        {
            if (_isPlaced || Locked) return false;
            if (!_blockGrid.TryAddBlock(transform.position, this)) return false;

            Place();
            return true;
        }
        
        public void ForcePlace()
        {
            transform.position = _blockGrid.GetNearestCellWorld(transform.position);
            
            if (!_blockGrid.TryAddBlock(transform.position, this)) return;

            Place();
        }

        private void Grab(Transform grabberTransform)
        {
            _coll2D.enabled = false;
            transform.parent = null;
            _grabTransform = grabberTransform;
            transform.localScale = 0.9f * _defaultScale;
            _isPlaced = false;
        }
        
        private void Place()
        {
            _coll2D.enabled = true;
            UnHover();
            SnapRotation();
            transform.parent = _blockGrid.transform;
            transform.localScale = _defaultScale;
            _isPlaced = true;
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
            _lastPosition = targetPosition;
            _isMoving = false;
        }

        public bool TryOccupy(Prop prop)
        {
            if (_prop != null) return false;
            _prop = prop;
            return true;
        }
        
        public void Empty()
        {
            _prop = null;
        }

        public void Lock() => Locked = true;
        public void Unlock() => Locked = false;
    }
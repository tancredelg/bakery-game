using UnityEngine;

public class Prop : MonoBehaviour, IMovable
{
    private Transform _grabTransform;
    private Vector3 _lastPosition;
    private Prop _prop;
    
    // State flags
    private bool _isPlaced;
    
    // Component references
    private BlockGrid _blockGrid;
    private SpriteRenderer _sr, _shadowSR;
    
    // Component default values
    private Color _defaultColor;
    
    
    private void Awake()
    {
        _blockGrid = FindObjectOfType<BlockGrid>();
        _sr = GetComponent<SpriteRenderer>();
        _shadowSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _defaultColor = _sr.color;
        
        _isPlaced = true;
    }

    private void Update()
    {
        if (!_isPlaced)
        {
            transform.position = _grabTransform.position;
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
        if (!_isPlaced) return false;
        
        if (_blockGrid.TryGetBlock(transform.position, out var baseItem)) baseItem.Empty();

        transform.parent = null;
        _grabTransform = grabTransform;
        _shadowSR.enabled = false;
        _isPlaced = false;
        
        return true;
    }

    public bool TryPlace()
    {
        if (_isPlaced) return false;
        
        if (_blockGrid.TryGetBlock(transform.position, out var block))
        {
            if (!block.TryOccupy(this)) return false;
            block.UnHover();
        }
        
        UnHover();
        transform.parent = _blockGrid.transform;
        transform.position = _blockGrid.GetNearestCellWorld(transform.position);
        _shadowSR.enabled = true;
        _isPlaced = true;
        
        return true;
    }
}
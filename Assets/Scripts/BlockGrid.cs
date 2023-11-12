using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockGrid : MonoBehaviour
{
    private Grid _grid;
    private Tilemap _tilemap;
    private Dictionary<Vector3Int, Block> _blockGrid;

    private void Awake()
    {
        _grid = GetComponentInParent<Grid>();
        _tilemap = FindObjectOfType<Tilemap>();
        _blockGrid = new Dictionary<Vector3Int, Block>();

        _tilemap.CompressBounds();
    }

    private void Start()
    {
        foreach (var block in FindObjectsOfType<Block>()) block.ForcePlace();
    }

    public Vector3 GetNearestCellWorld(Vector3 worldPosition)
    {
        return _grid.GetCellCenterWorld(_grid.WorldToCell(worldPosition));
    }

    private Vector3Int GetNearestCell(Vector3 worldPosition)
    {
        return _grid.WorldToCell(worldPosition);
    }

    public bool TryAddBlock(Vector3 worldPosition, Block block)
    {
        var cell = GetNearestCell(worldPosition);
        if (!_tilemap.HasTile(cell)) return false;
        return _blockGrid.TryAdd(cell, block);
    }

    public bool Remove(Vector3 worldPosition)
    {
        return _blockGrid.Remove(GetNearestCell(worldPosition));
    }

    public bool TryGetBlock(Vector3 worldPosition, out Block item)
    {
        return _blockGrid.TryGetValue(GetNearestCell(worldPosition), out item);
    }
    
    public bool IsEmpty(Vector3 worldPosition)
    {
        var cell = GetNearestCell(worldPosition);
        if (!_tilemap.HasTile(cell)) return false;
        return !_blockGrid.ContainsKey(cell);
    }
}
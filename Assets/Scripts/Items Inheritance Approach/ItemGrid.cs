using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Items_Inheritance_Approach
{
    public class ItemGrid : MonoBehaviour
    {
        private Grid _grid;
        private Tilemap _tilemap;
        private Dictionary<Vector3Int, BaseItem> _baseItemGrid;

        private void Awake()
        {
            _grid = GetComponentInParent<Grid>();
            _tilemap = FindObjectOfType<Tilemap>();
            _baseItemGrid = new Dictionary<Vector3Int, BaseItem>();

            _tilemap.CompressBounds();
        }

        private void Start()
        {
            foreach (var baseItem in FindObjectsOfType<BaseItem>())
            {
                baseItem.ForcePlace();
            }
        }

        public Vector3 GetNearestCellWorld(Vector3 worldPosition)
        {
            return _grid.GetCellCenterWorld(_grid.WorldToCell(worldPosition));
        }

        private Vector3Int GetNearestCell(Vector3 worldPosition) => _grid.WorldToCell(worldPosition);

        public bool TryAddBaseItem(Vector3 worldPosition, BaseItem baseItem)
        {
            var cell = GetNearestCell(worldPosition);
            if (!_tilemap.HasTile(cell)) return false;
            return _baseItemGrid.TryAdd(cell, baseItem);
        }

        public bool TryRemoveBaseItem(Vector3 worldPosition)
        {
            return _baseItemGrid.Remove(GetNearestCell(worldPosition));
        }

        public bool TryGetBaseItem(Vector3 worldPosition, out BaseItem item)
        {
            return _baseItemGrid.TryGetValue(GetNearestCell(worldPosition), out item);
        }
    
        public bool IsEmpty(Vector3 worldPosition)
        {
            var cell = GetNearestCell(worldPosition);
            if (!_tilemap.HasTile(cell)) return false;
            return !_baseItemGrid.ContainsKey(cell);
        }
    }
}
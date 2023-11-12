using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexGrid : MonoBehaviour
{
    [SerializeField][Range(3, 7)] private int StartSize;
    [SerializeField] private GameObject BaseTilePrefab;
    
    private Grid _grid;
    private Camera _cam;

    private GameObject _centreTile;
    private HashSet<Vector3Int> _generatedCells;

    private bool _editMode;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
        _grid.cellLayout = GridLayout.CellLayout.Hexagon;
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            Generate();

        if (Input.GetKeyDown(KeyCode.E))
            _editMode = !_editMode;

        if (_editMode)
        {
            var cellPos = GetTilePos(_cam.ScreenToWorldPoint(Input.mousePosition));
            if (GetNeighbourCells(cellPos).Any(n => _generatedCells.Contains(n)))
            {
                var tile = GenerateTileAt(cellPos).GetComponent<Tile>();
                tile.InitAsEditable();
            }
        }
    }

    private void Generate()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        
        _generatedCells = new HashSet<Vector3Int>();
        HashSet<Vector3Int> lastCellLayer = new() { Vector3Int.zero };
        
        for (int i = 1; i < StartSize; i++)
        {
            HashSet<Vector3Int> newCellLayer = new();
            foreach (var cell in lastCellLayer)
            {
                foreach (var neighbourCell in GetNeighbourCells(cell))
                {
                    if (lastCellLayer.Contains(neighbourCell) || newCellLayer.Contains(neighbourCell)) continue;
                    newCellLayer.Add(neighbourCell);
                }
            }
            _generatedCells.UnionWith(lastCellLayer);
            lastCellLayer = newCellLayer;
        }
        _generatedCells.UnionWith(lastCellLayer);

        StartCoroutine(InstantiateTiles());
    }

    IEnumerator InstantiateTiles()
    {
        var cells = _generatedCells.ToList();
        for (var i = 0; i < cells.Count; i++)
        {
            GenerateTileAt(cells[i]);
            yield return new WaitForSeconds((float)Math.Asin((float)i / (cells.Count * 20)));
        }
    }

    private GameObject GenerateTileAt(Vector3Int cell)
    {
        var worldPos = _grid.GetCellCenterWorld(cell);
        var tile = Instantiate(BaseTilePrefab, worldPos, Quaternion.identity, transform);
        var tileColor = new Color(0.7f, Random.Range(0.5f, 0.6f), Random.Range(0.3f, 0.4f));
        tile.GetComponent<SpriteRenderer>().color = tileColor;
        return tile;
    }

    private Vector3Int GetTilePos(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(_grid.GetCellCenterLocal(Vector3Int.RoundToInt(worldPos)));
    }

    private List<Vector3Int> GetNeighbourCells(Vector3Int cell)
    {
        var offset = cell.y % 2 == 0 ? Vector3Int.left : Vector3Int.right;
        
        var coords = new List<Vector3Int>
        {
            cell + Vector3Int.left,
            cell + Vector3Int.up,
            cell + Vector3Int.up + offset,
            cell + Vector3Int.right,
            cell + Vector3Int.down + offset,
            cell + Vector3Int.down,
        };
        
        return coords;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static int gridSize = 4;
    private readonly Transform[,] tilePositions = new Transform[gridSize, gridSize];
    private readonly Tile[,] _tiles = new Tile[gridSize, gridSize];

    [SerializeField] private Tile titlePrefab;

    private bool _isAnimating;

    private bool tilesUpdated;

    [SerializeField] private TileSettings tileSettings;
    // Start is called before the first frame update
    void Start()
    {
        GetTilePositions();
        TrySpawnTiles();
        TrySpawnTiles();
        TrySpawnTiles();
        UpdateTilePositions(true);
    }

    // Update is called once per frame
    void Update()
    {
        var xInput = Input.GetAxisRaw("Horizontal");
        var yInput = Input.GetAxisRaw("Vertical");

        if(!_isAnimating)
            TryMove(Mathf.RoundToInt(xInput), Mathf.RoundToInt(yInput));

    }

    public void GetTilePositions()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        int x = 0;
        int y = 0;
        foreach(Transform transform in this.transform)
        {
            tilePositions[x, y] = transform;
            x++;
            if(x>= gridSize)
            {
                x = 0;
                y++;
            }
        }
    }

    private bool TrySpawnTiles()
    {
        List<Vector2Int> availableSpots = new List<Vector2Int>();

        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                if (_tiles[i, j] == null)
                    availableSpots.Add(new Vector2Int(i, j));

        if(!availableSpots.Any())
            return false;

        int randomSpot = Random.Range(0, availableSpots.Count);
        Vector2Int spot = availableSpots[randomSpot];

        var tile = Instantiate(titlePrefab, transform.parent);
        tile.SetValue(GetRandomValue());
        _tiles[spot.x, spot.y] = tile;

        return true;
    }

    private int GetRandomValue()
    {
        var rand = Random.Range(0f, 1f);
        if (rand <= 0.8f)
            return 2;
        else
            return 4;
    }

    private void UpdateTilePositions(bool instant)
    {
        if (!instant)
        {
            _isAnimating = true;
            StartCoroutine(WaitForTileAnimation());
        }
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (_tiles[x, y] != null)
                    _tiles[x, y].SetPosition(tilePositions[x, y].position, instant);
    }

    private IEnumerator WaitForTileAnimation()
    {
        yield return new WaitForSeconds(tileSettings.animationTime);
        _isAnimating = false;
    }

    private void TryMove(int x, int y)
    {

        if (x==0 && y==0)
            return;

        if(Mathf.Abs(x)==1 && Mathf.Abs(y) == 1)
        {
            Debug.LogWarning($"Invalid move {x} {y}");
        }

        tilesUpdated = false;

        if (x == 0)
        {
            if (y > 0)
                TryMoveUp();
            else
                TryMoveDown();
        }
        else
        {
            if (x < 0)
                TryMoveLeft();
            else
                TryMoveRight();
        }

        if(tilesUpdated)
            UpdateTilePositions(false);
    }

    private void TryMoveRight()
    {
        Debug.LogWarning($"IMoved Right");
        for (int y = 0; y < gridSize; y++)
            for (int x = gridSize-1; x >=0; x--)
            {
                if (_tiles[x, y] == null) continue;

                for(int x2 = gridSize-1; x2 > x; x2--)
                {
                    if (_tiles[x2,y]!=null) continue;

                    tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
    }

    private void TryMoveLeft()
    {
        for(int y = 0; y < gridSize; y++)
            for(int x = 0; x<gridSize; x++)
            {
                if( _tiles[x, y] == null) continue;

                for(int x2 = 0; x2<x; x2++)
                {
                    if (_tiles[x2, y] != null) continue;

                    tilesUpdated = true;
                    _tiles[x2,y] = _tiles[x, y];
                    _tiles[x,y] = null;
                    break;
                }
            }
    }

    private void TryMoveDown()
    {
        for(int x=0; x<gridSize; x++)
            for(int y = gridSize-1; y >= 0; y--)
            {
                if(_tiles[x, y] == null) continue;

                for(int y2 =gridSize-1; y2>y; y2--)
                {
                    if (_tiles[x, y2]!= null) continue;

                    tilesUpdated = true;
                    _tiles[x,y2] = _tiles[x, y];
                    _tiles[x,y] = null;
                    break;
                }
            }
    }

    private void TryMoveUp()
    {
        for(int x = 0; x < gridSize; x++)
            for(int y = 0; y < gridSize; y++)
            {
                if (_tiles[x,y]==null) continue;

                for(int y2 = 0; y2<y; y2++)
                {
                    if (_tiles[x,y2]!= null) continue;

                    tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
    }
}

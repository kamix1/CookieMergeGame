using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameField : MonoBehaviour
{

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapOverlay;
    [SerializeField] private Tilemap tilemapVisual;
    [SerializeField] private Tile tileCookie;
    [SerializeField] private Tile tileToast;
    [SerializeField] private Tile tileMafin;
    [SerializeField] private Tile tilePancake;

    public Tilemap Tilemap => tilemap;
    public Tilemap TilemapOverlay => tilemapOverlay;
    public Tilemap TilemapVisual => tilemapVisual;

    [SerializeField] public Tile tileEmpty;
    [SerializeField] public Tile cookie;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }



    public void UpdateTileMap(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                Tilemap.SetTile(cell.cellPosition, GetTile(cell));
            }
        }
    }

    public void UpdateVisual(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                TilemapVisual.SetTile(cell.cellPosition, GetTileVisual(cell));
            }
        }
    }

    public Tile GetTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.CellType.empty:
                return tileEmpty;
            default: return null;
        }
    }
    public Tile GetTileVisual(Cell cell)
    {
        switch (cell.cookieType)
        {
            case Cell.CookieType.cookie:
                return tileCookie;
            case Cell.CookieType.toast:
                return tileToast;
            case Cell.CookieType.mafin:
                return tileMafin;
            case Cell.CookieType.pankeki:
                return tilePancake;
            default: return null;
        }
    }

    public void ShowPreviewTile(Vector3Int position, Tile tile)
    {
        tilemapOverlay.ClearAllTiles(); // убираем старый превью
        tilemapOverlay.SetTile(position, tile); // ставим новый
    }


    public void ClearPreviewTile()
    {
        tilemapOverlay.ClearAllTiles();
    }

    public Tile GetPreviewTileFor(string name)
    {
        if (name == "cookie")
        {
            return cookie;
        }
        else if (name == "toast")
        {
            return tileToast;
        }
        else if (name == "mafin")
        {
            return tileMafin;
        }
        else if (name == "pancake")
        {
            return tilePancake;
        }
        else
        {
            return tileEmpty;
        }
    }
}

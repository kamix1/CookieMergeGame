using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameField : MonoBehaviour
{
    public static GameField Instance { get; private set; }

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapOverlay;
    [SerializeField] private Tilemap tilemapVisual;
    [SerializeField] private Tile tileCookie;
    [SerializeField] private Tile tileToast;
    [SerializeField] private Tile tileMafin;
    [SerializeField] private Tile tilePancake;
    [SerializeField] private Tile tileCake;
    [SerializeField] private Tile tileDoubleCake;
    [SerializeField] private Tile tileDoubleGlazurCake;
    [SerializeField] private Tile tileGingerbreadMan;
    [SerializeField] private Tile tileGingerbreadManAlive;
    [SerializeField] private Tile tileGingerbreadJumperAlive;
    [SerializeField] private Tile tileGingerbreadManCoctail;
    [SerializeField] private Tile tileGingerbreadManSet;
    [SerializeField] private Tile tileMixer;
    [SerializeField] private Tile tileMicrowave;

    [SerializeField] private Sprite spriteCookie;
    [SerializeField] private Sprite spriteToast;
    [SerializeField] private Sprite spriteMafin;
    [SerializeField] private Sprite spritePancake;
    [SerializeField] private Sprite spriteCake;
    [SerializeField] private Sprite spriteDoubleCake;
    [SerializeField] private Sprite spriteDoubleGlazurCake;
    [SerializeField] private Sprite spriteGingerbreadMan;
    [SerializeField] private Sprite spriteGingerbreadManAlive;
    [SerializeField] private Sprite spriteGingerbreadJumperAlive;
    [SerializeField] private Sprite spriteGingerbreadManCoctail;
    [SerializeField] private Sprite spriteGingerbreadManSet;
    [SerializeField] private Sprite spriteMixer;
    [SerializeField] private Sprite spriteMicrowave;
    public Tilemap Tilemap => tilemap;
    public Tilemap TilemapOverlay => tilemapOverlay;
    public Tilemap TilemapVisual => tilemapVisual;

    [SerializeField] public Tile tileEmpty;
    [SerializeField] public Tile tilePlate;
    [SerializeField] public Tile cookie;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
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

    public void UpdateVisualCookies(Cell[,] cellsArray, CellVisual[,] cellVisuals)
    {
        int width = cellsArray.GetLength(0);
        int height = cellsArray.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(cellsArray[x,y].cookieType != Cell.CookieType.gingerbreadManAlive && cellsArray[x, y].cookieType != Cell.CookieType.gingerbreadJumperAlive)
                cellVisuals[x,y].SetSprite(GetSpriteVisual(cellsArray[x, y]));
            }
        }
    }
    public void UpdateVisualGingerbreads(Cell[,] cellsArray, CellVisual[,] cellVisuals)
    {
        int width = cellsArray.GetLength(0);
        int height = cellsArray.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cellsArray[x, y].cookieType == Cell.CookieType.gingerbreadManAlive || cellsArray[x, y].cookieType == Cell.CookieType.gingerbreadJumperAlive)
                    cellVisuals[x, y].SetSprite(GetSpriteVisual(cellsArray[x, y]));
            }
        }
    }



    public Tile GetTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.CellType.empty:
                return tileEmpty;
            case Cell.CellType.plate:
                return tilePlate;
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
            case Cell.CookieType.cake:
                return tileCake;
            case Cell.CookieType.doubleCake:
                return tileDoubleCake;
            case Cell.CookieType.doubleGlazurCake:
                return tileDoubleGlazurCake;
            case Cell.CookieType.gingerbreadManAlive:
                return tileGingerbreadManAlive;
            case Cell.CookieType.gingerbreadJumperAlive:
                return tileGingerbreadJumperAlive;
            case Cell.CookieType.microwave:
                return tileMicrowave;
            case Cell.CookieType.mixer:
                return tileMixer;
            case Cell.CookieType.gingerbreadMan:
                return tileGingerbreadMan;
            case Cell.CookieType.gingerbreadManCoctail:
                return tileGingerbreadManCoctail;
            case Cell.CookieType.gingerbreadManSet:
                return tileGingerbreadManSet;
            default: return null;
        }
    }

    public Sprite GetSpriteVisual(Cell cell)
    {
        switch (cell.cookieType)
        {
            case Cell.CookieType.cookie:
                return spriteCookie;
            case Cell.CookieType.toast:
                return spriteToast;
            case Cell.CookieType.mafin:
                return spriteMafin;
            case Cell.CookieType.pankeki:
                return spritePancake;
            case Cell.CookieType.cake:
                return spriteCake;
            case Cell.CookieType.doubleCake:
                return spriteDoubleCake;
            case Cell.CookieType.doubleGlazurCake:
                return spriteDoubleGlazurCake;
            case Cell.CookieType.gingerbreadManAlive:
                return spriteGingerbreadManAlive;
            case Cell.CookieType.gingerbreadJumperAlive:
                return spriteGingerbreadJumperAlive;
            case Cell.CookieType.microwave:
                return spriteMicrowave;
            case Cell.CookieType.mixer:
                return spriteMixer;
            case Cell.CookieType.gingerbreadMan:
                return spriteGingerbreadMan;
            case Cell.CookieType.gingerbreadManCoctail:
                return spriteGingerbreadManCoctail;
            case Cell.CookieType.gingerbreadManSet:
                return spriteGingerbreadManSet;
            default:
                return null;
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
        else if (name == "gingerbreadManAlive")
        {
            return tileGingerbreadManAlive;
        }
        else if (name == "gingerbreadJumperAlive")
        {
            return tileGingerbreadJumperAlive;
        }
        else if (name == "mixer")
        {
            return tileMixer;
        }
        else if (name == "microwave")
        {
            return tileMicrowave;
        }
        else
        {
            return tileEmpty;
        }
    }
}

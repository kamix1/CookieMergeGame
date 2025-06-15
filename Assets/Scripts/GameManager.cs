using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Cell[,] cellsArray;
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private GameField gameField;
    private string nextPlaceble;
    private string[] cookies = { "cookie", "toast", "mafin", "pancake", "gingerbreadMan"};
    private double[] cookiesProbability = { 0.8, 0.9, 0.95, 1 };
    Vector3Int clickedCellPosition;

    void Start()
    {
        cellsArray = new Cell[height, width];
        GenerateEmptyField(height, width);
        gameField.UpdateTileMap(cellsArray);
        GeneratePlacibleObject();
    }

    private void GenerateEmptyField(int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = new();
                cell.isPlayable = true;
                cell.cookieType = Cell.CookieType.unknown;
                cell.cellPosition = new Vector3Int(x, y);
                cell.type = Cell.CellType.empty;
                cell.isEmpty = true;
                cellsArray[y, x] = cell;
            }
        }
    }

    private void GeneratePlacibleObject()
    {
        System.Random random = new System.Random();
        double number = random.NextDouble();

        if (number < 0.6) nextPlaceble = cookies[0];
        else if (number < 0.8) nextPlaceble = cookies[4];
        else if (number < 0.9) nextPlaceble = cookies[1];
        else if (number < 0.95) nextPlaceble = cookies[2];
        else nextPlaceble = cookies[3];
    }

    public string GetNextPlacible()
    {
        return nextPlaceble;
    }

    void Update()
    {
        PreviewFunction();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
            int x = clickedCellPosition.x;
            int y = clickedCellPosition.y;
            if (cellsArray[y, x].isEmpty)
            {
                Placing();
                GeneratePlacibleObject();
                gameField.UpdateVisual(cellsArray);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
            int x = clickedCellPosition.x;
            int y = clickedCellPosition.y;

            if (InRange(x, y))
            {
                Debug.Log(cellsArray[y, x].isEmpty);
                Debug.Log(cellsArray[y, x].cookieType);
            }
        }
    }

    private void Placing()
    {
        Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
        int x = clickedCellPosition.x;
        int y = clickedCellPosition.y;

        if (InRange(x, y) && cellsArray[y, x].isEmpty)
        {
            Cell clickedCell = cellsArray[y, x];
            clickedCell.isEmpty = false;
            clickedCell.type = Cell.CellType.cookie;
            clickedCell.cookieType = Metamorfosis(nextPlaceble);
            Merge(clickedCell);
        }
    }

    private void Merge(Cell cell)
    {
        bool[,] visited = new bool[height, width];
        int count = CheckNearest(cell.cellPosition.x, cell.cellPosition.y, cell.cookieType, visited);
        while (count >= 3) //can merge
        {
            // Здесь не нужно заново получать x, y из mouse position,
            // лучше использовать координаты клетки cell
            int x = cell.cellPosition.x;
            int y = cell.cellPosition.y;

            if (InRange(x, y))
            {
                for (int i = height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (cellsArray[i, j] == cell)
                        {
                            cell.Merge();
                            continue;
                        }
                        else if (visited[i, j] && cellsArray[i, j] != cell)
                        {
                            cellsArray[i, j].cookieType = Cell.CookieType.unknown;
                            cellsArray[i, j].isEmpty = true;
                        }
                    }
                }
            }
            visited = new bool[height, width];
            count = CheckNearest(cell.cellPosition.x, cell.cellPosition.y, cell.cookieType, visited);
            
        }
    }


    private int CheckNearest(int x, int y, Cell.CookieType targetType, bool[,] visited)
    {
        if (!InRange(x, y)) return 0;
        if (visited[y, x]) return 0;
        if (cellsArray[y, x].cookieType != targetType) return 0;

        visited[y, x] = true;
        int count = 1;

        count += CheckNearest(x + 1, y, targetType, visited);
        count += CheckNearest(x - 1, y, targetType, visited);
        count += CheckNearest(x, y + 1, targetType, visited);
        count += CheckNearest(x, y - 1, targetType, visited);

        return count;
    }

    private Cell.CookieType Metamorfosis(string str)
    {
        return str switch
        {
            "cookie" => Cell.CookieType.cookie,
            "toast" => Cell.CookieType.toast,
            "mafin" => Cell.CookieType.mafin,
            "pancake" => Cell.CookieType.pankeki,
            "cake" => Cell.CookieType.cake,
            "gingerbreadMan" => Cell.CookieType.gingerbreadMan,
            _ => Cell.CookieType.unknown,
        };
    }

    private void PreviewFunction()
    {
        Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
        int x = clickedCellPosition.x;
        int y = clickedCellPosition.y;

        if (InRange(x, y))
        {
            Cell hoveredCell = cellsArray[y, x];
            if (hoveredCell.isEmpty)
            {
                Tile previewTile = gameField.GetPreviewTileFor(nextPlaceble);
                gameField.ShowPreviewTile(clickedCellPosition, previewTile);
            }
            else
            {
                gameField.ClearPreviewTile();
            }
        }
        else
        {
            gameField.ClearPreviewTile();
        }
    }

    private bool InRange(int x, int y) 
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}

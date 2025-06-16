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
    private System.Random random = new System.Random();

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
            if (clickedCellPosition == cellsArray[0, 0].cellPosition)
            {

            }
            if (cellsArray[y, x].isEmpty)
            {
                Placing();
                GingerbreadManAliveBehavior();
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
        if (targetType == Cell.CookieType.gingerbreadManAlive) return 0;
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

    private void GingerbreadManAliveBehavior()
    {
        List<Cell> gingerbreadList = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cellsArray[y, x].cookieType == Cell.CookieType.gingerbreadManAlive)
                {
                    gingerbreadList.Add(cellsArray[y, x]);
                }
            }
        }

        foreach (Cell cell in gingerbreadList)
        {
            Vector3 dir = MoveRandomDirection(cell);
            Vector3Int newPos = cell.cellPosition + Vector3Int.RoundToInt(dir);

            if (dir != Vector3.zero && InRange(newPos.x, newPos.y) && cellsArray[newPos.y, newPos.x].isEmpty)
            {
                cellsArray[cell.cellPosition.y, cell.cellPosition.x] = new Cell()
                {
                    isEmpty = true,
                    isPlayable = true,
                    type = Cell.CellType.empty,
                    cookieType = Cell.CookieType.unknown,
                    cellPosition = new Vector3Int(cell.cellPosition.x, cell.cellPosition.y)
                };

                cell.cellPosition = newPos;
                cellsArray[newPos.y, newPos.x] = cell;
            }
            else
            {
                bool[,] checkedGingerbreadMan = new bool[height, width];
                if(CheckNearestGingerbreadMans(cell, checkedGingerbreadMan))
                {

                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if(checkedGingerbreadMan[y,x] == true)
                            {
                                Debug.Log("x = " + x + "y =" + y);
                                cellsArray[y, x].cookieType = Cell.CookieType.gingerbreadMan;
                                cellsArray[y, x].isEmpty = false;
                            }
                        }
                    }
                    Merge(cell);
                    gameField.UpdateVisual(cellsArray);
                }
            }
        }
    }

    private bool CheckNearestGingerbreadMans(Cell cell, bool[,] checkedGingerbreadMan)
    {
        int x = cell.cellPosition.x;
        int y = cell.cellPosition.y;

        if (!InRange(x, y)) return false;
        if (checkedGingerbreadMan[y, x]) return false;
        checkedGingerbreadMan[y, x] = true;

        List<Vector3Int> directions = new List<Vector3Int>()
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int newPos = cell.cellPosition + dir;
            int newX = newPos.x;
            int newY = newPos.y;

            if (InRange(newX, newY))
            {
                var neighbor = cellsArray[newY, newX];
                if (neighbor.cookieType == Cell.CookieType.gingerbreadManAlive &&
                    MoveRandomDirection(neighbor) != Vector3.zero)
                {
                    return true;
                }
                else if(neighbor.cookieType == Cell.CookieType.gingerbreadManAlive &&
                    MoveRandomDirection(neighbor) == Vector3.zero)
                {
                    if(CheckNearestGingerbreadMans(neighbor, checkedGingerbreadMan))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private Vector3 MoveRandomDirection(Cell cell)
    {
        List<Vector3> directions = new List<Vector3>()
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),  
            new Vector3(0, 1, 0),   
            new Vector3(0, -1, 0)   
        };

        for (int i = directions.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            Vector3 temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        foreach (var dir in directions)
        {
            Vector3Int newPos = cell.cellPosition + Vector3Int.RoundToInt(dir);
            if (InRange(newPos.x, newPos.y) && cellsArray[newPos.y, newPos.x].isEmpty)
                return dir;
        }

        return Vector3.zero;
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
            "gingerbreadMan" => Cell.CookieType.gingerbreadManAlive,
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

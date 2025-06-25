using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Cell[,] cellsArray;
    private CellVisual[,] cellsArrayVisual;
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private GameField gameField;
    [SerializeField] private Button resetButton;
    private string nextPlaceble;
    private string[] cookies = { "cookie", "toast", "mafin", "pancake", "gingerbreadManAlive", "gingerbreadJumperAlive", "mixer", "microwave", "set", "dgc" };
    Vector3Int clickedCellPosition;
    private System.Random random = new System.Random();
    private bool[] usedIndices;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Canvas gameOverCanvas;
    private float ScaleRange;

    private void Awake()
    {
        gameOverCanvas.gameObject.SetActive(false);
        Instance = this;
    }
    private bool IsPlayableType(Cell.CookieType type)
    {
        return type != Cell.CookieType.gingerbreadManAlive &&
               type != Cell.CookieType.gingerbreadJumperAlive &&
               type != Cell.CookieType.unknown &&
               type != Cell.CookieType.mixer &&
               type != Cell.CookieType.microwave;
    }

    Cell.CookieType[] types = (Cell.CookieType[])System.Enum.GetValues(typeof(Cell.CookieType));
    void Start()
    {
        ScaleRange = 0f;
        cellsArrayVisual = new CellVisual[height, width];
        resetButton.onClick.AddListener(NewGame);
        NewGame();

    }

    public string GetNextPlacible()
    {
        return nextPlaceble;
    }

    private void GenerateEmptyField(int height, int width)
    {
        cellsArray = new Cell[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = new();
                if (y == height-1 && x == 0)
                {
                    cell.isPlayable = true;
                    cell.cookieType = Cell.CookieType.unknown;
                    cell.cellPosition = new Vector3Int(x, y);
                    cell.type = Cell.CellType.plate;
                    cell.isEmpty = true;
                    cellsArray[y, x] = cell;
                    continue;
                }
                cell.isPlayable = true;
                cell.cookieType = Cell.CookieType.unknown;
                cell.cellPosition = new Vector3Int(x, y);
                cell.type = Cell.CellType.empty;
                cell.isEmpty = true;
                cellsArray[y, x] = cell;
            }
        }
    }

    private void GenerateVisualField(int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cellsArrayVisual[y, x]?.gameObject != null)
                {
                    GameObject.Destroy(cellsArrayVisual[y, x].gameObject);
                }
                cellsArrayVisual[y,x] = new CellVisual(cellsArray[y,x].cellPosition, null, prefab);
            }
        }
    }

    private void GeneratePlacibleObject()
    {
        System.Random random = new System.Random();
        double number = random.NextDouble();

        if (number < 0.5) nextPlaceble = cookies[0];//cookie
        else if (number < 0.6) nextPlaceble = cookies[4]; //gingerbreadMan
        else if (number < 0.7) nextPlaceble = cookies[1]; //toast
        else if (number < 0.77) nextPlaceble = cookies[5]; //gingerbredJumper
        else if (number < 0.85) nextPlaceble = cookies[6]; //mixer
        else if (number < 0.9) nextPlaceble = cookies[7]; // microwave
        else if (number < 0.95) nextPlaceble = cookies[2]; //mafin
        else nextPlaceble = cookies[3]; // pancake
    }


    void Update()
    {
        PreviewFunction();

        if (Input.GetMouseButtonDown(0) && !isGameOver())
        {
            Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
            int x = clickedCellPosition.x;
            int y = clickedCellPosition.y;
            if (clickedCellPosition == cellsArray[height - 1, 0].cellPosition) //логика тарелки
            {
                if (cellsArray[y, x].isEmpty)
                {
                    Placing();
                    GingerbreadManAliveBehavior();
                    GeneratePlacibleObject();
                    gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
                }
                else
                {
                    SwapPlateCookie(cellsArray[y, x]);
                }
                gameField.UpdateVisualGingerbreads(cellsArray, cellsArrayVisual);
            }
            else if (cellsArray[y, x].isEmpty && nextPlaceble != "mixer" && nextPlaceble !="microwave")
            {
                Placing();
                gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
                GingerbreadManAliveBehavior(); // логика прыгунов и пряничных человечков
                GeneratePlacibleObject();
            }
            else if (!cellsArray[y,x].isEmpty && nextPlaceble == "mixer")
            {
                Mix(y, x);
                gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
                GingerbreadManAliveBehavior(); // логика прыгунов и пряничных человечков
                GeneratePlacibleObject();
            }
            else if (cellsArray[y, x].isEmpty && nextPlaceble == "microwave")
            {
                Microwave(y,x);
                gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
                GingerbreadManAliveBehavior(); 
                GeneratePlacibleObject();
            }
            gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
            
        }
        else if (isGameOver())
        {
            GameOver();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 clickWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedCellPosition = gameField.Tilemap.WorldToCell(clickWorldPosition);
            int x = clickedCellPosition.x;
            int y = clickedCellPosition.y;
            Debug.Log(cellsArray[y, x].cookieType);
        }
    }

    public void NewGame()
    {
        Debug.Log("worked");
        gameOverCanvas.gameObject.SetActive(false);
        GenerateEmptyField(height,width);
        GenerateVisualField(height, width);
        gameField.UpdateTileMap(cellsArray);
        GeneratePlacibleObject();
        ScoreManager.Instance.CheckAndSaveHighScore();
        ScoreManager.Instance.SetHighScoreText();
        ScoreManager.Instance.resetScore();
    }

    private bool isGameOver()
    {
        return !isEmptyTiles() && nextPlaceble != "mixer" && cellsArray[height - 1, 0].cookieType/*plate*/ != Cell.CookieType.mixer;
    }

    private bool isEmptyTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cellsArray[y, x].isEmpty)
                {
                    return true;
                }
            }
        }
        return false;        
    }
    private void Microwave(int y, int x)
    {
        int indicator = 0;
        Cell.CookieType temp = cellsArray[y, x].cookieType;
        for (int i = types.Length-1; i>=0; i--)
        {
            if (IsPlayableType(types[i]))
            {
                cellsArray[y, x].cookieType = types[i];

                indicator = Merge(cellsArray[y,x], indicator);  //если слилось то индикатор++
            }
            if(indicator > 0)
            {
                cellsArray[y, x].isPlayable = true;
                cellsArray[y, x].type = Cell.CellType.cookie;
                cellsArray[y, x].isEmpty = false;
                break;
            }
            else if(indicator == 0)
            {
                cellsArray[y, x].cookieType = Cell.CookieType.microwaveCoctail;
                cellsArray[y, x].isEmpty = false;
            }
        }
    }
    private void Mix(int y, int x)
    {
        if (cellsArray[y, x].cookieType != Cell.CookieType.gingerbreadManAlive && cellsArray[y, x].cookieType != Cell.CookieType.gingerbreadJumperAlive)
        {
            cellsArray[y, x].isEmpty = true;
            cellsArray[y, x].cookieType = Cell.CookieType.unknown;
        }
        else
        {
            cellsArray[y, x].cookieType = Cell.CookieType.gingerbreadMan;
            Merge(cellsArray[y,x]);
        }
    }

    private void GameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
    }
    private void SwapPlateCookie(Cell plate)
    {
        Cell.CookieType currentPlateCookieType = plate.cookieType;
        plate.cookieType = Metamorfosis(nextPlaceble);
        nextPlaceble = ReverseMetamorfosis(currentPlateCookieType);
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
            if(clickedCell.type != Cell.CellType.plate)
            clickedCell.type = Cell.CellType.cookie;
            clickedCell.cookieType = Metamorfosis(nextPlaceble);
            if (clickedCell.cookieType == Cell.CookieType.gingerbreadManAlive || clickedCell.cookieType == Cell.CookieType.gingerbreadJumperAlive)
            {
                gameField.UpdateVisualGingerbreads(cellsArray,cellsArrayVisual);
            }
            if (clickedCell.type != Cell.CellType.plate)
                ScoreManager.Instance.IncreaseScore(ScoreManager.Instance.CookieTypeToScore(clickedCell.cookieType));
            Merge(clickedCell);
        }

    }

    private void Merge(Cell cell)
    {
        bool[,] visited = new bool[height, width];
        int count = CheckNearest(cell.cellPosition.x, cell.cellPosition.y, cell.cookieType, visited);
        if(count<3)
            gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
        while (count >= 3)
        {
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
                            ScoreManager.Instance.IncreaseScore(ScoreManager.Instance.CookieTypeToScore(cell.cookieType));
                            continue;
                        }
                        else if (visited[i, j] && cellsArray[i, j] != cell)
                        {
                            CellVisual startCellVisual = cellsArrayVisual[i, j];
                            CellVisual targetCellVisual = cellsArrayVisual[cell.cellPosition.y, cell.cellPosition.x];
                            StartCoroutine(startCellVisual.MergeAnimation(startCellVisual, targetCellVisual, 0.2f));
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

    private int Merge(Cell cell, int indicator)
    {
        bool[,] visited = new bool[height, width];
        int count = CheckNearest(cell.cellPosition.x, cell.cellPosition.y, cell.cookieType, visited);
        while (count >= 3)
        {
            indicator++;
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
                            ScoreManager.Instance.IncreaseScore(ScoreManager.Instance.CookieTypeToScore(cell.cookieType));
                            continue;
                        }
                        else if (visited[i, j] && cellsArray[i, j] != cell)
                        {
                            CellVisual startCellVisual = cellsArrayVisual[i, j];
                            CellVisual targetCellVisual = cellsArrayVisual[cell.cellPosition.y, cell.cellPosition.x];
                            StartCoroutine(startCellVisual.MergeAnimation(startCellVisual, targetCellVisual, 0.2f));
                            cellsArray[i, j].cookieType = Cell.CookieType.unknown;
                            cellsArray[i, j].isEmpty = true;
                        }
                    }
                }
            }
            visited = new bool[height, width];
            count = CheckNearest(cell.cellPosition.x, cell.cellPosition.y, cell.cookieType, visited);
            
        }
        return indicator;
    }

    private int CheckNearest(int x, int y, Cell.CookieType targetType, bool[,] visited)
    {
        if (y == height - 1 && x == 0) return 0;
        if (targetType == Cell.CookieType.gingerbreadJumperAlive) return 0;
        if (targetType == Cell.CookieType.doubleGlazurCake) return 0;
        if (targetType == Cell.CookieType.gingerbreadManSet) return 0;
        if (targetType == Cell.CookieType.microwaveCoctail) return 0;
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
        bool gingerbreadJumpersCanMove = true;
        List<Cell> gingerbreadList = new();
        List<Cell> gingerbreadJumperList = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cellsArray[y, x].cookieType == Cell.CookieType.gingerbreadManAlive)
                {
                    gingerbreadList.Add(cellsArray[y, x]);
                }
                if (cellsArray[y, x].cookieType == Cell.CookieType.gingerbreadJumperAlive)
                {
                    gingerbreadJumperList.Add(cellsArray[y, x]);
                }
            }
        }

        foreach(Cell cell in gingerbreadJumperList)  // для прыгунов
        {
            if (cell.type != Cell.CellType.plate && gingerbreadJumpersCanMove)
            {
                usedIndices = new bool[width * height];
                Vector3Int randTilePosition = GetRandomEmptyTile();
                if (randTilePosition != new Vector3Int(-1, -1, 0)) //если есть куда перемещаться.
                {
                    CellVisual startCellVisual = cellsArrayVisual[cell.cellPosition.y, cell.cellPosition.x];
                    CellVisual targetCellVisual = cellsArrayVisual[randTilePosition.y, randTilePosition.x];
                    StartCoroutine(MoveJumperThenUpdate(startCellVisual, targetCellVisual, 0.5f));
                    cellsArray[cell.cellPosition.y, cell.cellPosition.x] = new Cell()
                    {
                        isEmpty = true,
                        isPlayable = true,
                        type = Cell.CellType.empty,
                        cookieType = Cell.CookieType.unknown,
                        cellPosition = new Vector3Int(cell.cellPosition.x, cell.cellPosition.y)
                    };
                    cell.cellPosition = randTilePosition;
                    cellsArray[randTilePosition.y, randTilePosition.x] = cell;
                }
                else //если некуда переместится 
                {
                    gingerbreadJumpersCanMove = false;
                }
            }
        }
        if(gingerbreadJumpersCanMove == false)
        {
            foreach (Cell cell in gingerbreadJumperList)  // для прыгунов
            {
                cellsArray[cell.cellPosition.y, cell.cellPosition.x].cookieType = Cell.CookieType.gingerbreadMan;
                cellsArray[cell.cellPosition.y, cell.cellPosition.x].isEmpty = false;
                Merge(cell);
                gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
            }
        }

        foreach (Cell cell in gingerbreadList) // для человечков
        {
            Vector3 dir = MoveRandomDirection(cell);
            Vector3Int newPos = cell.cellPosition + Vector3Int.RoundToInt(dir);

            if (cellsArray[cell.cellPosition.y, cell.cellPosition.x].type != Cell.CellType.plate) //если не в тарелке то использует логику
            {
                if (dir != Vector3.zero && InRange(newPos.x, newPos.y) && cellsArray[newPos.y, newPos.x].isEmpty && cellsArray[newPos.y, newPos.x].type != Cell.CellType.plate) //блок для передвижения
                {
                    Debug.Log("used");
                    CellVisual startCellVisual = cellsArrayVisual[cell.cellPosition.y, cell.cellPosition.x];
                    CellVisual targetCellVisul = cellsArrayVisual[newPos.y, newPos.x];
                    StartCoroutine(MoveThenUpdate(startCellVisual, targetCellVisul, 0.5f));
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
                    if (CheckNearestGingerbreadMans(cell, checkedGingerbreadMan))
                    {
                        
                    }
                    else
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                if (checkedGingerbreadMan[y, x] == true)
                                {
                                    cellsArray[y, x].cookieType = Cell.CookieType.gingerbreadMan;
                                    cellsArray[y, x].isEmpty = false;
                                }
                            }
                        }
                        Merge(cell);
                        gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
                    }
                }
            }
        }
    }

    public IEnumerator MoveThenUpdate(CellVisual startCell, CellVisual targetCell, float duration)
    {
        yield return StartCoroutine(startCell.MoveToPosition(startCell, targetCell, duration));
        gameField.UpdateVisualGingerbreads(cellsArray, cellsArrayVisual);
    }

    public IEnumerator MergeThenUpdate(CellVisual startCell, CellVisual targetCell, float duration)
    {
        yield return StartCoroutine(startCell.MergeAnimation(startCell, targetCell, duration));
        gameField.UpdateVisualCookies(cellsArray, cellsArrayVisual);
    } 
    public IEnumerator MoveJumperThenUpdate(CellVisual startCell, CellVisual targetCell, float duration)
    {
        yield return StartCoroutine(startCell.MoveJumperToPosition(startCell, targetCell, duration));
        gameField.UpdateVisualGingerbreads(cellsArray, cellsArrayVisual);
    }


    private Vector3Int GetRandomEmptyTile()
    {
        int totalTiles = width * height;

        // Проверка: все использованы
        if (usedIndices.All(b => b))
        {
            return new Vector3Int(-1, -1, 0);
        }

        int rand, row, col;
        int attempts = 0;

        do
        {
            rand = random.Next(0, totalTiles);
            attempts++;

        } while (usedIndices[rand]);

        usedIndices[rand] = true;

        row = rand / width;
        col = (width - (rand % width))-1;

        if (cellsArray[col, row].isEmpty && cellsArray[col, row].type != Cell.CellType.plate)
        {
            return cellsArray[col, row].cellPosition;
        }
        else
        {
            return GetRandomEmptyTile();
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
                    MoveRandomDirection(neighbor) != Vector3.zero)    //условие возможного движения
                {
                    return true;
                }
                if (neighbor.cookieType == Cell.CookieType.gingerbreadJumperAlive) //условие присутствия прыгуна рядом
                {
                    return true;
                }
            else if (neighbor.cookieType == Cell.CookieType.gingerbreadManAlive &&
                MoveRandomDirection(neighbor) == Vector3.zero)   //условие возможного движения для соседа
            {
                if (CheckNearestGingerbreadMans(neighbor, checkedGingerbreadMan))
                {
                    return true;
                }
            }
            }
        }

        return false; // если соседи не могут двигаться
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
            if (InRange(newPos.x, newPos.y) && cellsArray[newPos.y, newPos.x].isEmpty && cellsArray[newPos.y, newPos.x].type != Cell.CellType.plate)
                return dir;
        }

        return Vector3.zero;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
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
            "gingerbreadManAlive" => Cell.CookieType.gingerbreadManAlive,
            "gingerbreadJumperAlive" => Cell.CookieType.gingerbreadJumperAlive,
            "mixer" => Cell.CookieType.mixer,
            "microwave" => Cell.CookieType.microwave,
            "set" => Cell.CookieType.gingerbreadManSet,
            "dgc" => Cell.CookieType.doubleGlazurCake,
            _ => Cell.CookieType.unknown,
        };
    }

    private string ReverseMetamorfosis(Cell.CookieType type)
    {
        return type switch
        {
            Cell.CookieType.cookie => "cookie",
            Cell.CookieType.toast => "toast",
            Cell.CookieType.mafin => "mafin",
            Cell.CookieType.pankeki => "pancake",
            Cell.CookieType.cake => "cake",
            Cell.CookieType.gingerbreadManAlive => "gingerbreadManAlive",
            Cell.CookieType.gingerbreadJumperAlive => "gingerbreadJumperAlive",
            Cell.CookieType.mixer => "mixer",
            Cell.CookieType.microwave => "microwave",
            _ => "unknown",
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
            if (hoveredCell.isEmpty || nextPlaceble == "mixer")
            {
                Tile previewTile = gameField.GetPreviewTileFor(nextPlaceble);

                //if(ScaleRange < 0f) переделать под геймобджект
                //{
                //    ScaleRange += Time.deltaTime;
                //    previewTile.transform.local += ScaleRange;
                //}
                //else if(ScaleRange > 1f)
                //{
                //    ScaleRange += Time.deltaTime;
                //    previewTile.transform.lossyScale -= ScaleRange;
                //}
                
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

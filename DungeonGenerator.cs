using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    public Vector2 size;
    public int maxCells = 1000;
    public int startPos = 0;
    [SerializeField] int enemyDistance = 0;
    [SerializeField] int enemyDistanceLimit = 10;
    int enemyActualDistance;
    public GameObject room;
    public PlayerController playerChar;
    public GameObject[] enemy;
    public GameObject boss;
    public Vector2 offset;

    List<Cell> board;

    void Start()
    {
        enemyActualDistance = enemyDistanceLimit;
        MazeGenerator();
        playerChar.SetOffset((int)offset.x);
    }

    void GenerateDungeon()
    {
        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];
                if (currentCell.visited)
                {
                    enemyDistance++;
                    int temp = enemyDistance % enemyActualDistance;
                    var newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    if (temp == 0)
                    {
                        GameObject newEnemy = (GameObject)Instantiate(enemy[SceneSaver.Instance.runs], new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform);
                        enemyActualDistance = (int) Random.Range(1.0f, enemyDistanceLimit );
                    }
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    void MazeGenerator()
    {
        //Board generation
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        //Tracking values
        int currentCell = startPos;
        Stack<int> path = new Stack<int>();
        int tracker = 0;

        //Room generation
        while(tracker < maxCells)
        {
            tracker++;
            board[currentCell].visited = true;

            if (currentCell == board.Count - 1) 
            {
                if (SceneSaver.Instance.runs == 0) { 
                    var newEnemy = Instantiate(enemy[1], new Vector3((currentCell % size.x) * offset.x, 0, -(((int)(currentCell / size.y)) * offset.y)), Quaternion.identity, transform);
                    foreach(Transform t in newEnemy.transform) t.gameObject.tag = "Boss";
                }
                else { var newEnemy = Instantiate(boss, new Vector3((currentCell % size.x) * offset.x, 0, -(((int)(currentCell / size.y)) * offset.y)), Quaternion.identity, transform); } 
                break; 
            }

            //Check neighbour cells

            List<int> neighbours = CheckNeighbours(currentCell);

            if(neighbours.Count == 0)
            {
                if(path.Count == 0)
                {
                    break;
                } else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbours[Random.Range(0, neighbours.Count)];

                if (newCell > currentCell)
                {
                    //South or east
                    if(newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                } else
                {
                    //North or west
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbours(int cell)
    {
        List<int> neighbours = new List<int>();

        //check northern neighbour cell
        if(cell - size.x >= 0 && !board[Mathf.FloorToInt(cell-size.x)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell - size.x));
        }
        //check southern neighbour cell
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell + size.x));
        }
        //check eastern neighbour cell
        if ((cell +1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell + 1));
        }
        //check western neighbour cell
        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell - 1));
        }

        return neighbours;
    }
}

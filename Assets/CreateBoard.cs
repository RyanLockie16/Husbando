using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateBoard : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int xDimention;
    [SerializeField]
    private int yDimention;
    [SerializeField]
    private List<GameObject> BasicGems;
    [SerializeField]
    private List<GameObject> SpecialGems;
    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private Camera cam;

    private static GameObject[,] board;

    private Vector2Int[] swap;

    [SerializeField]
    private GameObject EmptyTile;

    public Vector2Int BoardSize { get; private set; }

     //% chance to spawn a basic tile
    void Start()
    {
        board = new GameObject[xDimention, yDimention];
        generateGrid();
        swap = new Vector2Int[2] { new Vector2Int(-1, -1), new Vector2Int(-1, -1) };
        BoardSize = new Vector2Int(xDimention, yDimention);
        Debug.Log(TileConfig.GetTileSize());
    }

    private void generateGrid() //Creates the initial grid and sets the background size and positions the camera
    {
        //This double for loop creates the grid
        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                if (Random.Range(0f, 100f) - TileConfig.GetSpawnChance() <= 0) { //An if statement to choose if it should spawn a basic or special tile
                    SpawnTile(row, col, false);
                }
                else
                {
                    SpawnTile(row, col, true);
                }
            }
        }
        cam.transform.position = new Vector3((float)xDimention / 2 - TileConfig.GetTileSize().x / 2f, (float)yDimention / 2 - TileConfig.GetTileSize().y / 2f, -10f); //Set cam position to center of the board
        var bg = Instantiate(backGround, new Vector3((float)xDimention / 2 - TileConfig.GetTileSize().x / 2f, (float)yDimention / 2 - TileConfig.GetTileSize().y / 2f), Quaternion.identity); //Creates background and sets it to the size of the grid
        bg.transform.localScale = new Vector3(xDimention + TileConfig.GetTileSize().x*2f, yDimention + TileConfig.GetTileSize().y*2f); //Sets the boarder of the grid so there is some white space
        float width = (2f * cam.orthographicSize) * cam.aspect;
        cam.transform.position = new Vector3(cam.transform.position.x + (width - bg.transform.localScale.x)/2f, cam.transform.position.y, cam.transform.position.z);
    }

    public void SpawnTile (int row, int col, bool IsSpecial) //Move all logic that actually spawns the tiles to a method for convience
    {
        if (IsSpecial)
        {
            int gem = Random.Range(0, SpecialGems.Count);
            Vector3 tilePos = new Vector3(row * TileConfig.GetTileSize().x, col * TileConfig.GetTileSize().y);
            board[row, col] = Instantiate(SpecialGems[gem], tilePos, Quaternion.identity);
            board[row, col].transform.SetParent(transform);
            board[row, col].name = $"{board[row, col].GetComponent<Gem>().GetGemType()} Special {row} {col}";
            board[row, col].GetComponent<Gem>().pos = new Vector2Int(row, col);
        }
        else {
            int gem = Random.Range(0, BasicGems.Count);
            Vector3 tilePos = new Vector3(row * TileConfig.GetTileSize().x, col * TileConfig.GetTileSize().y);
            board[row, col] = Instantiate(BasicGems[gem], tilePos, Quaternion.identity);
            board[row, col].transform.SetParent(transform);
            board[row, col].name = $"{board[row, col].GetComponent<Gem>().GetGemType()} Basic {row} {col}";
            board[row, col].GetComponent<Gem>().pos = new Vector2Int(row, col);
        }
        if (!board[row, col].GetComponent<Gem>().CanBePlaced())
        {
            Destroy(board[row, col]);
            SpawnTile(row, col, IsSpecial);
        }
    }

    public static Gem GetTile(Vector2Int pos) //A function that returns the gem at a given position
    {
        try
        {
            //Debug.Log(board[pos.x, pos.y].GetComponent<Gem>());
            return board[pos.x, pos.y].GetComponent<Gem>();
        } catch
        {
            Debug.Log("Hit Null Tile");
            return null;
        }
    }

    public void TilesToSwap(Gem gem) //A function that declares what tiles should be swapped
    {

        if (swap[0] == new Vector2(-1f, -1f)) //Sets the first tile reference 
        {
            swap[0] = gem.pos;
        }
        else //Sets the second tile reference
        {
            if (!gem.isNeighbor(swap[0])) //Checks if tiles are neighbors
            {
                Debug.Log("Tiles not next to eachother");
                board[swap[0].x, swap[0].y].GetComponent<Gem>().setSelected(false);
                gem.setSelected(false);
            }
            else
            {
                swap[1] = gem.pos;

                Swap();


            }
            swap = new Vector2Int[2] { new Vector2Int(-1, -1), new Vector2Int(-1, -1) };
        }
    }

    private void Swap() //Swap the position of two tiles and removes tile matches
    {
        GameObject temp1 = board[swap[0].x, swap[0].y];
        GameObject temp2 = board[swap[1].x, swap[1].y];
        Vector2Int pos1 = temp1.GetComponent<Gem>().pos;
        Vector2Int pos2 = temp2.GetComponent<Gem>().pos;

        //board[pos1.x, pos1.y] = temp2;
        //board[pos2.x, pos2.y] = temp1;

        temp1.GetComponent<Gem>().changePosition(pos2);
        temp2.GetComponent<Gem>().changePosition(pos1);
        //UpdateBoard();

        //temp1.GetComponent<Gem>().setSelected(false);
        //temp2.GetComponent<Gem>().setSelected(false);

        //UpdateAllNeighbors();

        temp1.GetComponent<Gem>().hasMatches();
        temp2.GetComponent<Gem>().hasMatches();

        //Debug.Log(temp1.GetComponent<Gem>().toBeDeleted);
        //Debug.Log(temp2.GetComponent<Gem>().toBeDeleted);
        FindDeletedTiles();
        
    }


    public void ChangeTileSpace(Vector2Int pos, GameObject g) //changes the reference of a gameobject in the board
    {
        board[pos.x, pos.y] = g;
        //UpdateAllNeighbors();
    }
    void Update()
    {
        
        if (swap[0] != new Vector2Int(-1, -1) && Input.GetKeyDown(KeyCode.Escape))
        {
            board[swap[0].x, swap[0].y].GetComponent<Gem>().setSelected(false);
            swap[0] = new Vector2Int(-1, -1);
        }
    }

    private void FindDeletedTiles() //Finds tiles that have toBeDeleted = true and destroys them
    {

        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                if (board[row, col].GetComponent<Gem>() != null && board[row, col].GetComponent<Gem>().toBeDeleted == true)
                {
                    Destroy(board[row, col].gameObject);
                    board[row, col] = Instantiate(EmptyTile, new Vector3(row, col), Quaternion.identity);
                    board[row, col].GetComponent<EmptyTile>().InIt(new Vector2Int(row, col), this);
                    //board[row, col].GetComponent<EmptyTile>().checkLocation();
                    Debug.Log("Spawned Empty Tile");
                }
                   
            }
        }

    }

    public void FindMatchesAllTiles()
    {
        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                if (board[row, col].GetComponent<Gem>() != null)
                {
                    board[row, col].GetComponent<Gem>().hasMatches();
                }

            }
        }

        FindDeletedTiles();
    }
}

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
    private List<GameObject> gems;
    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private Transform cam;

    private static GameObject[,] board;

    private Vector2Int[] swap;

    void Start()
    {
        board = new GameObject[xDimention, yDimention];
        generateGrid();
        swap = new Vector2Int[2] { new Vector2Int(-1, -1), new Vector2Int(-1, -1) };
    }

    private void generateGrid() //Creates the initial grid and sets the background size and positions the camera
    {
        //This double for loop creates the grid
        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                int gem = Random.Range(0, gems.Count);
                board[row, col] = Instantiate(gems[gem], new Vector3(row, col), Quaternion.identity);
                board[row, col].transform.SetParent(transform);
                board[row, col].name = $"{board[row, col].GetComponent<Gem>().GetGemType()} {row} {col}";
                board[row, col].GetComponent<Gem>().pos = new Vector2Int(row, col);
            }
        }
        Vector2 tileDimentions = board[0, 0].GetComponent<Gem>().GetTileSize(); //Gets Tile size for calculations later
        cam.transform.position = new Vector3((float)xDimention / 2 - tileDimentions.x / 2f, (float)yDimention / 2 - tileDimentions.y / 2f, -10f); //Set cam position to center of the board
        var bg = Instantiate(backGround, new Vector3((float)xDimention / 2 - tileDimentions.x / 2f, (float)yDimention / 2 - tileDimentions.y / 2f), Quaternion.identity); //Creates background and sets it to the size of the grid
        bg.transform.localScale = new Vector3(xDimention + 2f, yDimention + 2f); //Sets the boarder of the grid so there is some white space
    }


    public static Gem GetTile(Vector2Int pos) //A function that returns the gem at a given position
    {
        return board[pos.x, pos.y].GetComponent<Gem>();
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
        Vector3 pos1 = temp1.transform.position;
        Vector3 pos2 = temp2.transform.position;

        board[swap[0].x, swap[0].y].transform.position = pos2;
        board[swap[1].x, swap[1].y].transform.position = pos1;

        board[swap[0].x, swap[0].y] = temp2;
        board[swap[1].x, swap[1].y] = temp1;
        temp1.GetComponent<Gem>().pos = swap[1];
        temp2.GetComponent<Gem>().pos = swap[0];

        temp1.GetComponent<Gem>().setSelected(false);
        temp2.GetComponent<Gem>().setSelected(false);

        Debug.Log("Finding Neighbors");
        //temp1.GetComponent<Gem>().FindNeighbors();
        //temp2.GetComponent<Gem>().FindNeighbors();
        //temp1.GetComponent<Gem>().RemakeNeighbors();
        //temp2.GetComponent<Gem>().RemakeNeighbors();
        UpdateAllNeighbors();

        temp1.GetComponent<Gem>().hasMatches();
        temp2.GetComponent<Gem>().hasMatches();

    }

    private void FillGaps() //Method to fill in empty spaces
    {
        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                if (board[row, col] == null)
                {
                    int gem = Random.Range(0, gems.Count);
                    board[row, col] = Instantiate(gems[gem], new Vector3(row, col), Quaternion.identity);
                    board[row, col].transform.SetParent(transform);
                    board[row, col].name = $"{board[row, col].GetComponent<Gem>().GetGemType()} {row} {col}";
                    board[row, col].GetComponent<Gem>().pos = new Vector2Int(row, col);
                }
            }
        }
        UpdateAllNeighbors();
    }

    private void UpdateAllNeighbors() //Updates all the neighbors of every tile
    {
        for (int row = 0; row < xDimention; row++)
        {
            for (int col = 0; col < yDimention; col++)
            {
                board[row, col].GetComponent<Gem>().FindNeighbors();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        FillGaps();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField]
    private GemType type;

    private GameObject outline;

    [SerializeField]
    private Vector2Int dimentions = new Vector2Int(1, 1);

    public Vector2Int pos;

    private bool selected = false;

    private List<Gem> adjacnceyList = new List<Gem>();

    public bool toBeDeleted = false;

    void Start()
    {
        outline = transform.Find("Selected").gameObject;
        outline.SetActive(false);
        FindNeighbors();
    }

    private void Update()
    {
        //FindNeighbors();           //Added this in so that the neighbor list stays consistent as items move
    }

    public void changePosition (Vector2Int pos) //Method to swap pos var and objects location
    {
        this.pos = pos;
        transform.position = new Vector3(this.pos.x * dimentions.x, this.pos.y * dimentions.y);
        
    }

    private void OnMouseEnter() //Checks if the mouse is over a tile to see if the outline should be turned on
    {
        outline.SetActive(true);
    }
    private void OnMouseExit() //Removes the outline if the tile was not clicked
    {
        if (!selected)
        {
            outline.SetActive(false);
        }
    }

    private void OnMouseDown() //Sets the tile to selected if the player clicks it
    {
        if (!selected)
        {
            var c = gameObject.GetComponentInParent<CreateBoard>();
            setSelected(true);
            c.TilesToSwap(this);
        }
    }

    public void setSelected(bool isSelected) //Handles selecting and deslecting tiles.
    {
        selected = isSelected;
        outline.SetActive(isSelected);
    }
    public GemType GetGemType() //Returns the enum type for the gem
    {
        return type;
    }

    public Vector2 GetTileSize() //Returns the tile size of the gem
    {
        return dimentions;
    }

    public void Reset() //Resets the tile
    {
        adjacnceyList.Clear();
        selected = false;
    }

    public void FindNeighbors() //Finds the neighboring tiles
    {
        Reset();


        CheckTile(Vector2.up);
        CheckTile(Vector2.down);
        CheckTile(Vector2.left);
        CheckTile(Vector2.right);

    }

    private void CheckTile(Vector2 direction) //The fuctions actually sets the references for the neighbors list
    {
        Vector2 halfExt = new Vector2(0.25f, 0.25f);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position + (Vector3)direction, halfExt, 0f);

        foreach (Collider2D item in collider2Ds)
        {
            Gem gem = item.GetComponent<Gem>();
            if (gem != null)
            {
                adjacnceyList.Add(gem);
            }
        }
    }

    public bool isNeighbor(Vector2Int pos) //Checks if a givin tile is a neighbor to this tile.
    {
        foreach (Gem gem in adjacnceyList)
        {
            if (gem.pos == pos)
            {
                return true;
            }
        }
        return false;
    }


    public void hasMatches() //A public function that calls the other matching logic
    {
        List<Vector2Int> matches = findMatches(null);

        checkInLine(checkType.row, matches);

        checkInLine(checkType.col, matches);

    }

    private List<Vector2Int> findMatches(List<Vector2Int> m) //Returns a list of of the position of tiles that match the current tiles type
    {
        List<Vector2Int> Matches;
        if (m == null)
        {
            Matches = new List<Vector2Int>();
            Matches.Add(pos);
        }
        else
            Matches = m;



        foreach (Gem gem in adjacnceyList)
        {
            if (gem.GetGemType() == type && canPlace(Matches, gem.pos))
            {
                if (canPlace(Matches, pos))
                    Matches.Add(pos);

                Debug.Log("Found match at " + gem.pos);
                List<Vector2Int> temp = gem.findMatches(Matches);

                if (temp.Count > Matches.Count)
                    Matches.AddRange(temp);
                else if (canPlace(Matches, gem.pos))
                    Matches.Add(gem.pos);

                Debug.Log(Matches.Count);
                //return Matches;
            }
        }


        return Matches;
    }

    private void checkInLine(checkType cType, List<Vector2Int> m) //Checks and removes tiles that are in a row or col
    {
        List<Gem> toBeRemoved = new List<Gem>();
        switch (cType)
        {
            case checkType.col:
                for (int i = 1; i < m.Count; i++)
                {
                    if (m[i].x == pos.x && !toBeRemoved.Contains(CreateBoard.GetTile(m[i])))
                    {
                        toBeRemoved.Add(CreateBoard.GetTile(m[i]));
                    }
                }
                break;
            case checkType.row:
                for (int i = 1; i < m.Count; i++)
                {
                    if (m[i].y == pos.y && !toBeRemoved.Contains(CreateBoard.GetTile(m[i])))
                    {
                        toBeRemoved.Add(CreateBoard.GetTile(m[i]));
                    }
                }
                break;
        }
        if (toBeRemoved.Count >= 2)
        {
            for (int i = 0; i < toBeRemoved.Count; i++)
            {
                //Destroy(toBeRemoved[i].gameObject);
                toBeRemoved[i].toBeDeleted = true;
            }
            toBeDeleted = true;
            //Destroy(this.gameObject);
        }
    }

    private enum checkType //An enum that is used for the checkInLine() function that says what way we are checking
    {
        row, col
    }
    

    private bool canPlace(List<Vector2Int> m, Vector2Int pos) //Checks if a tile can be place (I made it cause List<>.contains was being weird)
    {
        for (int i = 0; i < m.Count; i++)
        {
            if (m[i] == pos)
            {
                return false;
            }
        }
        return true;
    }
}

public enum GemType //An Enum that corrisponds to the what type the gem is
{
    ICS, Math, Humanities, BioSci, Stats, Engineering
}

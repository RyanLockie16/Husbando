using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField]
    private GemType type;

    private GameObject outline;

    /*[SerializeField]
    private Vector2Int dimentions = new Vector2Int(1, 1);*/

    public Vector2Int pos;

    private bool selected = false;

    private List<Gem> adjacnceyList = new List<Gem>();

    public bool toBeDeleted = false;

    [SerializeField]
    private bool isSpecialGem = false; //When set to false it means that it is a normal gem
    void Start()
    {
        //gameObject.transform.localScale = new Vector3(TileConfig.GetTileSize().x, TileConfig.GetTileSize().y);
        outline = transform.Find("Selected").gameObject;
        outline.SetActive(false);
        FindNeighbors();
    }

    private void FixedUpdate()
    {
        FindNeighbors();  //Added this in so that the neighbor list stays consistent as items move
        //CheckBelow();
    }

    public void changePosition (Vector2Int pos) //Method to swap pos var and objects location
    {
        this.pos = pos;
        transform.position = new Vector3(this.pos.x * TileConfig.GetTileSize().x, this.pos.y * TileConfig.GetTileSize().y);
        gameObject.GetComponentInParent<CreateBoard>().ChangeTileSpace(pos, gameObject);
        setSelected(false);


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
        try
        {
            outline.SetActive(isSelected);
        }
        catch
        {
            outline = Instantiate(TileConfig.GetOutline());
            outline.SetActive(isSelected);
            Debug.Log("Could not find outline for some reason");
        }
    }
    public GemType GetGemType() //Returns the enum type for the gem
    {
        return type;
    }

    /*public Vector2Int GetTileSize() //Returns the tile size of the gem
    {
        return TileConfig.GetTileSize();
    }*/

    public void Reset() //Resets the tile
    {
        adjacnceyList.Clear();
        //selected = false;
        //toBeDeleted = false;
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
        Vector2 halfExt = new Vector2(TileConfig.GetTileSize().x*0.25f, TileConfig.GetTileSize().y*0.25f);
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


    private List<Gem> FindMatches (Vector2 dir) //Uses ray casts to see if there is a tile in the given direction and if it of the same type
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position,dir, (float)TileConfig.GetTileSize().x*1.2f);

        List<Gem> Matches = new();
        //Collider2D col = collider2D1;

        if (hit.collider != null)
        {
            //Debug.Log($"{gameObject.name} is adding a new tile");
            if (hit.transform.GetComponent<Gem>() != null) {
                Gem g = hit.transform.GetComponent<Gem>();
                if (g.GetGemType() == type && g != this && !g.toBeDeleted)
                    Matches.AddRange(g.FindMatches(dir));
            }
        }

        Matches.Add(this);
        return Matches;
    }


    public bool CanBePlaced()
    {
        List<Gem> row = FindMatches(Vector2.right);
        row.AddRange(FindMatches(-Vector2.right));
        row.Remove(this);
        //Debug.Log(row.Count);

        List<Gem> col = FindMatches(Vector2.up);


        col.AddRange(FindMatches(-Vector2.up));
        //col.Remove(this);
        Debug.Log(col.Count);

        if (row.Count > 2 || col.Count > 2)
        {
            return false;
        }
        return true;
    }
    public void hasMatches() //A public function that calls the other matching logic
    {
        if (toBeDeleted)
        {
            return;
        }
        List<Gem> row = FindMatches(Vector2.right);
        row.AddRange(FindMatches(-Vector2.right));
        row.Remove(this);
        Debug.Log(row.Count);
        
        List<Gem> col = FindMatches(Vector2.up);
        

        col.AddRange(FindMatches(-Vector2.up));
        col.Remove(this);
        Debug.Log(col.Count);

        
        if (row.Count > col.Count && row.Count > 2)
        {
            for (int i = 0; i < row.Count; i++)
            {
                row[i].toBeDeleted = true;
            }
        }
        else if (col.Count > 2)
        {
            for(int i = 0; i < col.Count; i++)
            {
                col[i].toBeDeleted = true;
            }
        }

    }

    public bool GetIsSpecial() //Returns if the tile is a basic or special tile ... false means it is basic, true means it is special
    {
        return isSpecialGem;
    }

}

public enum GemType //An Enum that corrisponds to the what type the gem is
{
    ICS, Math, Humanities, BioSci, Stats, Engineering
}

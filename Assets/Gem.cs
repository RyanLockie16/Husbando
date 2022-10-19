using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField]
    private GemType type;

    private GameObject outline;

    [SerializeField]
    private Vector2 dimentions = new Vector2(1f, 1f);

    public Vector2Int pos;

    private bool selected = false;

    private List<Gem> adjacnceyList = new List<Gem>();

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

    private void OnMouseEnter()
    {
        outline.SetActive(true);
    }
    private void OnMouseExit()
    {
        if (!selected)
        {
            outline.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (!selected)
        {
            var c = gameObject.GetComponentInParent<CreateBoard>();
            setSelected(true);
            c.TilesToSwap(this);
        }
    }

    public void setSelected(bool isSelected)
    {
        selected = isSelected;
        outline.SetActive(isSelected);
    }
    public GemType GetGemType()
    {
        return type;
    }

    public Vector2 GetTileSize()
    {
        return dimentions;
    }

    public void Reset()
    {
        adjacnceyList.Clear();
        selected = false;
    }

    public void FindNeighbors()
    {
        Reset();
        CheckTile(Vector2.up);
        CheckTile(Vector2.down);
        CheckTile(Vector2.left);
        CheckTile(Vector2.right);
        CheckTile(Vector2.right + Vector2.up);
        CheckTile(Vector2.left + Vector2.up);
        CheckTile(Vector2.right + Vector2.down);
        CheckTile(Vector2.left + Vector2.down);
    }

    private void CheckTile(Vector2 direction)
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

    public bool isNeighbor(Vector2Int pos)
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

    public void RemakeNeighbors()
    {
        setSelected(false);
        for (int i = 0; i < adjacnceyList.Count; i++)
        {
            adjacnceyList[i].FindNeighbors();
        }

    }

    public void hasMatches()
    {
        List<Vector2Int> matches = findMatches(null);

        Debug.Log(matches.Count);
        foreach (Vector2Int p in matches)
        {
            Debug.Log(CreateBoard.GetTile(p).name);
        }

        checkInLine(checkType.row, matches);

        checkInLine(checkType.col, matches);

    }

    private void checkInLine(checkType cType, List<Vector2Int> m)
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
        if (toBeRemoved.Count >= 3)
        {
            for (int i = 0; i < toBeRemoved.Count; i++)
            {
                Destroy(toBeRemoved[i].gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    private enum checkType
    {
        row, col
    }
    private List<Vector2Int> findMatches(List<Vector2Int> m)
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
                if (temp != null)
                    Matches.AddRange(temp);
                else if (canPlace(Matches, gem.pos))
                    Matches.Add(gem.pos);
                Debug.Log(Matches.Count);
                return Matches;
            }
        }


        return null;
    }

    private bool canPlace(List<Vector2Int> m, Vector2Int pos)
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

public enum GemType
{
    ICS, Math, Humanities, BioSci, Stats, Engineering
}

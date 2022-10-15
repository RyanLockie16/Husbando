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

    private void Update() {
        FindNeighbors();           //Added this in so that the neighbor list stays consistent as items move
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

    public void setSelected (bool isSelected)
    {
        selected=isSelected;
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
    }

    public void CheckTile(Vector2 direction)
    {
        Vector2 halfExt = new Vector2(0.25f, 0.25f);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position + (Vector3) direction, halfExt, 0f);

        foreach (Collider2D item in collider2Ds)
        {
            Gem gem = item.GetComponent<Gem>();
            if(gem != null)
            {
                adjacnceyList.Add(gem);
            }
        }
    }
    
    public bool isNeighbor(Vector2Int pos)
    {
        foreach (Gem gem in adjacnceyList)
        {
            if(gem.pos == pos)
            {
                return true;
            }
        }
        return false;
    }
   /*public List<Gem> FindMatches (Vector2 dir)
    {
        List<Gem> matches = new List<Gem>();
   }*/
}

public enum GemType
{
    ICS, Math, Humanities, BioSci, Stats, Engineering
}

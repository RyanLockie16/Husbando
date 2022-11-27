using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : MonoBehaviour
{
    // Start is called before the first frame update
    private float FillDelay = 0.5f; //Delay after match is found
    private Vector2Int pos;

    private CreateBoard board;

    private void FixedUpdate()
    {
        try
        {
            
            StartCoroutine(checkLocation());
        } catch
        {
            Debug.Log("Not Initalized");
        }
    }

    public void InIt(Vector2Int pos, CreateBoard board)
    {
        this.pos = pos;
        this.board = board;
        gameObject.transform.localScale = new Vector3(TileConfig.GetTileSize().x, TileConfig.GetTileSize().y);
        transform.position = new Vector3(pos.x * TileConfig.GetTileSize().x, pos.y * TileConfig.GetTileSize().y);
        
    }

    IEnumerator checkLocation()
    {
        yield return new WaitForSecondsRealtime(FillDelay);
        Debug.Log("Checking location");
        if (pos.y == board.BoardSize.y - 1)
        {
            
            SpawnNewTile();
        }
        else
        {
            
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, Vector2.up, 1.2f);
            if (hit && hit.transform.tag == "EmptyTile" && hit.transform.GetComponent<EmptyTile>() != this)
            {
                hit.transform.GetComponent<EmptyTile>().StartCoroutine(checkLocation());
                //checkLocation();
            }
            //else if (hit && !hit.transform.CompareTag("EmptyTile"))
            else if (hit && hit.transform.GetComponent<Gem>() != null)
            {
                
                Gem g = hit.transform.GetComponent<Gem>();
               
                transform.position = new Vector3(g.pos.x * TileConfig.GetTileSize().x, g.pos.y * TileConfig.GetTileSize().y);
                Vector2Int pos2 = g.pos;
                
                
                g.changePosition(pos);
                pos = pos2;
                board.ChangeTileSpace(pos, gameObject);
                //checkLocation();
                //checkLocation();
                //Destroy(gameObject);
            }
            else
            {
                //SpawnNewTile();
            }
            
        }
    }

    private void OnDestroy()
    {
        board.FindMatchesAllTiles();
    }

    private void SpawnNewTile()
    {
        /*int gem = Random.Range(0, gemList.Count);
        GameObject g = Instantiate(gemList[gem], new Vector3 (pos.x, pos.y), Quaternion.identity);
        g.transform.SetParent(board.transform);
        g.name = $"{g.GetComponent<Gem>().GetGemType()} {pos.x} {pos.y}";
        g.GetComponent<Gem>().pos = new Vector2Int(pos.x, pos.y);
        board.ChangeTileSpace(pos, g);*/
        if (Random.Range(0f, 100f) - TileConfig.GetSpawnChance() <= 0)
        { //An if statement to choose if it should spawn a basic or special tile
            board.SpawnTile(pos.x, pos.y, false);
        }
        else
        {
            board.SpawnTile(pos.x, pos.y, true);
        }

        Destroy(gameObject);
    }
}

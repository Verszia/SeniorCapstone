using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool walkable = true; //if a tile is walkable
    public bool current = false; // the current tile the PC is standing on
    public bool target = false; //the tile the PC wants to move to
    public bool selectable = false; //the tile that will check if the tile is able to be selected by the PC

    public List<Tile> adjacencyList = new List<Tile>();


    //needed BFS (breadth first search)

    
    public bool visited = false; //means the tile has been processed
    public Tile parent = null; //want to know who the parent of the tile is, in order to identify the tiles that are walkable and how to get there. find the path
    public int distance = 0; //how far each tile is from the start tile



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(current)
        {
            GetComponent<Renderer>().material.color = Color.magenta; //sets the current tile to magenta color
        }
        else if(target)
        {
            GetComponent<Renderer>().material.color = Color.green; //sets the target tile to green color
        }
        else if(selectable)
        {
            GetComponent<Renderer>().material.color = Color.red; //turns the selectable tiles red color.
        }
        else //if there is no current tile selectable,
        {
            GetComponent<Renderer>().material.color = Color.white; //the default color for the non-selectable tiles.
        }

    }

    public void Reset() //returns all values to their original states
    {
        adjacencyList.Clear();

        current = false;
        target = false; 
        selectable = false; 

        visited = false; 
        parent = null; 
        distance = 0; 

    }

    public void FindNeighbors(float jumpHeight) //every time we call the findneighbors function, we reset the tiles
    {
        Reset(); //to original state

        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
    }

    public void CheckTile(Vector3 direction, float jumpHeight) //checks each individual tiles to see if there is one next to us
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach(Collider item in colliders) //iterate through each of the colliders
        {

            Tile tile = item.GetComponent<Tile>();

            //from the center of the tile, up 1 (about 0.5), checks to see if there is something there. If there is something there, dont add it. if there is, add it, and it becomes part of our adjacency list

            if(tile != null && tile.walkable) //if there is no tile there and if there is no walkable tile there, return null. only if it's not null will it test if it is walkable
            {

                RaycastHit hit;

                if(!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1)) // will be from the tile's position, in the up direction (pos y), output the hit variable, looks for distance of one,
                {
                    //returns true if it hits something

                 adjacencyList.Add(tile);

                } 

            }
        }

    }
}

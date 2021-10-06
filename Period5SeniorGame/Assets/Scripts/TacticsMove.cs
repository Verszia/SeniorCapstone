using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile; //tracks the current tile


    public bool moving = false; //we only look for the selectable tiles when it's not moving
    public int move = 5;
    public float jumpHeight = 2;
    public float moveSpeed = 2;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();  //the direction the player is heading

    float halfHeight = 0;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    Vector3 jumpTarget;

    public float jumpVelocity = 4.5f;

    protected void Init()
    {

        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y; //gives us the halfheight of the player

    }


    public void GetCurrentTile()
    {
        Debug.Log("GetCurrentTile");
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }
    

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1)) //WILL LOCATE THE TILE whoops sorry caps
        {

            tile = hit.collider.GetComponent<Tile>();

        }

        return tile;
    }

       public void ComputeAdjacencyLists()
       {
           Debug.Log("ComputeAdjacencyLists");

            foreach(GameObject tile in tiles)
            {
                 Tile t = tile.GetComponent<Tile>();
                 t.FindNeighbors(jumpHeight);
            }
       }


       public void FindSelectableTiles()
       {

         //the BFS algorithm! Starts with one tile, goes outward for all the neighboring tiles AND all the neighboring tiles of THAT tile 
            Debug.Log("FindSelectableTiles");
            ComputeAdjacencyLists();
            GetCurrentTile();
            //time to incorporate BFS!

            Queue<Tile> process = new Queue<Tile>();

            process.Enqueue(currentTile); 
            currentTile.visited = true; //never want to come back to this current tile once it's processed
            //currentTile.parent = ??; //leave as null so we can find it when we are backtracking our path

            while (process.Count > 0) //will process only the nodes that are less than 0
            {
                Tile t = process.Dequeue(); //allow us to process this one tile

                selectableTiles.Add(t);
    
                Debug.Log("t.selectable = true");
                t.selectable = true; //all the selectable tiles will then turn red
                

                if(t.distance < move) //if we haven't hit the edge yet,
                {
                    
                    foreach(Tile tile in t.adjacencyList)
                    {

                        if(!tile.visited)  //if we already processed tile, do not process it again
                        {
                            tile.parent = t; //tile parent equal to the tile we set above. any tile adjacent set parent 
                            tile.visited = true;
                            tile.distance = 1 + t.distance; //keeps track of how far away from the start tile we are
                            process.Enqueue(tile);

                        }
                    }
               }
            }

       }

    //11 MINUTES INTO TUTORIAL 3

    public void MoveToTile(Tile tile)
    {

        //will complete the path

           path.Clear();
           tile.target = true;
           moving = true;

           //this is our end location

           Tile next = tile;
           while(next != null) //once player reaches target at end location, we have the entire path they took to get there to backtrack and walk through it before the player actually moves. This just outlines the path, basically.
           {

           //push the tile onto the path
                path.Push(next);
                next = next.parent; //start on target, then go to the parent of that one, then the parent of that one
           }
    }

    public void Move() //move the tile one tile to the next. 
    {
        Debug.Log("Move");
                //as long as there is something in the path,
        if(path.Count > 0)
        {

        //set the tile we're moving to

        Tile t = path.Peek();
        Vector3 target = t.transform.position; //get the target position, this is what we will move to

        //calculate the unit's position on top of the target tiile.
        target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y; //the halfHeight of the player AND the current tile


            if(Vector3.Distance(transform.position, target) >= 0.05f) //if the distance between the units. if you are 0.05 away, you haven't found it yet
            {

                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else //if we don't need to jump,
                {
                    CalculateHeading(target); //calculate the headinf to the current target
                    SetHorizontalVelocity(); //set the velocity equal to the movespeed times the heading

                }
                


                //face the direction we're gonna go
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime; //basic physics (lol)
            }
            else //once you are less than 0.05 away, tile center reached
            { 
            //still puts the player exactly in the center of the target tile, for functionality's assurance.

                transform.position = target;
                path.Pop();
            
            }
        
        }
        else     //when we reach the end,
        {
            //remove the selectable tiles since they are no longer active
            moving = false; //we are no longer moving
        
        }

    
    }

    protected void RemoveSelectableTiles()
    {
        Debug.Log("RemoveSelectableTiles");
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;

        }

    //this is why we created the list above
        foreach(Tile tile in selectableTiles)
        {
            tile.Reset(); //resets the current and target
        }

        selectableTiles.Clear(); //clear the list
    
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizontalVelocity() //move forward
    {
           velocity = heading * moveSpeed; //the heading is the direction we are moving
    }

    void Jump(Vector3 target) //jump, lol
    {
        if (fallingDown)
        {
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else //if none of these states are set, then we jump downward
        {
            PrepareJump(target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y; //set the target's y to the unit's y

        CalculateHeading(target);

        if(transform.position.y > targetY) //are we jumping up,
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            //so ww know when to stop and fall downwardm
            jumpTarget = transform.position + (target - transform.position / 2.0f); //finds the halfway point between the target and the current position (when jumping)
        }
        else //or are we jumping down?
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f; //good speed

            //how far are we jumping up?
            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
    //make the unit fall downward
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y <= target.y)
        {
            fallingDown = true;
            jumpingUp = false;
            movingEdge = false;


            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
            //velocity += Physics.gravity * Time.deltaTime;
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;


        //have we completed jumping over the target?
        if(transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
            

        }
    }

    void MoveToEdge() //will move us to the edge of the tile before we fall off
    {
        if(Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        } 
        else //once we've reached within 0.05 of the edge,
        {
            movingEdge = false;
            fallingDown = true;


            velocity /= 1.0f;
            Debug.Log("Little Hop");
            velocity.y = 1.5f; //little hop
            
        }
    }

}

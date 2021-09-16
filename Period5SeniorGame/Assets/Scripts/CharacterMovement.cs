using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{

    //8 MINUTES

    public Tilemap map;

    [SerializeField] private float movementSpeed;
 
    MouseInput mouseInput;

    private Vector3 destination; //the destination we want to move to

   public void Awake()
   {
       mouseInput = new MouseInput(); //sets the variable mouseInput to a MouseInput
   }

    public void OnEnable() //if the script is enabled, enable te function
    {
        mouseInput.Enable();

    }

    public void OnDisable() //if the script is disabled, disable the function
    {
        mouseInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //sets the destination at a current position so we aren't moving at the start of the game.

        destination = transform.position;
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick(); //once its (the action) been performed, then we will call this (MouseClick()) function.
    }

    public void MouseClick()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>(); //returns the mouse position in pixel coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); //converts pixel coordinates to point in 3D space. 

        //makes sure we are clicking the cell in the grid
        Vector3Int gridPosition = map.WorldToCell(mousePosition); //convert world coordinates to tilemap coords. even if the player clicks outside the map, the player still moves. Following if statement fixes this.
        if (map.HasTile(gridPosition)) //if the map has a tile on that grid position, then we can move to that position.
        {
            destination = mousePosition; //destination = the world position
        }

        //8 MINUTES
    }

    // Update is called once per frame
    void Update()
    {
        //this will move to our position

        if (Vector3.Distance(transform.position, destination) > 0.1f) // if the distance from our current position and the destination is greater than 0.1f //to make sure we are not already at the position so this doesn't get called every frame.
        
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime); //then move toward this position
        
        
    }
}

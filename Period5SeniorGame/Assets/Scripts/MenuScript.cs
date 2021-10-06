using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuScript
{
    //basically, this makes an option at the top of the toolbar in Unity. When you press the button, this code assigns a particular material to each of the tiles.
    //this is so you don't have to go through all 500 or so tiles and assign the materials to them individually. 

    [MenuItem("Tools/Assign Tile Material")] //makes a toolbar option at the top in Unity called tools. 

    public static void AssignTileMaterial()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile"); //finds all gameobjects with the tag "tile". Also makes an array of gameobjects called tiles
        Material material = Resources.Load<Material>("OpaqueBox"); //makes a new material and looks for the material named "Tile"

        foreach (GameObject t in tiles) //for each GameObject t in the tiles array,
        {
            t.GetComponent<Renderer>().material = material; //get the renderer's material component and apply our material we made above to each t in tiles[].
        }

    }

  
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

	void Start()
	{
		Init();
	}

	void Update()
	{
		Debug.Log("FindSelectableTiles Update!!!");


		Debug.DrawRay(transform.position, transform.forward);


		if(!moving)
        {
			FindSelectableTiles();
			CheckMouse();
		}
        else
        {
			Move();

        }

	}

	public void CheckMouse()
    {
		//basics of being able to select and click
		Debug.Log("CheckMouse PlayerMove");
		if(Input.GetMouseButtonUp(0)) //if left button is released,
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
            {
				if(hit.collider.tag == "Tile") //only if they have clicked on tiles
                {
					Tile t = hit.collider.GetComponent<Tile>();

					if(t.selectable) //if t is selectable
                    {
						//this will the the target we are going to move to
						MoveToTile(t);
		
                    }
                }

            }

        }

    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    GameController myGameController;
    public int myX, myY;
    // Start is called before the first frame update
    void Start()
    {
        myGameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myGameController.fallingEnabled && (myY > 0 && myY < myGameController.gridY))
        {
            Falling();
            print("moving cubes...");
        }
    }

    private void OnMouseDown()
    {
        if (myGameController.destroyCubeEnabled)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void Falling()
    {
        for (int x = 0; x < myGameController.gridX; x++)
        {
            for (int y = 0; y < myGameController.gridY; y++)
            {
                myGameController.MoveCube(gameObject, myX, myY--);

            }
        }
    }
}

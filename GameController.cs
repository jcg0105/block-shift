using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject cubePreFab, pusherPreFab;
    GameObject[,] grid;
    public int gridX, gridY;
    Vector3 cubePos;
    int matchesFound;
    List<GameObject> matches = new List<GameObject>();
    bool foundMatch;
    public bool actionPhaseActive;
    public bool destroyCubeEnabled;
    public bool fallingEnabled;
    public bool startCondition;
    public bool resolutionPhaseActive;
    float actionTurnTimer = 4.0f;
    float countdownStart = 3.0f;
    public Text countdownTimeTxt;
    public Text countdownTitleTxt;
    public Text actionPhaseTitleTxt;
    public Text actionPhaseTimeTxt;
    public int round, roundsPlayed;
    bool emptySpaceFound;
    static int gridWidth, gridHeight;
    static bool clearTiles = false;
    public bool newBlockSpawned;
    GameObject droppedCube;
    
    public int newScore;
    public int score;
    
    

    Color[] colors = { Color.white, Color.black, Color.blue, Color.green, Color.red, Color.yellow };


    // Start is called before the first frame update
    void Start()
    {
        round = 0;
        roundsPlayed = round;
        matchesFound = 0;
        newBlockSpawned = false;
        foundMatch = false;
        startCondition = true;
        actionPhaseActive = false;
        destroyCubeEnabled = false;
        fallingEnabled = false;
        resolutionPhaseActive = false;
        emptySpaceFound = false;
        gridX = 8;
        gridY = 5;
        gridWidth = 8;
        grid = new GameObject[gridX, gridY];
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCondition) //planning phase
        {
            actionPhaseTimeTxt.gameObject.SetActive(false);
            actionPhaseTitleTxt.gameObject.SetActive(false);

            if (countdownStart > 0)
            {
                countdownStart -= Time.deltaTime;
                countdownTitleTxt.text = "Get Ready... ";
                countdownTimeTxt.text = (countdownStart).ToString("0");
            }
            else if (countdownStart < 0)
            {
                countdownTimeTxt.gameObject.SetActive(false);
                countdownTitleTxt.gameObject.SetActive(false);
                actionPhaseActive = true;
                startCondition = false;
            }
        }
        else if (actionPhaseActive)
        {
            destroyCubeEnabled = true;
            actionPhaseTimeTxt.gameObject.SetActive(true);
            actionPhaseTitleTxt.gameObject.SetActive(true);

            if (actionTurnTimer > 0)
            {
                actionTurnTimer -= Time.deltaTime;
                actionPhaseTitleTxt.text = "Destroy as many blocks as you can!";
                actionPhaseTimeTxt.text = (actionTurnTimer).ToString("0");

            } else
            {
                actionPhaseActive = false;
                destroyCubeEnabled = false;
                actionPhaseTitleTxt.text = "Done!";
                actionPhaseTimeTxt.gameObject.SetActive(false);
                resolutionPhaseActive = true;
            }
        }

        else if (resolutionPhaseActive)
        {
            getScore();
            //print(score);
            round++;
            newBlockSpawned = false;
            
            fallingEnabled = true;
           // FillEmptySpace();
            //check for mathes
            if (round < roundsPlayed)
            {
                startCondition = true;
            }
            if (round == roundsPlayed)
            {
                print("game over");
                //load end screen
            }
           
            //remove matches + add scores
            //call for the next round
        }
    }

    void CreateGrid()
    {
        Color[] previousLeft = new Color[gridX];
        Color previousBelow = Color.clear;
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                cubePos = new Vector3(x * 2, y * 2, 0);
                grid[x, y] = Instantiate(cubePreFab, cubePos, Quaternion.identity);
                grid[x, y].GetComponent<CubeController>().myX = x;
                grid[x, y].GetComponent<CubeController>().myY = y;

                //CHECK FOR REPEATING TILES AND GET RID OF THEM!
                //list of possible colors
                List<Color> possibleColors = new List<Color>();
                //add all colors to list
                possibleColors.AddRange(colors);
                //remove colors to left and below current cube from list of possible colors
                possibleColors.Remove(previousLeft[x]);
                possibleColors.Remove(previousBelow);
                grid[x, y].GetComponent<Renderer>().material.color = possibleColors[Random.Range(0, possibleColors.Count)];

                previousLeft[x] = grid[x, y].GetComponent<Renderer>().material.color;
                previousBelow = grid[x, y].GetComponent<Renderer>().material.color;

            }
        }
    }
 
    bool CheckForHorizontalMatches(int x, int y)
    {
        if (grid[x, y] != null)
        {
            if (grid[x, y].GetComponent<Renderer>().material.color == grid[x + 1, y].GetComponent<Renderer>().material.color &&
                grid[x, y].GetComponent<Renderer>().material.color == grid[x + 2, y].GetComponent<Renderer>().material.color) //index out of bounds
            {
                print("match at x: " + x + "  y: " + y);
                return true;
            }
        }
        return false;
         
    } 

    bool CheckForVerticalMatches(int x, int y)
    {
        if (grid[x, y] != null)
        {
            if (grid[x, y].GetComponent<Renderer>().material.color == grid[x, y + 1].GetComponent<Renderer>().material.color 
                && grid[x, y].GetComponent<Renderer>().material.color == grid[x, y + 2].GetComponent<Renderer>().material.color) 
            {
                print("match at x: " + x + "  y: " + y);
                return true;
            }
        }
        return false;
    } 
    
    void AddMatch(int x, int y)
    {
        if (!matches.Contains(grid[x,y]))
        {
            matches.Add(grid[x, y]);
        }
    } 

    void IsMatch() 
    {
        foundMatch = false;
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (CheckForHorizontalMatches(x,y))
                {
                    AddMatch(x, y);
                    AddMatch(x + 1, y);
                    AddMatch(x + 2, y);
                    foundMatch = true;
                    print("added horizontal match to list starting with " + x + "  " + y);
                }

                if (CheckForVerticalMatches(x, y))
                {
                    AddMatch(x, y);
                    AddMatch(x, y+1);
                    AddMatch(x, y+2);
                    foundMatch = true;
                    print("added vertical match to list starting with " + x + "  " + y);
                }
            }
        }

        for (int i = 0; i<matches.Count; i++)
        {
            Destroy(matches[i]);
            matches[i] = null;
        }

        if (matches.Count < 3) //min number of matches
        {
            matches.Clear();
        }
    }

    public void MoveCube(GameObject newCube, int currX, int currY) 
    {

            if (grid[currX, currY] == null && currX >= 0 && currY >=0) //index out of range
            {
                print("space at " + currX + "  " + currY + "is null");
                grid[currX, currY] = newCube;
                cubePos = new Vector3(currX * 2, currY * 2, 0);
                newCube.transform.position = cubePos;
                grid[currX, currY].GetComponent<CubeController>().myX = currX;
                grid[currX, currY].GetComponent<CubeController>().myY = currY;
            }

        
    }

    public void getScore()
    {
        newScore = 0;
        for (int x = 0; x < gridX - 2; x++)
        {
            for (int y = 0; y < gridY - 2; y++)
            {
                if (foundMatch)
                {
                    newScore += 10;
                }
            }
        }
        score += newScore;
    }


    public void DropCubes()
    {
        for (int y = 0; y<=gridY; y++)
        {
            for (int x = 0; x<=gridX; x++)
            {
                int checkHeight = y;
                GameObject lowestInColumn = grid[x, checkHeight];
                GameObject dropTo;
                while (lowestInColumn == null && checkHeight <= gridY)
                {
                    checkHeight++;
                    lowestInColumn = grid[x, checkHeight];
                }
                if (lowestInColumn != null )
                {

                }
            }
        }
    }
    /*search for empty spaces in grid
    public void FindEmptySpace()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y <= gridY; y++)
            {
                if (grid[x, y] == null && x>=0 && y>=0)
                {
                    print("space found at: " + x + "  " + y);
                    grid[x, y+1] = droppedCube;
                    cubePos = new Vector3(x * 2, y * 2, 0);
                    droppedCube.transform.position = cubePos;
                    grid[x, y].GetComponent<CubeController>().myX = x;
                    grid[x, y].GetComponent<CubeController>().myY = y;
                    if (y == gridY)
                    {
                        cubePos = new Vector3(x * 2, y * 2, 0);
                        grid[x, y] = Instantiate(cubePreFab, cubePos, Quaternion.identity);
                        grid[x, y].GetComponent<CubeController>().myX = x;
                        grid[x, y].GetComponent<CubeController>().myY = y;
                        grid[x, y].GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
                    }
                    
                }
            }
        }
        //get the position at x, y+1 and set it to x, y ... do this for every cube above it
        //if there's no cube at y+1 then spawn a new one in x,y
    } */


}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{

    private bool gameCleared = false;

    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject CleatText;
    public GameObject wallPrefab;
    public GameObject ResetGameText;
    int[,] map;
    GameObject[,] field;


   Vector3 IndexToPosition(Vector2Int index)
   {
       return new Vector3(
           index.x - map.GetLength(1) / 2 + 0.5f, 
           index.y - map.GetLength(0) / 2 + 0.5f,0                      
           );
   }

    void ResetGame()
    {
      CleatText.SetActive(false);

        InitializeField();

        gameCleared = false;

        Vector2Int startPosition = new Vector2Int(1, 2); 
        GameObject player = field[startPosition.y, startPosition.x];
        if (player != null && player.tag == "Player")
        {
            player.transform.position = IndexToPosition(startPosition);
        }

    }
    


    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {

                if (field[y, x] == null) { continue; }

                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool IsCleared()
    {
        List<Vector2Int> goals = new List<Vector2Int>();
    
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
    
        }
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                return false;
            }
        }
        return true;
    }
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0) ||
            moveTo.x < 0 || moveTo.x >= field.GetLength(1))
        {
            return false; 
        }
    
    
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        
        }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
        }

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
        field[moveFrom.y,moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
        field[moveFrom.y, moveFrom.x] = null;
        return true;
    }

    void InitializeField()
    {

        

        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null)
                {
                    Destroy(field[y, x]);
                }
            }
        }

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 2) //found block
                {
                    field[y, x] = Instantiate(
                        boxPrefab, 
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                        );
                    
                }
                if (map[y, x] == 3) //goal
                {
                    field[y, x] = Instantiate(
                        goalPrefab, 
                        new Vector3(x, map.GetLength(0) - y, 0.01f),
                        Quaternion.identity
                        );


                }
                if (map[y, x] == 4) //wall
                {
                    field[y, x] = Instantiate(
                        wallPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                        );


                }
            }

        }



    }



    // Start is called before the first frame update
    void Start()
    {

       Screen.SetResolution(1280, 720, false);

        

        map = new int[,]{
         {4,4,4,4,4,4,4},
         {4,0,1,2,3,0,4},
         {4,0,0,4,0,0,4},
         {4,0,0,2,3,0,4},
         {4,0,0,0,0,0,4},
         {4,4,4,4,4,4,4},
         };
         field = new GameObject
             [
             map.GetLength(0),
             map.GetLength(1)
             ];


        InitializeField();

    }

   

    // Update is called once per frame
    void Update()
    {
        ResetGameText.SetActive(true);

        if (gameCleared == false)
        {
         if (Input.GetKeyDown(KeyCode.RightArrow))
         {
             Vector2Int playerIndex = GetPlayerIndex();
             MoveNumber(
                 playerIndex,
                 playerIndex + new Vector2Int(1, 0));
        }
         if (Input.GetKeyDown(KeyCode.LeftArrow))
         {
             Vector2Int playerIndex = GetPlayerIndex();
             MoveNumber(
                 playerIndex,
                 playerIndex + new Vector2Int(-1, 0));
         }
         if (Input.GetKeyDown(KeyCode.UpArrow))
         {
             Vector2Int playerIndex = GetPlayerIndex();
             MoveNumber(
                 playerIndex,
                 playerIndex + new Vector2Int(0, -1));
         }
         if (Input.GetKeyDown(KeyCode.DownArrow))
         {
             Vector2Int playerIndex = GetPlayerIndex();
             MoveNumber(
                 playerIndex,
                 playerIndex + new Vector2Int(0, 1));
         }
        }
        if (IsCleared() && !gameCleared)
        {
            CleatText.SetActive(true);
            gameCleared = true;
        }

        

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }

    }
}

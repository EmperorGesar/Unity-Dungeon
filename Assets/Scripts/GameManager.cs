using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject ground;
    public GameObject wall;
    public GameObject door;
    public GameObject lantern;
    public GameObject doll;
    public GameObject coin;
    public GameObject gem;

    public Canvas LaunchUI;
    public Canvas GameUI;
    public Canvas PauseUI;
    public Canvas GameoverUI;
    public Canvas LeaderboardUI;

    public TextMeshProUGUI roomText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI resurrectionText;
    public TMP_InputField inputField;

    public static GameObject doorActive;
    public static int coinTotal;
    public static bool isGameActive;
    public static bool isGameover;
    public static bool obtainedResurrection;
    
    private direction exit;
    private direction store;
    private List<GameObject> room1;
    private List<GameObject> room2;
    private bool isRoomActive;
    private int roomCount;
    private string nameNew;

    public enum direction
    {
        left, right, up, down, centre
    }

    void Start()
    {
        isGameActive = false;
        isGameover = false;
        isRoomActive = true;
        obtainedResurrection = false;
        room1 = new List<GameObject>();
        roomCount = 0;
        coinTotal = 0;
        inputField.characterLimit = 3;
    }

    public void StartGame()
    {
        isGameActive = true;
        LaunchUI.gameObject.SetActive(false);
        CreateNewRoom(direction.centre);
    }
    
    
    public void RestartGame()
    {
        if (isGameover)
        {
            Leaderboard.record.Add((roomCount, nameNew));
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    
    public void PauseGame()
    {
        GameUI.gameObject.SetActive(false);
        PauseUI.gameObject.SetActive(true);
        isGameActive = false;
    }
    
    public void ResumeGame()
    {
        PauseUI.gameObject.SetActive(false);
        GameUI.gameObject.SetActive(true);
        isGameActive = true;
    }

    public void ShowLeaderboard()
    {
        LeaderboardUI.gameObject.SetActive(true);
    }

    public void BackLaunch()
    {
        LeaderboardUI.gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && !isGameover)
        {
            if (!LaunchUI.gameObject.activeSelf)
            {
                if (isGameActive)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            } else if (LeaderboardUI.gameObject.activeSelf)
            {
                BackLaunch();
            }
        }
        if (isGameActive)
        {
            if (doorActive.gameObject.transform.localPosition == Vector3.zero && doorActive.CompareTag("closed"))
            {
                foreach (GameObject obj in PlayerController.structures)
                {
                    if (obj.CompareTag("raised"))
                    {
                        Destroy(obj);
                    }
                }
                if (isRoomActive)
                {
                    if (room1 != null)
                    {
                        foreach (GameObject obj in room1)
                        {
                            if (obj != null && obj.gameObject.name.Contains("door"))
                            {
                                obj.gameObject.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                                obj.gameObject.transform.localPosition += new Vector3(0f, -1.25f, 0f);
                                obj.tag = "raised";
                            }
                            else
                            {
                                Destroy(obj);
                            }
                        }
                    }
                    room1 = new List<GameObject>();
                }

                if (!isRoomActive)
                {
                    if (room2 != null)
                    {
                        foreach (GameObject obj in room2)
                        {
                            if (obj != null && obj.gameObject.name.Contains("door"))
                            {
                                obj.gameObject.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                                obj.gameObject.transform.localPosition += new Vector3(0f, -1.25f, 0f);
                                obj.tag = "raised";
                            }
                            else
                            {
                                Destroy(obj);
                            }
                        }
                    }
                    room2 = new List<GameObject>();
                }
                doorActive.tag = "opened";
                CreateNewRoom(exit);
            }

            coinText.text = "Coin\n" + coinTotal;
        }
        if (isGameover)
        {
            GameoverUI.gameObject.SetActive(true);
            nameNew = inputField.text;
        }

        if (obtainedResurrection)
        {
            resurrectionText.gameObject.SetActive(true);
        }
        else
        {
            resurrectionText.gameObject.SetActive(false);
        }
    }

    public void CreateNewRoom(direction entrance)
    {
        int width = Random.Range(3, 5) * 2 + 1;
        int length = Random.Range(4, 6) * 2 + 1;
        
        Vector3 anchor = new Vector3(-2.5f * (length - 1) / 2, 0f, -2.5f * (width - 1) / 2);
        
        if (entrance == direction.up)
        {
            anchor = new Vector3(2.5f, 0f, -2.5f);
        }
        else if (entrance == direction.right)
        {
            anchor = new Vector3(-2.5f, 0f, -2.5f * width);
        }
        else if (entrance == direction.down)
        {
            anchor = new Vector3(-2.5f * length, 0f, -2.5f * width + 5);
        }
        else if (entrance == direction.left)
        {
            anchor = new Vector3(-2.5f * length + 5f, 0f, 2.5f);
        }

        direction[] dirs = ((direction[]) Enum.GetValues(typeof(direction))).Where(dir => dir != entrance && dir != direction.centre).ToArray();
        exit = (direction) dirs.GetValue(Random.Range(0, dirs.Length));

        Vector3 pos;
        
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                pos = new Vector3(-2.5f * i, -2.5f, -2.5f * j) - anchor;
                if (isRoomActive)
                {
                    room1.Add(Instantiate(ground, pos, Quaternion.Euler(-90f, 0f, 0f)));
                } else {
                    room2.Add(Instantiate(ground, pos, Quaternion.Euler(-90f, 0f, 0f)));
                }
                if (i == 0)
                {
                    if (j == 1 && exit == direction.up)
                    {
                        pos = new Vector3(2.5f, -1.25f, -2.5f * j) - anchor;
                        doorActive = Instantiate(door, pos, Quaternion.Euler(-90f, 0f, 0f));
                        doorActive.tag = "locked";
                        if (isRoomActive)
                        {
                            room1.Add(doorActive);
                        } else {
                            room2.Add(doorActive);
                        }
                    } else if (j != 1 && entrance == direction.up || entrance != direction.up){
                        pos = new Vector3(2.5f, -1.25f, -2.5f * j) - anchor;
                        if (isRoomActive)
                        {
                            room1.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                        } else {
                            room2.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                        }
                    }

                    if (j == width - 2 && exit == direction.down)
                    {
                        pos = new Vector3(-2.5f * length, -1.25f, -2.5f * j) - anchor;
                        doorActive = Instantiate(door, pos, Quaternion.Euler(-90f, 0f, 0f));
                        doorActive.tag = "locked";
                        if (isRoomActive)
                        {
                            room1.Add(doorActive);
                        } else {
                            room2.Add(doorActive);
                        }
                    } else if (j != width - 2 && entrance == direction.down || entrance != direction.down) {
                        pos = new Vector3(-2.5f * length, -1.25f, -2.5f * j) - anchor;
                        if (isRoomActive)
                        {
                            room1.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                        } else {
                            room2.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                        }
                    }
                }
            }
        }

        for (int i = -1; i < length + 1; i++)
        {
            if (i == length - 2 && exit == direction.left)
            {
                pos = new Vector3(-2.5f * i, -1.25f, 2.5f) - anchor;
                doorActive = Instantiate(door, pos, Quaternion.Euler(-90f, 0f, 0f));
                doorActive.tag = "locked";
                if (isRoomActive)
                {
                    room1.Add(doorActive);
                } else {
                    room2.Add(doorActive);
                }
            } else if (i != length - 2 && entrance == direction.left || entrance != direction.left){
                pos = new Vector3(-2.5f * i, -1.25f, 2.5f) - anchor;
                if (isRoomActive)
                {
                    room1.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                } else {
                    room2.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                }
            }

            if (i == 1 && exit == direction.right)
            {
                pos = new Vector3(-2.5f * i, -1.25f, -2.5f * width) - anchor;
                doorActive = Instantiate(door, pos, Quaternion.Euler(-90f, 0f, 0f));
                doorActive.tag = "locked";
                if (isRoomActive)
                {
                    room1.Add(doorActive);
                } else {
                    room2.Add(doorActive);
                }
            } else if (i != 1 && entrance == direction.right || entrance != direction.right){
                pos = new Vector3(-2.5f * i, -1.25f, -2.5f * width) - anchor;
                if (isRoomActive)
                {
                    room1.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                } else {
                    room2.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
                }
            }
        }
        
        int numLantern = Random.Range(2, 5);
        List<Vector3> posStructure = new List<Vector3>();
        
        do
        {
            pos = new Vector3(-2.5f * Random.Range(0, length - 1), 0.4f, -2.5f * Random.Range(0, width - 1)) - anchor;
        } while (pos.x == 0 && pos.z == 0);
        posStructure.Add(pos);
        
        for (int i = 0; i < numLantern; i++)
        {
            PlayerController.monsters.Add(Instantiate(lantern, pos, Quaternion.Euler(-90f, 0f, 0f)));
            do
            {
                pos = new Vector3(-2.5f * Random.Range(0, length - 1), 0.4f, -2.5f * Random.Range(0, width - 1)) - anchor;
            } while (posStructure.Contains(pos) || pos.x == 0 && pos.z == 0);
            posStructure.Add(pos);
        }

        int numDoll = Random.Range(2, 5);
        for (int i = 0; i < numDoll; i++)
        {
            do
            {
                pos = new Vector3(-2.5f * Random.Range(0, length - 1), 0.4f, -2.5f * Random.Range(0, width - 1)) - anchor;
            } while (posStructure.Contains(pos) || pos.x == 0 && pos.z == 0);
            posStructure.Add(pos);
            pos += new Vector3(0f, 0.1f, 0f);
            PlayerController.monsters.Add(Instantiate(doll, pos, Quaternion.Euler(0f, 90f * Random.Range(0, 4), 0f)));
        }

        int numObstacle = Random.Range(4, 7);
        for (int i = 0; i < numObstacle; i++)
        {
            do
            {
                pos = new Vector3(-2.5f * Random.Range(1, length - 2), 0.4f, -2.5f * Random.Range(1, width - 2)) - anchor;
            } while (posStructure.Contains(pos) || pos.x == 0 && pos.z == 0);
            posStructure.Add(pos);
            pos += new Vector3(0f, -1.65f, 0f);
            if (isRoomActive)
            {
                room1.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
            } else {
                room2.Add(Instantiate(wall, pos, Quaternion.Euler(-90f, 0f, 0f)));
            }
        }

        int coinRate = Random.Range(0, 100);
        int gemRate = Random.Range(0, 100);

        if (coinRate < 50)
        {
            do
            {
                pos = new Vector3(-2.5f * Random.Range(0, length - 1), 0.4f, -2.5f * Random.Range(0, width - 1)) - anchor;
            } while (posStructure.Contains(pos) || pos.x == 0 && pos.z == 0);
            posStructure.Add(pos);
            pos += new Vector3(0f, -0.3f, 0f);
            if (isRoomActive)
            {
                room1.Add(Instantiate(coin, pos, Quaternion.Euler(-90f, 0f, 0f)));
            } else {
                room2.Add(Instantiate(coin, pos, Quaternion.Euler(-90f, 0f, 0f)));
            }
        }

        if (!obtainedResurrection && coinTotal > 4 && gemRate < 25)
        {
            do
            {
                pos = new Vector3(-2.5f * Random.Range(0, length - 1), 0.4f, -2.5f * Random.Range(0, width - 1)) - anchor;
            } while (posStructure.Contains(pos) || pos.x == 0 && pos.z == 0);
            posStructure.Add(pos);
            pos += new Vector3(0f, -0.2f, 0f);
            if (isRoomActive)
            {
                room1.Add(Instantiate(gem, pos, Quaternion.Euler(-90f, -135f, 0f)));
            } else {
                room2.Add(Instantiate(gem, pos, Quaternion.Euler(-90f, -135f, 0f)));
            }
        }

        if (exit == direction.up)
        {
            exit = direction.down;
        } 
        else if (exit == direction.down)
        {
            exit = direction.up;
        }
        else if (exit == direction.left)
        {
            exit = direction.right;
        }
        else if (exit == direction.right)
        {
            exit = direction.left;
        }

        roomCount += 1;
        roomText.text = "Room\n" + roomCount;
        
        isRoomActive = !isRoomActive;

    }
}

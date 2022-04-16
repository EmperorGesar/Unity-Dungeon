using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public GameObject forbidUp;
    public GameObject forbidRight;
    public GameObject forbidDown;
    public GameObject forbidLeft;
    
    public Canvas LaunchUI;
    
    private AudioSource bgm;

    private Vector3 dir;
    private Vector3 trans;
    private Vector3 facing;

    private bool tick;
    
    public static List<GameObject> structures;
    public static List<GameObject> monsters;
    
    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(0f, 180f, 0f);
        trans = new Vector3(0f, 0f, 0f);
        facing = new Vector3(0f, -1.25f, -2.5f);
        tick = true;
        structures = new List<GameObject>();
        monsters = new List<GameObject>();
        bgm = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isGameActive)
        {
            structures = FindObjectsOfType<GameObject>().ToList();
        
            if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
            {
                dir = new Vector3(0f, 90f, 0f);
                trans = Vector3.left;
                facing = new Vector3(2.5f, -1.25f, 0f);
                if (forbidUp.activeSelf && !isFrontWall())
                {
                    if (GameManager.obtainedResurrection)
                    {
                        GameManager.obtainedResurrection = false;
                    }
                    else
                    {
                        GameManager.isGameover = true;
                        GameManager.isGameActive = false;
                    }
                }
                MoveMaze();
            } else if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
                dir = new Vector3(0f, 180f, 0f);
                facing = new Vector3(0f, -1.25f, -2.5f);
                trans = Vector3.forward;
                if (forbidRight.activeSelf && !isFrontWall())
                {
                    if (GameManager.obtainedResurrection)
                    {
                        GameManager.obtainedResurrection = false;
                    }
                    else
                    {
                        GameManager.isGameover = true;
                        GameManager.isGameActive = false;
                    }
                }
                MoveMaze();
            } else if (Input.GetKeyDown("down") || Input.GetKeyDown("s")) {
                dir = new Vector3(0f, -90f, 0f);
                facing = new Vector3(-2.5f, -1.25f, 0f);
                trans = Vector3.right;
                if (forbidDown.activeSelf && !isFrontWall())
                {
                    if (GameManager.obtainedResurrection)
                    {
                        GameManager.obtainedResurrection = false;
                    }
                    else
                    {
                        GameManager.isGameover = true;
                        GameManager.isGameActive = false;
                    }
                }
                MoveMaze();
            } else if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
                dir = new Vector3(0f, 0f, 0f);
                facing = new Vector3(0f, -1.25f, 2.5f);
                trans = Vector3.back;
                if (forbidLeft.activeSelf && !isFrontWall())
                {
                    if (GameManager.obtainedResurrection)
                    {
                        GameManager.obtainedResurrection = false;
                    }
                    else
                    {
                        GameManager.isGameover = true;
                        GameManager.isGameActive = false;
                    }
                }
                MoveMaze();
            }
            
            transform.localEulerAngles = dir;

            if (monsters.Count == 0 && GameManager.doorActive.CompareTag("locked"))
            {
                GameManager.doorActive.gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                GameManager.doorActive.gameObject.transform.localPosition -= new Vector3(0f, -1.25f, 0f);
                GameManager.doorActive.tag = "closed";
            }

            foreach (GameObject obj in structures)
            {
                if (obj.gameObject.name.Contains("coin") && obj.transform.localPosition.x == 0 && obj.transform.localPosition.z == 0)
                {
                    GameManager.coinTotal += 1;
                    Destroy(obj);
                }
                if (obj.gameObject.name.Contains("gem") && obj.transform.localPosition.x == 0 && obj.transform.localPosition.z == 0)
                {
                    GameManager.coinTotal -= 1;
                    GameManager.obtainedResurrection = true;
                    Destroy(obj);
                }
            }
        }
        if (LaunchUI.gameObject.activeSelf)
        {
            if (bgm.isPlaying)
            {
                bgm.Stop();
            }
        } else
        {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
        }
    }

    public void DestroyMonster()
    {
        GameObject corpse = null;
            
        foreach (GameObject mon in monsters)
        {
            if (mon.gameObject.transform.localPosition.x == 0 && mon.gameObject.transform.localPosition.z == 0)
            {
                corpse = mon;
            }
        }
            
        if (corpse != null)
        {
            monsters.Remove(corpse);
            Destroy(corpse);
        }
    }

    public bool isFrontWall()
    {
        foreach (GameObject obj in structures)
        {
            if (obj != null && obj.transform.localPosition == facing)
            {
                return true;
            }
        }

        return false;
    }

    public void MoveMaze()
    {

        if (!isFrontWall())
        {
            foreach (GameObject obj in structures)
            {
                if (obj != null && obj.gameObject.name.Contains("Clone"))
                {
                    obj.transform.localPosition += 2.5f * trans;
                }
            }

            DestroyMonster();
            MoveMonster();
        }
    }

    public void MoveMonster()
    {
        forbidUp.SetActive(false);
        forbidRight.SetActive(false);
        forbidDown.SetActive(false);
        forbidLeft.SetActive(false);
        
        GameManager.direction[] dirs;
        
        if (!tick)
        {

            foreach (GameObject mon in monsters)
            {
                if (mon.gameObject.name.Contains("lantern"))
                {
                    mon.gameObject.transform.localPosition += Vector3.up;
                    dirs = (GameManager.direction[]) Enum.GetValues(typeof(GameManager.direction));
                    foreach (GameObject obj in structures)
                    {
                        if (obj != null && !obj.gameObject.name.Contains("ground"))
                        {
                            if (obj.gameObject.transform.localPosition.x ==
                                (mon.transform.localPosition + 2.5f * Vector3.left).x &&
                                obj.gameObject.transform.localPosition.z ==
                                (mon.transform.localPosition + 2.5f * Vector3.left).z)
                            {
                                dirs = dirs.Where(d => d != GameManager.direction.up).ToArray();
                            } 
                            else if (obj.gameObject.transform.localPosition.x ==
                                       (mon.transform.localPosition + 2.5f * Vector3.forward).x &&
                                       obj.gameObject.transform.localPosition.z ==
                                       (mon.transform.localPosition + 2.5f * Vector3.forward).z)
                            {
                                dirs = dirs.Where(d => d != GameManager.direction.right).ToArray();
                            }
                            else if (obj.gameObject.transform.localPosition.x ==
                                     (mon.transform.localPosition + 2.5f * Vector3.right).x &&
                                     obj.gameObject.transform.localPosition.z ==
                                     (mon.transform.localPosition + 2.5f * Vector3.right).z)
                            {
                                dirs = dirs.Where(d => d != GameManager.direction.down).ToArray();
                            }
                            else if (obj.gameObject.transform.localPosition.x ==
                                     (mon.transform.localPosition + 2.5f * Vector3.back).x &&
                                     obj.gameObject.transform.localPosition.z ==
                                     (mon.transform.localPosition + 2.5f * Vector3.back).z)
                            {
                                dirs = dirs.Where(d => d != GameManager.direction.left).ToArray();
                            }
                        }
                    }
                    
                    GameManager.direction next = (GameManager.direction) dirs.GetValue(Random.Range(0, dirs.Length));

                    if (next == GameManager.direction.up)
                    {
                        mon.transform.localPosition += 2.5f * Vector3.left;
                    } 
                    else if (next == GameManager.direction.right)
                    {
                        mon.transform.localPosition += 2.5f * Vector3.forward;
                    }
                    else if (next == GameManager.direction.down)
                    {
                        mon.transform.localPosition += 2.5f * Vector3.right;
                    }
                    else if (next == GameManager.direction.left)
                    {
                        mon.transform.localPosition += 2.5f * Vector3.back;
                    }
                }
            }
        }
        else
        {
            foreach (GameObject mon in monsters)
            {
                if (mon.gameObject.name.Contains("lantern"))
                {
                    mon.gameObject.transform.localPosition -= Vector3.up;
                
                    if (mon.gameObject.transform.localPosition.x == 2.5f &&
                        mon.gameObject.transform.localPosition.z == 2.5f)
                    {
                        forbidUp.SetActive(true);
                        forbidLeft.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == 2.5f &&
                             mon.gameObject.transform.localPosition.z == -2.5f)
                    {
                        forbidUp.SetActive(true);
                        forbidRight.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == -2.5f &&
                             mon.gameObject.transform.localPosition.z == 2.5f)
                    {
                        forbidDown.SetActive(true);
                        forbidLeft.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == -2.5f &&
                             mon.gameObject.transform.localPosition.z == -2.5f)
                    {
                        forbidDown.SetActive(true);
                        forbidRight.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == 5f &&
                             mon.gameObject.transform.localPosition.z == 0f)
                    {
                        forbidUp.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == 0f &&
                             mon.gameObject.transform.localPosition.z == -5f)
                    {
                        forbidRight.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == -5f &&
                             mon.gameObject.transform.localPosition.z == 0f)
                    {
                        forbidDown.SetActive(true);
                    } 
                    else if (mon.gameObject.transform.localPosition.x == 0f &&
                             mon.gameObject.transform.localPosition.z == 5f)
                    {
                        forbidLeft.SetActive(true);
                    } 
                }
            }
        }

        int turnRate;

        foreach (GameObject mon in monsters)
        {
            if (mon.gameObject.name.Contains("doll"))
            {
                dirs = (GameManager.direction[]) Enum.GetValues(typeof(GameManager.direction));
                bool isFrontWall = false;
                
                foreach (GameObject obj in structures)
                {
                    if (obj != null && !obj.gameObject.name.Contains("ground"))
                    {
                        if (obj.gameObject.transform.localPosition.x == 
                            (mon.transform.localPosition + 2.5f * Vector3.left).x && 
                            obj.gameObject.transform.localPosition.z == 
                            (mon.transform.localPosition + 2.5f * Vector3.left).z) 
                        { 
                            dirs = dirs.Where(d => d != GameManager.direction.up).ToArray();
                            if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 270f, 0f)) {
                                isFrontWall = true;
                            }
                        } 
                        else if (obj.gameObject.transform.localPosition.x == 
                                 (mon.transform.localPosition + 2.5f * Vector3.forward).x && 
                                 obj.gameObject.transform.localPosition.z == 
                                 (mon.transform.localPosition + 2.5f * Vector3.forward).z) 
                        { 
                            dirs = dirs.Where(d => d != GameManager.direction.right).ToArray(); 
                            if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 0f, 0f)) {
                                isFrontWall = true;
                            }
                        }
                        else if (obj.gameObject.transform.localPosition.x == 
                                 (mon.transform.localPosition + 2.5f * Vector3.right).x && 
                                 obj.gameObject.transform.localPosition.z == 
                                 (mon.transform.localPosition + 2.5f * Vector3.right).z) 
                        { 
                            dirs = dirs.Where(d => d != GameManager.direction.down).ToArray(); 
                            if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 90f, 0f)) {
                                isFrontWall = true;
                            }
                        }
                        else if (obj.gameObject.transform.localPosition.x == 
                                 (mon.transform.localPosition + 2.5f * Vector3.back).x && 
                                 obj.gameObject.transform.localPosition.z == 
                                 (mon.transform.localPosition + 2.5f * Vector3.back).z)
                        { 
                            dirs = dirs.Where(d => d != GameManager.direction.left).ToArray();
                            if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 180f, 0f)) {
                                isFrontWall = true;
                            }
                        }
                    }
                }

                GameManager.direction next = (GameManager.direction) dirs.GetValue(Random.Range(0, dirs.Length));
                turnRate = Random.Range(0, 100);

                if (turnRate < 50 || isFrontWall)
                {
                    if (next == GameManager.direction.up)
                    {
                        mon.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
                    }
                    else if (next == GameManager.direction.right)
                    {
                        mon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    else if (next == GameManager.direction.down)
                    {
                        mon.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
                    }
                    else if (next == GameManager.direction.left)
                    {
                        mon.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    }
                }
                else
                {
                    if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 270f, 0f))
                    {
                        mon.transform.localPosition += 2.5f * Vector3.left;
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 0f, 0f))
                    {
                        mon.transform.localPosition += 2.5f * Vector3.forward;
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 90f, 0f))
                    {
                        mon.transform.localPosition += 2.5f * Vector3.right;
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 180f, 0f))
                    {
                        mon.transform.localPosition += 2.5f * Vector3.back;
                    }
                }
                
                if (mon.gameObject.transform.localPosition.x == 5f &&
                    mon.gameObject.transform.localPosition.z == 0f &&
                    mon.gameObject.transform.localEulerAngles == new Vector3(0f, 270f, 0f))
                {
                    forbidUp.SetActive(true);
                } 
                else if (mon.gameObject.transform.localPosition.x == 0f &&
                         mon.gameObject.transform.localPosition.z == -5f &&
                         mon.gameObject.transform.localEulerAngles == new Vector3(0f, 0f, 0f))
                {
                    forbidRight.SetActive(true);
                } 
                else if (mon.gameObject.transform.localPosition.x == -5f &&
                         mon.gameObject.transform.localPosition.z == 0f && 
                         mon.gameObject.transform.localEulerAngles == new Vector3(0f, 90f, 0f))
                {
                    forbidDown.SetActive(true);
                } 
                else if (mon.gameObject.transform.localPosition.x == 0f &&
                         mon.gameObject.transform.localPosition.z == 5f &&
                         mon.gameObject.transform.localEulerAngles == new Vector3(0f, 180f, 0f))
                {
                    forbidLeft.SetActive(true);
                } 
                else if (mon.gameObject.transform.localPosition.x == 2.5f &&
                         mon.gameObject.transform.localPosition.z == 2.5f)
                {
                    if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 180f, 0f))
                    {
                        forbidUp.SetActive(true);
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 270f, 0f))
                    {
                        forbidLeft.SetActive(true);
                    }
                } 
                else if (mon.gameObject.transform.localPosition.x == 2.5f &&
                         mon.gameObject.transform.localPosition.z == -2.5f)
                {
                    if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 0f, 0f))
                    {
                        forbidUp.SetActive(true);
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 270f, 0f))
                    {
                        forbidRight.SetActive(true);
                    }
                } 
                else if (mon.gameObject.transform.localPosition.x == -2.5f &&
                         mon.gameObject.transform.localPosition.z == 2.5f)
                {
                    if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 180f, 0f))
                    {
                        forbidDown.SetActive(true);
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 90f, 0f))
                    {
                        forbidLeft.SetActive(true);
                    }
                } 
                else if (mon.gameObject.transform.localPosition.x == -2.5f &&
                         mon.gameObject.transform.localPosition.z == -2.5f)
                {
                    if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 0f, 0f))
                    {
                        forbidDown.SetActive(true);
                    }
                    else if (mon.gameObject.transform.localEulerAngles == new Vector3(0f, 90f, 0f))
                    {
                        forbidRight.SetActive(true);
                    }
                } 
            }
        }

        tick = !tick;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public static List<(int, string)> record = new List<(int, string)>();

    public TextMeshProUGUI first;
    public TextMeshProUGUI second;
    public TextMeshProUGUI third;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isGameActive && !GameManager.isGameover)
        {
            record.Sort(Comparer<(int, string)>.Default);
            record.Reverse();
            
            if (record.Count == 1)
            {
                first.text = "1. " + record[0].Item2 + "\t" + record[0].Item1 + " Rooms";
            }
            if (record.Count == 2)
            {
                first.text = "1. " + record[0].Item2 + "\t" + record[0].Item1 + " Rooms";
                second.text = "2. " + record[1].Item2 + "\t" + record[1].Item1 + " Rooms";
            }
            if (record.Count >= 3)
            {
                first.text = "1. " + record[0].Item2 + "\t" + record[0].Item1 + " Rooms";
                second.text = "2. " + record[1].Item2 + "\t" + record[1].Item1 + " Rooms";
                third.text = "3. " + record[2].Item2 + "\t" + record[2].Item1 + " Rooms";
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Limits : MonoBehaviour
{

    public static Limits Instance { get; private set; }
    public List<int> valuesX { get; private set; }
    public List<int> valuesY { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void InitializeLists()
    {
        valuesX = new List<int>();
        valuesY = new List<int>();
    }
    public void AddValues(int x, int y) 
    {
        valuesX.Add(x);
        valuesY.Add(y);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightPuzzle : MonoBehaviour
{

    [Tooltip("This value should be a code using initials of the colors " +
        "Red, Blue, Green, Yellow, Purple, Magenta, Orange, and Cyan." +
        " Ex. RBGYPMOC")]
    [SerializeField] private string code;
    [SerializeField] private UnityEvent OnPuzzleCompleted;

    [Header("WallLamp")]
    [SerializeField] private WallLamps redLamp;
    [SerializeField] private WallLamps blueLamp;
    [SerializeField] private WallLamps greenLamp;
    [SerializeField] private WallLamps yellowLamp;
    [SerializeField] private WallLamps cyanLamp;
    [SerializeField] private WallLamps magentaLamp;
    [SerializeField] private WallLamps purpleLamp;
    [SerializeField] private WallLamps orangeLamp;

    [Header("Lights")]
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject blueLight;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject cyanLight;
    [SerializeField] private GameObject magentaLight;
    [SerializeField] private GameObject purpleLight;
    [SerializeField] private GameObject orangeLight;


    private List<WallLamps> wallLamps = new List<WallLamps>();
    private Dictionary<char, GameObject> lights = new Dictionary<char, GameObject>();

    private List<char> currentInput = new List<char>();
    private bool matches;
    private bool isCorrect = false;
    private string inputString;

    private void Start()
    {
        wallLamps.Add(redLamp);
        wallLamps.Add(blueLamp);
        wallLamps.Add(greenLamp);
        wallLamps.Add(yellowLamp);
        wallLamps.Add(cyanLamp);
        wallLamps.Add(magentaLamp);
        wallLamps.Add(purpleLamp);
        wallLamps.Add(orangeLamp);

        lights.Add('R',redLight);
        lights.Add('B',blueLight);
        lights.Add('G',greenLight);
        lights.Add('Y',yellowLight);
        lights.Add('C',cyanLight);
        lights.Add('M',magentaLight);
        lights.Add('P',purpleLight);
        lights.Add('O',orangeLight);
    }

    public void AddValue(char value)
    {
        if (!isCorrect)
        {
            currentInput.Add(value);
            lights[value].SetActive(false);
            Debug.Log($"{value} added");
            CheckCode();
        }
    }

    private void ClearCurrent()
    {
        Debug.Log("Clear");
        currentInput.Clear();
        foreach (WallLamps wallLamp in wallLamps)
        {
            wallLamp.ResetColor();
        }
        foreach (KeyValuePair<char, GameObject> light in lights)
        {
            light.Value.SetActive(true);
        }
    }

    private void CheckCode()
    {
        if (currentInput.Count == code.Length)
        {
            inputString = "";
            foreach(char letter in currentInput)
            {
                inputString += letter;
            }
            if (inputString == code)
            {
                Debug.Log("Correct");
                isCorrect = true;
                OnPuzzleCompleted.Invoke();
            }
            else
            {
                Debug.Log("Incorrect");
                ClearCurrent();
            }
        }
        else
        {
            matches = true;
            for (int i = 0; i < currentInput.Count; i++)
            {
                if (code[i] != currentInput[i])
                {
                    matches &= false;
                }
            }
            if (!matches)
            {
                Debug.Log("Incorrect");
                ClearCurrent();
            }
        }
    }
}

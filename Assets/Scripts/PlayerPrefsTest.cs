using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Demonstrates storing and retrieving information using PlayerPrefs
public class PlayerPrefsTest : MonoBehaviour
{
    public InputField inputFieldAge;
    public InputField inputFieldName;


    private const string ppAge = "Age";

    void Start()
    {
        int age = PlayerPrefs.GetInt(ppAge);
        string name = PlayerPrefs.GetString("Name");


        inputFieldAge.text = age.ToString();
        inputFieldName.text = name;
    }

    
    void Update()
    {
        
    }


    public void SetAge()
    {
        PlayerPrefs.SetInt(ppAge, int.Parse(inputFieldAge.text));
    }

    public void SetName()
    {
        PlayerPrefs.SetString("Name", inputFieldName.text);
    }

}

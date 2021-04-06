using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class JSONTest : MonoBehaviour
{

    public InputField inputFieldAge;
    public InputField inputFieldName;




    void Start()
    {

    }


    void Update()
    {
        // Save/Load test
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        TestJSONSaver save;

        // Set data
        save.age = int.Parse(inputFieldAge.text);
        save.name = inputFieldName.text;


        string json = JsonUtility.ToJson(save); // Encode object data

        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/myjsonsave.save", false);
        // Write a string to that file.
        writer.WriteLine(json);
        // Close the file when finished.
        writer.Close();


    }

    public void LoadGame()
    {
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/myjsonsave.save", true);
        // Read string from file.
        string readString = reader.ReadToEnd();
        // Close file
        reader.Close();

        // Transfer data to UI
        TestJSONSaver save = JsonUtility.FromJson<TestJSONSaver>(readString);
        inputFieldAge.text = save.age.ToString();
        inputFieldName.text = save.name;
    }



}

[System.Serializable]
public struct TestJSONSaver
{
    public int age;
    public string name;
}

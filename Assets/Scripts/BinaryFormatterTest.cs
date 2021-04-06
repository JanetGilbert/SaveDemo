using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


public class BinaryFormatterTest : MonoBehaviour
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
        TestSaver save;

        // Set data
        save.age = int.Parse(inputFieldAge.text);
        save.name = inputFieldName.text;


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mysave.save");
        bf.Serialize(file, save);

        file.Close();

    }

    public void LoadGame()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/mysave.save", FileMode.Open);

        TestSaver save = (TestSaver)bf.Deserialize(file);

        file.Close();

        // Transfer data to UI
        inputFieldAge.text = save.age.ToString();
        inputFieldName.text = save.name;
    }



}

[System.Serializable]
public struct TestSaver
{
    public int age;
    public string name;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveDemo : MonoBehaviour
{
    //public string myName = "Janet";
    //public int myAge = 42;
    ThingToSave savedThing = new ThingToSave();

    void Start()
    {
        


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + "mysave.sav");
        // bf.Serialize(file, myName);
        // bf.Serialize(file, myAge);
        bf.Serialize(file, savedThing);
        file.Close();
    }


    void Update()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/" + "mysave.sav"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/" + "mysave.sav", FileMode.Open);

            ThingToSave loadedThing;
            // string loadedName = (String)bf.Deserialize(file);
            // int loadedAge = (int)bf.Deserialize(file);

            loadedThing= (ThingToSave)bf.Deserialize(file);

            Debug.Log("name:" + loadedThing.myName + " age:" + loadedThing.myAge);

            file.Close();

        }


    }
}

[System.Serializable]
public class ThingToSave
{
    public string myName = "Janet";
    public int myAge = 42;

   

}
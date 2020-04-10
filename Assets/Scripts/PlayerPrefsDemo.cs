using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayPrefsSmall x = new PlayPrefsSmall();

        x.Set();
        x.Get();
    }

    // Update is called once per frame
    void Update()
    {

    }
}


public class PlayPrefsSmall
{
    public void Set()
    {
        PlayerPrefs.SetString("Puppy", "Patches");

        PlayerPrefs.SetInt("Puppy", 5);

        PlayerPrefs.SetFloat("SoundVolume", 0.5f);
    }

    public void Get()
    {
        if (PlayerPrefs.HasKey("Puppy"))
        {
            Debug.Log(PlayerPrefs.GetString("Puppy"));
            Debug.Log(PlayerPrefs.GetInt("Puppy"));
        }
    }
}
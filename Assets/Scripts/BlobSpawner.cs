using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BlobSpawner : MonoBehaviour
{
    // Settable in editor
    [SerializeField]
    [Header("Set the blob prefab to spawn")]
    private Blob prefabToSpawn = null;

    [SerializeField]
    [Header("Seconds between spawns")]
    private float spawnTimeMax = 1.0f;

    [SerializeField]
    [Header("The color of the target blob")]
    private Color activeColor = Color.red;

    [SerializeField]
    [Header("The color of the non-target blobs")]
    private Color inActiveColor = Color.white;

    [SerializeField]
    [Header("Space at the edge of the screen where blobs are not spawned")]
    public float borderWidth = 0.1f;

    [SerializeField]
    [Header("How often the target switches between blobs")]
    private float switchTimeMax = 0.1f;

    // Timers
    private float spawnTimer;  // Countdown between spawns. //s 
    private float switchTimer; // Countdown between target blob switch. //s

    // Blobs
    private List<Blob> blobList = new List<Blob>(); // List of all blobs. //s
    private Blob target; // Target Blob //s


    void Start()
    {
        Spawn(); // Add one blob.
        SetTarget(blobList[0]); // Set initial target.
    }

    // Set the target, and set all other blobs to not be targets.
    void SetTarget(Blob newActive)
    {
        if (target != null)
        {
            target.BlobColor = inActiveColor;
        }

        target = newActive;
        target.BlobColor = activeColor;
    }

    // Is this blob the current target?
    public bool IsCurrent(Blob toCheck)
    {
        return toCheck == target;
    }

    
    void Update()
    {
        // Spawn blobs every spawnTimeMax seconds.
        spawnTimer -= Time.deltaTime;

        while (spawnTimer < 0.0f)
        {
            spawnTimer += spawnTimeMax;

            Spawn();
        }


        // Switch target every switchTimeMax seconds.
        switchTimer -= Time.deltaTime;

        if (switchTimer < 0.0f)
        {
            switchTimer = switchTimeMax;

            SetTarget(blobList[Random.Range(0, blobList.Count)]);
        }

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

    // Create blob and add it to the list.
    Blob Spawn()
    {
        float halfBorder = borderWidth / 2.0f;

        Vector2 pos = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(halfBorder, 1.0f - halfBorder),
                                                                   Random.Range(halfBorder, 1.0f - halfBorder)));
        Blob newBlob = Instantiate<Blob>(prefabToSpawn, new Vector3(pos.x, pos.y, 0.0f), Quaternion.identity);

        newBlob.transform.parent = transform;

        Blob target = newBlob.GetComponent<Blob>();
        blobList.Add(target);

        return newBlob;
    }

    // Remove blob from list.
    public void Remove(Blob toRemove)
    {
        blobList.Remove(toRemove);
    }


    /*
         // Timers
    private float spawnTimer;  // Countdown between spawns. //s 
    private float switchTimer; // Countdown between target blob switch. //s

    // Blobs
    private List<Blob> blobList = new List<Blob>(); // List of all blobs. //s
    private Blob target; // Target Blob //s
    */


    [System.Serializable]
    public struct SaveSpawner
    {
        public float spawnTimer;
        public float switchTimer;
        public List<SaveBlob> blobList;
        public int targetIndex;

    }

    public void SaveGame()
    {
        SaveSpawner save = CreateSaveData();


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mysave.save");
        bf.Serialize(file, save);

        file.Close();

    }

    public void LoadGame()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/mysave.save", FileMode.Open);

        SaveSpawner save = (SaveSpawner)bf.Deserialize(file);

        file.Close();

        RestoreSaveData(save);
    }


    public SaveSpawner CreateSaveData()
    {
        SaveSpawner saveSpawner = new SaveSpawner();

        saveSpawner.spawnTimer = spawnTimer;
        saveSpawner.switchTimer = switchTimer;

        saveSpawner.blobList = new List<SaveBlob>();

        for (int i = 0; i < blobList.Count; i++)
        {
            saveSpawner.blobList.Add(blobList[i].CreateSaveBlob());
        }

        saveSpawner.targetIndex = blobList.FindIndex(x => x == target);


        return saveSpawner;
    }

    // Restore and re-initialize game.
    public void RestoreSaveData(SaveSpawner save)
    {
        spawnTimer = save.spawnTimer;
        switchTimer = save.switchTimer;

        foreach (Blob blob in blobList)
        {
            Destroy(blob.gameObject); // Destroy existing blobs.
        }
        blobList = new List<Blob>();

        foreach (SaveBlob saveBlob in save.blobList)
        {
            Blob newBlob = Spawn(); // Respawn blobs.
            newBlob.RestoreSaveData(saveBlob);
        }

        SetTarget(blobList[save.targetIndex]); // Set target via index.
    }
}

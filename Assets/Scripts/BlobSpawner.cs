using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/* The main blob spawner and blob manager class that also serves as the game controller in this simple game.
 * It spawns blobs at regular time periods and manages the loading and saving of the game. */


// The spawner data that must be serialized to save the game. (non-Monobehaviour)
[System.Serializable]
public class BlobSpawnerData
{
    // Timers
    public float spawnTimer;  // Countdown between spawns.
    public float switchTimer; // Countdown between target blob switch.


}

// The main spawner class. (Monobehaviour)
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

    private BlobSpawnerData data = new BlobSpawnerData();

    // Blobs
    public List<Blob> blobList = new List<Blob>(); // List of all blobs.
    public Blob target; // Target Blob

    // List of blob data
    public List<BlobData> blobDataList = new List<BlobData>();


    void Start()
    {
        SetTarget(Spawn()); // Add one blob. Set as initial target.
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
        data.spawnTimer -= Time.deltaTime;

        while (data.spawnTimer < 0.0f)
        {
            data.spawnTimer += spawnTimeMax;

            Spawn();
        }


        // Switch target every switchTimeMax seconds.
        data.switchTimer -= Time.deltaTime;

        if (data.switchTimer < 0.0f)
        {
            data.switchTimer = switchTimeMax;

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

        blobList.Add(newBlob);
        blobDataList.Add(newBlob.Data);

        return newBlob;
    }

    // Recreate blob object from saved data.
    Blob SpawnFromData(BlobData blobData)
    {
        Blob newBlob = Instantiate<Blob>(prefabToSpawn);

        newBlob.RefreshAfterLoad(blobData);
        newBlob.transform.parent = transform;

        blobList.Add(newBlob);

        return newBlob;
    }

    // Remove blob from list.
    public void Remove(Blob toRemove)
    {
        blobList.Remove(toRemove);
    }

    // Serialize the game state
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mysave.save");

        bf.Serialize(file, data); // Save main game data.

        // Save the list of blobs.
        foreach (Blob blob in blobList)
        {
            blob.SetTransformForSave();
        }

        bf.Serialize(file, blobDataList);

        // Store index to target blob (rather than target blob.)
        int targetIndex = blobList.FindIndex(x => x == target); 
        bf.Serialize(file, targetIndex);

        file.Close();
    }


    // Deserialize the game and reinitialize objects.
    public void LoadGame()
    {
        // Destroy existing blobs.
        foreach (Blob blob in blobList)
        {
            Destroy(blob.gameObject); 
        }

        blobList = new List<Blob>();

        // Open file.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/mysave.save", FileMode.Open);
        data = (BlobSpawnerData)bf.Deserialize(file);

        // Load and reconstruct blob list.
        blobDataList = (List<BlobData>)bf.Deserialize(file);

        foreach (BlobData blobData in blobDataList)
        {
            SpawnFromData(blobData);
        }


        // Set target blob.
        int targetIndex = (int)bf.Deserialize(file);
        SetTarget(blobList[targetIndex]);

        file.Close();   
    }


}

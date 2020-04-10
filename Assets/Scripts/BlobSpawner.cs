using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Linq;

// Shortcuts
using Random = UnityEngine.Random;
using BlobKey = System.Collections.Generic.KeyValuePair<System.Guid, Blob>;


/* The main blob spawner and blob manager class that also serves as the game controller in this simple game.
 * It spawns blobs at regular time periods and manages the loading and saving of the game. */


// The spawner data that must be serialized to save the game. (non-Monobehaviour)
[Serializable]
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
    private Dictionary<Guid, Blob> blobDict = new Dictionary<Guid, Blob>(); // List of all blobs.
    private Guid targetGuid; // Id of target Blob

    // List of blob data
    private List<BlobData> blobDataList = new List<BlobData>();


    void Start()
    {
        // Add one blob. Set as initial target.

        Blob newBlob = Spawn();
        targetGuid = newBlob.Data.guid;
        blobDict[targetGuid].BlobColor = activeColor;
    }

    // Set the target, and set all other blobs to not be targets.
    void SetTarget(Blob newActive)
    {
        blobDict[targetGuid].BlobColor = inActiveColor;
        targetGuid = newActive.Data.guid;
        blobDict[targetGuid].BlobColor = activeColor;
    }

    // Is this blob the current target?
    public bool IsCurrent(Blob toCheck)
    {
        return toCheck.Data.guid == targetGuid;
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


        // Switch target randomly every switchTimeMax seconds.
        data.switchTimer -= Time.deltaTime;

        if (data.switchTimer < 0.0f)
        {
            data.switchTimer = switchTimeMax;

            BlobKey randomBlob = blobDict.ElementAt(Random.Range(0, blobDict.Count));
            SetTarget(randomBlob.Value);
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

        blobDict.Add(newBlob.Data.guid, newBlob);
        blobDataList.Add(newBlob.Data);

        return newBlob;
    }

    // Recreate blob object from saved data.
    Blob SpawnFromData(BlobData blobData)
    {
        Blob newBlob = Instantiate<Blob>(prefabToSpawn);

        newBlob.RefreshAfterLoad(blobData);
        newBlob.transform.parent = transform;

        blobDict.Add(newBlob.Data.guid, newBlob);

        return newBlob;
    }

    // Remove blob from list.
    public void Remove(Blob toRemove)
    {
        blobDict.Remove(toRemove.Data.guid);
        blobDataList.Remove(toRemove.Data);
    }

    // Serialize the game state
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mysave.save");

        bf.Serialize(file, data); // Save main game data.

        // Save the list of blobs.
        foreach (BlobKey blobPair in blobDict)
        {
            blobPair.Value.SetTransformForSave();
        }

        bf.Serialize(file, blobDataList);

        bf.Serialize(file, targetGuid);

        file.Close();
    }


    // Deserialize the game and reinitialize objects.
    public void LoadGame()
    {
        // Destroy existing blobs.
        foreach (BlobKey blob in blobDict)
        {
            Destroy(blob.Value.gameObject); 
        }

        blobDict = new Dictionary<Guid, Blob>();

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
        targetGuid = (Guid)bf.Deserialize(file);
        SetTarget(blobDict[targetGuid]);

        file.Close();   
    }


}

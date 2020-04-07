using System.Collections;
using System.Collections.Generic;
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
    private float spawnTimer;  // Countdown between spawns.
    private float switchTimer; // Countdown between target blob switch.

    // Blobs
    private List<Blob> blobList = new List<Blob>(); // List of all blobs.
    private Blob target; // Target Blob


    void Start()
    {
        Spawn(); // Add one blob.
        SetActive(blobList[0]); // Set initial target.
    }

    // Set the target, and set all other blobs to not be targets.
    void SetActive(Blob newActive)
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

            SetActive(blobList[Random.Range(0, blobList.Count)]);
        }
    }

    // Create blob and add it to the list.
    void Spawn()
    {
        float halfBorder = borderWidth / 2.0f;

        Vector2 pos = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(halfBorder, 1.0f - halfBorder),
                                                                   Random.Range(halfBorder, 1.0f - halfBorder)));
        Blob newBlob = Instantiate<Blob>(prefabToSpawn, new Vector3(pos.x, pos.y, 0.0f), Quaternion.identity);

        newBlob.transform.parent = transform;

        Blob target = newBlob.GetComponent<Blob>();
        blobList.Add(target);
    }

    // Remove blob from list.
    public void Remove(Blob toRemove)
    {
        blobList.Remove(toRemove);
    }

}

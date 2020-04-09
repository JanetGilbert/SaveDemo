using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Blobs move back and forth between two points.
 * If blobs are targets, they turn a different color, and shrink when clicked.
 * If they shrink to nothing then they are destroyed.
 * If blobs are clicked when they are not targets, they grow to a maximum size.*/

// The blob data that must be serialized to save the game. (non-Monobehaviour)
[System.Serializable]
public class BlobData
{
    // Scaling data
    public int size;
    public bool scaling = false;
    public float scaleTo = 1.0f;
    public float curScale = 1.0f;

    // Lerping data - store as serializable Vector3, access as normal Vector3
    private SerializableVector3 _start;
    public Vector3 Start
    {
        get
        {
            return _start;
        }
        set
        {
            _start = value;
        }
    }


    private SerializableVector3 _end;
    public Vector3 End
    {
        get
        {
            return _end;
        }
        set
        {
            _end = value;
        }
    }

    public float lerpTime;


    public SerializedTransform serializedTransform; // Set this on saving.
}


// The main blob class. (Monobehaviour)
public class Blob : MonoBehaviour
{
    // Settable in editor
    [SerializeField]
    [Header("Number of size steps until blob destroyed")]
    private int maxSizeStep = 3;

    [SerializeField]
    [Header("Movement lerp speed")]
    private float moveSpeed = 0.3f;

    [SerializeField]
    [Header("How fast the blobs scale up or down")]
    private float scaleSpeed = 1.0f;

    // Cached components
    [System.NonSerialized]
    private BlobSpawner spawner;
    [System.NonSerialized]
    private MeshRenderer meshRenderer;

    // Savable data
    private BlobData data = new BlobData();

    // Property with no backing variable to set material color.
    public Color BlobColor
    {
        set // Setter only, there is no need for a getter.
        {
            meshRenderer.material.color = value;
        }
    }

    /* Put things in Awake that don't depend on other objects.
     This is important if you are instantiating the object containing a component, so that the object is initialized
     and can be used straight away. */
    void Awake()
    {
        // Cache useful components.
        meshRenderer = GetComponent<MeshRenderer>();


        // Set movement pattern.
        data.Start = transform.position;
        data.End = data.Start + (Random.rotation * Vector3.forward) * Random.Range(1.0f, 5.0f);

        data.size = maxSizeStep;
    }

    void Start()
    {
        spawner = GetComponentInParent<BlobSpawner>(); // Depends on other object, so set in Start()

    }

    void Update()
    {
        // Scale down after target blob is clicked, up after non-target blob clicked.
        if (data.scaling)
        {
            if (data.curScale < data.scaleTo)
            {
                data.curScale += scaleSpeed * Time.deltaTime;

                if (data.curScale >= data.scaleTo)
                {
                    data.scaling = false;
                }
            }
            else if (data.curScale > data.scaleTo)
            {
                data.curScale -= scaleSpeed * Time.deltaTime;

                if (data.curScale <= data.scaleTo)
                {
                    data.scaling = false;
                }
            }

            transform.localScale = new Vector3(data.curScale, data.curScale, 1.0f);
        }

        // Lerp between two points.
        Vector3 newPos;

        if (data.lerpTime < 1.0f)
        {
            newPos = Vector3.Lerp(data.Start, data.End, data.lerpTime);
        }
        else
        {
            newPos = Vector3.Lerp(data.End, data.Start, data.lerpTime - 1.0f);
        }

        data.lerpTime += Time.deltaTime * moveSpeed;

        if (data.lerpTime > 2.0f)
        {
            data.lerpTime = 0.0f;
        }

        transform.position = newPos;
    }

    
    void OnMouseDown()
    {
        if (spawner.IsCurrent(this)) // If blob is target, decrease the size
        {
            data.size--;

            if (data.size <= 0)
            {
                data.size = 0;

                // Destroy blob when it shrinks to nothing.
                spawner.Remove(this);
                Destroy(this.gameObject); 
            }
        }
        else // If blob is not target, increase the size
        {
            data.size++;

            if (data.size > maxSizeStep)
            {
                data.size = maxSizeStep;
            }
        }

        // Set scale target.
        float scaled = 1.0f / maxSizeStep * data.size;
        data.scaleTo = scaled;
        data.scaling = true;
    }

    // Get the data needed for saving the game.
    public BlobData GetData()
    {
        // Since the transform may be set from anywhere, convert it to a storable format at the last moment before saving.
        // You could also do it every frame, but that's slightly inefficient.
        data.serializedTransform = new SerializedTransform(transform); 

        return data;
    }

    // Refresh blob after loading stored data.
    public void RefreshAfterLoad(BlobData newData)
    {
        data = newData;

        data.serializedTransform.LoadTransform(transform);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveBlob
{
    // Scaling data
    public int size;
    public bool scaling;
    public float scaleTo;
    public float curScale;

    // Lerping data
    public SerializableVector3 start;
    public SerializableVector3 end;
    public float lerpTime;

    // Transform
    public SerializableVector3 position;
    public SerializableVector3 scale;
    public SerializableQuaternion rotation;
}



/* Blobs move back and forth between two points.
 * If blobs are targets, they turn a different color, and shrink when clicked.
 * If they shrink to nothing then they are destroyed.
 * If blobs are clicked when they are not targets, they grow to a maximum size.*/
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
    private BlobSpawner spawner;
    private MeshRenderer meshRenderer;

    // Scaling data
    private int size; //s
    private bool scaling = false; //s
    private float scaleTo = 1.0f; //s
    private float curScale = 1.0f; //s

    // Lerping data
    private Vector3 start; //s
    private Vector3 end; //s
    private float lerpTime; //s

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
        start = transform.position;
        end = start + (Random.rotation * Vector3.forward) * Random.Range(1.0f, 5.0f);

        size = maxSizeStep;
    }

    void Start()
    {
        spawner = GetComponentInParent<BlobSpawner>(); // Depends on other object, so set in Start()

    }

    void Update()
    {
        // Scale down after target blob is clicked, up after non-target blob clicked.
        if (scaling)
        {
            if (curScale < scaleTo)
            {
                curScale += scaleSpeed * Time.deltaTime;

                if (curScale >= scaleTo)
                {
                    scaling = false;
                }
            }
            else if (curScale > scaleTo)
            {
                curScale -= scaleSpeed * Time.deltaTime;

                if (curScale <= scaleTo)
                {
                    scaling = false;
                }
            }

            transform.localScale = new Vector3(curScale, curScale, 1.0f);
        }

        // Lerp between two points.
        Vector3 newPos;

        if (lerpTime < 1.0f)
        {
            newPos = Vector3.Lerp(start, end, lerpTime);
        }
        else
        {
            newPos = Vector3.Lerp(end, start, lerpTime - 1.0f);
        }

        lerpTime += Time.deltaTime * moveSpeed;

        if (lerpTime > 2.0f)
        {
            lerpTime = 0.0f;
        }

        transform.position = newPos;
    }

    
    void OnMouseDown()
    {
        if (spawner.IsCurrent(this)) // If blob is target, decrease the size
        {
            size--;

            if (size <= 0)
            {
                size = 0;

                // Destroy blob when it shrinks to nothing.
                spawner.Remove(this);
                Destroy(this.gameObject); 
            }
        }
        else // If blob is not target, increase the size
        {
            size++;

            if (size > maxSizeStep)
            {
                size = maxSizeStep;
            }
        }

        // Set scale target.
        float scaled = 1.0f / maxSizeStep * size;
        scaleTo = scaled;
        scaling = true;
    }


    /*private int size; //s
    private bool scaling = false; //s
    private float scaleTo = 1.0f; //s
    private float curScale = 1.0f; //s

    // Lerping data
    private Vector3 start; //s
    private Vector3 end; //s
    private float lerpTime; //s

    */

   
    public SaveBlob CreateSaveBlob()
    {
        SaveBlob saveBlob = new SaveBlob();

        saveBlob.size = size;
        saveBlob.scaling = scaling;
        saveBlob.scaleTo = scaleTo;
        saveBlob.curScale = curScale;

        saveBlob.start = start;
        saveBlob.end = end;
        saveBlob.lerpTime = lerpTime;

        saveBlob.position = transform.position;
        saveBlob.scale = transform.localScale;
        saveBlob.rotation = transform.localRotation;


        return saveBlob;
    }


    // Restore and re-initialize blob.
    public void RestoreSaveData(SaveBlob saveBlob)
    {
        size = saveBlob.size;
        scaling = saveBlob.scaling;
        scaleTo = saveBlob.scaleTo;
        curScale = saveBlob.curScale;

        start = saveBlob.start;
        end = saveBlob.end;
        lerpTime = saveBlob.lerpTime;

        transform.position = saveBlob.position;
        transform.localScale = saveBlob.scale;
        transform.localRotation = saveBlob.rotation;
    }

}

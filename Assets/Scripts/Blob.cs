﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int size;
    private bool scaling = false;
    private float scaleTo = 1.0f;
    private float curScale = 1.0f;
    
    // Lerping data
    private Vector3 start;
    private Vector3 end;
    private float lerpTime;

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


}

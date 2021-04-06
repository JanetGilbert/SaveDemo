using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void Zoom(this Rigidbody v)
    {
        v.velocity *= 2.0f;
    }
}



public class CubeTest : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.Zoom();
    }
}

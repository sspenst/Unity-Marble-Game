using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 180.0f * Time.deltaTime, Space.World);
    }
}

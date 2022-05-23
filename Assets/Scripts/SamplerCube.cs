using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplerCube : MonoBehaviour
{
    public void SetSize(float ySize)
    {
        transform.localScale = Vector3.right * transform.localScale.x + Vector3.up * ySize + Vector3.forward * transform.localScale.z;
    }
}

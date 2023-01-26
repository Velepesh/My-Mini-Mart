using System.Collections;
using UnityEngine;

public class ModelRotation : MonoBehaviour
{
    public void Rotate(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player _target;
    [SerializeField] private Vector3 _offsetPosition;
    [SerializeField] private Vector3 _offsetRotation;

    private void LateUpdate()
    {
        Vector3 target = _target.transform.position;

        transform.position = target + _offsetPosition;
        transform.LookAt(target + _offsetRotation);
    }
}
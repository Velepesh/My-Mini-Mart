using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private bool _isPlaced;
    private Vector3 _targetPosition;
    private Vector3 _targetLocalPosition;

    public bool IsPlaced => _isPlaced;
    public Vector3 TargetPosition => _targetPosition;
    public Vector3 TargetLocalPosition => _targetLocalPosition;

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public void SetTargetLocalPosition(Vector3 targetLocalPosition)
    {
        _targetLocalPosition = targetLocalPosition;
    }

    public void Place() => _isPlaced = true;
    public void Take() => _isPlaced = false;
}
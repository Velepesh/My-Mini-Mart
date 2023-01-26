using UnityEngine;
using UnityEngine.Events;

public class TargetPoint<T> : MonoBehaviour
{
    private T _model;

    public T Model => _model;
    public bool IsEmpty => _model == null;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    public event UnityAction Cleared;

    public void Add(T model)
    {
        _model = model;
    }

    public void Clear()
    {
        _model = default(T);
        Cleared?.Invoke();
    }
}
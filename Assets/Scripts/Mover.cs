using UnityEngine;
using UnityEngine.Events;

public abstract class Mover : MonoBehaviour
{
    public abstract event UnityAction<bool> Moved;
    public abstract event UnityAction<bool> Stoped;

    protected abstract void Move();
}
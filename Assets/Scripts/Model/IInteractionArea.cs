using UnityEngine;
using UnityEngine.Events;

public interface IInteractionArea
{
    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);

    event UnityAction Entered;
    event UnityAction Exited;
}
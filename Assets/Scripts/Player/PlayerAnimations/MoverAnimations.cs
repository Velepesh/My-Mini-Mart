using UnityEngine;

[RequireComponent(typeof(Mover))]
public class MoverAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private Mover _mover;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    private void OnEnable()
    {
        _mover.Moved += OnMoved;
        _mover.Stoped += OnStoped;
    }

    private void OnDisable()
    {
        _mover.Moved -= OnMoved;
        _mover.Stoped -= OnStoped;
    }

    private void OnMoved(bool isEmptyHand)
    {
        SetAnimationStates(isEmptyHand, true);
    }

    private void OnStoped(bool isEmptyHand)
    {
        SetAnimationStates(isEmptyHand, false);
    }

    private void SetAnimationStates(bool isEmptyHand, bool IsMove)
    {
        _animator.SetBool(AnimatorMoverController.States.IsEmpty, isEmptyHand);

        if (isEmptyHand)
            _animator.SetBool(AnimatorMoverController.States.IsMove, IsMove);
        else
            _animator.SetBool(AnimatorMoverController.States.IsCarryMove, IsMove);
    }
}
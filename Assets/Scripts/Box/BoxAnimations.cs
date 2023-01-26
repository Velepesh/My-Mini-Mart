using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Box))]
[RequireComponent(typeof(Animator))]
public class BoxAnimations : MonoBehaviour
{
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private Vector3 _targetClientRotation;
    [SerializeField] private float _durationScalingAmimation;

    private Box _box;
    private Animator _animator;

    private void OnValidate()
    {
        _durationScalingAmimation = Mathf.Clamp(_durationScalingAmimation, 0f, float.MaxValue);
    }

    private void Awake()
    {
        _box = GetComponent<Box>();
        _animator = GetComponent<Animator>();

        Scale();
    }

    private void OnEnable()
    {
        _box.GoodsPacked += OnGoodsPacked;
        _box.Moved += OnMoved;
    }

    private void OnDisable()
    {
        _box.GoodsPacked -= OnGoodsPacked;
        _box.Moved -= OnMoved;
    }

    private void OnGoodsPacked()
    {
        _animator.SetTrigger(AnimatorBoxController.States.Close);
    }

    private void Scale()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(_targetScale, _durationScalingAmimation).OnComplete(() => _box.CompleteSpawn());
    }

    private void OnMoved(float duration)
    {
        transform.DORotate(_targetClientRotation, duration);
    }
}
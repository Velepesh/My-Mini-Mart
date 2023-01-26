using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(IInteractionArea))]
public class InteractionAreaAnimation : MonoBehaviour
{
    [SerializeField] private Transform _scaliningObject;
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private float _duration;

    private IInteractionArea _area;
    private Vector3 _starttedScale;

    private void OnValidate()
    {
        _duration = Mathf.Clamp(_duration, 0f, float.MaxValue);
    }

    private void Awake()
    {
        _area = GetComponent<IInteractionArea>();
        _starttedScale = _scaliningObject.localScale;
    }

    private void OnEnable()
    {
        _area.Entered += OnEntered;
        _area.Exited += OnExited;
    }

    private void OnDisable()
    {
        _area.Entered -= OnEntered;
        _area.Exited -= OnExited;
    }

    private void OnEntered()
    {
        _scaliningObject.DOScale(_targetScale, _duration);
    }

    private void OnExited()
    {
        _scaliningObject.DOScale(_starttedScale, _duration);
    }
}
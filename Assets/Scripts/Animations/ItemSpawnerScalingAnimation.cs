using System.Linq;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(GoodsGenerator))]
public class ItemSpawnerScalingAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private float _scalingSpeed;

    private GoodsGenerator _goodsGenerator;
    private Tween _tween;
    private Vector3 _startScale;
    private bool _isStoped;

    private void OnValidate()
    {
        _scalingSpeed = Mathf.Clamp(_scalingSpeed, 0f, float.MaxValue);
    }

    private void Awake()
    {
        _goodsGenerator = GetComponent<GoodsGenerator>();
        _startScale = transform.localScale;
        StopAnimation();
        OnTaken();
    }

    private void OnEnable()
    {
        _goodsGenerator.Filled += OnFilled;
        _goodsGenerator.Taken += OnTaken;
    }

    private void OnDisable()
    {
        _goodsGenerator.Filled -= OnFilled;
        _goodsGenerator.Taken -= OnTaken;
    }

    private void OnFilled()
    {
        _tween.Kill(true);
        StopAnimation();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(_targetScale, _scalingSpeed));
        sequence.Append(transform.DOScale(_startScale, _scalingSpeed));
    }

    private void OnTaken()
    {
        if (_isStoped)
        {
            StartAnimation();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(_targetScale, _scalingSpeed));
            sequence.Append(transform.DOScale(_startScale, _scalingSpeed));
            sequence.SetEase(Ease.Linear).SetLoops(-1);
            _tween = sequence;
        }
    }

    private void StartAnimation() => _isStoped = false;

    private void StopAnimation() => _isStoped = true;
}
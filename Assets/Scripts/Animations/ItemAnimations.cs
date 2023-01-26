using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Goods))]
public class ItemAnimations : MonoBehaviour
{
    [SerializeField] private Vector3 _targetShelfRotation;
    [SerializeField] private Vector3 _targetInventoryScale;

    private Goods _item;
    private Vector3 _startScale;
    
    private void Awake()
    {
        _item = GetComponent<Goods>();
        _startScale = transform.localScale;
    }

    private void OnEnable()
    {
        _item.AddedToInventory += OnAddedToInventory;
        _item.AddedToShelf += OnAddedToShelf;
    }

    private void OnDisable()
    {
        _item.AddedToInventory -= OnAddedToInventory;
        _item.AddedToShelf -= OnAddedToShelf;
    }

    private void OnAddedToInventory(float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(_targetInventoryScale, duration / 2f));
        sequence.Insert(0, transform.DORotate(Vector3.zero, duration));
        sequence.Append(transform.DOScale(_startScale, duration / 2f));
    }

    private void OnAddedToShelf(float duration)
    {
        transform.DORotate(_targetShelfRotation, duration);
    }
}
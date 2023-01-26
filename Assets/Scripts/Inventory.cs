using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : GoodsHolder
{
    [SerializeField] private int _capacity;
    [SerializeField] private Transform _itemsHolder;
    [SerializeField] private Transform _boxPoint;
    [SerializeField] private float _step;

    private List<Goods> _items = new List<Goods>();
    private Vector3 _spawnPosition;
    private Box _box;

    public bool IsEmpty => _items.Count == 0 && _box == null;
    public int ItemsCount => _items.Count;
    public Vector3 SpawnPosition => _spawnPosition;

    public event UnityAction<Vector3> Filled;
    public event UnityAction ItemTaken;

    private void OnValidate()
    {
        _capacity = Mathf.Clamp(_capacity, 0, int.MaxValue);
        _step = Mathf.Clamp(_step, 0, float.MaxValue);
    }

    private void Start()
    {
        _spawnPosition = _itemsHolder.localPosition;
    }

    public Goods GetGoods()
    {
        if (_items.Count > 0)
        {
            Goods item = _items[_items.Count - 1];
            _spawnPosition.y -= _step;
            _items.Remove(item);
            ItemTaken?.Invoke();

            return item;
        }

        return null;
    }

    public void SetBox(Box box)
    {
        box.transform.SetParent(_boxPoint);
        _box = box;
        _box.MoveBox(_boxPoint.position);
    }

    public override bool TryAddGoods(Goods goods)
    {
        if (goods == null)
            throw new ArgumentException(nameof(goods));

        if (_items.Count < _capacity)
        {
            _items.Add(goods);
            SetGoodsPosition(goods);

            if (IsFull())
                Filled?.Invoke(_spawnPosition);

            return true;
        }

        return false;
    }

    public void ResetInventory()
    {
        if(_box != null)
            Destroy(_box.gameObject);

        _items.Clear();
    }

    protected override bool IsFull()
    {
        return _capacity == _items.Count;
    }

    protected override void SetGoodsPosition(Goods goods)
    {
        goods.transform.SetParent(_itemsHolder);
        Move(goods, _spawnPosition);
        _spawnPosition.y += _step;
    }

    protected override void Move(Goods goods, Vector3 targetPosition)
    {
        Mover.MoveInLocalPosition(goods, targetPosition);
        goods.AddToInventory(Mover.Duration);
    }
}
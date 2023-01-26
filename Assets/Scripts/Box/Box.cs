using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Box : GoodsHolder
{
    [SerializeField] private ItemHolderOptions _options;
    [SerializeField] private Transform _goodsHolder;
    [SerializeField] private float _moveToClientDuration;
    
    private List<Goods> _goods = new List<Goods>();

    public bool IsBoxSpawn { get; private set; }
    public bool IsBoxClose { get; private set; }

    public event UnityAction GoodsPacked;
    public event UnityAction<float> Moved;

    private void OnValidate()
    {
        _moveToClientDuration = Mathf.Clamp(_moveToClientDuration, 0f, float.MaxValue);
    }

    public void Pack() =>  GoodsPacked?.Invoke();

    public void Close() => IsBoxClose = true;

    public void CompleteSpawn() => IsBoxSpawn = true;

    public void MoveBox(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, _moveToClientDuration);
        Moved?.Invoke(Mover.Duration);
    }

    public override bool TryAddGoods(Goods goods)
    {
        SetGoodsPosition(goods);
        _goods.Add(goods);
        return true;
    }

    protected override void SetGoodsPosition(Goods goods)
    {
        goods.transform.SetParent(_goodsHolder);

        Vector3 position = _options.StartItemPoint.localPosition;

        if (IsFull() == false)
        {
            if (_goods.Count != 0)
            {
                int layerNumber = _goods.Count / (_options.RowNumber * _options.ColumnNumber);

                if (layerNumber > _options.LayersNumber)
                    layerNumber = 0;

                int numberInLayer = _options.ColumnNumber * _options.RowNumber;
                position = _goods[_goods.Count - 1].TargetLocalPosition;

                if (_goods.Count == numberInLayer * layerNumber && _goods.Count * layerNumber % numberInLayer == 0)
                {
                    position = _options.StartItemPoint.localPosition;
                    position.y += (_options.LayerStepY * layerNumber);
                }
                else if (_goods.Count % _options.RowNumber == 0)
                {
                    position += _options.ColumnStep;
                    position.x = _options.StartItemPoint.localPosition.x;
                }
                else
                {
                    position.x += _options.RowStepX;
                }
            }
        }

        goods.SetTargetLocalPosition(position);
        Move(goods, position);
    }


    protected override void Move(Goods goods, Vector3 targetPosition)
    {
        Mover.MoveInLocalPosition(goods, targetPosition);
    }

    protected override bool IsFull()
    {
        int number = _options.RowNumber * _options.ColumnNumber * _options.LayersNumber;

        if (number <= _goods.Count)
            return true;

        return false;
    }
}
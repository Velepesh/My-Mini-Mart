using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Shelf : GoodsHolder, IInteractionArea, ITarget
{
    [SerializeField] private ItemHolderOptions _options;
    [SerializeField] private Transform _holder;
    [SerializeField] private List<ClientTargetPoint> _clientWaitPoints;
    [SerializeField] private Goods _prefab;
    [SerializeField] private Sprite _icon;

    private Stack<Goods> _goods = new Stack<Goods>();
    private Coroutine _fillShelfJob;

    public Sprite Icon => _icon;
    public IReadOnlyList<ClientTargetPoint> ClientTargetPoints => _clientWaitPoints;
    public bool IsEmpty => _goods.Count == 0;
    public int GoodsCount => _goods.Count;
    public Goods Prefab => _prefab;

    public event UnityAction Entered;
    public event UnityAction Exited;

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _fillShelfJob = StartCoroutine(FillShelf(player.Inventory));
            Entered?.Invoke();
        }

        if (other.TryGetComponent(out Client client))
        {
            if (Equals(client.ÑurrentTarget))
            {
                int neddedNumber = 0;

                if (client.CheckGoodsInList(_prefab, ref neddedNumber))
                    StartCoroutine(SetClientInventory(client, neddedNumber));
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (_fillShelfJob != null)
                StopCoroutine(_fillShelfJob);

            Exited?.Invoke();
        }
    }

    public override bool TryAddGoods(Goods goods)
    {
        if (goods == null)
            throw new ArgumentException(nameof(goods));

        SetGoodsPosition(goods);
        _goods.Push(goods);
        return true;
    }

    protected override bool IsFull()
    {
        return _goods.Count >= _options.ColumnNumber * _options.RowNumber;
    }

    protected override void SetGoodsPosition(Goods goods)
    {
        if (goods == null)
            throw new ArgumentException(nameof(goods));

        if (_goods.Count >= _options.ColumnNumber * _options.RowNumber)
            return;

        goods.transform.SetParent(_holder);

        Vector3 itemPosition = _options.StartItemPoint.localPosition;

        if (_goods.Count != 0)
        {
            itemPosition = _goods.Peek().TargetLocalPosition;

            if (_goods.Count % _options.RowNumber == 0)
            {
                itemPosition += _options.ColumnStep;
                itemPosition.x = _options.StartItemPoint.localPosition.x;
            }
            else
            {
                itemPosition.x += _options.RowStepX;
            }
        }

        goods.SetTargetLocalPosition(itemPosition);
        Move(goods, itemPosition);
    }

    protected override void Move(Goods goods, Vector3 targetPosition)
    {
        Mover.MoveInLocalPosition(goods, targetPosition);
        goods.AddToShelf(Mover.Duration);
    }

    private IEnumerator FillShelf(Inventory inventory)
    {
        while (true)
        {
            if (IsFull())
                break;

            Goods item = inventory.GetGoods();

            if (item != null)
            {
                TryAddGoods(item);
                yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
            }

            yield return null;
        }
    }

    private IEnumerator SetClientInventory(Client client, int neddedNumber)
    {
        int counter = 0;
      
        Goods goods = null;

        while (counter < neddedNumber)
        {
            if (client.IsReachedTarget)
            {
                if (goods == null)
                {
                    if (_goods.Count > 0)
                        goods = _goods.Pop();
                }
                else if (goods.IsPlaced)
                {
                    if (client.Inventory.TryAddGoods(goods))
                    {
                        counter++;

                        if (counter >= neddedNumber)
                        {
                            client.SetNextTarget();
                            break;
                        }
                        else
                        {
                            client.UpdateGoodsList(counter);

                            goods = null;
                            yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
                        }
                    }
                }             
            }

            yield return null;
        }
    }
}
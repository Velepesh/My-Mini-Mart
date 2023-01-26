using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Client : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private int _maxNumberOfOneGoods;
    [SerializeField] private MoneyGenerator _moneyGenerator;

    private Queue<ITarget> _targets = new Queue<ITarget>();
    private Dictionary<Goods, int> _neddedGoods = new Dictionary<Goods, int>();

    public bool IsEmptyHand => _inventory.IsEmpty;
    public ITarget СurrentTarget { get; private set; }
    public bool IsReachedTarget { get; private set; }
    public Inventory Inventory => _inventory;

    public event UnityAction<ITarget> TargetInited;
    public event UnityAction<Sprite, int, int> NeededGoodsChanged;
    public event UnityAction<ITarget> GoodsTaken;
    public event UnityAction<Client> ClientLeft;

    private void OnValidate()
    {
        _maxNumberOfOneGoods = Mathf.Clamp(_maxNumberOfOneGoods, 1, int.MaxValue);
    }

    public void Init(Queue<ITarget> targets)
    {
        if (targets == null)
            throw new InvalidOperationException(nameof(targets));

        _targets = targets;       
        List<Shelf> shelves = GetShelves(targets);

        GenerateGoodsList(shelves);
        SetNextTarget();
    }

    public void ReachedTarget()
    {
        IsReachedTarget = true;

        if (СurrentTarget is ClientSpawner)
        {
            ResetClient();
            ClientLeft?.Invoke(this);
        }
    }

    public void SetNextTarget()
    {
        СurrentTarget = _targets.Dequeue();
        TargetInited?.Invoke(СurrentTarget);
        UpdateGoodsList(0);
    }

    public void UpdateCashierTargetPoint()
    {
        if(СurrentTarget is Cashier cashier)
        {
            IsReachedTarget = false;
            TargetInited?.Invoke(СurrentTarget);
        }
    }

    public void UpdateGoodsList(int counter)
    {
        if (СurrentTarget is Shelf shelf)
            NeededGoodsChanged?.Invoke(shelf.Icon, _neddedGoods[shelf.Prefab], counter);
        else
            GoodsTaken?.Invoke(СurrentTarget);
    }

    public bool CheckGoodsInList(Goods prefab, ref int neededNumber)
    {
        foreach (var item in _neddedGoods)
        {
            Goods goods = item.Key;
            if (prefab.Equals(goods))
            {
                neededNumber = item.Value;
                return true;
            }
        }

        return false;
    }

    public Money RemoveMoney(Transform holder, Vector3 targetPosition, bool isDisableOnTarget = true)
    {
        return _moneyGenerator.MoveToTarget(holder, targetPosition, isDisableOnTarget).GetComponent<Money>();
    }

    private List<Shelf> GetShelves(Queue<ITarget> targets)
    {
        List<Shelf> shelves = new List<Shelf>();

        foreach (ITarget target in targets)
        {
            if (target is Shelf shelf)
                shelves.Add(shelf);
        }

        if (shelves.Count == 0)
            throw new ArgumentNullException(nameof(shelves));

        return shelves;
    }

    private void GenerateGoodsList(List<Shelf> shelves)
    {
        if(shelves.Count == 0)
            throw new ArgumentNullException(nameof(shelves));

        _neddedGoods = new Dictionary<Goods, int>();

        for (int i = 0; i < shelves.Count; i++)
        {
            int count = UnityEngine.Random.Range(1, _maxNumberOfOneGoods + 1);
            _neddedGoods.Add(shelves[i].Prefab, count);
        }
    }

    private void ResetClient()
    {
        Inventory.ResetInventory();
    }
}
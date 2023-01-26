using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Cashier : GoodsHolder, IInteractionArea, ITarget
{
    [SerializeField] private ItemHolderOptions _moneyPositionOptions;
    [SerializeField] private Sprite _icon;
    [SerializeField] private ClientTargetPoint _paymentPoint;
    [SerializeField] private List<ClientTargetPoint> _clientWaitPoints;
    [SerializeField] private Box _boxPrefab;
    [SerializeField] private Transform _boxPoint;

    private Coroutine _serveJob;
    private bool _isCashier;
    private Stack<Money> _monies = new Stack<Money>();

    public Sprite Icon => _icon;
    public IReadOnlyList<ClientTargetPoint> ClientTargetPoints => _clientWaitPoints;

    public event UnityAction Entered;
    public event UnityAction Exited;
    public event UnityAction<Client> ClientPaid;

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _isCashier = true;
            _serveJob = StartCoroutine(Serve(player));
            Entered?.Invoke();
        }

        if (other.TryGetComponent(out Client client))
        {
            if (Equals(client.ÑurrentTarget) && _paymentPoint.Model == client)
                StartCoroutine(FormBox(client));
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _isCashier = false;
            StopCoroutine(_serveJob);
            Exited?.Invoke();
        }
    }

    public override bool TryAddGoods(Goods goods)
    {
        if (goods == null)
            throw new ArgumentException(nameof(goods));

        SetGoodsPosition(goods);

        return true;
    }

    protected override bool IsFull()
    {
        int number = _moneyPositionOptions.RowNumber * _moneyPositionOptions.ColumnNumber * _moneyPositionOptions.LayersNumber;

        if(number <= _monies.Count)
            return true;

        return false;
    }

    protected override void SetGoodsPosition(Goods goods)
    {
        goods.transform.SetParent(_boxPoint);
        Move(goods, _boxPoint.position);
    }

    protected override void Move(Goods goods, Vector3 targetPosition)
    {
        Mover.MoveInGlobalPosition(goods, targetPosition);
    }

    private IEnumerator Serve(Player player)
    {
        while (true)
        {
            if (_monies.Count > 0)
            {
                player.AddMoney(_monies.Pop());
                yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
            }

            yield return null;
        }
    }

    private IEnumerator FormBox(Client client)
    {
        Inventory inventory = client.Inventory;
        Box box = InstantiateBox();
        int price = 0;

        while (inventory.ItemsCount > 0)
        {
            if (_isCashier && client.IsReachedTarget)
            {
                if(box == null)
                    box = InstantiateBox();

                if (box.IsBoxSpawn)
                {
                    Goods goods = inventory.GetGoods();
                    box.TryAddGoods(goods);
                    price += goods.Price;

                    yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
                }
            }

            yield return null;
        }

        StartCoroutine(PackingBox(box, client, price));
    }

    private Box InstantiateBox()
    {
        return Instantiate(_boxPrefab.gameObject, _boxPoint.position, Quaternion.identity).GetComponent<Box>();
    }

    private IEnumerator PackingBox(Box box, Client client, int price)
    {
        box.Pack();

        while (box.IsBoxClose == false)
            yield return null;

        client.Inventory.SetBox(box);
        Pay(client, price);
    }

    private void Pay(Client client, int price)
    {
        StartCoroutine(MoveMoney(price, client));
    }

    private IEnumerator MoveMoney(int price, Client client)
    {
        for (int i = 0; i < price; i++)
        {
            Vector3 targetPosition = GetMoneyPosition();
            Money money = client.RemoveMoney(_moneyPositionOptions.StartItemPoint, targetPosition, false);
            money.SetTargetPosition(targetPosition);
            _monies.Push(money);

            yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
        }

        ClientPaid?.Invoke(client);

        UpdateClientsPoints();
    }

    private void UpdateClientsPoints()
    {
        for (int i = 1; i < _clientWaitPoints.Count; i++)
        {
            if (_clientWaitPoints[i].Model is Client model)
                model.UpdateCashierTargetPoint();
        }
    }

    private Vector3 GetMoneyPosition()
    {
        if (IsFull())
            return _moneyPositionOptions.StartItemPoint.position;

        Vector3 moneyPosition = _moneyPositionOptions.StartItemPoint.position;

        if (_monies.Count != 0)
        {
            int layerNumber = _monies.Count / (_moneyPositionOptions.RowNumber * _moneyPositionOptions.ColumnNumber);

            if (layerNumber > _moneyPositionOptions.LayersNumber)
                layerNumber = 0;

            int numberInLayer = _moneyPositionOptions.ColumnNumber * _moneyPositionOptions.RowNumber;
            moneyPosition = _monies.Peek().TargetPosition;

            if (_monies.Count == numberInLayer * layerNumber && _monies.Count * layerNumber % numberInLayer == 0)
            {
                moneyPosition = _moneyPositionOptions.StartItemPoint.position;
                moneyPosition.y += (_moneyPositionOptions.LayerStepY * layerNumber);
            }
            else if (_monies.Count % _moneyPositionOptions.RowNumber == 0)
            {
                moneyPosition += _moneyPositionOptions.ColumnStep;
                moneyPosition.x = _moneyPositionOptions.StartItemPoint.position.x;
            }
            else
            {
                moneyPosition.x += _moneyPositionOptions.RowStepX;
            }
        }

        return moneyPosition;
    }
}
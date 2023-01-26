using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class GoodsGenerator : GoodsHolder, IInteractionArea
{
    [SerializeField] private Goods _prefab;
    [SerializeField] private float _timeBetweenSpawn;
    [SerializeField] private List<GoodsTargetPoint> _spawnPoints;
    [SerializeField] private Transform _startSpawnPoint;

    private float _expiredTime = 0f;
    private Coroutine _fillInventoryJob;
    private GoodsTargetPoint _currentPoint;

    public event UnityAction Filled;
    public event UnityAction Taken;
    public event UnityAction Entered;
    public event UnityAction Exited;

    private void OnValidate()
    {
        _timeBetweenSpawn = Mathf.Clamp(_timeBetweenSpawn, 0f, float.MaxValue);
    }

    private void OnEnable()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
            _spawnPoints[i].Cleared += OnCleared;
    }

    private void OnDisable()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
            _spawnPoints[i].Cleared -= OnCleared;
    }

    private void Update()
    {
        if (IsFull())
            return;
       
        if (_expiredTime >= _timeBetweenSpawn)
            TryAddGoods(_prefab);

        _expiredTime += Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _fillInventoryJob = StartCoroutine(FillInventory(player.Inventory));
            Entered?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StopCoroutine(_fillInventoryJob);
            Exited?.Invoke();
        }
    }

    public override bool TryAddGoods(Goods prefab)
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            _currentPoint = _spawnPoints[i];
            if (_currentPoint.IsEmpty)
            {
                Goods goods = SpawnItem(prefab, _currentPoint);
                SetGoodsPosition(goods);
                _expiredTime = 0f;

                if (IsFull())
                    Filled?.Invoke();

                return true;
            }
        }

        return false;
    }

    protected override void SetGoodsPosition(Goods goods)
    {
        Move(goods, _currentPoint.Position);
    }

    protected override void Move(Goods goods, Vector3 targetPosition)
    {
        Mover.MoveInGlobalPosition(goods, targetPosition);
    }

    protected override bool IsFull()
    {
        int counter = 0;

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            if (_spawnPoints[i].IsEmpty == false)
                counter++;
        }

        return counter == _spawnPoints.Count;
    }

    private Goods SpawnItem(Item prefab, GoodsTargetPoint point)
    {
        GameObject goodsObject = Instantiate(prefab.gameObject, _startSpawnPoint.position, Quaternion.identity);
        goodsObject.transform.SetParent(point.transform);

        Goods goods = goodsObject.GetComponent<Goods>();
        point.Add(goods);

        return goods;
    }

    private void OnCleared()
    {
        Taken?.Invoke();
    }

    private IEnumerator FillInventory(Inventory inventory)
    {
        while (true)
        {
            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                GoodsTargetPoint point = _spawnPoints[i];

                if (point.IsEmpty == false && point.Model.IsPlaced)
                {
                    if (inventory.TryAddGoods(point.Model))
                    {
                        point.Clear();
                        yield return new WaitForSeconds(Mover.DelayBetweenGoodsMove);
                    }
                }
            }

            yield return null;
        }
    }
}
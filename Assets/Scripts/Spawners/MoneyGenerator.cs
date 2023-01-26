using System;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using TMPro.EditorUtilities;

public class MoneyGenerator : ObjectPool
{
    [SerializeField] private Money _template;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private ItemMoverToTargetPosition _mover;
    [SerializeField] private float _addedMoneyDuration;

    private Quaternion _rotation => _template.transform.rotation;
    public float AddedMoneyDuration => _addedMoneyDuration;
    public Transform SpawnPoint => _spawnPoint;
    public Vector3 SpawnPointPosition => _spawnPoint.position;

    private void OnValidate()
    {
        _addedMoneyDuration = Mathf.Clamp(_addedMoneyDuration, 0f, float.MaxValue);
    }

    private void OnEnable()
    {
        StartGenerate();
    }

    public override void StartGenerate()
    {
        Init(_template.gameObject);
    }

    public GameObject MoveToTarget(Transform target, Vector3 position, bool isDisableOnTarget = true)
    {
        if (TryGetObject(out GameObject moneyObject))
        {
            if (moneyObject.TryGetComponent(out Money money))
            {
                moneyObject.transform.SetParent(target);
                moneyObject.SetActive(true);
                moneyObject.transform.position = _spawnPoint.position;
                moneyObject.transform.rotation = _rotation;
                _mover.MoveInGlobalPosition(money, position);

                if (isDisableOnTarget)
                {
                    CancellationTokenSource token = new CancellationTokenSource();
                    DisableMoney(position, token.Token);
                }

                return moneyObject;
            }
        }

        throw new ArgumentException(nameof(moneyObject));
    }


    private async void DisableMoney(Vector3 targetPositon, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(_mover.Duration), token);

        DisableObject(targetPositon);
    }
}
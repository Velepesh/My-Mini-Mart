using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IHaveWallet
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private MoneyGenerator _moneySpawner;
    [SerializeField] private ItemHolderOptions _moneyPositionOptions;

    public Wallet Wallet { get; private set; }
    public bool IsEmptyHand => _inventory.IsEmpty;
    public Inventory Inventory => _inventory;
    public bool HaveMoney => Wallet.HaveMoney;

    public void InitWallet(Wallet wallet)
    {
        Wallet = wallet;
    }

    public void DisableAllMonies()
    {
        _moneySpawner.DisableAllObjects();
    }

    public void RemoveMoney(Transform target, Vector3 position)
    {
        _moneySpawner.MoveToTarget(_moneySpawner.SpawnPoint, position);
        Wallet.RemoveMoney();
    }

    public void AddMoney(Money money)
    {
        StartCoroutine(AddedMoney(money));
        Wallet.AddMoney();
    }

    private IEnumerator AddedMoney(Money money)
    {
        money.transform.SetParent(_moneySpawner.SpawnPoint);
        float elapsedTime = 0;
        Vector3 startPosition = money.transform.position;

        while (elapsedTime < _moneySpawner.AddedMoneyDuration)
        {
            money.transform.position = Vector3.Lerp(startPosition, _moneySpawner.SpawnPointPosition,
                elapsedTime / _moneySpawner.AddedMoneyDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        money.gameObject.SetActive(false);
    }
}
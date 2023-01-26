using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BuyZone : MonoBehaviour, IHaveWallet, IInteractionArea
{
    [SerializeField] private float _waitingTimeBeforePay;
    [SerializeField] private float _timeBetweenPay;
    [SerializeField] private GoodsHolder _holder;

    private Coroutine _payForAreaJob;

    public Wallet Wallet { get; private set; }

    public event UnityAction<GoodsHolder> Spawned;
    public event UnityAction Entered;
    public event UnityAction Exited;

    private void OnValidate()
    {
        _waitingTimeBeforePay = Mathf.Clamp(_waitingTimeBeforePay, 0, float.MaxValue);
        _timeBetweenPay = Mathf.Clamp(_timeBetweenPay, 0, float.MaxValue);
    }

    public void InitWallet(Wallet wallet)
    {
        Wallet = wallet;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _payForAreaJob = StartCoroutine(PayForArea(player));
            Entered?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StopCoroutine(_payForAreaJob);
            Exited?.Invoke();
        }
    }

    private IEnumerator PayForArea(Player player)
    {
        yield return new WaitForSeconds(_waitingTimeBeforePay);

        while (Wallet.HaveMoney && player.HaveMoney)
        {
            player.RemoveMoney(player.transform, transform.position);
            Wallet.RemoveMoney();

            if (player.HaveMoney == false)
                player.DisableAllMonies();

            if (Wallet.Money == 0)
                SpawnItemHolder(player);

            yield return new WaitForSeconds(_timeBetweenPay);
        }
    }

    private void SpawnItemHolder(Player player)
    {
        GoodsHolder holder = Instantiate(_holder.gameObject, transform.position, Quaternion.identity).GetComponent<GoodsHolder>();
        Spawned?.Invoke(holder);
        player.DisableAllMonies();
        Destroy(gameObject);
    }
}
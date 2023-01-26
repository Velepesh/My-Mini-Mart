using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ItemMoverToTargetPosition
{
    [Range(0f, 1f)]
    [SerializeField] private float _duration = 0.15f;
    [Range(0f, 1f)]
    [SerializeField] private float _delayBetweenGoodsMove = 0.1f;

    public float Duration => _duration;
    public float DelayBetweenGoodsMove => _delayBetweenGoodsMove;

    public void MoveInLocalPosition(Item item, Vector3 targetPosition)
    {
        Validate(item);
        TakeItem(item);

        item.transform.DOLocalMove(targetPosition, _duration);

        PlaceItem(item);
    }

    public void MoveInGlobalPosition(Item item, Vector3 targetPosition)
    {
        Validate(item);
        TakeItem(item);

        item.transform.DOMove(targetPosition, _duration);

        PlaceItem(item);
    }

    private void PlaceItem(Item item)
    {
        CancellationTokenSource token = new CancellationTokenSource();
        WaitBeforePlaceItem(item, token.Token);
    }

    private async void WaitBeforePlaceItem(Item item, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(_duration), token);

        item.Place();
    }

    private void TakeItem(Item item)
    {
        item.Take();
    }
   
    private void Validate(Item item)
    {
        if (item == null)
            throw new ArgumentException(nameof(item));
    }
}
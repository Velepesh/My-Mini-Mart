using UnityEngine;

public abstract class GoodsHolder : MonoBehaviour
{
    [SerializeField] private ItemMoverToTargetPosition _mover;

    protected ItemMoverToTargetPosition Mover => _mover;

    public abstract bool TryAddGoods(Goods goods);
    protected abstract bool IsFull();
    protected abstract void SetGoodsPosition(Goods goods);
    protected abstract void Move(Goods goods, Vector3 targetPosition);
}
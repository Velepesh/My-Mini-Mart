using UnityEngine;
using UnityEngine.Events;

public class Goods : Item
{
    [SerializeField] private int _price;

    public int Price => _price;

    public event UnityAction<float> AddedToInventory;
    public event UnityAction<float> AddedToShelf;

    public void AddToInventory(float duration)
    {
        AddedToInventory?.Invoke(duration);
    }

    public void AddToShelf(float duration)
    {
        AddedToShelf?.Invoke(duration);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Cashier _cashRegister;
    [SerializeField] private List<BuyZone> _buyZones;
    [SerializeField] private List<Shelf> _shelves;
    [SerializeField] private List<GoodsGenerator> _goodsSpawners;
    [SerializeField] private ClientSpawner _clientSpawner;

    private void Awake()
    {
        UpdateClientSpawner();
    }

    private void OnEnable()
    {
        _cashRegister.ClientPaid += OnClientPaid;
        
        for (int i = 0; i < _buyZones.Count; i++)
            _buyZones[i].Spawned += OnSpawned;
    }

    private void OnDisable()
    {
        _cashRegister.ClientPaid -= OnClientPaid;
        
        for (int i = 0; i < _buyZones.Count; i++)
            _buyZones[i].Spawned -= OnSpawned;
    }

    private void UpdateClientSpawner()
    {
        _clientSpawner.Init(_shelves, _cashRegister);
    }

    private void OnSpawned(GoodsHolder holder)
    {
        if (holder is Shelf shelf)
        {
            _shelves.Add(shelf);
            UpdateClientSpawner();
        }
    }

    private void OnClientPaid(Client client)
    {
        _clientSpawner.ExitShop(client);
    }
}
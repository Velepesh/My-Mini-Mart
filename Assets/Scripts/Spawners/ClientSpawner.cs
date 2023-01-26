using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : ObjectPool, ITarget
{
    [SerializeField] private List<Client> _templates;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _maxClients;
    [SerializeField] private float _secondsBetweenSpawn;
    [SerializeField] private List<ClientTargetPoint> _disableClientPoints;
    [SerializeField] private Sprite _icon;

    private float _elapsedTime = 0f;
    private List<Client> _clients = new List<Client>();
    private List<Shelf> _shelves = new List<Shelf>();
    private Cashier _cashRegister;
    private bool _canSpawn => _clients.Count < _maxClients;

    public Sprite Icon => _icon;
    public IReadOnlyList<ClientTargetPoint> ClientTargetPoints => _disableClientPoints;

    private void OnValidate()
    {
        _maxClients = Mathf.Clamp(_maxClients, 0, int.MaxValue);
        _secondsBetweenSpawn = Mathf.Clamp(_secondsBetweenSpawn, 0, float.MaxValue);
    }

    private void OnDisable()
    {
        for (int i = 0; i < _clients.Count; i++)
            _clients[i].ClientLeft -= OnClientLeft;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime > _secondsBetweenSpawn && _canSpawn)
        {
            if (TryGetObject(out GameObject newClient))
            {
                Spawn(newClient);

                _elapsedTime = 0f;
            }
        }
    }

    public void Init(List<Shelf> shelves, Cashier cashRegister)
    {
        if (shelves == null)
            throw new ArgumentNullException(nameof(shelves));

        if (cashRegister == null)
            throw new ArgumentNullException(nameof(cashRegister));

        _shelves = shelves;
        _cashRegister = cashRegister;
        StartGenerate();
    }

    public override void StartGenerate()
    {
        for (int i = 0; i < _templates.Count; i++)
            Init(_templates[i].gameObject);

        Shuffle();
    }

    public void ExitShop(Client client)
    {
        _clients.Remove(client);
        client.SetNextTarget();
    }

    private void Spawn(GameObject clientObject)
    {
        if (clientObject.TryGetComponent(out Client client))
        {
            clientObject.SetActive(true);
            clientObject.transform.position = _spawnPoint.position;
            client.Init(GenerateTargets(_shelves, _cashRegister));

            _clients.Add(client);
            client.ClientLeft += OnClientLeft;

            return;
        }

        throw new ArgumentException(nameof(clientObject));
    }

    private void OnClientLeft(Client client)
    {
        for (int i = 0; i < _disableClientPoints.Count; i++)
        {
            if (_disableClientPoints[i].IsEmpty == false)
            {
                _disableClientPoints[i].Clear();
                break;
            }
        }

        client.ClientLeft -= OnClientLeft;
        DisableObject(client.gameObject);
    }

    private Queue<ITarget> GenerateTargets(List<Shelf> shelves, Cashier cashRegister)
    {
        Queue<ITarget> targets = new Queue<ITarget>();
        Shelf shelf = GetShelf(shelves);

        targets.Enqueue(shelf);
        targets.Enqueue(cashRegister);
        targets.Enqueue(this);

        return targets;
    }

    private Shelf GetShelf(List<Shelf> shelves)
    {
        Shelf shelf = shelves[0];

        for (int i = 1; i < shelves.Count; i++)
        {
            if (shelves[i].GoodsCount > shelf.GoodsCount)
                shelf = shelves[i];
        }

        return shelf;
    }
}
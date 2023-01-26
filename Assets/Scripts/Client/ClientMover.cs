using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Client))]
[RequireComponent(typeof(NavMeshAgent))]
public class ClientMover : Mover
{
    [SerializeField] private float _rotationDuration;

    private readonly float _remainingDistance = 0.1f;

    private Client _client;
    private ClientTargetPoint _point;
    private NavMeshAgent _agent;
    private bool _isMove;

    public override event UnityAction<bool> Moved;
    public override event UnityAction<bool> Stoped;

    private void Awake()
    {
        _client = GetComponent<Client>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _client.TargetInited += OnTargetInited;
    }

    private void OnDisable()
    {
        _client.TargetInited -= OnTargetInited;
    }

    private void Update()
    {
        if (_point == null)
            return;

        float distance = Vector3.Distance(_point.transform.position, transform.position);
        
        if (distance < _remainingDistance)
            Stop();
        else
            Move();
    }

    protected override void Move()
    {
        _agent.SetDestination(_point.Position);
        Moved?.Invoke(_client.IsEmptyHand);
    }

    private void Stop()
    {
        ClientTargetPoint point = _point;
        Rotate(point);
        CancellationTokenSource token = new CancellationTokenSource();

        Stoped?.Invoke(_client.IsEmptyHand);

        if(_isMove)
            WaitBeforeReachedTarget(token.Token);
    }

    private void Rotate(ClientTargetPoint point)
    {
        transform.DORotate(point.Rotation.eulerAngles, _rotationDuration);
    }

    private void OnTargetInited(ITarget target)
    {
        if (_point != null)
            _point.Clear();

        for (int i = 0; i < target.ClientTargetPoints.Count; i++)
        {
            ClientTargetPoint point = target.ClientTargetPoints[i];
           
            if (point.IsEmpty)
            {
                point = target.ClientTargetPoints[i];
                SetTarget(point);

                if (_isMove == false)
                    _isMove = true;

                return;
            }
        }

        throw new Exception("Too much clients");
    }

    private void SetTarget(ClientTargetPoint point)
    {
        _point = point;
        _point.Add(_client);
    }

    private async void WaitBeforeReachedTarget(CancellationToken token)
    {
        _isMove = false;
        await Task.Delay(TimeSpan.FromSeconds(_rotationDuration), token);

        _client.ReachedTarget();
    }
}
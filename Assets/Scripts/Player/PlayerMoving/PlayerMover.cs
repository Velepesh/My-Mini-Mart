using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Player))]
public class PlayerMover : Mover
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private MovementOptions _options;
    [SerializeField] private ModelRotation _model;

    private Player _player;

    public override event UnityAction<bool> Moved;
    public override event UnityAction<bool> Stoped;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (_joystick.Direction == Vector2.zero)
        {
            Stoped?.Invoke(_player.IsEmptyHand);
            return;
        }

        Move();
    }

    protected override void Move()
    {
        Vector3 direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;

        transform.Translate(direction * _options.MoveSpeed * Time.fixedDeltaTime);
        _model.Rotate(direction);

        Moved?.Invoke(_player.IsEmptyHand);
    }
}

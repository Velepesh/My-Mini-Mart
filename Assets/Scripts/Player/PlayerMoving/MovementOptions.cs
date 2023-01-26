using System;
using UnityEngine;


[Serializable]
public class MovementOptions
{
    [SerializeField] private float _moveSpeed;

    public float MoveSpeed => _moveSpeed;
}

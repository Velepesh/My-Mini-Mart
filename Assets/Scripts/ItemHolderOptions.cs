using System;
using UnityEngine;

[Serializable]
public class ItemHolderOptions
{

    [SerializeField] private int _rowNumber;
    [SerializeField] private int _columnNumber;
    [SerializeField] private int _layersNumber;
    [SerializeField] private Transform _startItemPoint;
    [SerializeField] private float _rowStepX;
    [SerializeField] private Vector3 _columnStep;
    [SerializeField] private float _layerStepY;

    public int RowNumber => _rowNumber;
    public int ColumnNumber => _columnNumber;
    public int LayersNumber => _layersNumber;
    public Transform StartItemPoint => _startItemPoint;
    public float RowStepX => _rowStepX;
    public Vector3 ColumnStep => _columnStep;
    public float LayerStepY => _layerStepY;
}
using TMPro;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections;

public class InventoryFillRateView : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private UILookAtCamera _lookAtCamera;
    [SerializeField] private TMP_Text _fullText;
    [SerializeField] private float _targetFontSize;
    [SerializeField] private float _fontSizeSpeed;

    private void OnValidate()
    {
        _targetFontSize = Mathf.Clamp(_targetFontSize, 0, float.MaxValue);
        _fontSizeSpeed = Mathf.Clamp(_fontSizeSpeed, 0, float.MaxValue);
    }

    private void Awake()
    {
        DisableText();
    }

    private void OnEnable()
    {
        _inventory.Filled += OnFilled;
        _inventory.ItemTaken += OnItemTaken;
    }

    private void OnDisable()
    {
        _inventory.Filled -= OnFilled;
        _inventory.ItemTaken -= OnItemTaken;
    }

    private void OnFilled(Vector3 spawnPosition)
    {
        transform.localPosition = spawnPosition;

        StartCoroutine(EnableText());
    }

    private IEnumerator EnableText()
    {
        _fullText.fontSize = 0;
        _fullText.enabled = true;
        _lookAtCamera.enabled = true;

        while (_fullText.fontSize < _targetFontSize)
        {
            _fullText.fontSize += Time.deltaTime * _fontSizeSpeed;

            if (_fullText.fontSize >= _targetFontSize)
            {
                _fullText.fontSize = _targetFontSize;
                break;
            }

            yield return null;
        }
    }

    private void OnItemTaken()
    {
        DisableText();
    }

    private void DisableText()
    {
        _fullText.enabled = false;
        _lookAtCamera.enabled = false;
    }
}
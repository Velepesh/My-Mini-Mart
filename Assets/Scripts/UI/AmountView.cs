using System;
using TMPro;
using UnityEngine;

public class AmountView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        try
        {
            Validate();
        }
        catch (Exception e)
        {
            enabled = false;
            throw e;
        }
    }

    private void Validate()
    {
        if (_text == null)
            throw new InvalidOperationException();
    }

    public void SetAmount(int amount)
    {
        _text.text = $"{amount}";
    }
}
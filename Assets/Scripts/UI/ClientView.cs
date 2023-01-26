using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientView : MonoBehaviour
{
    [SerializeField] private Client _client;
    [SerializeField] private Image _shelfIcon;
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private Image _goodsTakenIcon;
    [SerializeField] private Image _shelfBackground;
    [SerializeField] private Image _goodsTakenBackground;

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

        _client.NeededGoodsChanged += OnNeededGoodsChanged;
        _client.GoodsTaken += OnGoodsTaken;

        EnableBackground(_shelfBackground);
        DisableBackground(_goodsTakenBackground);
    }

    private void OnDisable()
    {
        _client.NeededGoodsChanged -= OnNeededGoodsChanged;
        _client.GoodsTaken -= OnGoodsTaken;
    }

    private void Validate()
    {
        if (_shelfIcon == null)
            throw new InvalidOperationException();

        if (_goodsTakenIcon == null)
            throw new InvalidOperationException();

        if (_numberText == null)
            throw new InvalidOperationException();
    }
        
    private void OnNeededGoodsChanged(Sprite icon, int neddedNumber, int counter)
    {
        _shelfIcon.sprite = icon;
        _numberText.text = counter + "/" + neddedNumber;
    }

    private void OnGoodsTaken(ITarget target)
    {
        EnableBackground(_goodsTakenBackground);
        DisableBackground(_shelfBackground);

        _goodsTakenIcon.sprite = target.Icon;
    }

    private void EnableBackground(Image background)
    {
        background.gameObject.SetActive(true);
    }

    private void DisableBackground(Image background)
    {
        background.gameObject.SetActive(false);
    }
}
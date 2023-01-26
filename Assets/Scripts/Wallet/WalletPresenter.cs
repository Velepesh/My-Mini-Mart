class WalletPresenter
{
    private AmountView _view;
    private Wallet _model;

    public WalletPresenter(AmountView view, Wallet model)
    {
        _view = view;
        _model = model;
    }

    public void Enable()
    {
        _model.MoneyChanged += OnMoneyChanged;

        OnMoneyChanged();
    }

    public void Disable()
    {
        _model.MoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged()
    {
        _view.SetAmount(_model.Money);
    }
}
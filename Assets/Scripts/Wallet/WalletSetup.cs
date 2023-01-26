using UnityEngine;

public class WalletSetup : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _havingWallet;
    [SerializeField] private AmountView _view;
    [SerializeField] private int _money;

    private IHaveWallet IHaveWallet => (IHaveWallet)_havingWallet;
    private WalletPresenter _presenter;
    private Wallet _model;

    private void Awake()
    {
        _model = new Wallet(_money);
        _presenter = new WalletPresenter(_view, _model);
        IHaveWallet.InitWallet(_model);
    }

    private void OnEnable()
    {
        _presenter.Enable();
    }

    private void OnDisable()
    {
        _presenter.Disable();
    }
}
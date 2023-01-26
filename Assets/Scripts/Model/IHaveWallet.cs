public interface IHaveWallet
{
    public void InitWallet(Wallet wallet);
    public Wallet Wallet { get; }
}
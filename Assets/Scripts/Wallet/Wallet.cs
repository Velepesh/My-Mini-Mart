using System;

[Serializable]
public class Wallet
{
    public int Money { get; private set; }
    public bool HaveMoney => Money > 0;
    
    public event Action MoneyChanged;

    public Wallet(int money)
    {
        if(money < 0)
            throw new ArgumentOutOfRangeException(nameof(money));

        Money = money;
    }

    public void AddMoney()
    {
        Money += 1;

        MoneyChanged?.Invoke();
    }

    public void RemoveMoney()
    {
        if (Money - 1 < 0)
        {
            Money = 0;
            return;
        }

        Money -= 1;

        MoneyChanged?.Invoke();
    }
}
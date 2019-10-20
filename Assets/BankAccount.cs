using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public delegate void BalanceChangeDelegate(double newBalance);

public class Bank
{
    private List<BalanceChangeDelegate> OnBalanceChange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get;
    } = new List<BalanceChangeDelegate>();

    private double _balance;
    public double Balance
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            return _balance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private set
        {
            _balance = value;
            this.BroadcastNewBalance(_balance);
        }
    }

    public Bank(double initialBalance = 0)
    {
        Balance = initialBalance;
    }

    public void Add(double amount)
    {
        // hey man this balance updater is probably seriously racey
        // remember to use synchronization or your money might go away and you'll be all :'(
        if (amount < 0)
        {
            throw new Exception("Hey use Spend if you want to spend your $£€. Don't make yourself happy by spending by adding negatives you fool!");
        }
        this.Balance += amount;
    }

    public void Spend(double amount)
    {
        // hey man this balance updater is probably seriously racey
        // remember to use synchronization or your money might go away and you'll be all :'(
        if (amount < 0)
        {
            throw new Exception("Hey use Add if you want to print your own $£€ you QE stalwart!");
        }

        if (this.Balance - amount < 0)
        {
            throw new Exception("Uh oh doesn't look like you can afford it.");
        }
        this.Balance -= amount;
    }

    public void RegisterBalanceReceiver(BalanceChangeDelegate receiver)
    {
        OnBalanceChange.Add(receiver);
    }

    private void BroadcastNewBalance(double newBalance)
    {
        foreach (BalanceChangeDelegate d in OnBalanceChange)
        {
            try
            {
                d(newBalance);
            }
            catch (Exception e)
            {
                Debug.LogFormat("Exception notifying balance: {0}", e.ToString());
            }
        }
    }
}

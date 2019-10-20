using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyValueControl : MonoBehaviour
{
    private void Update()
    {
        SetBalanceText(Resources.Bank.Balance);
    }

    public void GlobalMoney()
    {
        // nothing hack
    }

    public void SetBalanceText(double balance)
    {
        var prefix = "Bank: ";
        var suffix = "";
        var printBalance = balance;
        if (balance < -1000000000)
        {
            prefix = "YEE HOWDY BOY BETTER CHECK Your student loan 😢: ";
        }

        if (balance > 100000)
        {
            printBalance /= 1000;
            suffix = "K";
        }
        if (balance > 1000000)
        {
            printBalance = balance / 1000000;
            suffix = "m";
        }
        if (balance > 1000000000)
        {
            printBalance = balance / 1000000000;
            suffix = "Bn";
        }

        GetComponent<Text>().text = $"{prefix} ${printBalance}{suffix}";
    }
}

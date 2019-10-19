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

    public void SetBalanceText(double balance)
    {
        var prefix = "Bank: ";
        if (balance < -1000000000)
        {
            prefix = "YEE HOWDY BOY BETTER CHECK Your student loan 😢: ";
        }

        GetComponent<Text>().text = $"{prefix} ${balance}";
    }
}

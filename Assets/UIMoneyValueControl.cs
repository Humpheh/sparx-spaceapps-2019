using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyValueControl : MonoBehaviour
{
    public UIMoneyValueControl()
    {
        Resources.Bank.RegisterBalanceReceiver(SetBalanceText);
    }

    public void SetBalanceText(double balance)
    {
        var prefix = "";
        if (balance < (double)1000000000.00f)
        {
            prefix = "Your student loan 😢: ";
        }

        GetComponent<Text>().text = $"{prefix} ${balance}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyValueControl : MonoBehaviour
{
    public void GlobalMoney(double balance)
    {
        Resources.money.value = balance;
        GetComponent<Text>().text = $"$ {balance}";
    }
}

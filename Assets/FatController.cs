using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mosquitodefenders.Tickers;

public class FatController : MonoBehaviour
{
    private CommunityChestCard[] cards = {
        new CommunityChestCard(
            "Congratulations",
            "You won a research grant. You got £14950 in funding",
            WinCallback(14950)
        ),
        new CommunityChestCard(
            "Hard Brexit",
            "Unfortunately there was a hard Brexit, and your funding has been cut by £8000",
            LoseCallback(35000)
        ),
        new CommunityChestCard(
            "You won SpaceApps!",
            "Congratulations, you won an award in the NASA SpaceApps hackathon and get some prize money",
            WinCallback(8450000)
        )
    };

    public void CommunityChest(bool cardDrawn)
    {
        if (cardDrawn)
        {
            var card = pickCard();
            Modal.OpenModal(card.title, card.text, card.callback);
        }
    }

    private class CommunityChestCard
    {
        public string title;
        public string text;
        public Modal.ModalCallback callback;
        public CommunityChestCard(string title, string text, Modal.ModalCallback callback)
        {
            this.title = title;
            this.text = text;
            this.callback = callback;
        }
    }

    private CommunityChestCard pickCard()
    {
        return Utils.RandomInArr<CommunityChestCard>(cards);
    }

    static Modal.ModalCallback WinCallback(int money)
    {
        return delegate (string option)
        {
            Resources.Bank.Add(money);
        };
    }

    static Modal.ModalCallback LoseCallback(int money)
    {
        return delegate (string option)
        {
            Resources.Bank.Spend(money);
        };
    }
}

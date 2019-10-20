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
            "You won a research grant. You got £14950 in funding.",
            WinCallback(14950)
        ),
        new CommunityChestCard(
            "Hard Brexit",
            "Unfortunately there was a hard Brexit, and you lose out on £14500 of funding.",
            LoseCallback(14000)
        ),
        new CommunityChestCard(
            "You won SpaceApps!",
            "Congratulations, you won an award in the NASA SpaceApps hackathon and get some prize money!",
            WinCallback(8400)
        ),
        new CommunityChestCard(
            "Doctors getting ill",
            "Unfortunately one of your doctors has been affected by a mosquito-borne disease and will be out of action for a while.",
            AddRemoveDocCallback(-1)
        ),
        new CommunityChestCard(
            "Inspirational",
            "Your efforts have inspired other doctors to join your cause!",
            AddRemoveDocCallback(2)
        ),
        new CommunityChestCard(
            "Funding cut",
            "Due to new government policies, your funding has been cut by £10000.",
            LoseCallback(10000)
        ),
        new CommunityChestCard(
            "Funding boost",
            "New government policies on foreign aid mean that you get an additional £50000 in funding.",
            WinCallback(50000)
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

    static Modal.ModalCallback AddRemoveDocCallback(int number)
    {
        return delegate (string option)
        {
            Map.GetSingleton().AddRemoveRandomDoc(number);
        };
    }
}

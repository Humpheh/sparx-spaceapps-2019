using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl 
{
    public static void EndGame()
    {
        Modal.OpenModal(
            "Unlucky!",
            "The mosquitoes got the better of you this time...",
            delegate { Application.Quit(); });
    }
}

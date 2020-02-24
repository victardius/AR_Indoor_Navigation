using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class InitiateFade
{
    static bool areWeFading = false;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(Color col, float multiplier)
    {
        if (areWeFading)
        {
            Debug.Log("Already Fading");
            return;
        }

        GameObject init = new GameObject();
        init.name = "Fader";
        Canvas myCanvas = init.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        init.AddComponent<Fade>();
        init.AddComponent<CanvasGroup>();
        init.AddComponent<Image>();

        Fade scr = init.GetComponent<Fade>();
        scr.fadeDamp = multiplier;
        scr.fadeColor = col;
        scr.start = true;
        areWeFading = true;
        scr.InitiateFader();

    }

    public static void DoneFading()
    {
        areWeFading = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : View
{

    public Slider slider_Sound;
    public Slider slider_Music;

    public void ExitClick() {
        Hidden();
    }

    public void OnSoundValueChange(Slider slider) {
        PlayerPrefs.SetFloat(Const.Sound,slider.value);
    }

    public void OnBgSound(Slider slider)
    {
        PlayerPrefs.SetFloat(Const.Music, slider.value);
    }

    public override void Show()
    {
        base.Show();
        slider_Sound.value = PlayerPrefs.GetFloat(Const.Sound,0);
        slider_Music.value = PlayerPrefs.GetFloat(Const.Music, 0);
    }
}

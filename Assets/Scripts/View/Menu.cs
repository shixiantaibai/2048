using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public SelectModeGame smg;
    public SetPanel sp;
    public AudioClip bgClip;

    private void Start()
    {
        AudioManager._instance.PlayMusic(bgClip);
    }
    public void OnStartGameClick() {
        smg.Show();
    }

    public void OnSetClick() {
        sp.Show();
    }

    public void OnExitGameClick() {
        Debug.Log("游戏结束");
        Application.Quit();
    }
}

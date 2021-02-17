using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectModeGame : View
{
    public void OnSelectModelClick(int count) {
        PlayerPrefs.SetInt(Const.GameModel,count);
        SceneManager.LoadSceneAsync(1);
    }
   
}

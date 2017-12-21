using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class ChangeScene : MonoBehaviour {
    
    public void LoadMenu(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    public void exitGame()
    {
       
        Application.Quit();
    }


}

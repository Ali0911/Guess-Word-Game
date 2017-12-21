using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ConnectGame : MonoBehaviour {
    public InputField text;
    public  bool IsNullOrWhiteSpace( string value)
    {
        return string.IsNullOrEmpty(value) ||
            ReferenceEquals(value, null) ||
                string.IsNullOrEmpty(value.Trim());
    }
    public void LoadMenu(string sceneName)
    {
        
        if(!string.IsNullOrEmpty(text.text) && !IsNullOrWhiteSpace(this.text.text))
        {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("PlayerName",this.text.text);
                print(this.text.text);
                Application.LoadLevel(sceneName);
        }
      
        else
        {
            text.text = "";
        }
       
    }
   

}

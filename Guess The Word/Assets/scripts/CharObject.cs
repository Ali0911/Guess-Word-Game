using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharObject : MonoBehaviour {
    public char character;
    public Text text;
    public Image image;
    public RectTransform recTransform;
    public int index;

    [Header("Appearance")]
    public Color normalColor;
    public Color selectColor;

    bool isSelected = false;

    public CharObject Init(char c)
    {
        character = c;
        text.text = c.ToString();
        gameObject.SetActive(true);
        return this;
    }

	public void Select()
    {
        isSelected = !isSelected;

        image.color = isSelected ? selectColor : normalColor;

        if(isSelected)
        {
            WordScramble.main.Select(this);
        }
        else
        {
            WordScramble.main.UnSelect();
        }
    }
}

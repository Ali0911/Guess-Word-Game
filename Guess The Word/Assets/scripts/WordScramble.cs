using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class Result
{
    public int totalScore = 0;
    

    [Header("REF UI")]
    public Text textTime;
    public Text textTotalScore;
}



[System.Serializable]
public class Word
{
    public string word;
    [Header("leave empty if you want randomized")]
    public string desiredRandom;
    [Space(10)]
    public float timeLimit;

    public string GetString()
    {
        if(!string.IsNullOrEmpty(desiredRandom))
        {
            return desiredRandom;
        }
        string result = word;
        
        while(result==word)
        {
            result = "";
            List<char> characters = new List<char>(word.ToCharArray());
            while (characters.Count > 0)
            {
                int indexChar = Random.Range(0, characters.Count - 1);
                result += characters[indexChar];

                characters.RemoveAt(indexChar);


            }

        }
        return result;
    }
}

public class WordScramble : MonoBehaviour {
    public Word[] words;
    [Space(10)]
    public Result result;
    public int cnt = 0;
    [Header("UI REFERENCE")]
    public CharObject prefab;
    public Transform container;
    public float space;
    public float lerpSpeed = 5;
    public Image [] imgLife;
    List<CharObject> charObjects = new List<CharObject>();
    CharObject firstSelected;
    public int currentWord;


    public AudioSource startAudio;
    public AudioSource endAudio;

    public Image gameover;
    public Text answeredText;
    public Text scoreTitle;
    
    public static WordScramble main;

    private float totalScore;
    private int health = 5;

     void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start ()
    {
        gameover.enabled = false;
        endAudio.GetComponent<AudioSource>().mute = true;
        List<Word> list = new List<Word>();
        addWordsToList(list);
        Shuffle(list);
        equalListToWords(list);
        ShowScramble(currentWord);
        
        result.textTotalScore.text = result.totalScore.ToString();
        
    }

    // Update is called once per frame
    void Update() {
            
            RepositionObject();
            totalScore = Mathf.Lerp(totalScore, result.totalScore, Time.deltaTime * 5);
            result.textTotalScore.text = Mathf.RoundToInt(totalScore).ToString();
            if(cnt!=0)
            {
                 answeredText.enabled = true;
            }
            else
            {
                 answeredText.enabled = false;
        }
    }
   

    void RepositionObject()
    {
        if(charObjects.Count==0)
        {
            return;
        }

        float center = (charObjects.Count - 1) / 2;
        for (int i=0;i< charObjects.Count;i++)
        {
            charObjects[i].recTransform.anchoredPosition = 
                Vector2.Lerp(charObjects[i].recTransform.anchoredPosition, 
               new Vector2((i - center) * space, 0),lerpSpeed*Time.deltaTime);
            charObjects[i].index = i;
        }
    }

    /// <summary>
    /// Show a random word to the screen
    /// </summary>
    public void ShowScramble()
    {
        
        ShowScramble(Random.Range(0, words.Length - 1));
    }

    /// <summary>
    /// Show word from collection with desired index
    /// </summary>
    /// <param name="index">index of the element</param>
    public void ShowScramble(int index)
    {
        charObjects.Clear();
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);

        }

        if (index>words.Length-1)
        {
            Debug.LogError("index out of range, please enter range between 0-"+(words.Length-1));
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerName"), result.totalScore);
            PlayerPrefs.Save();
            LoadMenu();
            return;
        }

        char[] chars = words[index].GetString().ToCharArray();
        //print(chars[0]);
        foreach(char c in chars)
        {
            CharObject clone = Instantiate(prefab.gameObject).GetComponent<CharObject>();
            clone.transform.SetParent(container);

            charObjects.Add(clone.Init(c));
           // print(words[index].word);
        }

        currentWord = index;
        StartCoroutine(TimeLimit());
    }


    public void Swap(int indexA,int indexB)
    {
        CharObject tmpA = charObjects[indexA];
        charObjects[indexA] = charObjects[indexB];
        charObjects[indexB] = tmpA;


        charObjects[indexA].transform.SetAsLastSibling();
        charObjects[indexB].transform.SetAsLastSibling();

        CheckWord();
    }


    public void Select(CharObject charObject)
    {
        if(firstSelected)
        {
            Swap(firstSelected.index, charObject.index);

            //Unselected
            firstSelected.Select();
            charObject.Select();


        }
        else
        {
            firstSelected = charObject;
        }
    }

    public void UnSelect()
    {
        firstSelected = null;
    }
    /*
    public bool CheckWord()
    {
        string word = "";
        foreach (CharObject charObject in charObjects)
        {
            word += charObject.character;
        }

        if (word == words[currentWord].word)
        {
            currentWord++;
            ShowScramble(currentWord);

            return true;
        }

        return false;

    }
    */

    public void CheckWord()
    {
        StartCoroutine(CoCheckWord());
    }

        IEnumerator CoCheckWord()
    {
       
        yield return new WaitForSeconds(0.5f);
        print(words[currentWord].word);
       
        string word = "";
        foreach (CharObject charObject in charObjects)
        {
            word += charObject.character;
        }
        if(timeLimit<=0)
        {
            answeredText.text = "Das letzte Wort war" + " " + words[currentWord].word;
            --health;
            imgLife[health].enabled = false;
            cnt =- 1;
            if (health==0)
            {

                charObjects.Clear();
                foreach (Transform child in container)
                {
                    Destroy(child.gameObject);

                }
                startAudio.enabled = false;
                result.textTime.enabled = false;
                result.textTotalScore.enabled = false;
                scoreTitle.enabled = false;
                startAudio.GetComponent<AudioSource>().mute = true;
                endAudio.GetComponent<AudioSource>().mute = false;
                endAudio.GetComponent<AudioSource>().Play();
                gameover.enabled = true;
                
                

                print("Oyun bitti");
                print(result.totalScore);
                yield return new WaitForSeconds(4.7f);
                PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerName"),result.totalScore);
                PlayerPrefs.Save();
                LoadMenu();
            }
            
            
            if (words.Length == currentWord)
            {
                PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerName"), result.totalScore);
                PlayerPrefs.Save();
                LoadMenu();
            }
            currentWord++;
            ShowScramble(currentWord);
            
            yield break;
        }

        if (word == words[currentWord].word)
        {
            answeredText.text = "Das letzte Wort war" + " " + words[currentWord].word;
            currentWord++;
            result.totalScore += Mathf.RoundToInt(timeLimit);
            //result.textTotalScore.text = result.totalScore.ToString();

            //StopCoroutine(TimeLimit());
            cnt = 0;
            ShowScramble(currentWord);

            
        }
        if(words.Length==currentWord)
        {
            PlayerPrefs.SetInt(PlayerPrefs.GetString("PlayerName"), result.totalScore);
            PlayerPrefs.Save();
            LoadMenu();
        }

        
    }

    float timeLimit;
    IEnumerator TimeLimit()
    {
        timeLimit = words[currentWord].timeLimit;
        result.textTime.text = Mathf.RoundToInt(timeLimit).ToString();

        int myWord = currentWord;

        yield return new WaitForSeconds(1);


        while(timeLimit>0)
        {
            if (myWord != currentWord) { yield break; }
            
            timeLimit -= Time.deltaTime;
            result.textTime.text = Mathf.RoundToInt(timeLimit).ToString();
            yield return null;
        }

        //result.textTotalScore.text = result.totalScore.ToString();
        CheckWord();
    }




    public void LoadMenu()
    {
        Application.LoadLevel("finished");
    }

    public void addWordsToList(List<Word> list)
    {
        for (int i = 0; i < words.Length; i++)
        {
            list.Add(words[i]);
        }
    }
    
    public void Shuffle(List<Word> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Word temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void equalListToWords(List<Word> list)
    {
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = list[i];
            
        }
    }

   


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using Mono.Data.Sqlite;
public class Player
{
    public Player(string _name,int _score) { name = _name; score = _score; }
    public string name;
    public int score;
}

public class HighScores : MonoBehaviour {

    private string connectionString;
    public Text [] text = new Text[14];
    private List<Player> people;
    private List<Player> sortedPeople;
    private void Start()
    {   connectionString = "URI=file:"+Application.dataPath+"/HighScoreDB.sqlite";
        people = new List<Player>();
        GetScores();
        orderScores();
        sortedPeople = new List<Player>();
        sortedPeople = people;
        fillScoresToText();
        for (int i=0;i<sortedPeople.Count;i++)
        {
            print(sortedPeople[i].name+" "+sortedPeople[i].score);
        }
        
    }
    private void fillScoresToText()
    {
        int totalSize = 13;
        int stringSize;
        for(int i=0;i< sortedPeople.Count;i++)
        {
            stringSize = sortedPeople[i].name.Length;

            text[i].text =(i+1)+". "+sortedPeople[i].name +findSpace(stringSize, totalSize)+ sortedPeople[i].score;
        }
    }
    private string findSpace(int realSize, int constantSize)
    {
        string mytext="";
        for(int i=0;i< (constantSize- realSize);i++)
        {
            mytext +="-";
        }

        return mytext;
    }

    private void orderScores()
    {
        people.Sort((x, y) => y.score.CompareTo(x.score));
    }

    private void GetScores()
    {
        
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            
            dbConnection.Open();

            using(IDbCommand dbCmd=dbConnection.CreateCommand())
            {
                
                string sqlQuery = "SELECT * FROM HighScores";
                dbCmd.CommandText = sqlQuery;
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        
                        Debug.Log(reader.GetString(1)+" "+reader.GetInt32(2));
                        
                        people.Add(new Player(reader.GetString(1), reader.GetInt32(2)));
                    }

                    dbConnection.Close();
                    reader.Close();
                }
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class LoadScore : MonoBehaviour {
    public string _playerName;
    public string _playerScore;
    public Text playerScore;
    private string connectionString;
    // Use this for initialization
    void Start () {
        connectionString = "URI=file:" + Application.dataPath + "/HighScoreDB.sqlite";
        _playerName = PlayerPrefs.GetString("PlayerName").ToString();
        playerScore.text = PlayerPrefs.GetInt(PlayerPrefs.GetString("PlayerName")).ToString();
        _playerScore=PlayerPrefs.GetInt(PlayerPrefs.GetString("PlayerName")).ToString();
        print(_playerName);
        print(_playerScore);
        SetScores(_playerName, _playerScore);
    }

    private void SetScores(string _name,string _score)
    {

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("INSERT INTO HighScores(playerName,playerScore) VALUES(\"{0}\",\"{1}\")", _name, _score);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }

}

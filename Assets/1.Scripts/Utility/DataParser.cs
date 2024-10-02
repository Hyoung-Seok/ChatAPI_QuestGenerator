using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    private const string FOLDER_NAME = "DataBase";
    private const string DB_NAME = "NPC_DATA.db";

    private void Start()
    {
        var fullPath = Path.Combine(Application.streamingAssetsPath, FOLDER_NAME);
        fullPath = Path.Combine(fullPath, DB_NAME);
        var connectionString = "URI=file:" + fullPath;

        var dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        
        Debug.Log(dbConnection.State);

        var dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM " + "NPC_DATA";
        var dataReader = dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            var name = dataReader.GetString(0);
            Debug.Log(name);
        }
        
        dataReader.Close();
    }
}

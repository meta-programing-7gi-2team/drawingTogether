using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MySql.Data;
using MySql.Data.MySqlClient;

public class User_info
{
    public string User_ID { get; private set; }
    public string User_Password { get; private set; }
    public string User_Name { get; private set; }
    public string User_Image { get; set; }

    public User_info(string id, string password, string name, string Image)
    {
        User_ID = id;
        User_Password = password;
        User_Name = name;
        User_Image = Image;
    }
}

public class UserInfo_Manager : MonoBehaviour
{
    public User_info info;

    public MySqlConnection connection;
    public MySqlDataReader reader;

    [SerializeField] private string DB_Path = string.Empty;

    public static UserInfo_Manager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DB_Path = Application.dataPath + "/Database";
        string serverinfo = Server_Set(DB_Path);
        try
        {
            if (serverinfo.Equals(string.Empty))
            {
                return;
            }
            connection = new MySqlConnection(serverinfo);
            connection.Open();
            Debug.Log("DB Open connection compelete");
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private string Server_Set(string path)
    {
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string JsonString = File.ReadAllText(path + "/config.json");
        JsonData UserData = JsonMapper.ToObject(JsonString);
        string serverInfo = $"Server={UserData[0]["IP"]};" +
                          $"Database={UserData[0]["TableName"]};" +
                          $"Uid={UserData[0]["ID"]};" +
                          $"Pwd={UserData[0]["PW"]};" +
                          $"Port={UserData[0]["PORT"]};" +
                          "CharSet=utf8;";
        return serverInfo;
    }

    private bool connection_Check(MySqlConnection con)
    {
        if (con.State != System.Data.ConnectionState.Open)
        {
            con.Open();
            if (con.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
        }
        return true;
    }

    public static Dictionary<string, bool> loggedInUsers = new Dictionary<string, bool>();

    public bool Login(string id, string password)
    {
        try
        {
           // if (loggedInUsers.ContainsKey(id) && loggedInUsers[id])
           // {
           //     Debug.Log("이미 로그인 중인 계정입니다.");
           //     return false;
           // }

            if (!connection_Check(connection))
            {
                return false;
            }
            string SQL_Command = string.Format($@"SELECT User_ID, User_Password, User_Name, Image FROM user_info WHERE User_ID='{id}' AND User_Password='{password}';");
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string Id = (reader.IsDBNull(0)) ? string.Empty : (string)reader["User_ID"];
                    string Pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["User_Password"];
                    string Name = (reader.IsDBNull(2)) ? string.Empty : (string)reader["User_Name"];
                    string lmage = (reader.IsDBNull(3)) ? string.Empty : (string)reader["Image"];

                    if (!Id.Equals(string.Empty) || !Pass.Equals(string.Empty))
                    {
                        info = new User_info(Id, Pass, Name, lmage);

                        loggedInUsers[id] = true;

                        if (!reader.IsClosed) reader.Close();
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (!reader.IsClosed) reader.Close();
            return false;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }

    //public void Logout(string id)
    //{
    //    if (loggedInUsers.ContainsKey(id))
    //    {
    //        loggedInUsers[id] = false;
    //        Debug.Log($"{id} 로그아웃");
    //    }
    //}

    public bool SetUp(string id, string password, string name)
    {
        try
        {
            if (!connection_Check(connection))
            {
                return false;
            }

            string Overlap_Check = string.Format($@"SELECT User_ID, User_Password, User_Name, Image FROM user_info WHERE User_ID='{id}';");
            MySqlCommand chkcmd = new MySqlCommand(Overlap_Check, connection);
            reader = chkcmd.ExecuteReader();
            if (reader.HasRows)
            {
                if (!reader.IsClosed) reader.Close();
                return false;
            }
            if (!reader.IsClosed) reader.Close();

            string SQL_Command = string.Format($@"INSERT INTO user_info VALUES ('{id}', '{password}', '{name}', 'IM000');");
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            reader = cmd.ExecuteReader();
            
            if (!reader.IsClosed) reader.Close();
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }

    public bool Change(string id, string password, string name, string image)
    {
        try
        {
            if (!connection_Check(connection))
            {
                return false;
            }

            string SQL_Command = "UPDATE User_info SET Image = @image WHERE User_Name = @name";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@Image", image);

            info = new User_info(id, password, name, image);

            int NonQuery = cmd.ExecuteNonQuery();

            return NonQuery > 0;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
}

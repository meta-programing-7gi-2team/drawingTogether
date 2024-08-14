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
    public bool IsLoggedIn { get; set; }

    public User_info(string id, string password, string name, string Image)
    {
        User_ID = id;
        User_Password = password;
        User_Name = name;
        User_Image = Image;
        IsLoggedIn = false;
    }
}

public class UserInfo_Manager : MonoBehaviour
{
    public User_info info;

    public MySqlConnection connection;
    public MySqlDataReader reader;
    public string U_Image;

    [SerializeField] private string DB_Path = string.Empty;

    public Dictionary<string, User_info> allUsers = new Dictionary<string, User_info>();

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
            LoadAllUsersFromDatabase();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void Start()
    {
        LoadAllUsersFromDatabase();

        foreach (var user in allUsers)
        {
            if (!user.Value.IsLoggedIn)
            {
                Debug.Log($"로그인되지 않은 계정: {user.Key}");
            }
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

    void LoadAllUsersFromDatabase()
    {
        try
        {
            string SQL_Command = "SELECT User_ID, User_Password, User_Name, Image FROM user_info";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string id = reader["User_ID"].ToString();
                string password = reader["User_Password"].ToString();
                string name = reader["User_Name"].ToString();
                string image = reader["Image"].ToString();

                allUsers[id] = new User_info(id, password, name, image);
            }
            if (!reader.IsClosed) reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            if (!reader.IsClosed) reader.Close();
        }
    }

    public bool Login(string id, string password)
    {
        if (allUsers.ContainsKey(id) && allUsers[id].IsLoggedIn)
        {
            Debug.Log("이미 로그인 중인 계정입니다.");
            return false;
        }

        if (allUsers.ContainsKey(id) && allUsers[id].User_Password == password)
        {
            allUsers[id].IsLoggedIn = true;
            info = allUsers[id];

            Debug.Log($"{id}로그인 성공");

            foreach (var user in allUsers)
            {
                Debug.Log($"ID: {user.Key}, 로그인 상태: {user.Value.IsLoggedIn}");
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsAlreadyLoggedIn(string id)
    {
        return allUsers.ContainsKey(id) && allUsers[id].IsLoggedIn;
    }

    public void Logout(string id)
    {
        if (allUsers.ContainsKey(id))
        {
            allUsers[id].IsLoggedIn = false;
            Debug.Log($"{id} 로그아웃");
        }
    }

    public bool SetUp(string id, string password, string name)
    {
        try
        {
            if (allUsers.ContainsKey(id))
            {
                Debug.Log("아이디가 중복되었습니다.");
                return false;
            }

            string SQL_Command = $"INSERT INTO user_info VALUES ('{id}', '{password}', '{name}', 'IM000');";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            cmd.ExecuteNonQuery();

            allUsers[id] = new User_info(id, password, name, "IM000");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
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

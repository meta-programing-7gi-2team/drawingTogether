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
    public bool LogIn { get; set; }

    public User_info(string id, string password, string name, string Image)
    {
        User_ID = id;
        User_Password = password;
        User_Name = name;
        User_Image = Image;
        LogIn = false;
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
            if (!user.Value.LogIn)
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

    private bool Connection_Check(MySqlConnection con)
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
            string SQL_Command = "SELECT User_ID, User_Password, User_Name, Image, Login FROM user_info";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string id = reader["User_ID"].ToString();
                string password = reader["User_Password"].ToString();
                string name = reader["User_Name"].ToString();
                string image = reader["Image"].ToString();
                bool logIn = Convert.ToBoolean(reader["LogIn"]);

                allUsers[id] = new User_info(id, password, name, image) { LogIn = logIn };
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
        try
        {
            if (!Connection_Check(connection))
            {
                return false;
            }

            string checkLoginQuery = $"SELECT LogIn FROM user_info WHERE User_ID ='{id}'";
            MySqlCommand checkCmd = new MySqlCommand(checkLoginQuery, connection);
            object logIn = checkCmd.ExecuteScalar();

            if (logIn != null && Convert.ToInt32(logIn) == 1)
            {
                Debug.Log("이미 로그인 중인 계정입니다.");
                return false;
            }

            string SQL_Command = $"SELECT User_ID, User_Password, User_Name, Image, Login FROM user_info WHERE User_ID='{id}' AND User_Password='{password}'";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string Id = reader["User_ID"].ToString();
                    string Pass = reader["User_Password"].ToString();
                    string Name = reader["User_Name"].ToString();
                    string Image = reader["Image"].ToString();

                    if (Id == id && Pass == password)
                    {
                        info = new User_info(Id, Pass, Name, Image);

                        reader.Close();

                        string updateLoginStatus = $"UPDATE user_info SET LogIn = 1 WHERE User_ID = '{id}'";
                        MySqlCommand updateCmd = new MySqlCommand(updateLoginStatus, connection);
                        updateCmd.ExecuteNonQuery();

                        info.LogIn = true;

                        return true;
                    }
                }
            }

            reader.Close();
            return false;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }

    public bool IsAlreadyLoggedIn(string id)
    {
        if (!Connection_Check(connection))
        {
            return false;
        }

        string checkLoginQuery = $"SELECT LogIn FROM user_info WHERE User_ID ='{id}'";
        MySqlCommand checkCmd = new MySqlCommand(checkLoginQuery, connection);
        object logIn = checkCmd.ExecuteScalar();

        return logIn != null && Convert.ToInt32(logIn) == 1;
    }

    public void Logout(string id)
    {
        try
        {
            if (!Connection_Check(connection))
            {
                return;
            }

            string updateLogoutStatus = $"UPDATE user_info SET LogIn = 0 WHERE User_ID = '{id}'";
            MySqlCommand cmd = new MySqlCommand(updateLogoutStatus, connection);
            cmd.ExecuteNonQuery();

            if (allUsers.ContainsKey(id))
            {
                allUsers[id].LogIn = false;
            }

            Debug.Log($"{id} 로그아웃");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        if (info != null && info.LogIn)
        {
            Logout(info.User_ID);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (info != null && info.LogIn)
            {
                Logout(info.User_ID);
            }
        }
    }

    public bool SignUp(string id, string password, string name)
    {
        try
        {
            if (allUsers.ContainsKey(id))
            {
                Debug.Log("아이디가 중복되었습니다.");
                return false;
            }

            string SQL_Command = $"INSERT INTO user_info (User_ID, User_Password, User_Name, Image, LogIn) VALUES ('{id}', '{password}', '{name}', 'IM000', 0);";
            MySqlCommand cmd = new MySqlCommand(SQL_Command, connection);
            cmd.ExecuteNonQuery();

            allUsers[id] = new User_info(id, password, name, "IM000") { LogIn = false };
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
            if (!Connection_Check(connection))
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

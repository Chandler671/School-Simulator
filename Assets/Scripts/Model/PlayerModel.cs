using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJson
{
    public string username;
    public bool isNewPlayer = true;
    public int heroId;
}


public class PlayerModel
{

    private PlayerJson _playerJson;
    public PlayerJson playerJson
    {
        get
        {
            return _playerJson;
        }
        set
        {
            if (value == null)
            {
                Debug.LogWarning("尝试设置空的 PlayerJson 数据");
                _playerJson = new PlayerJson();
            }
            else
            {
                _playerJson = value;
            }
        }
    }

    public void SavePlayerData()
    {
        if (playerJson.username == null)
        {
            Debug.Log("用户id为空，保存失败");
            return;
        }
        Debug.Log($"已保存用户：{_playerJson.username} 数据");
        PlayerPrefsManager.Instance.SetObject<PlayerJson>(_playerJson.username, _playerJson);    
    }

    private static PlayerModel _instance;
    public static PlayerModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerModel();
                _instance._playerJson = new PlayerJson();
            }
            return _instance;
        }
    }

}

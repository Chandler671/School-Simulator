using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginModel
{
    private static LoginModel _instance;
    public static LoginModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LoginModel();
            }
            return _instance;
        }
    }

    // 模拟登录验证（在实际项目中替换为网络请求）
    public void SimulateLoginValidation(string username)
    {
        // 获取用户数据
        PlayerJson playerJson = PlayerPrefsManager.Instance.GetObject<PlayerJson>(username);
    
        if (playerJson == null)
        {
            // 如果存档不存在，创建新的
            playerJson = new PlayerJson();
            playerJson.username = username;
            PlayerPrefsManager.Instance.SetObject<PlayerJson>(username, playerJson);
            Debug.Log($"当前存档不存在该用户:{username}，已创建新的存档");
        }
        else
        {
            Debug.Log($"已找到用户:{username} 的存档");
        }
        PlayerModel.Instance.playerJson = playerJson;
    }
    
}

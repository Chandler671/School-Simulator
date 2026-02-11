using System;

//登录成功
public class login_Success : GameEvent
{
    public string UserName;

    public login_Success(string userName)
    {
        UserName = userName;
    }
}
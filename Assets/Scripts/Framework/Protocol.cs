using System;

//登录成功
public class login_Success : GameEvent
{
    public string UserName;
    public int HeroId;

    public login_Success(string userName, int heroId)
    {
        UserName = userName;
        HeroId = heroId;
    }
}
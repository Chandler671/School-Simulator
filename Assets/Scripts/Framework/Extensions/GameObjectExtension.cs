using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
     public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => obj.GetComponent<T>() ? obj.GetComponent<T>() : obj.AddComponent<T>();
}

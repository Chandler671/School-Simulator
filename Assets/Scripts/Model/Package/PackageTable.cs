using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PackageTable : ScriptableObject 
{
    public List<PackageTableItem> DataList = new List<PackageTableItem>();
}

[System.Serializable]
public class PackageTableItem
{
    public int id;
    public string type;
    public string name;
    public string description;
    public int maxStack;
    public int star;
    public string MaterialRequirement;
    public string imgPath;

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageTable : BaseTable<PackageTableItem>
{

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
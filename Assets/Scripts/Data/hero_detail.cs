using System;
using System.Collections;

public class hero_detail : BaseTable<hero_detail_json>
{
    public hero_detail_json getDataByID(int id)
    {
        foreach (hero_detail_json item in this.DataList)
        {
            if (id == item.id)
            {
                return item;
            }
        }
        return null;
    }
}

[System.Serializable]
public class hero_detail_json
{
    public int id;
    public string name;
    public string description;
    public string imgPath;
    public string gender;
    public string heroEff;
}
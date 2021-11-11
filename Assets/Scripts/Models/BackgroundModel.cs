using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundModel 
{
    public int LayerNumber { get; set; }
    public float ScrollSpeed { get; set; }
    public float YPosition { get; set; }
    public float XScale { get; set; }
    public float YScale { get; set; }
}

public class BackgroundListModel : IData
{
    public int StageNumber { get; set; }
    public List<BackgroundModel> List { get; set; }

    public int GetId()
    {
        return StageNumber;
    }

    public string GetName()
    {
        return string.Empty;
    }
}

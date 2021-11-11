using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo : IData
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public QuestType Type { get; set; }
    public int Value { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return string.Empty;
    }

}

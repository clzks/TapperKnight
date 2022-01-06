using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo : IData
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public string CharacterName { get; set; }
    public string PrefabName { get; set; }
    public QuestType Type { get; set; }
    public int Value { get; set; }
    public string Condition { get; set; }
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return CharacterName;
    }

}

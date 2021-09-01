using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataModel
{
    public int Id { get; set; }
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public int CurrExp { get; set; }

    public static CharacterDataModel CreateNewDataModel(int id)
    {
        CharacterDataModel dm = new CharacterDataModel();
        dm.Id = id;
        dm.Level = 1;
        dm.MaxLevel = 10;
        dm.CurrExp = 0;

        return dm;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel
{
    public int Id { get; set; }
    public string NameKR { get; set; }
    public string PrefabName { get; set; }
    public float Hp { get; set; }
    public float MinSpeed { get; set; }
    public float MaxSpeed { get; set; }
    public float HpDecreasePerSecond { get; set; }
    public float Defence { get; set; }
}

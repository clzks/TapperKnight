using System.Collections.Generic;


public class PlayerModel
{
    public string Id { get; set; }
    public List<CharacterDataModel> OwnCharacterList { get; set; }
    public int OwnGold { get; set; }
    public int OwnScore { get; set; }
    public int RequiredExperience { get; set; }
    public int IncreaseRequiredExperience { get; set; }
    public int HpPerLevelUp { get; set; }
}

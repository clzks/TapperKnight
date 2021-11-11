using System.Collections.Generic;


public class PlayerModel : IData
{
    public string Id { get; set; }
    public List<CharacterDataModel> OwnCharacterList { get; set; }
    public int OwnGold { get; set; }
    public int TotalRunningRecord { get; set; }
    public int RequiredExperience { get; set; }
    public int IncreaseRequiredExperience { get; set; }
    public int HpPerLevelUp { get; set; }

    public static PlayerModel MakeSamplePlayerModel()
    {
        var list = DataManager.Get().GetCharacterList();

        PlayerModel model = new PlayerModel
        {
            Id = "Id",
            OwnGold = 0,
            TotalRunningRecord = 0,
        };

        return model;
    }

    public int GetId()
    {
        return 0;
    }

    public string GetGooglePlayId()
    {
        return Id;  
    }

    public string GetName()
    {
        return Id;
    }
}

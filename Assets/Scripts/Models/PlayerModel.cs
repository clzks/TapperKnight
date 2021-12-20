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

    public static PlayerModel MakePlayerModel()
    {
        var list = DataManager.Get().GetCharacterList();

        PlayerModel model = new PlayerModel
        {
            Id = "Id",
            OwnGold = 0,
            TotalRunningRecord = 0,
        };

        model.OwnCharacterList = new List<CharacterDataModel>();

        foreach (var item in list.Values)
        {
            if(true == item.IsRetention)
            {
                model.OwnCharacterList.Add(CharacterDataModel.CreateNewDataModel(item.Id));
            }
        }

        return model;
    }

    public void ResetPlayerModel()
    {
        OwnCharacterList.Clear();
        OwnGold = 0;
        TotalRunningRecord = 0;

        var list = DataManager.Get().GetCharacterList();

        foreach (var item in list.Values)
        {
            if (true == item.IsRetention)
            {
                OwnCharacterList.Add(CharacterDataModel.CreateNewDataModel(item.Id));
            }
        }
    }

    public void AddCharacter(int id)
    {
        OwnCharacterList.Add(CharacterDataModel.CreateNewDataModel(id));
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

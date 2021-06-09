using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScene : MonoBehaviour
{
    private DataManager _dataManager;
    private ObjectPoolManager _objectPool;
    [SerializeField] private RectTransform _contentTransform;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _maxSpeedText;
    [SerializeField] private Text _minSpeedText;
    private CharacterIcon _selectIcon;
    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        _dataManager = DataManager.Get();
    }

    private void Start()
    {
        LoadCharacterIcon();
    }

    public void LoadCharacterIcon()
    {
        var playerModel = _dataManager.GetPlayerModel();
        var Icon = _objectPool.GetCharcterIcon();
        var Characters = _dataManager.GetCharacterList();

        var Images = _objectPool.GetCharacterIconImageList();

        foreach (var item in Characters)
        {
            CharacterModel model = item.Value;
            CharacterIcon Ci = Instantiate(Icon).GetComponent<CharacterIcon>();
            if(playerModel.OwnCharacterList.Contains(model.Id))
            {
                Ci.ActivateIcon(true);
            }
            Ci.transform.SetParent(_contentTransform);
            Ci.SetCharacterSprite(Images[model.PrefabName]);
            Ci.SetClickAction(() => ClickIconAction(model, Ci));
            Ci.SetCharacter(model);
        }

    }

    public void ClickIconAction(CharacterModel model, CharacterIcon icon)
    {
        if (icon == _selectIcon)
        {
            return;
        }

        if (null == _selectIcon)
        {
            _selectIcon = icon;
        }
        else 
        {
            _selectIcon.ExecuteDeselect();
            _selectIcon = icon;
        }

        _nameText.text = model.NameKR;
        _hpText.text = model.Hp.ToString();
        _maxSpeedText.text = model.MaxSpeed.ToString();
        _minSpeedText.text = model.MinSpeed.ToString();
    }
}

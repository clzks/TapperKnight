using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private LobbyCharacter _character;
    [SerializeField] private CharacterSelect _characterSelect;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        GameManager.Get().SetSceneType(SceneType.Lobby);
    }

    public void SetActiveCharacterSelect()
    {
        _startButton.gameObject.SetActive(false);
        _characterSelect.SetActiveCharacterSelectUI(true);
    }

    public void OnClickBackToLobby()
    {
        _startButton.gameObject.SetActive(true);
        _characterSelect.SetActiveCharacterSelectUI(false);
    }
}

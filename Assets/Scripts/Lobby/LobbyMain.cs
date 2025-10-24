using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class LobbyMain : MonoBehaviour
{
    public CharacterArea characterArea;
    public DescriptionArea descriptionArea;
    public Transform parentTransform;
    
    void Start()
    {
        characterArea.onSelected += CharacterSelected;
    }

    private void CharacterSelected(GameObject obj)
    {
        DataManager.Instance.SelectedCharacter = obj;
        descriptionArea.SetText(DataManager.Instance.selectedCharacterData.name, DataManager.Instance.selectedCharacterData.description);
    }
}
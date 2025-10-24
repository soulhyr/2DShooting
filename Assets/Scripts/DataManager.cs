using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    private GameObject selectedCharacter;

    public GameObject SelectedCharacter
    {
        get { return selectedCharacter; }
        set { selectedCharacter = value; }
    }
    public CharacterSlot selectedCharacterSlot
    {
        get
        {
            if (selectedCharacter == null) return null;
            return selectedCharacter.GetComponent<CharacterSlot>();
        }
    }
    public CharacterData selectedCharacterData
    {
        get
        {
            if (selectedCharacter == null) return null;
            CharacterSlot c = selectedCharacterSlot;
            return c.characterData;
        }
    }

    // public FighterController selectedFighterController { get { return selectedCharacter.GetComponent<FighterController>(); } }
    
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
                instance = new DataManager();
            return instance;
        }
    }

    private DataManager() { }

    public CharacterData[] LoadCahracterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/character_data");
        if (textAsset == null)
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다.");
            return null;
        }
        
        CharacterData[] characterDataList = JsonConvert.DeserializeObject<CharacterData[]>(textAsset.text);
        return characterDataList;
    }
}
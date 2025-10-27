using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LobbyMain : MonoBehaviour, IObjectProvider
{
    
    public ObjectManager objectManager;
    public CharacterArea characterArea;
    public DescriptionArea descriptionArea;
    public TMP_Text infoText;
    public float blinkInterval = 0.2f;

    private GameObject fighter;
    private Coroutine blinkCoroutine;
    private ObjectType selectedCharacterType = ObjectType.None;

    public GameObject MakeObject(ObjectType type, Vector3? pos = null) => objectManager.MakeObject(type, pos);
    public GameObject[] GetPool(ObjectType type) => objectManager.GetPool(type);

    private void Awake()
    {
        Debug.Log("Awake");
        DataManager.Instance.InitializeData();
    }

    void Start()
    {
        objectManager.SetupPlayerSlot();
        characterArea.onSelected += OnCharacterSelected;
        characterArea.Setup(this);
        characterArea.LoadAllCharacterSlot();
    }
    
    private void OnCharacterSelected(CharacterSlot slot)
    {
        if (selectedCharacterType == slot.playerData.objectType)
        {
            StopAllCoroutines();
            Utility.ChangeSceneAsync<GameMain>(GameDef.Scenes.Game, selectedCharacterType);
            return;
        }
        
        selectedCharacterType = slot.playerData.objectType; 
        
        descriptionArea.SetText(slot.playerData.name, slot.playerData.description);
        
        if (fighter != null)
            fighter.SetActive(false);
        fighter = objectManager.MakeObject(slot.playerData.objectType, new Vector3(0, 0, 0));
        Player player = fighter.GetComponent<Player>();
        player.SetupProvider(this);
        player.TestFlight();

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        infoText.gameObject.SetActive(true);
        blinkCoroutine = StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            infoText.enabled = !infoText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
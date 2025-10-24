using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public Image image;
    public TMP_Text nameText;

    public Action onSelected;
    
    public CharacterData characterData;
    
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            image.color = Color.white;
            onSelected();
        });
    }
    
    public void SetData(CharacterData data)
    {
        Sprite newSprite = Resources.Load<Sprite>($"Arts/{data.spriteName}");
        image.sprite = newSprite;
        nameText.text = data.name;
        characterData = data.Clone();
    }

    public void SetNormal()
    {
        image.color = Color.gray;
    }
}
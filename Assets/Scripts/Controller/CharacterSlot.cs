using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public Image image;
    public TMP_Text nameText;

    public Action<CharacterSlot> onSelected;

    [HideInInspector]
    public PlayerData playerData;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => onSelected?.Invoke(this));
    }

    public void SetData(PlayerData data, Sprite sprite)
    {
        playerData = data.Clone();
        image.sprite = sprite;
        nameText.text = playerData.name;
    }

    public void SetNormal() => image.color = new Color(0.2f, 0.2f, 0.2f);
    public void SetSelected() => image.color = Color.white;
}
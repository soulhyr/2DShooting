using TMPro;
using UnityEngine;

public class DescriptionArea : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public void SetText(string nm, string description)
    {
        nameText.text = nm;
        descriptionText.text = description;
    }
}
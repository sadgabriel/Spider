using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


class FacilitySelectionPanel : Panel
{
    [SerializeField] private UnityEngine.UI.Image leftChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI leftChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI leftChoiceDescriptionText;
    [SerializeField] private UnityEngine.UI.Image middleChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI middleChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI middleChoiceDescriptionText;
    [SerializeField] private UnityEngine.UI.Image rightChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI rightChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI rightChoiceDescriptionText;

    public void SetLeftChoice(FacilityData facilityData)
    {
        leftChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        leftChoiceNameText.text = facilityData.facilityName;
        leftChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    public void SetMiddleChoice(FacilityData facilityData)
    {
        middleChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        middleChoiceNameText.text = facilityData.facilityName;
        middleChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    public void SetRightChoice(FacilityData facilityData)
    {
        rightChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        rightChoiceNameText.text = facilityData.facilityName;
        rightChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    public void OnclickLeftChoice()
    {
        // Handle left choice selection
        Debug.Log("Left choice selected");
    }

    public void OnclickMiddleChoice()
    {
        // Handle middle choice selection
        Debug.Log("Middle choice selected");
    }

    public void OnclickRightChoice()
    {
        // Handle right choice selection
        Debug.Log("Right choice selected");
    }
}
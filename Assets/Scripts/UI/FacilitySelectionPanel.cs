using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


class FacilitySelectionPanel : Panel<List<FacilityData>>
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

    private FacilityData leftChoiceData;
    private FacilityData middleChoiceData;
    private FacilityData rightChoiceData;

    public override void Show(List<FacilityData> data = null)
    {
        if (data != null && data.Count >= 3)
        {
            SetLeftChoice(data[0]);
            SetMiddleChoice(data[1]);
            SetRightChoice(data[2]);
        }
        gameObject.SetActive(true);
    }

    private void SetLeftChoice(FacilityData facilityData)
    {
        leftChoiceData = facilityData;
        leftChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        leftChoiceNameText.text = facilityData.facilityName;
        leftChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    private void SetMiddleChoice(FacilityData facilityData)
    {
        middleChoiceData = facilityData;
        middleChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        middleChoiceNameText.text = facilityData.facilityName;
        middleChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    private void SetRightChoice(FacilityData facilityData)
    {
        rightChoiceData = facilityData;
        rightChoiceIcon.sprite = Sprite.Create(facilityData.iconTexture, new Rect(0, 0, facilityData.iconTexture.width, facilityData.iconTexture.height), new Vector2(0.5f, 0.5f));
        rightChoiceNameText.text = facilityData.facilityName;
        rightChoiceDescriptionText.text = facilityData.facilityDescription;
    }

    public void ChooseLeft()
    {
        ProceedToBuildingPanel(leftChoiceData);
    }

    public void ChooseMiddle()
    {
        ProceedToBuildingPanel(middleChoiceData);
    }

    public void ChooseRight()
    {
        ProceedToBuildingPanel(rightChoiceData);
    }

    private void ProceedToBuildingPanel(FacilityData facilityData)
    {
        UIManager.Instance.HideAllPanels();
        UIManager.Instance.ShowPanel(PanelType.FacilityBuilding, facilityData);
    }
}
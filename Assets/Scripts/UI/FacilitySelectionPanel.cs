using System.Collections.Generic;
using UnityEngine;

public class FacilitySelectionPanel : Panel<List<FacilityData>>
{
    private const int NumChoices = 3;

    [SerializeField] private UnityEngine.UI.Image leftChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI leftChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI leftChoiceDescriptionText;

    [SerializeField] private UnityEngine.UI.Image middleChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI middleChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI middleChoiceDescriptionText;

    [SerializeField] private UnityEngine.UI.Image rightChoiceIcon;
    [SerializeField] private TMPro.TextMeshProUGUI rightChoiceNameText;
    [SerializeField] private TMPro.TextMeshProUGUI rightChoiceDescriptionText;

    [SerializeField] private FacilityData dummyFacilityData;

    private FacilityData leftChoiceData;
    private FacilityData middleChoiceData;
    private FacilityData rightChoiceData;

    private List<FacilityData> lastData;

    public override void Show(List<FacilityData> data = null)
    {
        if (data == null) data = lastData;
        
        lastData = data;

        while (data.Count < NumChoices)
        {
            data.Add(dummyFacilityData);
        }

        SetChoice(data[0], out leftChoiceData, leftChoiceIcon, leftChoiceNameText, leftChoiceDescriptionText);
        SetChoice(data[1], out middleChoiceData, middleChoiceIcon, middleChoiceNameText, middleChoiceDescriptionText);
        SetChoice(data[2], out rightChoiceData, rightChoiceIcon, rightChoiceNameText, rightChoiceDescriptionText);

        gameObject.SetActive(true);
    }

    private void SetChoice(
        FacilityData data,
        out FacilityData storage,
        UnityEngine.UI.Image icon,
        TMPro.TextMeshProUGUI nameText,
        TMPro.TextMeshProUGUI descText)
    {
        storage = data;
        icon.sprite = Sprite.Create(data.IconTexture, new Rect(0, 0, data.IconTexture.width, data.IconTexture.height), new Vector2(0.5f, 0.5f));
        nameText.text = data.Name;
        descText.text = data.Description;
    }

    public void ChooseLeft() => ProceedToBuildingPanel(leftChoiceData);
    public void ChooseMiddle() => ProceedToBuildingPanel(middleChoiceData);
    public void ChooseRight() => ProceedToBuildingPanel(rightChoiceData);

    private void ProceedToBuildingPanel(FacilityData facilityData)
    {
        if (facilityData != dummyFacilityData)
        {
            GameStateManager.Instance.SetUiState(UiState.FacilityBuilding, facilityData);
        }
    }
}
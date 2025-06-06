using UnityEngine;

class FacilitySelectionPanel : Panel
{
    [SerializeField] private GameObject LeftChoiceIcon;
    [SerializeField] private GameObject LeftChoiceNameText;
    [SerializeField] private GameObject LeftChoiceDescriptionText;
    [SerializeField] private GameObject MiddleChoiceIcon;
    [SerializeField] private GameObject MiddleChoiceNameText;
    [SerializeField] private GameObject MiddleChoiceDescriptionText;
    [SerializeField] private GameObject RightChoiceIcon;
    [SerializeField] private GameObject RightChoiceNameText;
    [SerializeField] private GameObject RightChoiceDescriptionText;

    public void SetLeftChoice(
        Sprite icon,
        string name,
        string description)
    {
        LeftChoiceIcon.GetComponent<SpriteRenderer>().sprite = icon;
        LeftChoiceNameText.GetComponent<UnityEngine.UI.Text>().text = name;
        LeftChoiceDescriptionText.GetComponent<UnityEngine.UI.Text>().text = description;
    }

    public void SetMiddleChoice(
        Sprite icon,
        string name,
        string description)
    {
        MiddleChoiceIcon.GetComponent<SpriteRenderer>().sprite = icon;
        MiddleChoiceNameText.GetComponent<UnityEngine.UI.Text>().text = name;
        MiddleChoiceDescriptionText.GetComponent<UnityEngine.UI.Text>().text = description;
    }

    public void SetRightChoice(
        Sprite icon,
        string name,
        string description)
    {
        RightChoiceIcon.GetComponent<SpriteRenderer>().sprite = icon;
        RightChoiceNameText.GetComponent<UnityEngine.UI.Text>().text = name;
        RightChoiceDescriptionText.GetComponent<UnityEngine.UI.Text>().text = description;
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
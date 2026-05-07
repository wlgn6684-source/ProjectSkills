using System;
using UnityEngine;

public class UI_TargetHover : OpenableUIBase
{
    [SerializeField] Vector2 shiftedPosition;

    [SerializeField] TMPro.TextMeshProUGUI nameText;
    [SerializeField] UnityEngine.UI.Image skillIcon;

    CharacterBase target;
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseHover += HoverInfoChange;
        //InputManager.OnMouseMove -= MoveToMouse;
        //InputManager.OnMouseMove += MoveToMouse;
    }

    void Update()
    {
        if(target == null) return;
            transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + (Vector3)shiftedPosition;
    }

    public override void UnRegistration(UIManager manager)
    {
        base.UnRegistration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseMove -= MoveToMouse;
    }
    private void HoverInfoChange(GameObject newTarget, GameObject oldTarget)
    {
        CharacterBase asCharacter = newTarget?.GetComponent<CharacterBase>();
        if (asCharacter) 
        {   
            nameText.SetText(newTarget.name);
            //HealthBar.value = asCharacter.GetModule<HitPointModule>().Percent;  
            Open(); 
        }
        else Close();
        target = asCharacter;   
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition + shiftedPosition;
    }   
}

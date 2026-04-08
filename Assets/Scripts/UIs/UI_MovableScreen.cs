using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_MovableScreen : UI_ScreenBase
{   

    [SerializeField] List<UIBase> popupList = new();
    Vector3 popupPosition = Vector3.zero;
    Vector3 popupShift = new(20.0f, -20.0f);

    UI_DraggableWindow currentDragTarget = null;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnCancelButton += (value) => UIManager.ClaimToggleUI(UIType.Menu);
        InputManager.OnMouseMove -= MouseMove;
        InputManager.OnMouseMove += MouseMove;
        InputManager.OnMouseLeftButton -= MouseLeft;
        InputManager.OnMouseLeftButton += MouseLeft;
        UIManager.OnPopUp -= PopUp;
        UIManager.OnPopUp += PopUp;
    }

    public override void UnRegistration(UIManager manager)
    {
        base.UnRegistration(manager);
        InputManager.OnMouseMove -= MouseMove;
        InputManager.OnMouseLeftButton -= MouseLeft;
        UIManager.OnPopUp -= PopUp;
    }
    protected override GameObject OnSetChild(GameObject newChild)
    {

        UIManager.ClaimSetUI(newChild);
        if (newChild)
        { 
            UI_DraggableWindow asDraggable  = newChild.GetComponentInChildren<UI_DraggableWindow>();
            if (asDraggable)
            {
                asDraggable.OnDragStart -= SetDragTarget;
                asDraggable.OnDragStart += SetDragTarget;
            }
        }
        
        return base.OnSetChild(newChild);
    }
    protected override void OnUnsetChild(GameObject oldChild)
    {
        UIManager.ClaimUnsetUI(oldChild);
        if (oldChild)
        { 
            UI_DraggableWindow asDraggable = oldChild.GetComponentInChildren<UI_DraggableWindow>();
            if (asDraggable)
            {
                asDraggable.OnDragStart -= SetDragTarget;
            }
        }
        base.OnUnsetChild(oldChild);
    }

    void SetDragTarget(UI_DraggableWindow dragTarget, Vector2 startPosition)
    {
        currentDragTarget = dragTarget;
        if (currentDragTarget)
        {
            currentDragTarget.SetMouseStartPosition(startPosition);
        }
    }

    void MouseLeft(bool value, Vector2 screenPosition, Vector3 worldPosition)
    { 
        if(!value) currentDragTarget = null;
    }

    void MouseMove(Vector2 screenPositon, Vector3 worldPosition)
    {
        if (currentDragTarget)
        {
            currentDragTarget.SetMouseCurrentPosition(screenPositon);
        }
    }

    

        

    void PopUp(string title, string context, string confirm)
        {
            GameObject newChild = SetChild(ObjectManager.CreateObject("PopUp", transform));
            if (newChild)
            {
                newChild.transform.localPosition = GetNextPopupPosition();

                if (newChild.TryGetComponent(out UIBase newUI))
                {
                    if (!popupList.Contains(newUI)) popupList.Add(newUI);
                }

                if (newChild.TryGetComponent(out ISystemMessagePossible target))
                {
                    target.SetSystemMessage(title, context, confirm);
                }
                if (newChild.TryGetComponent(out IConfirmable confirmTarget))
                {
                    confirmTarget.SetConfirmAction(() =>
                        {
                            if (newUI) popupList.Remove(newUI);
                            UnsetChild(newChild);
                            ObjectManager.DestroyObject(newChild);
                        });
                }



                //newChild.transform.localPosition = bestScore;
                // popupPosition += popupShift;
            }

        }
    

    public Vector3 GetNextPopupPosition()
    {
        Vector3 bestScore = Vector3.zero;
        if(popupList.Count == 0) return bestScore;

        foreach (UIBase currentPopup in popupList)
        {
            Vector3 currentScore = currentPopup.transform.localPosition;
            if (bestScore.x < currentScore.x) bestScore.x = currentScore.x;
            if (bestScore.y > currentScore.y) bestScore.y = currentScore.y;
        }
        return bestScore + popupShift;
    }
}

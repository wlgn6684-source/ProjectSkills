using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
    
public class InteractionModule : CharacterModule
{


    // 현재 바라보거나 범위 안에 있는 상호작용 대상
    protected IInteractable currentInteractable;

    // 대상 변경 이벤트
    public event Action<IInteractable> OnInteractableChanged;

    //--------------------------------------------------------------------
    // 현재 대상 설정
    //--------------------------------------------------------------------

    public virtual void SetInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
            return;

        currentInteractable = interactable;

        OnInteractableChanged?.Invoke(currentInteractable);
    }

    //--------------------------------------------------------------------
    // 현재 대상 가져오기
    //--------------------------------------------------------------------

    public IInteractable GetInteractable()
    {
        return currentInteractable;
    }

    //--------------------------------------------------------------------
    // 상호작용 가능 여부
    //--------------------------------------------------------------------

    public bool CanInteract(CharacterBase character)
    {
        if (currentInteractable == null)
            return false;

        return currentInteractable.IsInteractable(character);
    }

    //--------------------------------------------------------------------
    // 실제 상호작용 실행
    //--------------------------------------------------------------------

    public bool TryInteract()
    {
        if (!CanInteract(Owner))
            return false;

        return currentInteractable.Interact(Owner);
    }

    //--------------------------------------------------------------------
    // UI 표시용 이름
    //--------------------------------------------------------------------

    public string GetInteractName()
    {
        if (currentInteractable == null)
            return string.Empty;

        return currentInteractable.InteractName(Owner);
    }
}

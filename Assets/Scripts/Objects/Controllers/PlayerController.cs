using UnityEngine;

public class PlayerController : ControllerBase
{
    protected override void OnPossess(CharacterBase newCharacter)
    { 
        base.OnPossess(newCharacter);
        InputManager.OnMouseRightButton -= MoveToMousePosition;
        InputManager.OnMouseRightButton += MoveToMousePosition;
    }
    protected override void OnUnPossess(CharacterBase oldCharacter)
    {
        base.OnUnPossess(oldCharacter);
        InputManager.OnMouseRightButton -= MoveToMousePosition;
    }

    public void MoveToMousePosition(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (value) CommandMoveToDestination(worldPosition, 0.0f);
    }
}

 
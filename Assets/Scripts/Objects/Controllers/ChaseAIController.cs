using UnityEngine;

public class ChaseAIController : AIController
{
    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;
    }

    protected override void OnUnPossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;
    }

    protected override void Think(float deltaTime)
    {
        if (!FocusTarget) return;
        CommandMoveToDestination(FocusTarget.transform.position, 1.0f);
        
    }
}

using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;



public enum InteractType
{   
    None,
    Talk, Take, Trade, Move, Work,
    Length
}

public interface IInteractable
{
    // UI 표시용 이름
    public string InteractName(CharacterBase character);

    // 현재 상호작용 가능한 상태인지
    public bool IsInteractable(CharacterBase character)
        => character.GetModule<HitPointModule>() != null;

    // 실제 상호작용 실행
    public bool Interact(CharacterBase character);
}


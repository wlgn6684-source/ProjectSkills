using UnityEngine;

public class InteractionModule : CharacterModule
{
    public interface IInteractable
    {
        public string InteractName(CharacterBase character);
        public bool IsInteractable(CharacterBase character) => character.GetModule<HitPointModule>();
        public bool Interact(CharacterBase character);
    }
}

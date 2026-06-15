using UnityEngine;

//[CreateAssetMenu(fileName = "InfoSkill", menuName = "Scriptable Objects/InfoSkill")]
public abstract class InfoSkill : ScriptableObject
{
    public Sprite icon;
    public string displayName;
    public string explain;
}

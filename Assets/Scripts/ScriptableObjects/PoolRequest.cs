using UnityEngine;



[System.Serializable]

//없을 수 없으면 struct

public struct PoolSetting
{
    public string poolName;
    public GameObject target;
    public uint countInitial;
    public uint countAdditional;
}

//                           기본파일명                              파일 위치 경로
[CreateAssetMenu(fileName = "PoolRequest", menuName = "PoolReQuests/DefaultPoolRequest")]
public class PoolRequest : ScriptableObject
{
    public PoolSetting[] settings;
}

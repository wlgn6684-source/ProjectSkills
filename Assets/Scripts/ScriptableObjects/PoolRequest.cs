using UnityEngine;

//                           기본파일명                              파일 위치 경로
[CreateAssetMenu(fileName = "PoolRequest", menuName = "PoolReQuests/DefaultPoolRequest")]
public class PoolRequest : ScriptableObject
{
    public PoolSetting[] settings;
}

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]

//없을 수 없으면 struct
public struct PoolSetting
{
    public string poolName;
    public GameObject target;
    public int countInitial;
    public int countAdditional;
}



public class ObjectManager : ManagerBase
{
    //시리얼 라이즈 가능한 => 유니티에서 보기 위해서 쓴 것
    //직렬화 변수
    [SerializeField] PoolSetting[] testSettings;
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        yield return null;
    }

    protected override void OnDisconnected()
    {
    
    }

    //오브젝트 풀링
    //만드는 과정을 줄이고 싶음 로딩중에 하고 싶다.

    public static GameObject CreateObject(GameObject prefab, Transform parent = null)
    {

        if (prefab == null) return null;
        GameObject result = Instantiate(prefab, parent);
        RegistrationObject(result);
        return result;
    }

    public static GameObject CreateObject(GameObject prefab, Vector3 position)
    {

        GameObject result = CreateObject(prefab);
        if (result) result.transform.position = position;
        return result;
    }

    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;

        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;

        }
        return result;
    }

    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Space space = Space.Self)
    {

        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position;
                    break;

                case Space.Self:
                    result.transform.localPosition = position; 
                    break;

            }
            
        }
        return result;
    }

    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position;
                    result.transform.rotation = rotation;
                    break;

                case Space.Self:
                    result.transform.localPosition = position;
                    result.transform.localRotation = rotation;
                    break;

            }
            

        }
        return result;
    }
    
    
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position;
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale;
                    break;
                    //float scaledScaleX = scale.x * (result.transform.localScale.x / result.transform.lossyScale.x);
                    //float scaledScaleY = scale.y * (result.transform.localScale.y / result.transform.lossyScale.y);
                    //float scaledScaleZ = scale.z * (result.transform.localScale.z / result.transform.lossyScale.z);
                    //break;

                case Space.Self:
                    result.transform.localPosition = position;
                    result.transform.localRotation = rotation;
                    result.transform.localScale = scale;
                    break;

            }
            

        }
        return result;
    }

    

    public static void RegistrationObject(GameObject target)
    {
        if (target)
        {
            foreach (var current in target.GetComponentsInChildren<IFunctionable>())
            {
                current.RegistrationFunctions();
            }
        }
    }

    public static void DestroyObject(GameObject target)
    {
        if (!target) return;
        UnregistrationObject(target);
        Destroy(target);
    }

    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;
        foreach (var current in target.GetComponentsInChildren<IFunctionable>())
        {
            current.UnregistrationFunctions();
        }
    }
}

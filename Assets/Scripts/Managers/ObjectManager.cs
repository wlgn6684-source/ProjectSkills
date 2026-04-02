using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class ObjectManager : ManagerBase
{   
    //변수가 아니라 상수인 셈
    readonly string[] globalPoolSettings =
    {
        "GlobalCharacterPool",
        "GlobalControllerPool",
        "GlobalEffectPool",
        "GlobalObjectPool",
        "GlobalUIPool"
    };
    //시리얼 라이즈 가능한 => 유니티에서 보기 위해서 쓴 것
    //직렬화 변수
    //[SerializeField] PoolSetting[] testSettings;

    List<PoolRequest> loadedPoolRequests = new();

    static Dictionary<string, ObjectPoolModule> poolDictionary = new();

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        RegistrationPool(globalPoolSettings);
        InitializePool();

        yield return null;
    }

    protected override void OnDisconnected()
    {
    
    }
    //오브젝트 풀링
    //만드는 과정을 줄이고 싶음 로딩중에 하고 싶다.

    public static GameObject CreateObject(string wantName, Transform parent = null)
    {
        GameObject result =null;

        wantName = wantName.ToLower();

        if (poolDictionary.TryGetValue(wantName, out ObjectPoolModule pool))
        {
            result = pool.CreateObject(parent);
        }

        else 
        {
            //데이터에 있는지 확인
            if (DataManager.TryLoadDataFile<GameObject>(wantName, out GameObject prefab))
            {
                if (prefab) result = Instantiate(prefab, parent);
            }
            
        }

        if (!result) UIManager.ClaimErrorMessage(SystemMessage.ObjectNameNotFound(wantName));

        RegistrationObject(result);//둘중에 하나라도 하고 없으면 말고!
        return result;

    }
    public static GameObject CreateObject(GameObject prefab, Transform parent = null)
    {

        if (prefab == null) return null;
        GameObject result = Instantiate(prefab, parent);
        RegistrationObject(result);
        return result;
    }


    public static GameObject CreateObject(string wantName, Vector3 position)
    {

        GameObject result = CreateObject(wantName);
        if (result) result.transform.position = position;
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position)
    {

        GameObject result = CreateObject(prefab);
        if (result) result.transform.position = position;
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;

        }
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
    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;

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

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Space space = Space.Self)
    {

        GameObject result = CreateObject(wantName, parent);
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

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
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
    
    
    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position;
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale;
                    break;
                case Space.Self:
                    result.transform.localPosition = position;
                    result.transform.localRotation = rotation;
                    result.transform.localScale = scale;
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
                current?.RegistrationFunctions();
            }
        }
    }

    public static void DestroyObject(GameObject target)
    {
        if (!target) return;
        UnregistrationObject(target);
        if (target.TryGetComponent(out PooledObject pool))
        {
            pool.OnEnqueue();
        }
        else
        {
            Destroy(target);
        }
        
    }

    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;
        foreach (var current in target.GetComponentsInChildren<IFunctionable>())
        {
            current.UnregistrationFunctions();
        }
    }

    public void RegistrationPool(string poolName)
    {   
        poolName = poolName.ToLower();
        PoolRequest currentRequest = DataManager.LoadDataFile<PoolRequest>(poolName);
        if (currentRequest == null) return;
        if (currentRequest.settings == null) return;
        loadedPoolRequests.Add(currentRequest);
        foreach (PoolSetting currentSetting in currentRequest.settings)
        {
            string currentName = currentSetting.poolName.ToLower();
            GameObject currentPrefab = currentSetting.target;
            if (currentPrefab == null) continue;
            if (poolDictionary.ContainsKey(currentName)) continue;

            poolDictionary.Add(currentName, new(currentSetting));
        }
    }

    //가변인자 => 인자의 개수가 무한정 늘어날 수 있는 함수
    //변인 => Parameter 변인들 Parameters 줄이면 params
    
    public void RegistrationPool(params string[] poolNames)
    {
        foreach (string poolName in poolNames)
        { 
            RegistrationPool(poolName);
        }
    }

    public void InitializePool()
    {
        foreach (ObjectPoolModule currentPool in poolDictionary.Values)
        {
            currentPool?.Initialize();
        }
    }
}

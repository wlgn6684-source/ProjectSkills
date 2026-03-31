using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ObjectPoolModule
{
    PoolSetting _setting;
    public PoolSetting Setting => _setting;

    Transform rootTransform;

    Queue<GameObject> prepareQueue = new();

    List<GameObject> inProgressList = new();

    public ObjectPoolModule(PoolSetting newSetting)
    { 
        _setting = newSetting;
        
    }
    public void Initialize()
    {
        rootTransform = new GameObject(Setting.poolName).transform;
        Setting.target?.TryAddComponent<PooledObject>();
        PrepareObjects(Setting.countInitial);

    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        EnqueueObject(result);
        return result;
    }
    
    //uint 마이너스가 존재하면 안됨
    //unsigned 부호없는
    void PrepareObjects(uint count)
    {
        if (!Setting.target) return;
        for (int i = 0; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);

        }
    }

    void PrepareObjects(uint count, out GameObject activeObject)
    {
        if (!Setting.target) 
        {   
            activeObject = null;
            return;
        }

        activeObject = CreateFromPrefab();

        for (int i = 1; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    public GameObject CreateFromPrefab()
    {
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        if (result)
        { 
            result.name = Setting.poolName;
            if (result.TryGetComponent(out PooledObject pool ))
            {
                pool.OnEnqueueEvent -= DestroyObject;
                pool.OnEnqueueEvent += DestroyObject;
            }
        }
        return result;
    }
    public GameObject CreateObject(Transform parent = null)
    {
        GameObject result;
        if (!prepareQueue.TryDequeue(out result))
        {
            PrepareObjects(Setting.countAdditional, out result);
        }

        if (result)
        {
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnDequeue();
            }
            result.transform.SetParent(parent);
            result.SetActive(true);
        }

        return result;
    }

    public void DestroyObject(GameObject target)
    {
        EnqueueObject(target);
        if (target)
        { 
            target.transform.SetParent(rootTransform);
        }
    }

    public void EnqueueObject(GameObject target)
    {
        if (!target) return;
        {
            target.SetActive(false);

            target.name = Setting.poolName;
            prepareQueue.Enqueue(target);

        }
    }
}
 
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
        for (int i = 0; i < _setting.countInitial; i++)
        {
            PrepareObject(); 
        }

    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        if (result)
        { 
            result.SetActive(false);

            result.name = Setting.poolName;
            prepareQueue.Enqueue(result);

        }
        return result;
    }
    public GameObject CreateObject()
    {
        GameObject result;
        if (!prepareQueue.TryDequeue(out result))
        {
            PrepareObject();
        }

        return result;
    }

    public void DestroyObject(GameObject target)
    { 
        
    }
}
 
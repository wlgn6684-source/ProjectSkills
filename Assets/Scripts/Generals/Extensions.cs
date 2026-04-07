using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics.Geometry;
using UnityEngine;

//확장 메소드들을 가지고 있을 친구들
//영토 확장
//확장판
//원본에 없던 개념을 추가
//내가 원하는 클래스에 내가 원하는 기능을 추가해줄 수 있다.

// ex) Normalize는 본인을 1로 바꾸는것
//     normalized는 크기가 1인 값을 돌려주는 것
//     float를 정규화하면 어떤 모양이 될까?
//      13.0f => 1.0f
//      -2.0f => -1.0f
//      0.0f => 0.0f
//      float => float
public static class Extensions
{
    public static float normalized(this float target)
    {
        if (target > 0)      return 1.0f;
        else if (target < 0) return -1.0f;
        else                 return 0.0f;
    }

    // Try Add Component => 추가를 시도 => 있는지 확인 => 없으면 추가
    public static T TryAddComponent<T>(this GameObject target) where T : Component
    { 
        T result = null;

        if (target == null) return result; //RVO

        result = target.GetComponent<T>() ?? target.AddComponent<T>();
        //if (result is null) result = target.AddComponent<T>();
        //result ??= target.AddComponent<T>();

        return result;
    }

    public static T TryAddComponent<T>(this Component target) where T : Component
    {
        if (target == null) return null;
        else                return target.gameObject.TryAddComponent<T>(); //NRVO
    }

    public static IEnumerator WaitforTask(this Task targetTask)
    {
        yield return new WaitUntil(() => targetTask.IsCompleted);
        targetTask.Dispose();
    }

    public static float GetPenetratedDistance(float aHalf, float bHalf, float aPos, float bPos)
    {
       
        
            float absAHalf = Mathf.Abs(aHalf);
            float absBHalf = Mathf.Abs(bHalf);

            float minSpace = absAHalf + absBHalf;
            float distance = aPos - bPos;
            float penetration = minSpace - Mathf.Abs(distance);

            penetration *= Mathf.Sign(distance);
            return penetration;

        
    }

    public static Vector2 AABB(this Rect A, Rect B)
    {   
        Vector2 result = Vector2.zero;
        Vector2 aMin = A.min;
        Vector2 aMax = A.max;
        Vector2 aHalf = A.size * .5f;
        Vector2 bMin = B.min;
        Vector2 bMax = B.max;
        Vector2 bHalf = B.size * .5f;

        if (aMax.x > bMin.x && bMax.x > aMin.x)
        {
            result.x =  GetPenetratedDistance(aHalf.x, bHalf.x, A.position.x, B.position.x);
        }
        
        if (aMax.y > bMin.y && bMax.y > aMin.y)
        {
            result.y =  GetPenetratedDistance(aHalf.y, bHalf.y, A.position.y, B.position.y);
        }

        return result;
    }

    public static float GetOutBoundDistance(float inMin, float outMin, float inMax, float outMax)
    { 
        float result = 0.0f;

        bool leftOut = inMin < outMin;
        bool rightOut = inMax > outMax;

        if (leftOut ^ rightOut)
        { 
            if(leftOut) result = outMin - inMin; 
            if(rightOut) result = outMax - inMax;
        }
        
        return result;
    }
    public static Vector2 InversedAABB(this Rect target, Rect bound)
    {
        Vector2 result;
        result.x = GetOutBoundDistance(target.xMin, bound.xMin, target.xMax, bound.xMax);
        result.y = GetOutBoundDistance(target.yMin, bound.yMin, target.yMax, bound.yMax);
        return result;
    }
}

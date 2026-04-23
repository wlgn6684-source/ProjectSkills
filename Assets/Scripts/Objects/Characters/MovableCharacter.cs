using System;
using UnityEngine;


public class MovableCharacter : CharacterBase, IRunnable, IFunctionable
{
    [SerializeField] Animator anim;
    

    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;



    public void RegistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;
    }
    public void UnregistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= MovementUpdate;
    }

    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position;
        PhysicsUpdate(deltaTime);
        Vector3 positionDelta = transform.position - originPosition;
        //if(positionDelta.sqrMagnitude == 0) 
        AnimationUpdate(positionDelta);
    }

    public void AnimationUpdate(Vector3 moveDelta)
    {
        if (!anim) return;
        anim.SetFloat("MoveX", LookRotation.x);
        anim.SetFloat("MoveY", LookRotation.y);
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }

    public void PhysicsUpdate(float deltaTime)
    {
        UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
    }

    public virtual float GetMoveSpeed() => 5.0f;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;
    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
        _lookRotation = delta.normalized;
    }

    public void UpdateToDirection(float deltaTime)
    {
        if (targetDirection is null) return;
        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);

    }
    public void UpdateToDestination(float deltaTime)
    {
        if (targetDestination is null) return;
        Vector3 currentMoveDirection = (targetDestination.Value - transform.position);
        float distance = currentMoveDirection.magnitude;
        if (distance > targetTolerance)
        {
            currentMoveDirection.Normalize();
            float currentMoveSpeed = GetMoveSpeed(deltaTime);
            float resultMoveSpeed = Mathf.Min(currentMoveSpeed, distance);
            Translate(resultMoveSpeed * currentMoveDirection);
        }
    }

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null;
        targetDestination = destination;
        targetTolerance = tolerance;
    }

    public void MoveToDirection(Vector3 direction)
    {
        targetDestination = null;
        targetDirection = direction.normalized;
    }


    public void StopMovement()
    {
        targetDestination = null;
        targetDirection = null;
    }
    
    

}


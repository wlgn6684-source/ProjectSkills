using UnityEngine;


public class MovableCharacter : CharacterBase, IRunnable, IFunctionable
{
    protected Vector3 targetDestination;
    protected float targetTolerance;

    void Start()
    {
        RegistrationFunctions();
    }

    public void RegistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= PhysicsUpdate;
        GameManager.OnPhysicsCharacter += PhysicsUpdate;
    }
    public void UnregistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= PhysicsUpdate;    
    }

    public void PhysicsUpdate(float deltaTime)
    {
        Vector3 currentMoveDirection = (targetDestination - transform.position);
        float distance = currentMoveDirection.magnitude;
        if (distance > targetTolerance)
        {
            currentMoveDirection.Normalize();
            transform.position +=  deltaTime * 5.0f * currentMoveDirection;
        }
    }

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDestination = destination;
        targetTolerance = tolerance;
    }

    public void MoveToDirection(Vector3 direction)
    {
     
    }


    public void StopMovement()
    {
     
    }

}


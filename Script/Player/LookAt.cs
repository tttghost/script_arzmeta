using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public FacingDirection facingDirection;
    private void Start()
    {
        
    }

    private void Update()
    {
        transform.localRotation = LookAt2D(transform.position, target.position, facingDirection);
    }

    public enum FacingDirection
    {
        UP = 90,
        DOWN = 270,
        LEFT = 0,
        RIGHT = 180,
    }
    public Quaternion LookAt2D(Vector2 startingPosition, Vector2 targetPosition, FacingDirection facing)
    {
        Vector2 direction = targetPosition - startingPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= (float)facing;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

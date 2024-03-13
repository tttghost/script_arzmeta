using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    private CharacterController controller;
    private Vector3 direction;
    private float moveSpeed = 6f;
    private float gravity = 15f;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        controller = GetComponent<CharacterController>();
        direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        rotate();
    }

    private void move()
    {
        direction = new Vector3(inputManager.vertical, 0, -inputManager.horizontal);
        direction = direction * moveSpeed * Time.deltaTime;
        direction.y = direction.y - gravity * Time.deltaTime;
        controller.Move(direction);
    }

    private void rotate()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.x = mousePosition.x / Screen.width;
        mousePosition.y = mousePosition.y / Screen.height;
        Vector3 middleToMousePosition = mousePosition - new Vector3(1 / 2f, 1 / 2f, 0);
        float angle = Vector2.SignedAngle(new Vector2(middleToMousePosition.x, middleToMousePosition.y), new Vector2(-1, 0));
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}

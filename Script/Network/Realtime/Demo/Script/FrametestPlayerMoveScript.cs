using UnityEngine;

public class FrametestPlayerMoveScript : MonoBehaviour
{
    private CharacterController cc;
    private float hAxis = 0;
    private float vAxis = 0;
    private Vector3 InputDir;
    private float moveSpeed = 5;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (cc.isGrounded)
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");

            InputDir = new Vector3(-vAxis, 0, hAxis).normalized;
            InputDir *= moveSpeed;

            if (Input.GetButton("Jump"))
            {
                InputDir.y = 5;
            }
        }

        InputDir.y -= 20 * Time.deltaTime;
        
        cc.Move(InputDir * Time.deltaTime);
    }
}

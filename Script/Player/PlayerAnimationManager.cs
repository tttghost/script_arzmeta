using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("moving", (Mathf.Abs(inputManager.vertical) + Mathf.Abs(inputManager.horizontal)));
    }
}

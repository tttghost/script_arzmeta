using UnityEngine;

[System.Serializable]
public class TPSAnimation
{
	[HideInInspector] public bool useAnimation;
	[HideInInspector] public float blend;

	[HideInInspector] public int movement;
	[HideInInspector] public int grounded;
	[HideInInspector] public int jump;
	[HideInInspector] public int dash;
	
	public float Blend { get => blend; set => blend = value; }

	public void Start()
	{
		useAnimation = true;

		movement = Animator.StringToHash(Cons.Movement);
		grounded = Animator.StringToHash(Cons.Grounded);
		jump = Animator.StringToHash(Cons.Jump);
		dash = Animator.StringToHash(Cons.Dash);
	}
}

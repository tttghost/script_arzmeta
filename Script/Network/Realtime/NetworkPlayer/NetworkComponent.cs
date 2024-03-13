using UnityEngine;

namespace FrameWork.Network
{
	public class NetworkComponent : MonoBehaviour
	{
		[Header("Network")]
		public string clientId;
		[HideInInspector] public string memberId;
		public int objectId;

		[Header("Network Interval")]
		[SerializeField] protected float targetFrame = 60f;
		[SerializeField] protected float interval = 0.1f;
		protected float lerpAmount = 0.1f;
		protected int currentStep = 0;
		protected int totalStep = 6;

		public bool isMine;

		protected virtual void Awake()
		{
			interval = .1f;
			lerpAmount = .1f;
			currentStep = 0;
			totalStep = (int)(targetFrame * interval);
		}

		protected virtual void Start() { }

		protected virtual void OnDestroy() { }

		protected virtual void Dispose() { }

		internal void SetNetworkId(string _memberCode, int _objectId)
		{
			this.clientId = _memberCode;
			this.memberId = _memberCode.Split('_')[0];
			this.objectId = _objectId;
		}
	}
}
using System;
using System.Collections.Generic;
using Google.Protobuf;
using MEC;
using Protocol;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace FrameWork.Network
{
	public class NetworkTransform : NetworkComponent
	{
		#region Members

		CharacterController characterController;

		float lerpSpeed = 5f;

		Vector3 position;
		Quaternion rotation;

		CoroutineHandle handle_update;

		#endregion



		#region Initialize

		protected override void OnDestroy()
		{
			base.OnDestroy();

			RemoveHandler();

			Timing.KillCoroutines(handle_update);
		}

		protected override void Awake()
		{
			base.Awake();

			characterController = GetComponent<CharacterController>();
		}

		protected override void Start()
		{
			base.Start();

			AddHandler();

			position = this.transform.position;
			rotation = this.transform.rotation;

			handle_update = Timing.RunCoroutine(Co_Update());
		}

		private void Update()
		{
			if (isMine) return;

			if (recieved)
			{
				var lerpPosition = Vector3.Lerp(this.transform.position, position, lerpSpeed * Time.deltaTime);
				var lerpRotation = Quaternion.Lerp(this.transform.rotation, rotation, lerpSpeed * Time.deltaTime);

				Vector3 moveDir = (position - this.transform.position).normalized;
				float moveDistance = Vector3.Distance(position, this.transform.position);
				float moveSpeed = moveDistance / (lerpSpeed * Time.deltaTime);

				this.transform.position = lerpPosition;
				this.transform.rotation = lerpRotation;

				if (Equals(this.transform.position, position))
				{
					recieved = false;
				}
			}
		}

		private IEnumerator<float> Co_Update()
		{
			if (!isMine) yield break;

			while (true)
			{
				var position = this.transform.position;

				yield return Timing.WaitForSeconds(.1f);

				if (!Equals(this.transform.position, position))
				{
					C_BASE_SET_TRANSFORM();
				}
			}
		}

		private void AddHandler()
		{
			if (isMine || Single.RealTime == null) return;

			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_TRANSFORM, this, S_BASE_SET_TRANSFORM);
		}

		public void RemoveHandler()
		{
			if (isMine || Single.RealTime == null) return;

			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_TRANSFORM, this);
		}

		#endregion



		#region Send

		private void C_BASE_SET_TRANSFORM()
		{
			var packet = new C_BASE_SET_TRANSFORM();

			packet.Position = NetworkUtils.UnityVector3ToProtocolVector3(this.transform.position);
			packet.Rotation = NetworkUtils.UnityVector3ToProtocolVector3(this.transform.eulerAngles);
			packet.ObjectId = objectId;

			Single.RealTime.Send(packet);
		}

		public void SetTransfrom() => C_BASE_SET_TRANSFORM();

		#endregion



		#region Recieve

		private void S_BASE_SET_TRANSFORM(PacketSession session, IMessage _packet)
		{
			var packet = _packet as S_BASE_SET_TRANSFORM;

			if (objectId != packet.ObjectId) return;

			position = NetworkUtils.ProtocolVector3ToUnityVector3(packet.Position);
			rotation = NetworkUtils.ProtocolVector3ToUnityQuaternion(packet.Rotation);

			recieved = true;
		}

		bool recieved = false;

		#endregion


		public void KillUpdate()
		{
			Timing.KillCoroutines(handle_update);
		}
	}

	struct PlayerTransform
	{
		public Vector3 position;
		public Quaternion rotation;
	}
}
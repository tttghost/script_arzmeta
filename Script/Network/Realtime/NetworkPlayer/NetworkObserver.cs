using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using MEC;
using Protocol;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace FrameWork.Network
{
	public class NetworkObserver : NetworkComponent
	{
		#region Members

		[SerializeField] List<NetworkComponent> networkComponents = new List<NetworkComponent>();

		AvatarPartsController avatarPartsController;
		Outline outline;

		public bool isDestroyInstant = true;

		public AgoraUser agorauser { get; private set; }

		#endregion



		#region Initialize

		protected override void OnDestroy()
		{
			base.OnDestroy();

			RemoveHandler();

			Dispose();
		}

		protected override void Awake()
		{
			base.Awake();

			avatarPartsController = GetComponent<AvatarPartsController>();
			outline = GetComponentInChildren<Outline>();
			agorauser = GetComponentInChildren<AgoraUser>(true);
		}

		protected override void Start()
		{
			base.Start();

			AddHandler();

			Find();
		}

		private void AddHandler()
		{
			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_REMOVE_OBJECT, this, S_BASE_REMOVE_OBEJCT);
			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA_NOTICE, this, S_BASE_OBJECT_DATA_NOTICE);
			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA, this, S_BASE_SET_OBJECT_DATA);
			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME, this, S_SET_NICKNAME);
			RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this, S_SET_NICKNAME_NOTICE);
		}

		public void RemoveHandler()
		{
			if (!Single.RealTime) return;

			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_REMOVE_OBJECT, this);
			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA_NOTICE, this);
			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA, this);
			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME, this);
			RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this);
		}

		#endregion



		#region Receive

		private void S_BASE_SET_OBJECT_DATA(PacketSession _session, IMessage _packet)
		{
			S_BASE_SET_OBJECT_DATA packet = _packet as S_BASE_SET_OBJECT_DATA;

			if (packet.Success)
			{
				DEBUG.LOG("Avatar has successfully changed..!");
			}
		}

		private void S_BASE_OBJECT_DATA_NOTICE(PacketSession _session, IMessage _packet)
		{
			S_BASE_SET_OBJECT_DATA_NOTICE packet = _packet as S_BASE_SET_OBJECT_DATA_NOTICE;

			if (packet.ObjectId != objectId) return;

			avatarPartsController.SetAvatarParts(packet.ObjectData,
			() =>
			{
				if (outline) outline.Initialize();

                if (ArzMetaManager.Instance
                    && ArzMetaManager.Instance.PhoneController.isPhone)
                {
					MyPlayer.instance.SetMyPlayerVisible(false);
                }
			});
		}



		private void S_BASE_REMOVE_OBEJCT(PacketSession _session, IMessage _packet)
		{
			S_BASE_REMOVE_OBJECT packet = _packet as S_BASE_REMOVE_OBJECT;

			for (var i = 0; i < packet.GameObjects.Count; i++)
			{
				if (packet.GameObjects[i] != objectId) continue;

				var networkHandler = FindObjectOfType<NetworkHandler>();

				Remove(this);
				Remove(this.GetComponent<NetworkAnimator>());
				Remove(this.GetComponent<NetworkTransform>());

				Timing.RunCoroutine(Co_DestroyPlayerObject());
			}
		}

		IEnumerator<float> Co_DestroyPlayerObject()
		{
			if (isDestroyInstant) Destroy(this.gameObject);

			else
			{
				Single.Scene.FadeOut();

				yield return Timing.WaitUntilTrue(() => Single.Scene.fadeOut);

				Destroy(this.gameObject);
			}

			Util.Pool(Define.EF_Puff, this.transform.position + Vector3.up, this.transform.rotation);
		}



		private void S_SET_NICKNAME(PacketSession _session, IMessage _packet)
		{
			S_SET_NICKNAME packet = _packet as S_SET_NICKNAME;

			if (packet.Success)
			{
				DEBUG.LOG("Nickname has successfully changed..!");
			}
		}

		private void S_SET_NICKNAME_NOTICE(PacketSession _session, IMessage _packet)
		{
			S_SET_NICKNAME_NOTICE packet = _packet as S_SET_NICKNAME_NOTICE;

			if (packet.ClientId == clientId)
            {
                this.GetComponentInChildren<HUDParent>().SetNickName(packet.Nickname);

                var networkHandler = FindObjectOfType<NetworkHandler>();
                if (isMine)
                {
                    var clientData = networkHandler.Clients[clientId];
                    var nickname = clientData.nickname;

                    clientData.nickname = packet.Nickname;

                    networkHandler.Nicknames.Remove(nickname);
                    networkHandler.Nicknames.Add(packet.Nickname, clientData);

					MyPlayer.instance.changeNickName?.Invoke();
                    DEBUG.LOG($"{nickname} has changed nickname to {packet.Nickname}");
                }

                networkHandler.Clients[packet.ClientId].nickname = packet.Nickname;
			}
		}

		#endregion



		#region Core Methods

		public void Find()
		{
			networkComponents.Clear();

			networkComponents = this.transform.GetComponentsInChildren<NetworkComponent>().Where(component => component != this).ToList();

			isMine = (clientId == LocalPlayerData.MemberCode);
		}

		protected internal void Add(NetworkComponent _networkComponent)
		{
			if (networkComponents.Contains(_networkComponent)) return;

			networkComponents.Add(_networkComponent);
		}

		private void Remove(NetworkComponent _networkComponent)
		{
			if (!networkComponents.Contains(_networkComponent)) return;

			networkComponents.Remove(_networkComponent);
		}

		internal void SetClientId(string _memberCode, int objectId)
		{
			Find();

			SetNetworkId(_memberCode, objectId);

			isMine = (_memberCode == LocalPlayerData.MemberCode);

			foreach (var element in networkComponents)
			{
				element.SetNetworkId(_memberCode, objectId);
				element.isMine = isMine;
			}
		}

		protected override void Dispose()
		{
			base.Dispose();

			if (ObjectPooler.instance == null) return;

			var hand = this.transform.Search(Cons.RIGHTHAND);

			if (hand != null)
			{
				var repoolObject = hand.GetComponentInChildren<RePoolObject>();

				if (repoolObject != null)
				{
					repoolObject.Repool();
				}
			}
		}

		#endregion
	}
}
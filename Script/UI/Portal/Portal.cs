using UnityEngine;
using System.Collections.Generic;
using FrameWork.UI;

public abstract class Portal : MonoBehaviour
{
	public RoomType roomType;       // 이동할 씬
    public ScenePortal scenePortal; // 포탈 웨이포인트 일치시키면 해당위치로 스폰

    protected InteractionArea interactionArea;
	protected virtual void Start()
	{
		interactionArea = GetComponent<InteractionArea>();
		interactionArea._ontriggerEnter.AddListener((other) => Warp());
    }
    private void Warp()
	{
        SceneLogic.instance.spawnType = SceneLogic.SpawnType.WayPoint;
        LocalContentsData.scenePortal = scenePortal;

		if (roomType == RoomType.None)
        {
			roomType = LocalContentsData.isMainLand ? RoomType.Arz : RoomType.Busan;
        }

        Single.RealTime.EnterRoom(roomType);
	}
}

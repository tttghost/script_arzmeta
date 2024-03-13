using UnityEngine;

public class Portal_GameZone : Portal
{
	//protected override void Start()
	//{
	//	base.Start();

	//	spawnPoint = IsFirstFloor() ? SpawnPoint.First : SpawnPoint.Second;

	//	if(LocalContentsData.isMainLand)
	//	{
	//		position.Add(spawnPoint, new Vector3(-7.86f, -0.01f, -11.97f));
	//		rotation.Add(spawnPoint, new Vector3(0f, 40f, 0f));
	//	}

	//	else
	//	{
	//		position.Add(spawnPoint, new Vector3(-10.50f, 0.02f, -35.06f));
	//		rotation.Add(spawnPoint, new Vector3(0f, 20f, 0f));
	//	}
	//}

	//protected override void Warp()
	//{
	//	base.Warp();

	//	if(isWorld)
	//	{
	//		LocalPlayerData.GameSpawnFloor = spawnPoint.ToString();

	//		Single.RealTime.EnterRoom(RoomType.Game);
	//	}
	//}

	//private bool IsFirstFloor()
	//{
	//	return this.gameObject.name == "Portal_GameZone_1floor";
	//}
}
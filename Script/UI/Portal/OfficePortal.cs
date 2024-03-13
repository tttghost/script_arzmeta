// using FrameWork.UI;
// using UnityEngine;
//
// // 사용 X 
// public class OfficePortal : BasePortal
// {
//     private Panel_OfficeRoom _panelWorldOfficeRoom = null;
//
// #if UNITY_EDITOR
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.F7))
//         {
//             Move();
//         }
//     }
// #endif
//
//     protected override void Start()
//     {
//         base.Start();
//
//         _panelWorldOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(Cons.Panel_OfficeRoom);
//     }
//     
//     /// <summary>
//     /// 포탈 클릭 or 부딫혔을 때 활성화
//     /// </summary>
//     protected override void Move()
//     {
//         SceneLogic.instance.PushPanel(_panelWorldOfficeRoom);
//         SceneLogic.instance.SetCurOpenStartCallback(()=> _panelWorldOfficeRoom.PanelInit());
//     }
//
//
// }
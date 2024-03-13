
/****************************************************************************************************
 * 
 *					PopupBase.cs
 * 
 ****************************************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using MEC;
using System;

namespace FrameWork.UI
{


    public class PopupBase : UIBase
    {
        #region 변수
        protected Animator popupAnimator;
        protected Button btn_PopupExit;
        private string popupOpen = "PopupOpen";
        private string popupClose = "PopupClose";
        #endregion

        protected override void SetMemberUI()
        {
            btn_PopupExit = GetUI_Button(nameof(btn_PopupExit), Back);
            
            popupAnimator = GetComponent<Animator>();
            if (popupAnimator == null)
            {
                // 애니메이터가 Null 일 경우, 추가하고 팝업 기본 컨트롤러를 연결해 준다.
                popupAnimator = gameObject.AddComponent<Animator>();
                popupAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("BuildAssetBundle/UI/Animation/PopupCommon"); //김지수
            }
        }

        public override void Back(int cnt = 1)
        {
            if (BackAction_Custom != null)
            {
                BackAction_Custom.Invoke();
                BackAction_Custom = null;
            }
            else
            {
                SceneLogic.instance.PopPopup(); //팝업 닫아줌
            }

            cnt--;
            if(cnt > 0)
            {
                SceneLogic.instance.Back(cnt);
            }
        }


        #region Open & Close Coroutine
        public IEnumerator<float> Co_OpenPopup()
        {
            _myGameObject.SetActive(true);
            transform.SetAsLastSibling();

            SceneLogic.instance.OnCurOpenStartAct();
            OpenStartAct();
            yield return Co_OpenStartAct().WaitUntilDone();

            popupAnimator.enabled = true;
            popupAnimator.Play(popupOpen);
            yield return Timing.WaitForSeconds(.55f);
            popupAnimator.enabled = false;

            SceneLogic.instance.OnCurOpenEndAct();
            OpenEndAct();
            yield return Co_OpenEndAct().WaitUntilDone();
        }

        public IEnumerator<float> Co_ClosePopup()
        {
            PopupAction();

            SceneLogic.instance.OnCurCloseStartAct();
            CloseStartAct();
            yield return Co_SetCloseStartAct().WaitUntilDone();
            
            popupAnimator.enabled = true;
            popupAnimator.Play(popupClose);
            yield return Timing.WaitForSeconds(.375f);
            popupAnimator.enabled = false;

            SceneLogic.instance.OnCurCloseEndAct();
            CloseEndAct();
            yield return Co_SetCloseEndAct().WaitUntilDone();

            gameObject.SetActive(false);
        }
        #endregion


        /// <summary>
        /// 확인(Confirm)
        /// </summary>
        protected virtual void OnConfirm()
        {
            SceneLogic.instance.PopPopup();
        }

        /// <summary>
        /// 취소(Cancel)
        /// </summary>
        protected virtual void OnCancel()
        {
            SceneLogic.instance.PopPopup();
        }

        /// <summary>
        /// 팝업 액션
        /// </summary>
        protected virtual void PopupAction() { }



    }
}
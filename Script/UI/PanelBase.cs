/**********************************************************************************************
 * 
 *                  PanelBase.cs
 *                  
 *                      - UI Base Class
 *                      - 자식 UI(Button, Text...ect) 등을 보관 및 가져오기
 * 
 **********************************************************************************************/
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UI
{
    public class PanelBase : UIBase
    {
        ////Back키 커스텀 액션
        //public Action BackAction_Panel;

        protected override void SetMemberUI()
        {
            base.SetMemberUI();
        }

        /// <summary>
        /// Back 버튼
        /// </summary>
        /// <param name="cnt"></param>
        public override void Back(int cnt = 1)
        {
            if (BackAction_Custom != null)
            {
                BackAction_Custom();
                BackAction_Custom = null;
            }
            else
            {
                SceneLogic.instance.PopPanel(cnt); //패널 닫아줌
            }

            cnt--;
            if(cnt > 0)
            {
                SceneLogic.instance.Back(cnt);
            }
        }

        #region Panel Open, Close, Callback

        #region OpenClose

        /// <summary>
        /// 패널 오픈 코루틴
        /// </summary>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        public IEnumerator<float> Co_OpenPanel(bool isShowAnimation = true)
        {
            //_myGameObject.SetActive(true);
            transform.SetAsLastSibling();

            SceneLogic.instance.OnCurOpenStartAct();
            OpenStartAct();
            yield return Co_OpenStartAct().WaitUntilDone();

            if (isShowAnimation)
            {
                yield return Single.Scene.Co_FadeIn(2.5f).WaitUntilDone();
            }

            SceneLogic.instance.OnCurOpenEndAct();
            OpenEndAct();
            yield return Co_OpenEndAct().WaitUntilDone();
        }

        /// <summary>
        /// 패널 클로즈 코루틴
        /// </summary>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        public IEnumerator<float> Co_ClosePanel(bool isShowAnimation = true)
        {
            if (!gameObject.activeSelf)
            {
                yield break;
            }

            SceneLogic.instance.OnCurCloseStartAct();
            CloseStartAct();
            yield return Co_SetCloseStartAct().WaitUntilDone();

            if (isShowAnimation)
            {
                yield return Single.Scene.Co_FadeOut(2.5f).WaitUntilDone();
            }

            SceneLogic.instance.OnCurCloseEndAct();
            CloseEndAct();
            yield return Co_SetCloseEndAct().WaitUntilDone();
        }
        #endregion

        #endregion


        #region Panel Show, Hide

        public Action showInstant;
        public Action hideInstant;
        public Action showStart;
        public Action showEnd;
        public Action hideStart;
        public Action hideEnd;

        /// <summary>
        /// show and hide panel via canvasgroup
        /// </summary>
        public virtual void Show(bool _show, float _delay = 0f, bool _instant = false)
        {
            float target = _show ? 1f : 0f;

            if (_instant)
            {
                this.GetComponent<CanvasGroup>().alpha = _show ? 1f : 0f;
                this.GetComponent<CanvasGroup>().blocksRaycasts = _show;

                Action action = _show ? showInstant : hideInstant;
                action?.Invoke();

                return;
            }

            FadeUtils.FadeCanvasGroup(this.GetComponent<CanvasGroup>(), target, 2f, _delay,
                () =>
                {
                    Action action = _show ? showStart : hideStart;
                    action?.Invoke();
                },
                () =>
                {
                    Action action = _show ? showEnd : hideEnd;
                    action?.Invoke();
                }
            );
        }

        #endregion

        protected virtual void SetSafeArea()
        {
            var rectTransform = GetComponent<RectTransform>();

            rectTransform.offsetMin = new Vector2(50, 50);// Left, Bottom
            rectTransform.offsetMax = new Vector2(-50, -50); // Right, Top
            rectTransform.pivot = Vector2.one * 0.5f;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }
    }
}


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
    MIT License

    Copyright (c) 2018 kiepng

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
namespace PullToRefresh
{
    public class UIRefreshControl : MonoBehaviour
    {
        [Serializable] public class RefreshControlEvent : UnityEvent { }

        // isDistance과 isSpeed가 모두 false면 둘 다 적용되는 것으로 처리
        [SerializeField] bool isDistance = true; // 거리로 갱신
        [SerializeField] bool isSpeed = true; // 속도로 갱신
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private float m_PullDistanceRequiredRefresh = 150f;
        [SerializeField] private float m_PullSpeedRequiredRefresh = 1500f;
        //[SerializeField] private Animator m_LoadingAnimator;
        [SerializeField] RefreshControlEvent m_OnRefresh = new RefreshControlEvent();

        UnityAction OnDragStartCallback = null;
        UnityAction OnDragEndCallback = null;

        private float m_InitialPosition;
        private float m_Progress;
        private bool m_IsPulled;
        private bool m_IsRefreshing;
        private Vector2 m_PositionStop;
        private IScrollable m_ScrollView;

        private float oldYPosition;
        private float currentYPosition;
        private float velocity;

        /// <summary>
        /// Progress until refreshing begins. (0-1)
        /// </summary>
        public float Progress
        {
            get { return m_Progress; }
        }

        /// <summary>
        /// Refreshing?
        /// </summary>
        public bool IsRefreshing
        {
            get { return m_IsRefreshing; }
        }

        /// <summary>
        /// Callback executed when refresh started.
        /// </summary>
        public RefreshControlEvent OnRefresh
        {
            get { return m_OnRefresh; }
            set { m_OnRefresh = value; }
        }

        /// <summary>
        /// 드래그 시작 시 실행되는 콜백 - 한효주
        /// </summary>
        /// <param name="action"></param>
        public void SetOnDragStartCallback(UnityAction action)
        {
            OnDragStartCallback = action;
        }

        /// <summary>
        /// 드래그 끝 시 실행되는 콜백 - 한효주
        /// </summary>
        /// <param name="action"></param>
        public void SetOnDragEndCallback(UnityAction action)
        {
            OnDragEndCallback = action;
        }

        /// <summary>
        /// Call When Refresh is End.
        /// 코루틴 기능 포함 - 한효주
        /// </summary>
        public void EndRefreshing(float t = 0)
        {
            //m_IsPulled = false;
            //m_IsRefreshing = false;

            //if (OnDragEndCallback != null) OnDragEndCallback.Invoke();
            //m_LoadingAnimator.SetBool(_activityIndicatorStartLoadingName, false);

            if (endRefreshing != null)
            {
                StopCoroutine(endRefreshing);
            }
            endRefreshing = StartCoroutine(Co_EndRefreshing(t));

        }

        private Coroutine endRefreshing;
        private IEnumerator Co_EndRefreshing(float t)
        {
            yield return new WaitForSeconds(t);

            m_IsRefreshing = false;
            if (OnDragEndCallback != null) OnDragEndCallback.Invoke();

            Debug.Log("로딩 끝");
        }

        //const string _activityIndicatorStartLoadingName = "Loading";

        private void Start()
        {
            if (!isDistance && !isSpeed) isDistance = isSpeed = true;
            m_InitialPosition = oldYPosition = GetContentAnchoredPosition();
            m_PositionStop = new Vector2(m_ScrollRect.content.anchoredPosition.x, m_InitialPosition - m_PullDistanceRequiredRefresh);
            m_ScrollView = m_ScrollRect.GetComponent<IScrollable>();
            m_ScrollRect.onValueChanged.AddListener(OnScroll);
        }

        //private void LateUpdate()
        //{

            //if (!m_IsPulled)
            //{
            //    return;
            //}

            //if (!m_IsRefreshing)
            //{
            //    return;
            //}

            //m_ScrollRect.content.anchoredPosition = m_PositionStop;
        //}

        private void OnScroll(Vector2 normalizedPosition)
        {
            var distance = m_InitialPosition - GetContentAnchoredPosition();

            if (distance < 0f)
            {
                return;
            }

            OnPull(distance);
        }

        private void OnPull(float distance)
        {
            if (m_IsRefreshing && Math.Abs(distance) < 1f)
            {
                m_IsRefreshing = false;
            }

            if (m_IsPulled && m_ScrollView.Dragging)
            {
                return;
            }

            if (isDistance && !isSpeed)
            {
                if (!GetDistancePass(distance)) return;
            }
            else if (!isDistance && isSpeed)
            {
                if (!GetSpeedPass()) return;
            }
            else
            {
                if (!GetDistancePass(distance) && !GetSpeedPass()) return;
            }

            // Start animation when you reach the required distance while dragging.
            if (m_ScrollView.Dragging)
            {
                m_IsPulled = true;

                if (OnDragStartCallback != null) OnDragStartCallback.Invoke();
                //m_LoadingAnimator.SetBool(_activityIndicatorStartLoadingName, true);
            }

            // ドラッグした状態で必要距離に達したあとに、指を離したらリフレッシュ開始
            if (m_IsPulled && !m_ScrollView.Dragging)
            {
                m_IsPulled = false;
                m_IsRefreshing = true;
                m_OnRefresh.Invoke();
            }

            m_Progress = velocity = 0f;
        }

        /// <summary>
        /// 거리 계산 갱신 - 한효주
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool GetDistancePass(float distance)
        {
            m_Progress = distance / m_PullDistanceRequiredRefresh;

            if (m_Progress < 1f)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 속도 계산 갱신 - 한효주
        /// </summary>
        /// <returns></returns>
        private bool GetSpeedPass()
        {
            currentYPosition = GetContentAnchoredPosition();
            var dis = (currentYPosition - oldYPosition);
            velocity = dis / Time.deltaTime;
            oldYPosition = currentYPosition;

            if (velocity > -m_PullSpeedRequiredRefresh)
            {
                return false;
            }
            return true;
        }

        private float GetContentAnchoredPosition()
        {
            return m_ScrollRect.content.anchoredPosition.y;
        }
    }
}

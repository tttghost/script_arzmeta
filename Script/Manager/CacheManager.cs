using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 *              씬에 존재하는 '유니크' 한 컴포넌트를 캐쉬하는 싱글톤 매니저
 * 
 *              SetCache<T>()로 캐싱하고 GetCache<T>()로 가져온다.
 *              SceneLogic.cs 에서 캐싱하면 좋을 듯
 *              
 *              2023-08-02
 *              
 */

namespace FrameWork.Cache
{
    public class CacheManager : MonoBehaviour
    {
        #region 변수
        private static CacheManager _instance = null;
        public static CacheManager instance
        {
            get
            {
                if (_instance == null)
                {
                    if (FindObjectOfType<CacheManager>() == null)
                    {
                        string name = typeof(CacheManager).Name;
                        GameObject go = new GameObject(name);
                        go.name = name;
                        _instance = go.AddComponent<CacheManager>();
                    }
                    else
                    {
                        _instance = FindObjectOfType<CacheManager>();
                    }
                }
                return _instance;
            }
        }
        [ReadOnly] public SerializableDictionary<string, Component> caches = new SerializableDictionary<string, Component>();
        #endregion


        #region 함수
        /// <summary>
        /// 아이템 셋 캐쉬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetCache<T>() where T : Component
        {
            string TName = typeof(T).Name;
            if (caches.ContainsKey(TName))
            {
                //DEBUG.LOGERROR("해당 컴포넌트가 이미 추가됨");
                return;
            }

            T[] t = FindObjectsOfType<T>();
            if (t == null || t.Length > 1)
            {
                //DEBUG.LOGERROR("해당 컴포넌트가 없음");
                //DEBUG.LOGERROR("같은 컴포넌트가 2개 이상 존재");
                return;
            }

            caches.Add(TName, t[0]);
        }

        /// <summary>
        /// 아이템 겟 캐쉬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCache<T>() where T : Component
        {
            string TName = typeof(T).Name;
            if (!caches.ContainsKey(TName))
            {
                //DEBUG.LOGERROR("찾을 수 있는 캐쉬데이터 없음");
                return default(T);
            }
            return caches[TName] as T;
        }
        #endregion
    }
}
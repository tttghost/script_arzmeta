using Cysharp.Threading.Tasks;
using LightShaft.Scripts;
using MEC;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YoutubePlayer;

public class ScreenManager : Singleton<ScreenManager>
{
    private YoutubePlayerLivestream youtubePlayerLivestream;

    protected override void AWAKE()
    {
        base.AWAKE();
        youtubePlayerLivestream = gameObject.AddComponent<YoutubePlayerLivestream>();
        gameObject.AddComponent<YoutubePlayer.YoutubePlayer>();
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) //스토리지
        //{
        //    Debug.Log("===============");
        //    StartMediaPlayer(mediaPlayer, path, eMediaPlayType.storage);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2)) //일반
        //{
        //    Debug.Log("===============");
        //    StartMediaPlayer(mediaPlayer, path, eMediaPlayType.youtubeNormal);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3)) //라이브
        //{
        //    Debug.Log("===============");
        //    StartMediaPlayer(mediaPlayer, path, eMediaPlayType.youtubeLive);
        //}
    }



    /// <summary>
    /// 미디어플레이어 시작
    /// </summary>
    /// <param name="path"></param>
    /// <param name="eMediaPlayType"></param>
    /// <param name="mediaPlayer"></param>
    //public async void StartMediaPlayer(MediaPlayer mediaPlayer, string path, eScreenContentType eMediaPlayType)
    //{
    //    switch (eMediaPlayType)
    //    {
    //        case eScreenContentType.none:
    //        case eScreenContentType.storage:
    //            {
    //                OnMediaPlayer(mediaPlayer, path);
    //            }
    //            break;
    //        case eScreenContentType.youtubeNormal:
    //            {
    //                string result = await YoutubePlayer.YoutubePlayer.GetRawVideoUrlAsync(path, YoutubeDlCli.YtDlp);
    //                OnMediaPlayer(mediaPlayer, result);
    //            }
    //            break;
    //        case eScreenContentType.youtubeLive:
    //            {
    //                StartCoroutine(youtubePlayerLivestream.DownloadYoutubeUrl(path, (result) => OnMediaPlayer(mediaPlayer, result)));
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public void StartMediaPlayer(MediaPlayer mediaPlayer, string path, eScreenContentType eMediaPlayType)
    //{
    //    switch (eMediaPlayType)
    //    {
    //        case eScreenContentType.none:
    //        case eScreenContentType.storage:
    //        case eScreenContentType.youtubeNormal:
    //            {
    //                OnMediaPlayer(mediaPlayer, path);
    //            }
    //            break;
    //        case eScreenContentType.youtubeLive:
    //            {
    //                StartCoroutine(youtubePlayerLivestream.DownloadYoutubeUrl(path, (result) => OnMediaPlayer(mediaPlayer, result)));
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}
    public async UniTask<string> GetMediaPath(string path, eScreenContentType eMediaPlayType)
    {
        string result = string.Empty;
        switch (eMediaPlayType)
        {
            case eScreenContentType.storage:
                {
                    result = Path.Combine(Single.Web.StorageUrl, "screen", path);
                }
                break;
            case eScreenContentType.youtubeNormal:
                {
                    result  = Util.ConvertPlayableYoutubeLink(path);
                    //result = await YoutubePlayer.YoutubePlayer.GetRawVideoUrlAsync(path, YoutubeDlCli.YtDlp); // 서버닫힘
                }
                break;
            case eScreenContentType.youtubeLive:
                {
                    bool isDone = false;
                    StartCoroutine(youtubePlayerLivestream.DownloadYoutubeUrl(path, (res) => { result = res; isDone = true; }));
                    await UniTask.WaitUntil(()=> isDone);
                }
                break;
        }
        return result;
    }

}


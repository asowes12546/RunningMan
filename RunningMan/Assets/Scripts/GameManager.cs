
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text barT;
    public Image barI;

    public void Replay(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartLoading()
    {
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("關卡1");//取得載入場景資料
        ao.allowSceneActivation = false;                         //取消載入  true 會直接進入場景
        
        while (ao.isDone == false)                               //如果載入還沒完成  跑TEXT  BAR條
        {
            barT.text = ao.progress /0.9f * 100 + " / 100";      //更新文字
            barI.fillAmount = ao.progress / 0.9f;                //更新BAR條
            yield return null;                                   //等待

            if (ao.progress == 0.9f && Input.anyKey)                 //如果 資料跑完  且  按下任意鍵
            {
                ao.allowSceneActivation = true;                      //自動轉換場景
            }
        }
    }
}

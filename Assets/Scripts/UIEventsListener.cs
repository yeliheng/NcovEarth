using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
* 2020-03-08
* UI事件监听
* Written By Yeliheng
*/
public class UIEventsListener : MonoBehaviour
{

    public GameObject aboutBtn;
    public GameObject closeBtn;
    public GameObject exitBtn;
    public GameObject infoBtn;
    public GameObject panel;
    private void Start()
    {
       // aboutBtn = GameObject.Find("AboutButton");
        aboutBtn.GetComponent<Button>().onClick.AddListener(OnAboutButtonClick);
        //closeBtn = GameObject.Find("CloseButton");
        closeBtn.GetComponent<Button>().onClick.AddListener(OnCloseButtonClick);
        exitBtn.GetComponent<Button>().onClick.AddListener(OnExitButtonClick);
        infoBtn.GetComponent<Button>().onClick.AddListener(OnInfoButtonClick);
    }
    // Use this for initialization
    public void OnAboutButtonClick()
    {
        //Debug.Log("按钮被按下！");
        if(panel.activeInHierarchy != true)
            panel.SetActive(true);
    }

    public void OnCloseButtonClick()
    {
        if (panel.activeInHierarchy != false)
            panel.SetActive(false);
    }

    public void OnExitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;     
        #else
            Application.Quit();
        #endif
    }

    public void OnInfoButtonClick()
    {
        Application.OpenURL("https://news.qq.com/zt2020/page/feiyan.htm#/");
    }

}

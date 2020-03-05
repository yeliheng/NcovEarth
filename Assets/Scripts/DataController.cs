using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
/*
 * 2020-03-03
 * 取得数据
 * Written By Yeliheng
 */
public class DataController : MonoBehaviour {
	private WWWForm form;
	private WWW hostObj;
	public void Start() {
		StartCoroutine(sendRequest());
	}

	private IEnumerator sendRequest()
	{
		//构建数据
		form = new WWWForm();
		form.AddField("name", "disease_h5");
		hostObj = new WWW("https://view.inews.qq.com/g2/getOnsInfo", form);
		while (!hostObj.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		if (hostObj.error != null)
		{
			Debug.LogError(hostObj.error);
		}
		//Debug.Log(w.text);
		
	}

	/*获取中国确诊总数*/
	public string getChinaTotal()
	{
		string total;
		JsonData raw = JsonMapper.ToObject(hostObj.text);
		string json = raw[1].ToString();
		JsonData data = JsonMapper.ToObject(json);
		Debug.Log("中国确诊总数:" + data["chinaTotal"][0].ToString());
		total = data["chinaTotal"][0].ToString();
		return total;
	}
}

using Assets.Scripts;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 2020-03-03
 * 粒子系统控制
 * Written By Yeliheng
 */

public class ParticleController : MonoBehaviour {
    [SerializeField] private int total;
    public int proportion = 10;
    public const int LEVEL_LOW = 10;//轻度地区
    public const int LEVEL_MIDDLE = 100;//中度
    public const int LEVEL_HIGH = 1000;//严重
    protected float lat;//纬度
    protected float lng;//经度
    public GameObject cube;//测试
    void Start () {
        StartCoroutine(renderData());
       


        // em.type = ParticleSystemEmissionType.Time;

        /*                em.SetBursts(
                            new ParticleSystem.Burst[]{
                                new ParticleSystem.Burst(0.1f, 1000),
                            });*/
        // em.SetBurst(1,new ParticleSystem.Burst(1f,1000));
    }

    void Update () {	
	}

    private IEnumerator renderData()
    {
        WWWForm form;
        WWW hostObj;
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
        JsonData raw = JsonMapper.ToObject(hostObj.text);
        string json = raw[1].ToString();
        JsonData data = JsonMapper.ToObject(json);
         total = int.Parse(data["chinaTotal"][0].ToString()) / proportion;
        Debug.Log("中国确诊总数:" + data["chinaTotal"][0].ToString());
        this.setParticle(total);
        /*其他国家*/
        
        StartCoroutine(getPosFromServer("韩国"));
    }


    private void setParticle(int total)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Play();
        var main = ps.main;
         if (total < LEVEL_LOW || total > LEVEL_LOW && total < LEVEL_MIDDLE)
        {
            main.startColor = Color.white;
        }
        else if (total > LEVEL_MIDDLE && total < LEVEL_HIGH)
        {
            main.startColor = Color.yellow;
        }
        else
        {
            main.startColor = Color.red;
        }
        main.maxParticles = total;
        var em = ps.emission;
        em.enabled = true;
    }

    /**
    * 经纬度转换，试试看
    * 思路:以y = 0为赤道，计算球体大小，依次向上叠加
    * 直径700,一份87.5，对应180 半径350
    * 比如中国的： https://google.cn/maps/api/geocode/json?address=%E4%B8%AD%E5%9B%BD&key=AIzaSyCM9K-fqdpiaYdf580-MSoF-J6eBM3xllc
    */
    private IEnumerator getPosFromServer(string address)
    {
       // WWWForm form;
        WWW hostObj;
        //构建数据
        KeyUtil keyObj = new KeyUtil();
        hostObj = new WWW("https://google.cn/maps/api/geocode/json?address=" + address + "&key=" + keyObj.getKey());
        while (!hostObj.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (hostObj.error != null)
        {
            Debug.LogError(hostObj.error);
        }
        JsonData raw = JsonMapper.ToObject(hostObj.text);
        JsonData location = JsonMapper.ToObject(raw["results"][0]["geometry"]["location"].ToJson());
        float lat = float.Parse(location["lat"].ToString());
        float lng = float.Parse(location["lng"].ToString());
        Debug.Log("国家: " + address + " 纬度: " + lat + " 经度: " + lng);
       // cube = GetComponent<GameObject>();
        posTransform(lat,lng,cube);
    }

    //坐标转换测试(成功) 2020-03-06 14:20:35
    private void posTransform(float lat,float lng,GameObject cube) 
    {
        int r = 350;
        //cube.transform.position = new Vector3(-z,x,y);
       cube.transform.position = Quaternion.AngleAxis(lng, -Vector3.up) * Quaternion.AngleAxis(lat, -Vector3.right) * new Vector3(0, 0, r);
    }
}

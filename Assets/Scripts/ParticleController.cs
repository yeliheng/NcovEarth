using Assets.Scripts;
using LitJson;
using System;
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
    public const int LEVEL_LOW = 5;//轻度地区
    public const int LEVEL_MIDDLE = 20;//中度
    public const int LEVEL_HIGH = 100;//严重
    protected float lat;//纬度
    protected float lng;//经度
    public GameObject earth;//地球对象
    public Material parMat;//粒子材质
    void Start () {
        StartCoroutine(getNcovDetail());
        // em.type = ParticleSystemEmissionType.Time;

        /*                em.SetBursts(
                            new ParticleSystem.Burst[]{
                                new ParticleSystem.Burst(0.1f, 1000),
                            });*/
        // em.SetBurst(1,new ParticleSystem.Burst(1f,1000));
    }

    void Update () {	
	}

    private IEnumerator getNcovDetail()
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
        JsonData data = JsonMapper.ToObject(json);//data字段数据
        JsonData province = JsonMapper.ToObject(data["areaTree"][0]["children"].ToJson());
        for(int i = 0; i < province.Count; i++)
        {
            int proTotal = int.Parse(province[i]["total"]["confirm"].ToString()) / proportion;
            getPosFromFile(province[i]["name"].ToString(), proTotal); 
        }
        //中国比较特殊，先获取中国
         total = int.Parse(data["chinaTotal"][0].ToString()) / proportion;
        Debug.Log("中国确诊总数:" + data["chinaTotal"][0].ToString());
        //专门针对中国获取一个
       //this.getPosFromFile("中国", total);
        /*-----------------国外-----------------*/
        WWW foreignHostObj = new WWW("https://view.inews.qq.com/g2/getOnsInfo?name=disease_other");
        /*遍历所有国家*/
        while (!foreignHostObj.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (foreignHostObj.error != null)
        {
            Debug.LogError(foreignHostObj.error);
        }
        JsonData foreignRaw = JsonMapper.ToObject(foreignHostObj.text);
        JsonData foreignData = JsonMapper.ToObject(foreignRaw[1].ToString());
        JsonData foreignList = JsonMapper.ToObject(foreignData["foreignList"].ToJson());
        
      //  Debug.Log(foreignData.Count);
       for(int i = 0; i < foreignList.Count; i++)
        {
            Debug.Log("国家名称: " + foreignList[i]["name"] + " 确诊人数: " + foreignList[i]["confirm"]);
            //从服务器获取(暂时弃用)
            // StartCoroutine(getPosFromServer(foreignList[i]["name"].ToString(),int.Parse(foreignList[i]["confirm"].ToString())));
            //从本地文件获取
            total = int.Parse(foreignList[i]["confirm"].ToString()) / proportion;
            getPosFromFile(foreignList[i]["name"].ToString(), total);
        }
        
    }


    private void setParticle(int total,ParticleSystem ps)
    {
        var main = ps.main;
        main.playOnAwake = false;
        main.startSize = 5;
        //ps.Play();  
        ps.gameObject.GetComponent<ParticleSystemRenderer>().material = parMat;

         if (total < LEVEL_LOW || total > LEVEL_LOW && total < LEVEL_MIDDLE)
        {
           main.startColor = new Color(255,255,255);
        }
        else if (total > LEVEL_MIDDLE && total < LEVEL_HIGH)
        {
            main.startColor = new Color(255,255,0);
        }
        else
        {
            main.startColor = new Color(255,0,0);
        }
        main.maxParticles = total;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        var em = ps.emission;
        em.enabled = true;
        ps.Play();
    }

    //从服务器获取经纬度数据(后期做缓存)
    private IEnumerator getPosFromServer(string address,int total)
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
        //开始进行数据渲染
        // earth = this.gameObject;
        GameObject gameObject = new GameObject();
        //gameObject.name = address;
        gameObject.transform.parent = transform;
        ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.playOnAwake = false;
        renderData(lat,lng,ps,total);
        //posTransform(lat,lng,cube);
    }

    //从本地获取经纬度数据
    private void getPosFromFile(string address, int total)
    {
        string content = System.IO.File.ReadAllText(@"./Assets/Scripts/countries.json");
        JsonData raw = JsonMapper.ToObject(content);
        try
        {
            float lng = float.Parse(raw[address][0].ToString());
            float lat = float.Parse(raw[address][1].ToString());
           // Debug.Log("国家: " + address + " 经度: " + lng + " 纬度: " + lat);
            GameObject gameObject = new GameObject(address);
            gameObject.transform.parent = transform;
            ParticleSystem ps = gameObject.AddComponent<ParticleSystem>();
            ps.Stop();
            renderData(lat, lng, ps, total);

        }
        catch(Exception e)
        {
            Debug.LogWarning("没有找到国家: " + address);
        }
        

            
    }

    //坐标转换测试(成功) 2020-03-06 14:20:35
    private void posTransformTest(float lat,float lng,GameObject cube) 
    {
        int r = 350;
        //cube.transform.position = new Vector3(-z,x,y);
       cube.transform.position = Quaternion.AngleAxis(lng, -Vector3.up) * Quaternion.AngleAxis(lat, -Vector3.right) * new Vector3(0, 0, r);
    }

    /*数据渲染*/
    private void renderData(float lat, float lng, ParticleSystem ps,int total)
    {
        int r = 350;
       // ps = GetComponent<ParticleSystem>();
        ps.transform.position = Quaternion.AngleAxis(lng, -Vector3.up) * Quaternion.AngleAxis(lat, -Vector3.right) * new Vector3(0, 0, r); 
        //var em = ps.emission;
        setParticle(total, ps);

    }
}

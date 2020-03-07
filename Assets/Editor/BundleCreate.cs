using UnityEngine;
using UnityEditor;

public class BundleCreate
{

    //拓展编辑菜单
    [MenuItem("AssetsBundle/CreatAssetsBudles")]

    static void CreateAsset()
    {

        //存放打包好文件的路径
        string dir = Application.dataPath + "/AssetBuild";

        //将资源中命名的所有资源打包处理（参数：路径，有依附关系，安卓平台）
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

    }

}

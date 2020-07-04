using UnityEngine;

/*
 * 2020-03-08
 * 摄影机围绕目标物体(仿谷歌地球)
 * Written By Yeliheng
 */
public class CameraController : MonoBehaviour
{
    Vector2 screenPos = new Vector2();//手指触屏的位置
    public Transform target;//获取旋转目标
    public int speed = 5;
    //平滑跟随速度
    private float xVelocity = 1f;
    private float yVelocity = 1f;
    void Start()
    {
        Input.multiTouchEnabled = true;// 允许多点触控
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN
        cameraRotate();
        cameraZoom();
#elif UNITY_ANDROID
         cameraRotateForAndroid();
#elif UNITY_WEBGL
        cameraRotate();
        cameraZoom();
#endif
    }
    private void cameraRotate() //摄像机围绕目标旋转操作
    {
       // transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime); //摄像机围绕目标旋转
        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
        //右键
        if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.Translate(Vector3.left * (mouse_x * 15f) * Time.deltaTime);
            transform.Translate(Vector3.up * (mouse_y * 15f) * Time.deltaTime);
        }
        //左键
        if (Input.GetKey(KeyCode.Mouse0))
        {
           // Debug.Log("X轴:" + mouse_x + " Y: " + mouse_y);
            transform.RotateAround(target.transform.position, Vector3.up, mouse_x * 1.5f);
            transform.RotateAround(target.transform.position, transform.right, mouse_y * 1.5f);
        }
    }

    private void cameraZoom() //摄像机滚轮缩放
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            transform.Translate(Vector3.forward * 5f);


        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            transform.Translate(Vector3.forward * -5f);
    }


    //安卓平台的摄影机处理
    private void cameraRotateForAndroid()
    {
        if (Input.touchCount <= 0)
            return;
        // 1个手指触摸屏幕
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                // 手指触屏的位置
                screenPos = Input.touches[0].position;
            }
            // 手指移动
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {
                // 移动摄像机
                //transform.RotateAround(new Vector3(Input.touches[0].deltaPosition.x * Time.deltaTime, Input.touches[0].deltaPosition.y * Time.deltaTime, 0));
                transform.RotateAround(target.transform.position, Vector3.up, Input.touches[0].deltaPosition.x * Time.deltaTime * 1.5f);
                transform.RotateAround(target.transform.position, Vector3.right, Input.touches[0].deltaPosition.y * Time.deltaTime * 1.5f);
            }
        }
        else if (Input.touchCount > 1)
        {
            // 两个手指的位置
            Vector2 finger1 = new Vector2();
            Vector2 finger2 = new Vector2();
            // 两个手指的移动
            Vector2 mov1 = new Vector2();
            Vector2 mov2 = new Vector2();

            for (int i = 0; i < 2; i++)
            {
                Touch touch = Input.touches[i];
                if (touch.phase == TouchPhase.Ended)
                    break;
                if (touch.phase == TouchPhase.Moved)
                {
                    float mov = 0;
                    if (i == 0)
                    {
                        finger1 = touch.position;
                        mov1 = touch.deltaPosition;
                    }
                    else
                    {
                        finger2 = touch.position;
                        mov2 = touch.deltaPosition;
                        if (finger1.x > finger2.x)
                            mov = mov1.x;
                        else
                            mov = mov2.x;
                        if (finger1.y > finger2.y)
                            mov += mov1.y;
                        else
                            mov += mov2.y;
                        transform.Translate(0, 0, mov * Time.deltaTime * 5f);
                    }
                }
            }
        }
    }
            
}
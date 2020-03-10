using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;//获取旋转目标
    public int speed = 5;
    //平滑跟随速度
    private float xVelocity = 1f;
    private float yVelocity = 1f;
    void Start()
    {

    }

    void Update()
    {
        cameraRotate();
        cameraZoom();
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
           // mouse_x = Mathf.SmoothDampAngle(mouse_x, target.transform.position.x, ref xVelocity, 1f);
           // mouse_y = Mathf.SmoothDampAngle(mouse_y, target.transform.position.y, ref yVelocity, 1f);
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
}
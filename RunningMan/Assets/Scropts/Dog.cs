using UnityEngine;

public class Dog : MonoBehaviour
{
    #region 變數區域

    [Header("跳躍次數")]
    public int jumpCount = 2;
    public float jumpHigh = 50f;
    public bool isGround = true;
    [Tooltip("判定角色是否在地上")]
    public string player = "eric";
    [Header("速度"),Range(1f,10f)]
    public float speed = 1f;

    public Animator anim;

    public Transform dog,camera;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {//依照巾貞數 執行N次
        run();
    }

    void run()
    {
        //鏡頭移動
        camera.Translate(speed * Time.deltaTime, 0, 0);
        //人物移動
        dog.Translate(speed * Time.deltaTime, 0,0);
        
    }

    //跳躍方法
    public void jump()
    {
        dog.Translate(0, jumpHigh * Time.deltaTime, 0);
        anim.SetBool("跳躍開關",true);       
    }

    //滑行方法
    public void slide()
    {
        anim.SetBool("滑行開關",true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //anim.SetBool("跳躍開關", false);
        //anim.SetBool("滑行開關", false);
    }


}

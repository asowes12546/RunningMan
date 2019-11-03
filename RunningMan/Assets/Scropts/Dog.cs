using UnityEngine;

public class Dog : MonoBehaviour
{
    #region 變數區域

    [Header("跳躍次數")]
    public int jumpCount = 2;
    public float jumpHigh = 50f;
    public bool isGround = false;
    [Tooltip("判定角色是否在地上")]
    public string player = "eric";
    [Header("速度"),Range(1f,10f)]
    public float speed = 1f;

    private Animator anim;
    private CapsuleCollider2D cc2d;
    private Rigidbody2D r2d;

    private Transform camera;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        anim.GetComponent<Animator>();
        cc2d.GetComponent<CapsuleCollider2D>();
        r2d.GetComponent<Rigidbody2D>();

        camera = GameObject.Find("Main Camera").transform;
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
        transform.Translate(speed * Time.deltaTime, 0,0);
        
    }

    //跳躍方法
    public void jump()
    {
        if (isGround == true)
        {
            //dog.Translate(0, jumpHigh * Time.deltaTime, 0);
            anim.SetBool("跳躍開關", true);
            r2d.AddForce(new Vector2(0,jumpHigh));
            isGround = false;
        }
        
    }

    //滑行方法 膠囊體變小
    public void slide()
    {
        anim.SetBool("滑行開關",true);
        cc2d.offset = new Vector2(-0.095f,-0.98f);
        cc2d.size = new Vector2(0.8f,1.1f);
    }

    //重設跳躍及滑行動畫 
    public void ResetAnimator()
    {
        anim.SetBool("跳躍開關", false);
        anim.SetBool("滑行開關", false);

        //膠囊體變回原大小
        cc2d.offset = new Vector2(-0.095f, -0.3f);
        cc2d.size = new Vector2(0.8f, 2.5f);
    }

    //碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "地板")
        {
            isGround = true;
        }
    }
}

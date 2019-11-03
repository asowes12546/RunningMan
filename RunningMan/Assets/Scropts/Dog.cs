using UnityEngine;
using UnityEngine.UI;

public class Dog : MonoBehaviour
{
    #region 變數區域

    [Header("跳躍次數")]
    public int jumpCount = 2;
    [Header("跳躍高度")]
    public float jumpHigh = 50f;
    [Header("是否在地上")]
    public bool isGround = false;
    [Tooltip("判定角色是否在地上")]
    public string player = "eric";
    [Header("速度"),Range(1f,10f)]
    public float speed = 1f;
    [Header("血條")]
    public float hp = 100f;
    private float maxHp;
    public Image hpBar;
    [Header("障礙物傷害值")]
    public float damageX = 20f;


    public AudioClip jumpSound, slideSound;  //音效
    public Color damageColor;                //受傷的顏色

    private Animator anim;                   //動畫
    private CapsuleCollider2D cc2d;          //碰撞體
    private Rigidbody2D r2d;                 //剛體
    private AudioSource as2d;                //聲音
    private SpriteRenderer sr;               //圖片渲染器
    

    private Transform cam;                   //坐標

    #endregion

    // 開啟遊戲時 執行一次
    void Start()
    {
        maxHp = hp;

        anim = GetComponent<Animator>();
        cc2d = GetComponent<CapsuleCollider2D>();
        r2d = GetComponent<Rigidbody2D>();
        as2d = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        cam = GameObject.Find("Main Camera").GetComponent<Transform>();  //搜尋主相機的座標
    }

    // 依照巾貞數 執行N次
    void Update()
    {
        run();
    }

    void run()
    {
        //鏡頭移動
        cam.Translate(speed * Time.deltaTime, 0, 0);
        //人物移動
        transform.Translate(speed * Time.deltaTime, 0,0);
        
    }

    //跳躍方法
    public void jump()
    {
        if (isGround == true)
        {
            anim.SetBool("跳躍開關", true);
            r2d.AddForce(new Vector2(0,jumpHigh));  //向上的力
            isGround = false;
            as2d.PlayOneShot(jumpSound,1f);
        }
        
    }

    //滑行方法 膠囊體變小
    public void slide()
    {
        anim.SetBool("滑行開關",true);
        cc2d.offset = new Vector2(-0.095f,-0.98f);
        cc2d.size = new Vector2(0.8f,1.1f);
        as2d.PlayOneShot(slideSound, 1f);
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

    //顯示 圖片渲染器 變回正常
    private void ShowRenderer()
    {
        //sr.enabled = true;
        sr.color = Color.white;
    }

    //受傷時發生的事情
    private void Damage()
    {
        Debug.Log("受傷!");
        //sr.enabled = false;
        sr.color = damageColor;       //變成受傷方法
        Invoke("ShowRenderer",0.3f);  //0.3秒後變回正常

        hp -= damageX;
        hpBar.fillAmount = hp / maxHp;
    }

    //碰到地板時發生的事情   isTrigger要關閉(不會穿透)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "地板")
        {
            isGround = true;
        }
    }

    //碰到障礙物時 發生的事情  isTrigger要打開(會穿透)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "障礙物")
        {
            Damage();
        }
    }
}

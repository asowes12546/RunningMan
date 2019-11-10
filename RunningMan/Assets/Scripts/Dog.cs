using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections;

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
    [Header("鑽石")]
    public int countDiamond;
    public Text textDiamond;
    [Header("櫻桃")]
    public int countCherry;
    public Text textCherry;
    [Header("遺失血量")]
    public float lose;

    public GameObject final;
    public Text textD, textC, textTime, textTotal;
    public int scoreD, scoreC, scoreTime, scoreTotal,finalTime;


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
        Run();
        LoseHp();
        //scoreTime += (int)Time.timeScale;
    }

    void Run()
    {
        //鏡頭移動
        cam.Translate(speed * Time.deltaTime, 0, 0);
        //人物移動
        transform.Translate(speed * Time.deltaTime, 0,0);
        //scoreTime = DateTime.Now.Second
    }

    //跳躍方法
    public void Jump()
    {
        if (hp <= 0) return;

        if (isGround == true)
        {
            anim.SetBool("跳躍開關", true);
            r2d.AddForce(new Vector2(0,jumpHigh));  //向上的力
            isGround = false;
            as2d.PlayOneShot(jumpSound,1f);
        }
        
    }

    //滑行方法 膠囊體變小
    public void Slide()
    {
        if (hp <= 0) return;

        anim.SetBool("滑行開關",true);
        cc2d.offset = new Vector2(-0.095f,-0.98f);
        cc2d.size = new Vector2(0.8f,.5f);
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
        sr.color = damageColor;       //變成受傷顏色
        Invoke("ShowRenderer",0.3f);  //0.3秒後變回正常

        hp -= damageX;
        hpBar.fillAmount = hp / maxHp;

        Dead();
    }

    //碰到地板時發生的事情   isTrigger要關閉(不會穿透)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "地板")
        {
            isGround = true;
        }

        if (collision.gameObject.name == "道具")
        {
            EatCherry(collision);
            countCherry++;
            textCherry.text = countCherry+"";
        }
    }

    //碰到障礙物時 發生的事情  isTrigger要打開(會穿透)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "障礙物")
        {
            Damage();
        }

        if (collision.tag == "鑽石")
        {
            EatDiamond(collision);
            countDiamond++;
            textDiamond.text = countDiamond+"";
        }

        if (collision.name == "死亡區域")
        {
            hp = 0;
            Dead();
        }
    }

    private void EatCherry(Collision2D collision)
    {
        Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();

        //給一個空的 Vector3
        Vector3 hitPos = Vector3.zero;
        //抓取碰到櫻桃的點
        ContactPoint2D hit = collision.contacts[0];

        //將碰到的點的X及Y  除100  取得物件中心點
        hitPos.x = hit.point.x - 0.01f * hit.normal.x;
        hitPos.y = hit.point.y - 0.01f * hit.normal.y;

        //將中心點放入偵測物件  將碰到的物件變成空的
        tilemap.SetTile(tilemap.WorldToCell(hitPos),null);
    }

    private void EatDiamond(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }

    private void LoseHp()
    {
        hp -= Time.deltaTime * lose;
        hpBar.fillAmount = hp / maxHp;

        Dead();
    }

    private void Dead()
    {
        if (hp <= 0)
        {
            speed = 0;
            anim.SetBool("死亡開關",true);

            SetFinal();
        }
    }

    private void SetFinal()
    {
        if (final.activeInHierarchy == false)
        {
            final.SetActive(true);

            /*textD.text = scoreD + "";
            textC.text = scoreC + "";
            textTime.text = scoreTime + "";
            textTotal.text = (scoreD + scoreC + scoreTime) + "";*/

            finalTime = (int)Time.time;

            StartCoroutine(FinalTime( countDiamond, scoreD, 100, textD, .1f));
            StartCoroutine(FinalTime( countCherry, scoreC,  50, textC, .1f,countDiamond * 0.2f));
            StartCoroutine(FinalTime( finalTime, scoreTime, 20, textTime, .1f,(countDiamond+countCherry) *0.2f));
        }
    }

    //float wait=0 是預設值用法  如果呼叫的方法不給該值 則是給予設定的預設值  且此用法參數要擺在後面
    private IEnumerator FinalTime(int count, int finalScore, int score,Text text,float second,float wait=0)
    {
        yield return new WaitForSeconds(wait);

        while (count > 0)
        {
            count--;
            finalScore += score;
            text.text = finalScore + "";
            yield return new WaitForSeconds(second);
        }
    }

    /*private IEnumerator FinalTime()
    {
        while (countDiamond > 0)
        {
            countDiamond--;
            scoreD += 100;
            textD.text = scoreD + "";
            yield return new WaitForSeconds(.1f);
        }

        while (countCherry > 0)
        {
            countCherry--;
            scoreC += 100;
            textC.text = scoreC + "";
            yield return new WaitForSeconds(.1f);
        }

        while (finalTime > 0)
        {
            finalTime--;
            scoreTime += 20;
            textTime.text = scoreTime + "";
            yield return new WaitForSeconds(.1f);
        }
    }*/
}

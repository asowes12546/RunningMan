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
    //public int scoreD, scoreC, scoreTime, scoreTotal,finalTime;
    public int finalTime;
    public int[] scores = new int[4];


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

        hp += 50;
        hpBar.fillAmount = hp / maxHp;

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
            
            finalTime = (int)Time.timeSinceLevelLoad;

            StartCoroutine(FinalTime( countDiamond, 0, 100, textD, .1f));
            StartCoroutine(FinalTime( countCherry, 1,  50, textC, .1f,countDiamond * 0.1f));
            StartCoroutine(FinalTime( finalTime, 2, 20, textTime, .05f,(countDiamond+countCherry) *0.1f));
        }
    }

    //float wait=0 是預設值用法  如果呼叫的方法不給該值 則是給予設定的預設值  且此用法參數要擺在後面
    //第一個參數 是櫻桃/鑽石/時間的數量  第二個參數是分數要放的陣列   第三個參數是一個櫻桃/鑽石/時間的分數  第四個參數是Text 
    //第五個參數是 一個分數中跳分數的間隔時間   第六個參數是 第一個分數跳完 要到下個分數的間隔時間
    private IEnumerator FinalTime(int count, int scoreIndex, int score,Text text,float second,float wait=0)
    {
        yield return new WaitForSeconds(wait);

        while (count > 0)
        {
            count--;
            scores[scoreIndex] += score;
            text.text = scores[scoreIndex] + "";
            yield return new WaitForSeconds(second);
        }

        //如果索引值 不是3  將值傳到索引值3裡面
        if(scoreIndex != 3 ) scores[3]+=scores[scoreIndex];
        //如果索引值到2的時候  準備進行索引3的行為
        if(scoreIndex == 2){
        int total = scores[3] / 10;
        scores[3] = 0;
        StartCoroutine(FinalTime( total, 3, 10, textTotal, .01f,0.1f));
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

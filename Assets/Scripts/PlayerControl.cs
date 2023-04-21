using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameObject UIM;
    UIManager UIManagerScript;

    float moveSpeed;     //이동 속도
    float sightAngle; //시야각 범위

    GameObject[] AccelPosition;     //엑셀 도착 지점
    Rigidbody playerRigidbody;  //리짓바디

    public int maxPlayerHP = 10;  //플레이어 개인정보 피 에너지 웨폰레벨 쉴드레벨
    public int playerHP = 10;
    public float playerEg = 100.0f;
    public int weaponLV = 1;
    public int shieldLV = 1;


    // Start is called before the first frame update
    void Start()
    {
        UIManagerScript = UIM.GetComponent<UIManager>();
        moveSpeed = 5.0f;
        sightAngle = 80f;
        AccelPosition = GameObject.FindGameObjectsWithTag("AccelPosition");
        playerRigidbody = GetComponent<Rigidbody>();
        playerHP = maxPlayerHP;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            
            playerEg -= 20; //이동시 에너지 20까임
        }

        Thumb();
        if (playerEg < 100) // 에너지 까이면 지속적으로 채우기
        {
            playerEg += 0.1f;
        }
        else //100을 넘겼을시 그냥 100으로 고정
        {
            playerEg = 100f;
        }
    }

    //컨트롤러 조이스틱
    void Thumb()
    {
        if (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick))
        {
            Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (thumbstick.x < 0) //왼쪽
            {

            }
            else if (thumbstick.x > 0) //오른쪽
            {

            }
            else if (thumbstick.y > 0) // 위
            {

            }
            else if(thumbstick.y<0) //아래
            {


            }
        }

        //임시방편 pc 이동
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(hAxis, 0, vAxis).normalized;

        playerRigidbody.velocity = inputDir * moveSpeed;

        //transform.LookAt(transform.position + inputDir);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyWeapon")
        {
            if (playerHP > 0)
            {
                playerHP--;
                UIManagerScript.healths[playerHP].gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.tag == "Juice")
        {
            if (playerHP < 10)
            {
                UIManagerScript.healths[playerHP].gameObject.SetActive(true);
                playerHP++;
            }
        }
    }

    //타겟이 시야 내에 있는가??
    bool IsTargetInSight()
    {

        //타겟의 방향 
        Vector3 targetDir = (AccelPosition[0].transform.position - transform.position).normalized; //크기가 1인 벡터로 만듬 -> 노멀값
        float dot = Vector3.Dot(transform.forward, targetDir);  //내적 -> |a||b|cos@ - |a||b| = 1(생략 가능)

        //내적을 이용한 각 계산하기
        //thetha = cos^-1( a dot b / |a||b|)
        //Mathf.Rad2Deg을 이용하여 라디안 값을 각도로 변환
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        //Debug.Log("타겟과 AI의 각도 : " + theta);
        if (theta <= sightAngle) return true;   //시야각 내부에 있음
        else return false;


        return false;

    }




    public void GunLevelUp()
    {
        if (weaponLV >= 3) //최대 3레벨 제한
        {
            weaponLV = 3;
        }
        else //최대 레벨 아닐 시 렙업
        {
            weaponLV++;
        }
    }
    public void HPLevelUp()
    {
        if(maxPlayerHP >= 160) //최대 3레벨 제한
        {
            maxPlayerHP = 160;
        }
        else //최대 레벨 아닐 시 렙업
        {
            maxPlayerHP += 20;
        }
    }
    public void ShieldLevelUp()
    {
        if (shieldLV >= 3) //최대 3레벨 제한
        {
            shieldLV = 3;
        }
        else //최대 레벨 아닐 시 렙업
        {
            shieldLV++;
        }
    }
}

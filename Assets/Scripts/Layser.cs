using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Layser : MonoBehaviour
{
    private GameManagers gameManager;
    private SceneManagers sceneManager;
    private LineRenderer layser;        // 레이저
    public Color lineColor;
    private RaycastHit Collided_object; // 충돌된 객체
    private GameObject currentObject;   // 가장 최근에 충돌한 객체를 저장하기 위한 객체

    static float raycastDistance = 100f; // 레이저 포인터 감지 거리
    public float rayRenderDistance;

    public GameObject Llayser;
    public GameObject Rlayser;

    string sceneName = "";
    int chooseCard;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagers>();
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManagers>();
        chooseCard = 0;
        // 스크립트가 포함된 객체에 라인 렌더러라는 컴포넌트를 넣고있다.
        layser = this.gameObject.GetComponent<LineRenderer>();
        // 라인이 가지개될 색상 표현
        lineColor = Color.black;
        layser.material.color = lineColor;
        // 레이저의 꼭지점은 2개가 필요 더 많이 넣으면 곡선도 표현 할 수 있다.
        layser.positionCount = 2;
        // 레이저 굵기 표현
        layser.startWidth = 0.01f;
        layser.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        layser.SetPosition(0, transform.position); // 첫번째 시작점 위치
                                                   // 업데이트에 넣어 줌으로써, 플레이어가 이동하면 이동을 따라가게 된다.
                                                   //  선 만들기(충돌 감지를 위한)
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);

        // 충돌 감지 시
        if (Physics.Raycast(transform.position, transform.forward, out Collided_object, raycastDistance))
        {
            layser.SetPosition(1, Collided_object.point);

            //충돌한 물체 이름 확인후 이동할 씬 설정
            if (Collided_object.collider.gameObject.name == "ClickStart")
            {
                sceneName = "OpeningScene";
            }
            else if (Collided_object.collider.gameObject.name == "ClickOption")
            {
                sceneName = "OptionScene";
            }
            else if (Collided_object.collider.gameObject.name == "ClickExit")
            {
                sceneName = "ExitScene";
            }
            else if (Collided_object.collider.gameObject.name == "ReturnOption")
            {
                sceneName = "TitleScene";
            }
            else if (Collided_object.collider.gameObject.name == "ReturnExit")
            {
                sceneName = "TitleScene";
            }
            else if (Collided_object.collider.gameObject.name == "ReturnStart")
            {
                sceneName = "TitleScene";
            }
            else sceneName = null;

            //충돌한 물체 이름 확인후 강화 선택
            if (Collided_object.collider.gameObject.name == "CardHP" )
            {
                chooseCard = 1;
            }
            else if (Collided_object.collider.gameObject.name == "CardGun")
            {
                chooseCard = 2;
            }
            else if (Collided_object.collider.gameObject.name == "CardEnergy")
            {
                chooseCard = 3;
            }

        }
        else
        {
            // 레이저에 감지된 것이 없기 때문에 레이저 초기 설정 길이만큼 길게 만든다.
            layser.SetPosition(1, transform.position + (transform.forward * rayRenderDistance));
            sceneName = null;
        }
    }

    private void LateUpdate()
    {
        // 왼쪽 트리거 버튼을 누를 경우 + 동시에 눌리는걸 막기
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        { 
            Rlayser.SetActive(false);   //오른쪽 레이저 비활성화, 그래야 레이저 하나만 인식
            lineColor = Color.magenta;  //레이저 색 변경
            layser.material.color = lineColor;

            if (chooseCard != 0)
            {
                if(chooseCard == 1)
                    gameManager.playerInfo.HPLevelUP(sceneManager.nextSceneName);
                if (chooseCard == 2)
                    gameManager.playerInfo.WeaponLeveUP(sceneManager.nextSceneName);
                if (chooseCard == 3)
                    gameManager.playerInfo.EnergyLevelUP(sceneManager.nextSceneName);
                chooseCard = 0;
            }

            //씬이동
            if (sceneName != null)
                LoadingSceneManager.LoadScene(sceneName);
            

        }
        // 오른쪽 트리거 버튼을 누를 경우 + 동시에 눌리는걸 막기
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Llayser.SetActive(false);    //왼쪽 레이저 비활성화, 그래야 레이저 하나만 인식
            lineColor = Color.magenta;  //레이저 색 변경
            layser.material.color = lineColor;

            if(sceneName == "ExitScene")
            {
                Application.Quit();
            }
            
            if(chooseCard != 0)
                LoadingSceneManager.LoadScene(sceneManager.nextSceneName);

            if (sceneName != null)
                LoadingSceneManager.LoadScene(sceneName);
            //강화 활성화
            
        }

        //왼쪽 트리거 버튼을 뗄 경우          
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            Rlayser.SetActive(true);    //오른쪽 레이저 활성화
            lineColor = Color.black;     //레이저 색 
            layser.material.color = lineColor;
            Rlayser.GetComponent<LineRenderer>().material.color = lineColor;    //오른쪽 레이저도 색 변경해주어야 함
        }
        //오른쪽 트리거 버튼을 뗄 경우  
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            Llayser.SetActive(true);    //왼쪽 레이저 활성화
            lineColor = Color.black;    //레이저 색 
            layser.material.color = lineColor;
            Llayser.GetComponent<LineRenderer>().material.color = lineColor;    //왼쪽 레이저도 색 변경해주어야 함
        }
    }
}

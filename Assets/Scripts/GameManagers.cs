using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


/*
 * 게임 시작 조건
 * 스토리 진행이 끝나야함 isScripting
 * 총이 소환되어야함 playercontrol스크립트에 isHaveWeapon
 */

public class PlayerInfor    //계속해서 플레이어에게 전달되어야 할 정보를 저장할 클래스
{
    private int weaponLV;    //무기 레벨
    private int energyLV;    //에너지소모량 결정하는 레벨
    private int hpLV;   //탄창 수를 늘려주는 레벨
    private bool isMoveAble;    //플레이어가 움직일 수 있는지 변수

    public void Init()
    {
        weaponLV = 1;
        energyLV = 1;
        hpLV = 1;
        isMoveAble = false;
    }

    public void WeaponLeveUP(string name)
    {
        if (weaponLV < 3)
        {
            weaponLV++;
            LoadingSceneManager.LoadScene(name);
        }
        else
            Debug.Log("강화 최대치 입니다");
    }
    public void EnergyLevelUP(string name)
    {
        if (energyLV < 3)
        {
            energyLV++;
            LoadingSceneManager.LoadScene(name);
        }
        else
            Debug.Log("강화 최대치 입니다");
    }
    public void HPLevelUP(string name)
    {
        if (hpLV < 3)
        {
            hpLV++;
            LoadingSceneManager.LoadScene(name);
        }
        else
            Debug.Log("강화 최대치 입니다");
    }

    public int WeaponLV()
    {
        return weaponLV;
    }
    public int EnergyLV()
    {
        return energyLV;
    }
    public int HPLV()
    {
        return hpLV;
    }

}

public class GameManagers : MonoBehaviour
{
    public PlayerInfor playerInfo;
    SceneManagers sceneManagerScript;
    StoryScript storyScript;
    OVRPlayerController ovrPlayerControl;
    MonsterManager monsterManagerScript;

    private GameObject player;
    private PlayerControl playerScript;
    public float subSpeed = 0.0f; //자막 속도
    private string sceneName;   //씬이름을 받아와 현재 어떤 씬인지 확인하기 위한 변수
    private bool isPlaying;     //플레이가 진행 중인지 확인
    private bool isStageClear;  //스테이지가 클리어 됐나 확인하는 변수
    public bool isBossDead;

    //씬이 변경될 때마다 호출되는 3함수들
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  //델리게이트 체인 추가
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(GameObject.Find("Story"))
            storyScript = GameObject.Find("Story").GetComponent<StoryScript>();
        if(GameObject.Find("MonsterManage"))
            monsterManagerScript = GameObject.Find("MonsterManage").GetComponent<MonsterManager>();
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerControl>();
            ovrPlayerControl = player.GetComponent<OVRPlayerController>();
        }
        isPlaying = false;
        isStageClear = false;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  //델리게이트 체인 제거;
    }

    // Start is called before the first frame update
    void Awake()
    {
        //초기화 함수
        Init();
    }

    private void Update()
    {
        SettingGame();
        if(monsterManagerScript)
            isStageClear = monsterManagerScript.isStageClear;
        if (storyScript)
        {
            if (storyScript.IsStoryComplete() && playerScript.IsHaveWeapon() && !playerScript.IsDeath())
                isPlaying = true;   //이 변수가 true일 때 모든 인게임 동작 실행(조정하기)
        }
        if (isStageClear)
        {
            isPlaying = false;
            ovrPlayerControl.Acceleration = 0;

        }
        if (playerScript.IsDeath())
        {
            ovrPlayerControl.Acceleration = 0;
        }
    }

    //초기 설정 함수
    private void Init()
    {
        playerInfo = new PlayerInfor();
        playerInfo.Init();
        sceneManagerScript = GameObject.Find("SceneManager").GetComponent<SceneManagers>();
        isPlaying = false;
        isStageClear = false;
        isBossDead = false;
        DontDestroyOnLoad(gameObject);
        
    }
    
    private void SettingGame()
    {
        if (isPlaying)
        {
            ovrPlayerControl.Acceleration =  0.1f;
        }
        else
        {
            ovrPlayerControl.Acceleration = 0;
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
    public bool IsStageClear()
    {
        return isStageClear;
    }
}

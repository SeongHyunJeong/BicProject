using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour
{
    private string currentSceneName;
    public string nextSceneName;
    public int stage = 0;
    private GameManagers gameManagerScript;    //게임 매니저 스크립트

    private void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagers>();
        DontDestroyOnLoad(this);
    }


    private void Update()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        CheckClearCondition();
    }
    
    public bool CheckClearCondition()
    {
        if ((Input.GetKeyDown(KeyCode.X) || OVRInput.GetDown(OVRInput.RawButton.A)) && currentSceneName != "UpgradeScene")
        {
            if (currentSceneName == "TitleScene")
            {
                nextSceneName = "OpeningScene";
                LoadingSceneManager.LoadScene(nextSceneName);
            }

            if (gameManagerScript.IsStageClear())
            {
                if ((currentSceneName == "Stage01" || currentSceneName == "Stage02"))
                {
                    nextSceneName = "UpgradeScene";
                    stage++;
                }
                if (currentSceneName == "Stage03")
                {
                    nextSceneName = "EndingScene";
                }

                LoadingSceneManager.LoadScene(nextSceneName);
            }
            
        }

        if (currentSceneName == "UpgradeScene")
        {
            if (stage == 1)
                nextSceneName = "Stage02";
            else if (stage == 2)
                nextSceneName = "Stage03";
        }



        return false;  // 클리어 조건이 충족되지 않았을 때 false를 반환
    }
}
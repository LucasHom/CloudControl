using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI midgewasText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI statHeaderText;
    [SerializeField] private TextMeshProUGUI statResultText;

    [SerializeField] private GameObject midgeReaction;
    [SerializeField] private Sprite sad;
    [SerializeField] private Sprite happy;

    [SerializeField] private TMP_FontAsset newFont;
    [SerializeField] private GameObject replayButton;

    public bool didWin = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        


        if (didWin)
        {
            resultText.color = new Color(244f / 255f, 195f / 255f, 38f / 255f);
            midgewasText.color = new Color(244f / 255f, 195f / 255f, 38f / 255f);
            resultText.text = "protected";
            midgeReaction.GetComponent<Image>().sprite = happy;
            StartCoroutine(EmphasizeObject.Emphasize(midgeReaction, Vector3.one, 1.1f));
        }
        else
        {
            resultText.text = "slimed out";
            midgeReaction.GetComponent<Image>().sprite = sad;
        }

        replayButton.SetActive(false);
        statHeaderText.text = "";
        statResultText.text = "";

        StartCoroutine(DisplayStats());

        
    }

    private IEnumerator DisplayStats()
    {
        
        //Accuracy or water shot
        yield return new WaitForSecondsRealtime(0.4f);
        
        //statHeaderText.text += "Accuracy:\n";
        //statResultText.text += $"{Mathf.Ceil(WaterCollision.waterHitCount / GenerateWater.waterShotCount)}%\n";
        statHeaderText.text += "Water shot:\n";
        statResultText.text += $"{GenerateWater.waterShotCount}\n";

        //Possible Rewards
        yield return new WaitForSecondsRealtime(0.4f);
        statHeaderText.text += "Possible Rewards:\n";
        statResultText.text += $"${CitizenManager.possibleRewards}\n";

        //Rewards Collected
        yield return new WaitForSecondsRealtime(0.4f);
        statHeaderText.text += "Rewards Collected:\n";
        statResultText.text += $"${CitizenManager.rewardsCollected}\n";

        //Times hit
        yield return new WaitForSecondsRealtime(0.4f);
        statHeaderText.text += "Times Hit:\n";
        statResultText.text += $"{Player.timesHit}\n";

        //Times Midge hit
        yield return new WaitForSecondsRealtime(0.4f);
        statHeaderText.text += "Times Midge Hit:\n";
        statResultText.text += $"{CitizenManager.timesMidgeHit}\n";

        //Most Umbrellas Stacked
        yield return new WaitForSecondsRealtime(0.4f);
        statHeaderText.text += "Most Umbrellas Stacked:\n";
        statResultText.text += $"{Umbrella.mostActiveUmbrellasEver}\n";

        //Replay Button
        yield return new WaitForSecondsRealtime(0.4f);
        replayButton.SetActive(true);

    }
}

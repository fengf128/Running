using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ExtractionResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text detailsText;
    [SerializeField] private bool pauseGame = true;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void ShowSuccess(GameObject player, string itemName)
    {
        if (resultPanel == null || titleText == null || detailsText == null)
        {
            Debug.LogError("Extraction result UI references are not assigned.", this);
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player is missing from the extraction result.", this);
            return;
        }

        Health health = player.GetComponent<Health>();
        PlayerHydration hydration = player.GetComponent<PlayerHydration>();

        string healthValue = health == null
            ? "--"
            : $"{Mathf.CeilToInt(health.CurrentHealth)}/{Mathf.CeilToInt(health.MaxHealth)}";

        string hydrationValue = hydration == null
            ? "--"
            : $"{Mathf.CeilToInt(hydration.CurrentHydration)}/{Mathf.CeilToInt(hydration.MaxHydration)}";

        titleText.text = "撤离成功";
        detailsText.text =
            $"任务物品：{itemName}\n" +
            $"生命：{healthValue}\n" +
            $"饮水：{hydrationValue}";

        resultPanel.SetActive(true);

        if (pauseGame)
        {
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex < 0)
        {
            Debug.LogError("Add the current scene to Scenes In Build before restarting.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void OnDestroy()
    {
        if (pauseGame && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}

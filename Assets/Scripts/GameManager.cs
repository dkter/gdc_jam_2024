using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private float enemySpawnFrequency = 1f; // seconds between enemy spawn
    private float enemySpawnClock = 0f;
    public GameObject enemyPrefab;

    private void Awake()
    {
        // Ensure that there is only one GameManager instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this GameManager between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Game Started");
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");
        Time.timeScale = 0; 
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        Time.timeScale = 1; 
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#endif
    }

    private void Update()
    {
        // Example of handling input for pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        enemySpawnClock += Time.deltaTime;
        if (enemySpawnClock > enemySpawnFrequency)
        {
            enemySpawnClock = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        float x = Random.Range(-1f, 1f); 
        float y = Random.Range(-1f, 1f);
        Vector3 direction = new Vector3(x, y, 0).normalized;

        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = 10f * direction;
    }


}
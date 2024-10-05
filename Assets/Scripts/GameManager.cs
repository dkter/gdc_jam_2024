using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    private float enemySpawnFrequency = 1f; // seconds between enemy spawn
    private float enemySpawnClock = 0f;
    
    public GameObject enemyPrefab;
    public GameObject wallPrefab;
    private List<Enemy> enemies = new List<Enemy>();
    private int enemyUpdateIndex = 0;
    public GameObject enemyParent;

    public GameObject player;
    public GameObject camera;

    public GameObject mapParent;

    private void Awake()
    {
        // Ensure that there is only one GameManager instance
        if (I == null)
        {
            I = this;
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

        //update enemy trajectories one enemy per frame
        if (enemies.Count > 0)
            enemies[enemyUpdateIndex].UpdateTarget();

        enemyUpdateIndex++;
        if (enemyUpdateIndex > enemies.Count-1)
        {
            enemyUpdateIndex = 0;
        }


    }

    private void UpdateMap()
    {
        
    }

    void Summon(DrawingIndicator drawingIndicator)
    {
        if (drawingIndicator.shape == Shape.Circle)
        {
            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.transform.position = drawingIndicator.shapeCentre;
        } else if (drawingIndicator.shape == Shape.Line)
        {
            GameObject newWall = Instantiate(wallPrefab);
            newWall.transform.position = new Vector3(
                drawingIndicator.shapeCentre.x,
                drawingIndicator.shapeCentre.y,
                -1f
            );
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1f, 0, 0), drawingIndicator.shapeVector);
            newWall.transform.rotation = rotation;
        }
    }

    void SpawnEnemy(Vector3? pos = null)
    {
        Enemy newEnemy;
        newEnemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        newEnemy.gameObject.transform.parent = enemyParent.transform;

        if (pos == null)
        {
            // randomize position
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            Vector3 direction = new Vector3(x, y, 0).normalized;

            newEnemy.gameObject.transform.position = player.transform.position + 10f * direction;

        }
        else
        {
            // or place the enemy where you want
            newEnemy.gameObject.transform.position = (Vector3)pos;
        }

        // store the enemy 
        enemies.Add(newEnemy);
        print(newEnemy);
    }


}

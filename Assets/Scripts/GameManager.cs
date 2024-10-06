using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float initMana = 100;
    public float wallCost = 15;
    public float circleCost = 25;

    public static GameManager I { get; private set; }
    private float enemySpawnFrequency = 1f; // seconds between enemy spawn
    private float enemySpawnClock = 0f;
    
    public GameObject enemyPrefab;
    public GameObject wallPrefab;
    public GameObject bombPrefab;
    public GameObject turretPrefab;
    private List<Enemy> enemies = new List<Enemy>();
    private int enemyUpdateIndex = 0;
    public GameObject enemyParent;

    public GameObject player;
    private Player playerScript;
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

        playerScript = player.GetComponent<Player>();
        playerScript._mana = new Mana();
        playerScript._mana.SetMax(initMana);
        playerScript._mana.SetMana(initMana);
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

    private void SummonFailSplash()
    {
        Debug.Log("Summon Failed!");
    }

    void Summon(DrawingIndicator drawingIndicator)
    {
        Debug.Log("shape: " + drawingIndicator.shape.ToString());
        if (drawingIndicator.shape == Shape.Circle)
        {
            var result = playerScript._mana.ConsumeMana(circleCost);
            if (result == SummonState.Success)
            {
                GameObject newBomb = Instantiate(bombPrefab);
                newBomb.transform.position = drawingIndicator.shapeCentre;
                newBomb.GetComponent<Bomb>().enemyParent = enemyParent;
            }
            else
            {
                SummonFailSplash();
            }

        }
        else if (drawingIndicator.shape == Shape.Line)
        {
            var result = playerScript._mana.ConsumeMana(wallCost);
            if (result == SummonState.Success)
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
            else
            {
                SummonFailSplash();
            }
        } else if (drawingIndicator.shape == Shape.Rectangle) {
            var result = playerScript._mana.ConsumeMana(circleCost);
            if (result == SummonState.Success)
            {
                GameObject newTurret = Instantiate(turretPrefab);
                newTurret.transform.position = drawingIndicator.shapeCentre;
                newTurret.GetComponent<Turret>().enemyParent = enemyParent;
            }
            else
            {
                SummonFailSplash();
            }
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
            float x = UnityEngine.Random.Range(-1f, 1f);
            float y = UnityEngine.Random.Range(-1f, 1f);
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

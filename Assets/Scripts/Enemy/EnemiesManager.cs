using System.Collections.Generic;
using UnityEngine;
public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;

    // public GameObject[] enemies;
    public List<GameObject> enemies = new List<GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("to manny enemiesManager in scene, destroying: " + gameObject.name);
            Destroy(this);
        }
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("enemy"));
    }

    public void AddTarget(GameObject target)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //checks if the object has been killed on not then assign the player to the target
            if (enemies[i] != null)
            {
                AIManager ai = enemies[i].GetComponent<AIManager>();
                if (ai.player == null)
                {
                    ai.player = target;
                }
                else if (ai.player2 == null)
                {
                    ai.player2 = target;
                }
            }
        }
    }

    //checks if all the enemies are dead
    public void RemoveEnemy(GameObject Enemy)
    {
        enemies.Remove(Enemy);

        if (enemies.Count <= 0)
        {
            //Debug.Log("ez win");
            //UIManager.instance.ChangeScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        //checks if every vip is dead
        bool missionComplete = true;
        for (int i = 0; i < enemies.Count;i++) { if (enemies[i].GetComponent<AIManager>().vip) { missionComplete = false; } }
        //checks if it is a kill all vip mission
        if (Elevator.instance != null && Elevator.instance.vipMission)
            Elevator.instance.objectiveDone = missionComplete;
    }
}

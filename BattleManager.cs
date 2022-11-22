using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private bool battleActive;
    public GameObject battleScene;
    public Transform[] playerPositions;

    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;
    public List<BattleChar> activeBattlers = new List<BattleChar>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) {
            BattleStart(new string[] {"Spider", "Sorcerer", "Goblin"});
        }
    }

    public void BattleStart(string[] enemiesToSpawn) {
        if(!battleActive) {
            //Battle is now active
            battleActive = true;

            //Make sure player can't move
            GameManager.instance.battleActive = true;

            //Center the postion of the Camera
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.y);
            
            //Activates the Battle Scene
            battleScene.SetActive(true);

            //Turns on the battle scene music
            AudioManager.instance.PlayBGM(0);

            for(int i = 0; i < playerPositions.Length; i++) {
                if(GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                    for(int k = 0; k < playerPrefabs.Length; k++) {
                        if(playerPrefabs[k].charName == GameManager.instance.playerStats[i].charName) {
                            BattleChar newPlayer = Instantiate(playerPrefabs[k], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);
                        }
                    }
                }
            }
        }
    }
}

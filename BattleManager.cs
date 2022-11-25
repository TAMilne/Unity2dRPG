using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int currentTurn;
    public bool turnWaiting;

    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;
    public DamageNumber damageNumber;

    public Text[] playerName, playerHP, playerMP;

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
            BattleStart(new string[] {"Troglodyte", "Spider", "Wasp", "Turtle"});
        }

        if(battleActive) {
            if(turnWaiting) {
                if(activeBattlers[currentTurn].isPlayer) {
                    uiButtonsHolder.SetActive(true);
                } else {
                    uiButtonsHolder.SetActive(false);

                    //Enemy Should Attack
                    StartCoroutine(EnemyMoveCo());
                }
            }
            if(Input.GetKeyDown(KeyCode.N)) {
                NextTurn();
            }
        }
    }

    public void BattleStart(string[] enemiesToSpawn) {
        if(!battleActive) {
            //Battle is now active
            battleActive = true;

            //Make sure player can't move
            GameManager.instance.battleActive = true;

            //Center the postion of the Camera
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            
            //Activates the Battle Scene
            battleScene.SetActive(true);

            //Turns on the battle scene music
            AudioManager.instance.PlayBGM(0);

            //Sets player position on Battle Scene and loads their stats
            for(int i = 0; i < playerPositions.Length; i++) {
                if(GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                    for(int k = 0; k < playerPrefabs.Length; k++) {
                        if(playerPrefabs[k].charName == GameManager.instance.playerStats[i].charName) {
                            BattleChar newPlayer = Instantiate(playerPrefabs[k], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);

                            CharStats thePlayer = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHp = thePlayer.currentHP;
                            activeBattlers[i].maxHp = thePlayer.maxHP;
                            activeBattlers[i].currentMp = thePlayer.currentMP;
                            activeBattlers[i].maxMp = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].weaponPwr = thePlayer.wpnPwr;
                            activeBattlers[i].armrPwr = thePlayer.armorPwr;
                        }
                    }
                }
            }

            //Loop through enemies passed as arg
            for(int i = 0; i < enemiesToSpawn.Length; i++) {
                //If it's blank then don't do anything
                if(enemiesToSpawn[i] != "") {
                    //Loop through enemy prefab list
                    for(int k = 0; k < enemyPrefabs.Length; k++) {
                        //If entry in prefab equals enemy to spawn, then do it
                        if(enemyPrefabs[k].charName == enemiesToSpawn[i]) {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[k], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }
            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count);
            UpdateUIStats();
        }
    }

    public void NextTurn() {
        currentTurn++;
        if(currentTurn >= activeBattlers.Count) {
            currentTurn = 0;
        }
        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle() {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for(int i = 0; i < activeBattlers.Count; i++) {
            if(activeBattlers[i].currentHp < 0) {
                activeBattlers[i].currentHp = 0;
            }


            if(activeBattlers[i].currentHp == 0) {
                //Is Dead
            } else {
                if(activeBattlers[i].isPlayer) {
                    allPlayersDead = false;
                } else {
                    allEnemiesDead = false;
                }
            }
        }

        if(allEnemiesDead || allPlayersDead) {
            if(allEnemiesDead) {
                //end battle in victory
            } else {
                //end battle in defeat
            }

            battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false;
        }
    }

    public IEnumerator EnemyMoveCo() {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack() {
        List<int> players = new List<int>();

        for(int i = 0; i < activeBattlers.Count; i++) {
            if(activeBattlers[i].isPlayer && activeBattlers[i].currentHp > 0) {
                players.Add(i);
            }
        }

        int selectedTarget = players[Random.Range(0, players.Count)];

        //Test
        //activeBattlers[selectedTarget].currentHp -= 20;

        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;
        for(int i = 0; i < movesList.Length; i++) {
            if(movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]) {
                movePower = movesList[i].movePower;
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
            }
        }
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower) {
        float attackPower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPwr;
        float defensePower = activeBattlers[target].defence + activeBattlers[target].armrPwr;
         
        float damageCalc = (attackPower/defensePower) * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);

        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToGive + ") damage to " + activeBattlers[target].charName);
        activeBattlers[target].currentHp -= damageToGive;
        Instantiate(damageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damageToGive);

        UpdateUIStats();
    }

    public void UpdateUIStats() {
        for(int i = 0; i < playerName.Length; i++) {
            if(activeBattlers.Count > i) {
                if(activeBattlers[i].isPlayer) {
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = playerData.currentHp + "/" + playerData.maxHp;
                    playerMP[i].text = playerData.currentMp + "/" + playerData.maxMp;
                } else {
                    playerName[i].gameObject.SetActive(false);
                }
            } else {
                playerName[i].gameObject.SetActive(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    public string charName;
    public int playerLevel = 1;
    public int currentEXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseExp = 1000;

    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP = 30;
    public int [] mpLvlincrease;
    public int strength;
    public int defence;
    public int wpnPwr;
    public int armorPwr;
    public string equippedWpn;
    public string equippedArm;
    public Sprite charImage;




    // Start is called before the first frame update
    void Start()
    {
        expToNextLevel = new int[maxLevel];
        expToNextLevel[1] = baseExp;

        for(int i=2; i < expToNextLevel.Length; i++) {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i-1] * 1.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) {
            AddExp(500);
        }
    }

    public void AddExp(int expToAdd) {
        currentEXP += expToAdd;
        if(currentEXP > expToNextLevel[playerLevel]) {
            currentEXP -= expToNextLevel[playerLevel];
            playerLevel++;

            //Determine whether to add Strength or Defense based on odd or even
            if(playerLevel%2 == 0) {
                strength++;
            } else {
                defence++;
            }

            //Different way to increment stats
            maxHP = Mathf.FloorToInt(maxHP * 1.05f);
            currentHP = maxHP;

            maxMP += mpLvlincrease[playerLevel];
            currentMP = maxMP;
        }
    }
}

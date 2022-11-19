using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerName;
    public bool[] questMarkerComplete;
    public static QuestManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        questMarkerComplete = new bool[questMarkerName.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) {
           Debug.LogError(CheckIfComplete("quest test"));
           MarkQuestComplete("quest test");
           MarkQuestIncomplete("fight the beast");
        }

        if(Input.GetKeyDown(KeyCode.O)) {
           SaveQuestData();
        }

        if(Input.GetKeyDown(KeyCode.P)) {
            LoadQuestData();
        }
    }

    public int GetQuestNumber(string questToFind) {
        for(int i = 0; i< questMarkerName.Length; i++) {
            if(questMarkerName[i] == questToFind) {
                return i;
            }
        }
        //Finds the empty slot in the quest array
        Debug.LogError("Quest: " + questToFind + " doesn't exist.");
        return 0;
    }

    public bool CheckIfComplete(string questToCheck) {
        if(GetQuestNumber(questToCheck) !=0)   {
            return questMarkerComplete[GetQuestNumber(questToCheck)];
        }
        return false;
    }

    public void MarkQuestComplete(string questToMark) {
        questMarkerComplete[GetQuestNumber(questToMark)] = true;

        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark) {
        questMarkerComplete[GetQuestNumber(questToMark)] = false;

        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects() {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();

        if(questObjects.Length > 0) {
            for(int i = 0; i < questObjects.Length; i++) {
                questObjects[i].CheckCompletion();
            }
        }
    }

    public void SaveQuestData() {
        for( int i = 0; i < questMarkerName.Length; i++) {
            if(questMarkerComplete[i]) {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerName[i], 1);
            } else {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerName[i], 0);
            }
        }
    }

    public void LoadQuestData() {
        for(int i = 0; i < questMarkerName.Length; i++) {
            int valueToSet = 0;
            if(PlayerPrefs.HasKey("QuestMarker_" + questMarkerName[i])) {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerName[i]);
            }
            if(valueToSet == 0) {
                questMarkerComplete[i] = false;
            } else {
                questMarkerComplete[i] = true; 
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour
{   
    public string[] lines;

    private bool canActivate;

    public bool isPerson = true;

    //Quest Variables
    public bool shouldActivatQuest;
    public string questToMark;
    public bool markComplete;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canActivate && Input.GetButtonDown("Fire1") && !DialogueManager.instance.dialogBox.activeInHierarchy) {
            DialogueManager.instance.ShowDialog(lines, isPerson);
            DialogueManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete);
        }   
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            canActivate=true;
        }
    }

        private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player") {
            canActivate=false;
        }
    }
}

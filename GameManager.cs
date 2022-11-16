using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public CharStats[] playerStats;

    public bool gameMenuOpen, dialogActive, fadingBetweenAreas;

    public string[] itemsHeld;
    public int[] numberOfItems;
    public Item[] referenceItems;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
      if(gameMenuOpen || dialogActive || fadingBetweenAreas) {
        PlayerController.instance.canMove = false;
      } else {
        PlayerController.instance.canMove = true;
      }

      if(Input.GetKeyDown(KeyCode.J)) {
        AddItem("Iron Armor");
        //AddItem("Blah");

        RemoveItem("HP Potion");
        RemoveItem("Iron Armor");
        //RemoveItem("whoops");
      }
    }

    public Item GetItemDetails(string item) {
        for(int i=0; i < referenceItems.Length; i++) {
          if(referenceItems[i].itemName == item) {
            return referenceItems[i];
          }
        }
        return null;
    }

    public void SortItems() {
      bool itemAfterSpace = true;

      while(itemAfterSpace) {
        itemAfterSpace = false;

        for(int i = 0; i < itemsHeld.Length - 1; i++) {
          if(itemsHeld[i] == "") {

            itemsHeld[i] = itemsHeld[i + 1];
            itemsHeld[i + 1] = "";

            numberOfItems[i] = numberOfItems[i + 1];
            numberOfItems[i + 1] = 0;

            //Testing for while loop controller
            if(itemsHeld[i] != "") {
              itemAfterSpace = true;
            }
          }
        }
      }
    }

    public void AddItem(string itemToAdd) {
      int newItemPosition = 0;
      bool foundSpace = false;
      SortItems();
      for(int i=0; i<itemsHeld.Length; i++){
        if(itemsHeld[i] == "" || itemsHeld[i] == itemToAdd) {
          newItemPosition = i;
          i = itemsHeld.Length;
          foundSpace = true;
        }
      }

      
      if(foundSpace) {
        //checks to make sure item is actually an item
        bool itemExists = false;
        for(int i = 0; i < referenceItems.Length; i++) {
          if(referenceItems[i].itemName == itemToAdd) {
            itemExists = true;
            i = referenceItems.Length;
          }
        }

        //If the item exists add it to position
        if(itemExists) {
          itemsHeld[newItemPosition] = itemToAdd;
          numberOfItems[newItemPosition]++;
        } else {
          Debug.LogError(itemToAdd + " Doesn't Exist!");
        }
      }
      GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemToDelete) {
      bool foundItem = false;
      int itemPosition = 0;

      for(int i = 0; i < itemsHeld.Length; i++) {
        if(itemsHeld[i] == itemToDelete) {
          foundItem = true;
          itemPosition = i;

          i = itemsHeld.Length;
        }
      }

      if(foundItem) {
        numberOfItems[itemPosition]--;
        if(numberOfItems[itemPosition] <= 0) {
          itemsHeld[itemPosition] = "";
        }
        GameMenu.instance.ShowItems();
      } else {
        Debug.LogError("Couldn't find " + itemToDelete);
      }

    }
}

using System.Collections;
using System.Collections.Generic;
using SkeletonEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] Image opponent;
    [SerializeField] GameObject reviveWindow;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject charStats;
    [SerializeField] Slider lifeBar;
    [SerializeField] Image manaBar;
    [SerializeField] GameObject inventory;
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject controls;

    [SerializeField] Image[] storedItems;

    [SerializeField] GameObject skills;
    [SerializeField] GameObject deathscreen;
    [SerializeField] GameObject combatScreen;

    [SerializeField] Image[] items;
    [SerializeField] Image[] minions;

    [SerializeField] Inventory inventoryScript;
    [SerializeField] PlayerController play;
    [SerializeField] Button returnNec;
    [SerializeField] Button Creatures;
    bool inFight = false;
    int inventorystate;

    //note for myself: Create array of gameObjects to store UI windows for easier access
    void Start()
    {
        reviveWindow.SetActive(false);
        if (SceneSaver.Instance.runs > 0) { menu.SetActive(false); StartGame(); }
        else { menu.SetActive(true); charStats.SetActive(false); }
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        inventory.SetActive(false);
        skills.SetActive(false);
        deathscreen.SetActive(false);
        combatScreen.SetActive(false);
        returnNec.gameObject.SetActive(false);
        controls.SetActive(false);
        foreach (Image item in storedItems) item.enabled = false;
    }

    //Update inventory slots (note for myself: Update method to actively access item within array for better structuring. / Implement amount count)
    public void UpdateInventory(int slot, int item)
    {
        storedItems[slot].enabled = !storedItems[slot].enabled;
    }

    public void ReviveWindow(bool status, int type)
    {
        reviveWindow.SetActive(status);
        SetRevPic(type);
    }

    //close or open inventory according to its set number ( 1 for items 2 for minions)
    public void OpenInventory(int set)
    {
        inventorystate = set;
        if (set == 0) 
        { 
            inventory.SetActive(false); 
            if(inFight == true) combatScreen.SetActive(true);
        }

        else if (set == 1)
        {
            if (inFight) combatScreen.SetActive(false);
            if (inventoryScript.CheckInventory() == true) { storedItems[0].enabled = true; storedItems[0].sprite = items[0].sprite; }
            inventory.SetActive(true);
        }
        else
        {
            if(inFight) combatScreen.SetActive(false);
            List<int> temp = inventoryScript.CheckMinions();
            if(temp.Count != 0) {
                for (int i = 0; i < temp.Count; i++) //set sprite according to minion level
                {
                    if (temp[i] < 3)
                    {
                        storedItems[i].enabled = true;
                        storedItems[i].sprite = minions[0].sprite;
                    }
                    else if (temp[i] >= 3 && temp[i] < 5)
                    {
                        storedItems[i].enabled = true;
                        storedItems[i].sprite = minions[1].sprite;
                    }
                    else if (temp[i] >= 5)
                    {
                        storedItems[i].enabled = true;
                        storedItems[i].sprite = minions[2].sprite;
                    }
                }
            }
            inventory.SetActive(true);
        }
    }

    public void showDeathScreen()
    {
        deathscreen.SetActive(true);
    }

    public void ToggleCombatScreen(bool bswitch)
    {
        combatScreen.SetActive(bswitch);
        Battlemode(bswitch);
        play.SetFight(bswitch);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Starts intro screen once at the beginning of the game. (access via UI-button)
    public void Intro()
    {
        menu.SetActive(false);
        startScreen.SetActive(true);
    }

    public void StartGame()
    {
        charStats.SetActive(true);
        startScreen.SetActive(false);
    }

    //Sets reference picture for revive window after defeating an opponent
    public void SetRevPic(int level)
    {
        if (level < 3) opponent.sprite = minions[0].sprite;
        else if (level < 5) opponent.sprite = minions[1].sprite;
        else opponent.sprite = minions[2].sprite;
    }

    //set health and max health on UI side
    public void SetMaxHealth(int health)
    {
        lifeBar.maxValue = health;
        lifeBar.value = health;
    }

    //set health on UI side (for life recovery)
    public void SetHealth(int health)
    {
        lifeBar.value = health;
    }

    public void Battlemode(bool stat)
    {
        inFight = stat;
    }

    //Use item in inventory
    public void UseInventory(int loc)
    {
        inventoryScript.AccessInv(inventorystate, loc);
        OpenInventory(0);
    }

    //switch between minion inventory button and return necromancer button in combat UI
    public void SwitchButton(bool stat)
    {
        if (stat)
        {
            Creatures.gameObject.SetActive(false);
            returnNec.gameObject.SetActive(true);
        } else
        {
            Creatures.gameObject.SetActive(true);
            returnNec.gameObject.SetActive(false);
        }

    }

    public void ShowEndScreen()
    {
        endScreen.SetActive(true);
    }

    public void ToggleControls(bool stat)
    {
        controls.SetActive(stat);
    }
}

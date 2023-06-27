using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<int> minions = new List<int>();
    List<int> items = new List<int>();
    List<int> amount = new List<int>();
    [SerializeField] UIController uIController;
    [SerializeField] PlayerController player;

    public void Awake()
    {
        if(SceneSaver.Instance != null)
        {
            minions = SceneSaver.Instance.min;
            items = SceneSaver.Instance.it;
            amount = SceneSaver.Instance.am;
        }
    }

    public void AddItem(int itemtype)
    {
        if(items.Contains(itemtype))
        {
            int loc = items.IndexOf(itemtype);
            amount[loc]++;
        }
        else
        {
            items.Add(itemtype);
            amount.Add(1);
            uIController.UpdateInventory((items.Count - 1), itemtype);
        }
    }


    public void RemoveItem(int itemtype)
    {
        int loc = items.IndexOf(itemtype);
        if (amount[loc] > 1) amount[loc]--;
        else
        {
            amount.RemoveAt(loc);
            items.RemoveAt(loc);
            uIController.UpdateInventory(loc, itemtype);
        }
    }

    public void AddMinion(int level)
    {
        minions.Add(level);
    }

    public void RemoveMinion(int level)
    {
        int i = 0;
        foreach(int minion in minions)
        {
            if (minion == level)
            {
                minions.Remove(i);
                break;
            }
            else i++;
        }
    }

    //Save properties over scene changes
    public void SaveProperties()
    {
        SceneSaver.Instance.min = minions;
        SceneSaver.Instance.it = items;
        SceneSaver.Instance.am = amount;
        player.SaveProgress();
    }

    public bool CheckInventory()
    {
        if (items.Count > 0) return true;
        else return false;
    }

    public List<int> CheckMinions()
    {
        return minions;
    }

    //access inventory array
    public void AccessInv(int set, int loc)
    {
        if (set == 1)
        {
            RemoveItem(3); //fixed value as just one item exists so far
            player.RecieveDamage(-100);
        }
        else if (set == 2)
        {
            player.SwitchCharacter(minions[loc], false);
        }
    }
}

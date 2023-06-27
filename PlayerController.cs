using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLevel = 1;
    int lifeBar;
    bool main = true;

    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    Vector3 targetRoomPos;
    Vector3 prevTargetRoomPos;
    Vector3 targetRotation;

    int inv = 1;
    int min = 2;

    [SerializeField] Inventory inventory;
    [SerializeField] UIController ui;
    [SerializeField] Combat combat;
    [SerializeField] Cooldown codo;
    [SerializeField] bool uiact = false;
    [SerializeField] int range = 5;
    bool walk = true;
    bool fight = true;
    public bool cd = false;
    int offset = 1;
    int expCap = 200;
    int expAm = 0;

    int level;
    int life;

    void Start()
    {
        targetRoomPos = Vector3Int.RoundToInt(transform.position);

        if (SceneSaver.Instance != null && SceneSaver.Instance.lev != 0)
        {
            playerLevel = SceneSaver.Instance.lev;
            expAm = SceneSaver.Instance.exp;
        }
        lifeBar = 100 * playerLevel;
        ui.SetMaxHealth(lifeBar);

        if (SceneSaver.Instance.runs > 0) StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !fight && !uiact) RotateLeft();
        if (Input.GetKeyDown(KeyCode.D) && !fight && !uiact) RotateRight();
        if (Input.GetKeyDown(KeyCode.S) && !fight && !uiact) RotateBack();
        if (Input.GetKeyDown(KeyCode.W) && walk == true && !fight && !uiact) MoveForward();
        if (Input.GetKeyDown(KeyCode.I) && !fight && !uiact) { ui.OpenInventory(inv); UIOpen(true); }
        if (Input.GetKeyDown(KeyCode.M) && !fight && !uiact) { ui.OpenInventory(min); UIOpen(true); }
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        Vector3 direction = Vector3.forward;
        Ray checker = new Ray(transform.position, transform.TransformDirection(direction * range));

        if(Physics.Raycast(checker, out RaycastHit hit, range))
        {
            if (hit.collider.tag == "Environment")
            {
                walk = false;
            }
            else if (hit.collider.tag == "Enemy" || hit.collider.tag == "Boss")
            {
                GameObject opponent = hit.transform.gameObject;
            }
            else if (hit.collider.tag == "Space") { walk = true; }
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MoveFowardFunc(Vector3 pos)
    {
        for (int i = 0;  i < offset; i++)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * transitionSpeed);
        }
    }

    void MovePlayer()
    {
        if (true)
        {
            prevTargetRoomPos = targetRoomPos;
            Vector3 targetPosition = targetRoomPos;

            if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
            if (targetRotation.y < 0f) targetRotation.y = 270f;

            MoveFowardFunc(targetPosition);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
        }
    }

    //Damage on player or minion
    public void RecieveDamage(int damage)
    {
        lifeBar -= damage;
        ui.SetHealth(lifeBar);
        if (lifeBar <= 0 && main) { ui.ToggleCombatScreen(false); ui.showDeathScreen(); }
        else if (lifeBar <= 0 && !main) { SwitchCharacter(1, true); }
    }

    //LevelUP with life recovery
    public void LevelUP()
    {
        playerLevel++;
        lifeBar = 100 * playerLevel;
        ui.SetMaxHealth(lifeBar);
    }

    void RotateLeft() { if (AtRest) targetRotation -= Vector3.up * 90f; }
    void RotateRight() { if (AtRest) targetRotation += Vector3.up * 90f; }
    void RotateBack() { if (AtRest) targetRotation += Vector3.up * 180f; }
    void MoveForward() { if (AtRest) targetRoomPos += transform.forward * offset; }

    //check if character is in action 
    bool AtRest
    {
        get
        {
            if ((Vector3.Distance(transform.position, targetRoomPos) < 0.5f) &&
                (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.5f) && uiact == false) return true;
            else return false;
        }
    }

    public void UIOpen(bool stat)
    {
        uiact = stat;
    }
    public void SetOffset(int val)
    {
        offset = val;
    }

    //Attack move with damage calculation and cooldown
    public void Attack()
    {
        if (!cd)
        {
            int damage = (int)Random.Range(10.0f * playerLevel, 20.0f * playerLevel);
            combat.PlayerAttack(damage);
            cd = true;
            codo.SetTimer();
        } 
    }

    //Start game or return from UI usage
    public void StartGame()
    {
        fight = false;
    }


    public void ResetCD()
    {
        cd = false;
    }

    public void GainExperience(int amount)
    {
        expAm += amount;
        if(expAm >= expCap && main)
        {
            expCap = expCap * 2;
            expAm = 0;
            LevelUP();
        }
    }

    public bool IsMinion()
    {
        return !main;
    }

    public void returnNecro()
    {
        SwitchCharacter(1, true);
    }

    
    public void SwitchCharacter(int temp, bool returnNec)
    {
        
        if (main)
        {
            ui.SwitchButton(main);
            life = lifeBar;
            level = playerLevel;

            playerLevel = temp - 1;
            LevelUP();
            main = false;
            if (fight) 
            {
                combat.PlayerAttack(0); //skip one turn for character switch
                cd = true;
                codo.SetTimer();
            }
        } else if (!returnNec)
        {
            playerLevel = temp - 1;
            LevelUP();
            if (fight)
            {
                combat.PlayerAttack(0); //skip one turn for character switch
                cd = true;
                codo.SetTimer();
            }
        } else if (returnNec)
        {
            ui.SwitchButton(main);
            playerLevel = level;
            lifeBar = life;
            main = true;
        }
    }

    //Save progress over scene
    public void SaveProgress()
    {
        if (main) SceneSaver.Instance.lev = playerLevel;
        else SceneSaver.Instance.lev = level;
        SceneSaver.Instance.exp = expAm;
        ui.Restart();
    }

    public void SetFight(bool status)
    {
        fight = status;
    }
}

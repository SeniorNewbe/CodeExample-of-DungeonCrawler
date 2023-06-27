using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletonEditor
{

    public class OpponentController : MonoBehaviour
    {
        [SerializeField] Animator animator;

        PlayerController playerController;
        UIController uIController;
        Inventory inventoryController;
        Transform target;
        Combat combatscript;

        public int monsterLevel = 1;
        int lifeBar;

        private void Awake()
        {
            lifeBar = 100 * monsterLevel;
        }

        void Start() {
            animator = GetComponentInChildren<Animator>();
            var temp = GameObject.Find("GlobalSystems");
            combatscript = temp.GetComponent<Combat>();
            inventoryController = temp.GetComponent<Inventory>();
            temp = GameObject.Find("Player 1");
            playerController = temp.GetComponent<PlayerController>();
            target = temp.GetComponent<Transform>();
            temp = GameObject.Find("Canvas");
            uIController = temp.GetComponent<UIController>();
        }

        private void Update()
        {
            float dist = Vector3.Distance(target.position, transform.position);
            if(dist >= 3)
            {
                var dir = target.position - transform.position;
                dir.y = 0;
                var rotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 500);
            } 
        }

        //Opponent takes damage, if it survives, it strikes back
        public void TakeDamage(int Damage)
        {
            lifeBar -= Damage;
            if (lifeBar >= 0)
            {
                if (monsterLevel < 5) animator.SetTrigger("Hit1");
                else animator.SetTrigger("take_damage");
                StartCoroutine(Attack());
            }
            else
            {
                foreach (Transform child in transform) //check for boss tags and the dungeon level
                {
                    if(child.tag == "Boss" && SceneSaver.Instance != null)
                    {
                        SceneSaver.Instance.runs++;
                        if (SceneSaver.Instance.runs >= 3) uIController.ShowEndScreen();
                        else inventoryController.SaveProperties();
                    }else child.tag = "Space"; 
                }
                playerController.GainExperience(100 * monsterLevel);
                if (monsterLevel < 5) animator.SetTrigger("Fall1"); //play death animation according to level
                else animator.SetTrigger("death");
                uIController.ReviveWindow(true, monsterLevel);
            }
        }

        //Opponent attack with delays for animations
        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.5f);
            if (monsterLevel < 5) animator.SetTrigger("Attack1h1");
            else 
            {
                int dec = Random.Range(0, 2);
                if(dec < 1) animator.SetTrigger("attack1");
                else animator.SetTrigger("attack2");
            }
            int damage = (int) Random.Range(1.0f * monsterLevel, 10.0f * monsterLevel);
            playerController.RecieveDamage(damage);
        }

        //Revive minion while playing animation
        public void ReviveMinion()
        {
            animator.SetTrigger("Up");
            StartCoroutine(Revive());
        }

        //Store minion in inventory before destroy object
        IEnumerator Revive()
        {
            inventoryController.AddMinion(monsterLevel);
            yield return new WaitForSeconds(2);
            DestroyMe();
        }

        //accessed via GameObject
        public void SetLevel(int level) => monsterLevel = level;

        //Accessed via GameObject
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") combatscript.StartCombat(gameObject);
            else if (other.tag == "Boss") DestroyMe();
        }

        public void DestroyMe()
        {
            Destroy(gameObject);
        }
    }
}
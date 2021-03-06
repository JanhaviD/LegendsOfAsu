﻿using System.Collections;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    public int playerAttackPower = 3;
    public int basicAttackPower = 2;
    public int powerAttackPower = 3;
    public int allSideAttackPower = 1;

    public float basicAttackRangeX = 1;
    public float powerAttackRangeX = 3;
    public float allSideAttackRadius = 1;
    public float attackRangeY = 1;

    public float powerAttackPressTime = 5f;
    private float powerAttackTimer = 0f;

    public float allSideAttackPressTime = 5f;
    private float allSideAttackTimer = 0f;

    public int baseScore = 100;
    public int baseScoreMultiplerHits = 10;
    public float maxTimeWithoutHit = 50f;
    private int hitCount = 0;
    //private int ScoreMultiplerHits = 10;
    private int scoreMultipler = 1;
    private float hitTimer = 0f;
    private bool didPlayerHit = false;

    public Transform baiscAttackPos;
    public Transform powerAttackPos;
    public Transform allSideAttackPos;

    public bool enemyInRange = false;
    public bool defenseMode;
    public bool inAir;
    public bool isPlayerHit;
    public bool isPlayerDisable;
    public bool isAnimationPlaying = false;
    public int waitTime = 3;

    public Animator animator;
    public LayerMask whatIsEnemies;
    public GameManager gameManager;

    private EnemyAI enemy;
    private PlayerController pc;
    private bool playerFacingRight;

    private string basicAttackKey;
    private string allSideAttackKey;
    private string powerAttackKey;
    private string defenceKey;

    void Start()
    {
        //playerFacingRight = gameObject.GetComponent<PlayerController>().facingRight;
        pc = GetComponent<PlayerController>();

        defenseMode = false;

        if (pc.playerId == 1)
        {
            basicAttackKey = "Fire1Player1";
            allSideAttackKey = "Fire2Player1";
            powerAttackKey = "Fire1Player1";
            defenceKey = "DefencePlayer1";
        }
        else
        {
            basicAttackKey = "Fire1Player2";
            allSideAttackKey = "Fire2Player2";
            powerAttackKey = "Fire1Player2";
            defenceKey = "DefencePlayer2";
        }
    }

    void Update()
    {
        isPlayerDisable = pc.isPlayerDisable;
        isPlayerHit = pc.isPlayerHit;
        inAir = pc.isInAir;
        animator.SetBool("slashAttack", false);
        animator.SetBool("magicAttack", false);
        animator.SetBool("heavyAttack", false);
        animator.SetBool("defense", false);

        if (isPlayerDisable)
        {
            animator.SetBool("idle", true);
            return;
        }
        
        if (hitTimer >= maxTimeWithoutHit)
        {
            ResetScoreMultipler();
            hitTimer = 0f;
            hitCount = 0;
        }

        if (didPlayerHit)
        {
            hitTimer = 0f;
            didPlayerHit = false;
        }

        if (hitCount > baseScoreMultiplerHits * scoreMultipler)
        {
            hitCount = 0;
            hitTimer = 0;
            scoreMultipler++;
            //Debug.Log(scoreMultipler);
        }

        hitTimer += Time.deltaTime;


        if (Input.GetButtonUp(powerAttackKey))
        {
            animator.SetBool("slashAttack", true);
            Collider2D[] enemiesToHit = Physics2D.OverlapBoxAll(baiscAttackPos.position, new Vector2(basicAttackRangeX, attackRangeY), 0, whatIsEnemies);

            if (enemiesToHit.Length != 0)
            {
                enemy = enemiesToHit[0].gameObject.GetComponent<EnemyAI>();
                enemy.TakeDamage(playerAttackPower);

                gameManager.AddScore(baseScore * scoreMultipler);
                hitCount++;
                didPlayerHit = true;
            }
        }


        //all side attack
        if (Input.GetButton(allSideAttackKey) && !isPlayerHit && !isPlayerDisable)
        {
            allSideAttackTimer += Time.deltaTime;
            animator.SetBool("magicAttack", true);

            if (allSideAttackTimer >= allSideAttackPressTime)
            {
                //isAnimationPlaying = true;
                //StartCoroutine(WaitForAnimationComplete(waitTime));
                Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(allSideAttackPos.position, allSideAttackRadius, whatIsEnemies);
                for (int i = 0; i < enemiesToHit.Length; i++)
                {
                    enemy = enemiesToHit[i].gameObject.GetComponent<EnemyAI>();
                    enemy.TakeDamage(allSideAttackPower);

                    gameManager.AddScore(baseScore * scoreMultipler);
                    hitCount++;
                    didPlayerHit = true;
                }

                allSideAttackTimer = 0;
            }
        }

        //power attack
        //if (Input.GetButton(powerAttackKey) && !isPlayerHit && !isPlayerDisable)
        //{
        //    animator.SetBool("heavyAttack", true);
        //    powerAttackTimer += Time.deltaTime;

        //    if (powerAttackTimer >= powerAttackPressTime)
        //    {
        //        animator.SetBool("heavyAttack", true);
        //        Collider2D[] enemiesToHit = Physics2D.OverlapBoxAll(powerAttackPos.position, new Vector2(basicAttackRangeX, attackRangeY), 0, whatIsEnemies);
        //        for (int i = 0; i < enemiesToHit.Length; i++)
        //        {
        //            enemy = enemiesToHit[i].gameObject.GetComponent<EnemyAI>();
        //            enemy.TakeDamage(powerAttackPower);

        //            gameManager.AddScore(baseScore * scoreMultipler);
        //            hitCount++;
        //            didPlayerHit = true;
        //        }
        //        powerAttackTimer = 0;
        //    }
        //}
        //if (Input.GetButtonUp(powerAttackKey))
        //{
        //    if (powerAttackTimer < powerAttackPressTime)
        //    {
        //        animator.SetBool("slashAttack", true);
        //        Collider2D[] enemiesToHit = Physics2D.OverlapBoxAll(baiscAttackPos.position, new Vector2(basicAttackRangeX, attackRangeY), 0, whatIsEnemies);

        //        if (enemiesToHit.Length != 0)
        //        {
        //            enemy = enemiesToHit[0].gameObject.GetComponent<EnemyAI>();
        //            enemy.TakeDamage(playerAttackPower);

        //            gameManager.AddScore(baseScore * scoreMultipler);
        //            hitCount++;
        //            didPlayerHit = true;
        //        }
        //    }

        //    powerAttackTimer = 0f;
        //}


        if (Input.GetButton(defenceKey) && !isPlayerHit && !isPlayerDisable)
        {
            defenseMode = true;
            animator.SetBool("defense", true);
            //Debug.Log("defenseMode ON");
        }
        if (Input.GetButtonUp(defenceKey))
        {
            //Debug.Log("defenseMode OFF");
            defenseMode = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(baiscAttackPos.position, new Vector3(basicAttackRangeX, attackRangeY, 0));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(powerAttackPos.position, new Vector3(powerAttackRangeX, attackRangeY, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(allSideAttackPos.position, allSideAttackRadius);
    }

    public void ResetScoreMultipler()
    {
        //Debug.Log("reset multupler");
        scoreMultipler = 1;
    }
    IEnumerator WaitForAnimationComplete(float pauseDuration)
    {
        isAnimationPlaying = true;
        yield return new WaitForSeconds(pauseDuration + 0.1f);
        isAnimationPlaying = false;
    }
}

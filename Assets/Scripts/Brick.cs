
using System;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;

    public int Hitpoints = 1;

    public ParticleSystem DestroyEffect;

    public static event Action<Brick> OnBrickDestruction;

    private void Awake()
    {
        this.sr = this.GetComponent<SpriteRenderer>();
        this.boxCollider = this.GetComponent<BoxCollider2D>();
           
        Ball.OnLightningBallEnable += OnLightningBallEnable;
        Ball.OnLightningBallDisable += OnLightningBallDisable;
    }

    private void OnLightningBallEnable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = true;
        }
    }

    private void OnLightningBallDisable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        ApplyCollisionLogic(ball);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        Debug.Log($"Ball entered trigger of {gameObject.name}. IsLightningBall: {ball?.isLightningBall}");
        ApplyCollisionLogic(ball);


    }

    private void OnDisable()
    {
        Debug.Log($"Brick {gameObject.name} disabled. Unsubscribing from events.");
        Ball.OnLightningBallEnable -= OnLightningBallEnable;
        Ball.OnLightningBallDisable -= OnLightningBallDisable;
        BrickManager.Instance?.RemainingBricks.Remove(this);
    }


    private void ApplyCollisionLogic(Ball ball)
    {
        Debug.Log($"Brick {gameObject.name} hit! Hitpoints before: {Hitpoints}");

        this.Hitpoints--;

        if (this.Hitpoints <= 0 || (ball != null && ball.isLightningBall))
        {
            Debug.Log($"Brick {gameObject.name} destroyed!");
            if (BrickManager.Instance.RemainingBricks.Contains(this))
            {
                BrickManager.Instance.RemainingBricks.Remove(this);
                Debug.Log($"Brick removed from RemainingBricks. Remaining count: {BrickManager.Instance.RemainingBricks.Count}");
            }
            OnBrickDestruction?.Invoke(this);
            SpawnDestroyEffect();
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log($"Brick {gameObject.name} hitpoints reduced to: {Hitpoints}");
            this.sr.sprite = BrickManager.Instance.Sprites[this.Hitpoints - 1];
        }
    }



    //private void ApplyCollisionLogic(Ball ball)
    //{
    //    this.Hitpoints--;

    //    if (this.Hitpoints <= 0 || (ball != null && ball.isLightningBall))
    //    {
    //        BrickManager.Instance.RemainingBricks.Remove(this);
    //        OnBrickDestruction?.Invoke(this);
    //        OnBrickDestroy();
    //        SpawnDestroyEffect();
    //        Destroy(this.gameObject); // Destroy the brick
    //    }
    //    else
    //    {
    //        this.sr.sprite = BrickManager.Instance.Sprites[this.Hitpoints - 1];
    //    }
    //}

    private void OnBrickDestroy()
    {
        float buffSpawnChance = UnityEngine.Random.Range(0, 100f);
        float deBuffSpawnChance = UnityEngine.Random.Range(0, 100f);
        bool alreadySpawned = false;

        if (buffSpawnChance <= CollectablesManager.Instance.BuffChance)
        {
            alreadySpawned = true;
            Collectable newBuff = this.SpawnCollectable(true);
        }

        if (deBuffSpawnChance <= CollectablesManager.Instance.DebuffChance && !alreadySpawned)
        {
            Collectable newDebuff = this.SpawnCollectable(false);
        }
    }

    private Collectable SpawnCollectable(bool isBuff)
    {
        List<Collectable> collection;

        if (isBuff)
        {
            collection = CollectablesManager.Instance.AvailableBuffs;
        }
        else
        {
            collection = CollectablesManager.Instance.AvailableDebuffs;
        }

        int buffIndex = UnityEngine.Random.Range(0, collection.Count);
        Collectable prefab = collection[buffIndex];
        Collectable newCollectable = Instantiate(prefab, this.transform.position, Quaternion.identity) as Collectable;

        return newCollectable;
    }


    private void SpawnDestroyEffect()
    {
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPosition = new Vector3(brickPos.x, brickPos.y, brickPos.z - 0.2f);
        GameObject effect = Instantiate(DestroyEffect.gameObject, spawnPosition, Quaternion.identity);

        MainModule mm = effect.GetComponent<ParticleSystem>().main;
        mm.startColor = this.sr.color;

        Destroy(effect, DestroyEffect.main.startLifetime.constant);
    }

    public void Init(Transform containerTransform, Sprite sprite, Color color, int hitpoints)
    {
        this.transform.SetParent(containerTransform);
        this.sr.sprite = sprite;
        this.sr.color = color;
        this.Hitpoints = hitpoints;
    }

    //private void OnDisable()
    //{
    //    Ball.OnLightningBallEnable -= OnLightningBallEnable;
    //    Ball.OnLightningBallDisable -= OnLightningBallDisable;

    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    BulletState state;
    float reachTime;
    Vector3 startPos, goTo;
    Rigidbody2D rb;
    [HideInInspector] public int id;
    [HideInInspector] public PlayerCircularController player;

    private void Awake()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        state = BulletState.init;
    }

    private void Update()
    {
        if (id == 1)
        {
            if ((transform.position - startPos).magnitude > 8f && state == BulletState.init)
            {
                state = BulletState.started;
                goTo = Random.insideUnitCircle * player.radius;
                Vector3 direction = (goTo - transform.position).normalized;
                transform.rotation = Quaternion.Euler(0, 0, 90 + Vector3.SignedAngle(Vector3.right, direction, Vector3.forward));
                startPos = transform.position;
                rb.velocity = direction * player.SpeedOfBullet(id) * Time.deltaTime;
            }

            if ((transform.position - startPos).magnitude > 15 && state == BulletState.started)
            {
                state = BulletState.reached;
                Destroy(gameObject);
            }
        }
        else if (id == 2)
        {
            if ((transform.position - startPos).magnitude > 5f && state == BulletState.init)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject bulletNew = Instantiate(gameObject, transform.position, transform.rotation);
                    float angleToRotate = (i - 2) * 30 * Mathf.PI / 180;
                    Vector3 newVel = new Vector3(rb.velocity.x * Mathf.Cos(angleToRotate) - rb.velocity.y * Mathf.Sin(angleToRotate), rb.velocity.x * Mathf.Sin(angleToRotate) + rb.velocity.y * Mathf.Cos(angleToRotate));
                    bulletNew.GetComponent<Rigidbody2D>().velocity = newVel;
                    bulletNew.GetComponent<BulletScript>().id = 4;
                    bulletNew.GetComponent<BulletScript>().state = BulletState.spreaded;
                    bulletNew.GetComponent<BulletScript>().startPos = transform.position;
                    bulletNew.transform.rotation = Quaternion.Euler(0, 0, 90 + Vector3.SignedAngle(Vector3.right, bulletNew.GetComponent<Rigidbody2D>().velocity, Vector3.forward));
                }
                Destroy(gameObject);
            }
        }
        else if (id == 3)
        {
            if ((transform.position - startPos).magnitude > 7.7f && state == BulletState.init)
            {
                state = BulletState.walled;
                reachTime = Time.time;
                rb.velocity = Vector3.zero;
                transform.localScale = new Vector3(1, 2, 1);
                if (player.player == Player.PLAYER_1)
                    GetComponent<SpriteRenderer>().sprite = ArenaManager.instance.P1Wall;
                if (player.player == Player.PLAYER_2)
                    GetComponent<SpriteRenderer>().sprite = ArenaManager.instance.P2Wall;
            }
            if (state == BulletState.walled && Time.time - reachTime > 3)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if ((transform.position - startPos).magnitude > 3.5f)
            {
                GameObject temp = Instantiate(ArenaManager.instance.shotgunBulletSpread, transform.position + Vector3.back, transform.rotation);
                temp.transform.localEulerAngles -= new Vector3(0, 0, 90);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player1" && player.player == Player.PLAYER_2)
        {
            if (ArenaManager.instance.winner == 0)
                ScoreManager.instance.score2++;
            ScoreManager.instance.UpdateScore();
            if (ArenaManager.instance.winner == 0)
            {
                ArenaManager.instance.winner = 2;
                if (ScoreManager.instance.score2 > 2)
                    ScoreManager.instance.epicWinner = 2;
            }
            Destroy(gameObject);
        }
        if (collision.gameObject.name == "Player2" && player.player == Player.PLAYER_1)
        {
            if (ArenaManager.instance.winner == 0)
                ScoreManager.instance.score1++;
            ScoreManager.instance.UpdateScore();
            if (ArenaManager.instance.winner == 0)
            {
                ArenaManager.instance.winner = 1;
                if (ScoreManager.instance.score1 > 2)
                    ScoreManager.instance.epicWinner = 1;
            }
            Destroy(gameObject);
        }
    }

    enum BulletState
    {
        idle,
        init,
        started,
        reached,
        spreaded,
        walled
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCircularController : MonoBehaviour
{
    public Player player;

    public GameObject bullet;

    [HideInInspector]
    public int moveDir, moveDirForDeacc;
    [HideInInspector]
    public bool overrideControl;
    [HideInInspector]
    public List<int> bullets = new List<int>();
    public float radius = 3.825f;
    float moveSpeed = 2f;

    [HideInInspector]
    public float angle;
    float inputTime, releaseTime, deacceleration, shoot1Time;
    bool moveAuto;
    Vector3 startPos;
    KeyCode leftButton, rightButton, shootButton;
    Rigidbody2D rb;
    PlayerCircularController otherPlayer;
    SpriteRenderer icon;
    public static int controller;
    public Sprite P1bullet1icon, P1bullet2icon, P1bullet3icon, P2bullet1icon, P2bullet2icon, P2bullet3icon, P1bulletsprite, P2bulletsprite;

    public void Init()
    {
        if (player == Player.PLAYER_1)
        {
            leftButton = KeyCode.A;
            rightButton = KeyCode.D;
            shootButton = KeyCode.W;
            angle = 3 * Mathf.PI / 2;
            otherPlayer = ArenaManager.instance.player2;
        }
        else if (player == Player.PLAYER_2)
        {
            leftButton = KeyCode.LeftArrow;
            rightButton = KeyCode.RightArrow;
            shootButton = KeyCode.UpArrow;
            angle = Mathf.PI / 2;
            otherPlayer = ArenaManager.instance.player2;
        }
        else
            Debug.LogError("EMPTY PLAYER PRESENT");

        startPos = transform.position;
        icon = transform.GetChild(1).GetComponent<SpriteRenderer>();
        moveDir = moveDirForDeacc = 0; overrideControl = moveAuto = false;
        inputTime = 0;
        shoot1Time = Time.time - 10;
        controller = 0;
        rb = GetComponent<Rigidbody2D>();
        for (int i = 0; i < 3; i++)
            bullets.Add(ReturnBulletNumber());
        icon.sprite = IconOfActiveBullet(bullets[0]);
    }

    private void Update()
    {
        //acceleration = SetAcceleration(Time.time - inputTime);
        
        //Increase Angle Automatically (Collide)
        if (moveAuto)
        {
            if (controller == 0)
                controller = (int)player;
            if (controller == (int)player)
            {
                if (moveDir == 1)
                {
                    angle -= moveSpeed * Time.deltaTime;
                    otherPlayer.angle += moveSpeed * Time.deltaTime;
                }
                else if (moveDir == -1)
                {
                    angle += moveSpeed * Time.deltaTime;
                    otherPlayer.angle -= moveSpeed * Time.deltaTime;
                }
                else
                {
                    if (otherPlayer.moveDir == 1)
                    {
                        angle += moveSpeed * Time.deltaTime;
                        otherPlayer.angle -= moveSpeed * Time.deltaTime;
                    }
                    else if (otherPlayer.moveDir == -1)
                    {
                        angle -= moveSpeed * Time.deltaTime;
                        otherPlayer.angle += moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        if (angle > otherPlayer.angle)
                        {
                            angle += moveSpeed * Time.deltaTime;
                            otherPlayer.angle -= moveSpeed * Time.deltaTime;
                        }
                        else
                        {
                            angle -= moveSpeed * Time.deltaTime;
                            otherPlayer.angle += moveSpeed * Time.deltaTime;
                        }
                    }
                }
            }
        }

        //MOVE ACCORDING TO THE INPUT
        if (!overrideControl && !moveAuto)
        {
            if (Input.GetKey(leftButton))
            {
                angle -= moveSpeed * /*acceleration **/ Time.deltaTime;
                moveDir = -1;
                moveDirForDeacc = 0;
                if (inputTime == 0)
                    inputTime = Time.time;
            }
            else if (Input.GetKey(rightButton))
            {
                angle += moveSpeed * /*acceleration **/ Time.deltaTime;
                moveDir = 1;
                moveDirForDeacc = 0;
                if (inputTime == 0)
                    inputTime = Time.time;
            }
            else
            {
                if (moveDir != 0)
                {
                    moveDirForDeacc = moveDir;
                    releaseTime = Time.time;
                }
                Deaccelerate(moveDirForDeacc, releaseTime);
                moveDir = 0;
                inputTime = 0;
            }
        }

        transform.position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)); //WAS LOCKED UNTIL SOME INPUT COMES OR OTHER PLAYER HITS YOU

        if (angle >= 2 * Mathf.PI || angle <= -2 * Mathf.PI)
            angle = 0;
        
        transform.rotation = Quaternion.Euler(0, 0, PlayerRotation());

        if (Input.GetKeyDown(shootButton) && Time.time - shoot1Time > 1.5f && !overrideControl)
            ShootBullet(bullet, bullets[0]);
    }

    void ShootBullet(GameObject bulletToFire, int typeOfBullet)
    {
        bullets.RemoveAt(0);
        bullets.Add(ReturnBulletNumber());
        icon.sprite = IconOfActiveBullet(bullets[0]);
        shoot1Time = Time.time;
        GameObject bulletFired = Instantiate(bulletToFire, transform.position, transform.rotation);
        if (player == Player.PLAYER_1)
        {
            bulletFired.GetComponent<SpriteRenderer>().sprite = P1bulletsprite;
            bulletFired.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * SpeedOfBullet(typeOfBullet) * Time.deltaTime;
        }
        if (player == Player.PLAYER_2)
        {
            bulletFired.GetComponent<SpriteRenderer>().sprite = P2bulletsprite;
            bulletFired.GetComponent<Rigidbody2D>().velocity = -transform.up.normalized * SpeedOfBullet(typeOfBullet) * Time.deltaTime;
        }
        bulletFired.GetComponent<BulletScript>().player = this;
        bulletFired.GetComponent<BulletScript>().id = typeOfBullet;
    }

    Sprite IconOfActiveBullet(int type)
    {
        if (player == Player.PLAYER_1)
        {
            if (type == 1)
                return P1bullet1icon;
            if (type == 2)
                return P1bullet2icon;
            if (type == 3)
                return P1bullet3icon;
        }
        if (player == Player.PLAYER_2)
        {
            if (type == 1)
                return P2bullet1icon;
            if (type == 2)
                return P2bullet2icon;
            if (type == 3)
                return P2bullet3icon;
        }
        return null;
    }

    //RETURNS SPEED OF BULLET DEPENDING ON THE TYPE OF BULLET
    public int SpeedOfBullet(int type)
    {
        if (type == 1)
            return 1000;
        if (type == 2)
            return 600;
        return 1000;
    }

    //ROTATES PLAYER ACCORDING SO THAT IT IS ALWAYS LOOKING AT THE CENTRE
    float PlayerRotation()
    {
        float zAngle = 180;
        if (player == Player.PLAYER_1)
            zAngle += Vector3.SignedAngle(Vector3.up, transform.position, Vector3.forward);
        if (player == Player.PLAYER_2)
            zAngle += Vector3.SignedAngle(Vector3.down, transform.position, Vector3.forward);
        return zAngle;
    }

    //DEACCELERATES THE MOVESPEED AFTER THE PLAYER INPUT STOPS INCOMING
    void Deaccelerate(int moveDir, float releaseTiming)
    {
        deacceleration = SetDeacceleration(Time.time - releaseTiming);

        if (deacceleration < 0.01f)
        {
            releaseTiming = 0;
            moveDirForDeacc = 0;
            return;
        }

        if (moveDirForDeacc == 1)
            angle += moveSpeed * deacceleration * Time.deltaTime;
        else if (moveDirForDeacc == -1)
            angle -= moveSpeed * deacceleration * Time.deltaTime;
    }

    //RETURNS RANDOM BULLET TYPE
    int ReturnBulletNumber()
    {
        float value = Random.value;
        if (value > 0.67f)
            return 1;
        else if (value > 0.33f)
            return 2;
        else return 3;
    }
    
    //RETURNS HOW MUCH TO ACCELERATE THE MOVESPEED
    float SetAcceleration(float holdTime)
    {
        if (holdTime > 0.3f) return 1;
        else if (holdTime > 0.25f) return 0.65f;
        else if (holdTime > 0.2f) return 0.4f;
        else if (holdTime > 0.1f) return 0.2f;
        else if (holdTime > 0.05f) return 0.03f;
        else return 0f;
    }

    //RETURNS HOW MUCH TO DEACCELERATE THE MOVESPEED
    float SetDeacceleration(float released)
    {
        if (released > 0.4f) return 0;
        else if (released > 0.3f) return 0.01f;
        else if (released > 0.2f) return 0.2f;
        else if (released > 0.1f) return 0.4f;
        else if (released > 0.02f) return 0.65f;
        else return 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            overrideControl = true;
            moveAuto = true;
            inputTime = 0;
            StartCoroutine(SetFalse());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            overrideControl = true;
            moveAuto = true;
            inputTime = 0;
            StartCoroutine(SetFalse());
        }
    }

    //RESETS THE CONTROL BACK TO PLAYER INSTEAD OF AUTO
    IEnumerator SetFalse()
    {
        float waitTime = ReturnWaitTime(Time.time - inputTime);
        yield return new WaitForSeconds(waitTime);
        moveAuto = false;
        controller = 0;
        moveDir = 0;
        yield return new WaitForSeconds(0.1f);
        overrideControl = false;
    }

    //RETURNS HOW LONG TO WAIT AFTER THE COLLISION OF 2 PLAYERS TO STOP THE BOUNCE BACK
    float ReturnWaitTime(float time)
    {
        if (time > 0.8f) return 0.18f;
        else if (time > 0.6f) return 0.16f;
        else if (time > 0.4f) return 0.12f;
        else if (time > 0.2f) return 0.1f;
        else return 0f;
    }
}

public enum Player
{
    NONE = 0,
    PLAYER_1 = 1,
    PLAYER_2 = 2
}
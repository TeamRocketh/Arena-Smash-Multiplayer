using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCircularControllerSimple : MonoBehaviour
{
    [HideInInspector]
    public int moveDir;
    float radius = 4.48f, moveSpeed = 2f;

    int playerID;
    float angle;
    bool overrideControl, moveAuto;
    Vector3 startPos;
    KeyCode leftButton, rightButton;
    Rigidbody2D rb;
    PlayerCircularControllerSimple otherPlayer;

    private void Awake()
    {
        if (gameObject.name == "Player1") { leftButton = KeyCode.A; rightButton = KeyCode.D; angle = 3 * Mathf.PI / 2; playerID = 1; otherPlayer = GameObject.Find("Player2").GetComponent<PlayerCircularControllerSimple>(); }
        else if (gameObject.name == "Player2") { leftButton = KeyCode.LeftArrow; rightButton = KeyCode.RightArrow; angle = Mathf.PI / 2; playerID = 2; otherPlayer = GameObject.Find("Player1").GetComponent<PlayerCircularControllerSimple>(); }
        else Debug.LogError("Name the Player GameObjects as Player1 and Player2");
        startPos = transform.position;
        moveDir = 0; overrideControl = moveAuto = false;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //MOVE ACCORDING TO THE INPUT
        if (!overrideControl && !moveAuto)
        {
            if (Input.GetKey(leftButton))
            {
                angle -= moveSpeed * Time.deltaTime;
                moveDir = -1;
            }
            else if (Input.GetKey(rightButton))
            {
                angle += moveSpeed * Time.deltaTime;
                moveDir = 1;
            }
            else moveDir = 0;
        }

        //Increase Angle Automatically (Collide)
        if (moveAuto)
        {
            if (moveDir == 1)
                angle -= moveSpeed * Time.deltaTime;
            else if (moveDir == -1)
                angle += moveSpeed * Time.deltaTime;
            else
            {
                if (otherPlayer.moveDir == 1)
                    angle += moveSpeed * Time.deltaTime;
                else if (otherPlayer.moveDir == -1)
                    angle -= moveSpeed * Time.deltaTime;
            }
        }

        transform.position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)); //WAS LOCKED UNTIL SOME INPUT COMES OR OTHER PLAYER HITS YOU

        if (angle >= 2 * Mathf.PI || angle <= -2 * Mathf.PI)
            angle = 0;

        transform.rotation = Quaternion.Euler(0, 0, PlayerRotation());
    }

    //ROTATES PLAYER ACCORDING SO THAT IT IS ALWAYS LOOKING AT THE CENTRE
    float PlayerRotation()
    {
        float zAngle = 180;
        if (playerID == 1)
            zAngle += Vector3.SignedAngle(Vector3.up, transform.position, Vector3.forward);
        if (playerID == 2)
            zAngle += Vector3.SignedAngle(Vector3.down, transform.position, Vector3.forward);
        return zAngle;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            overrideControl = true;
            moveAuto = true;
            StartCoroutine(SetFalse());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            overrideControl = true;
            moveAuto = true;
            StartCoroutine(SetFalse());
        }
    }

    //RESETS THE CONTROL BACK TO PLAYER INSTEAD OF AUTO
    IEnumerator SetFalse()
    {
        yield return new WaitForSeconds(0.3f);
        moveAuto = false;
        yield return new WaitForSeconds(0.1f);
        overrideControl = false;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Birds : MonoBehaviour
{
    private const float JUMP_AMOUNT = 100f;

    private static Birds instance;

    public static Birds GetInstance()
    {
        return instance;
    }

    
    public event EventHandler OnDied;

    private Rigidbody2D birdRigidbody2D;
    public event EventHandler OnStartedPlaying;

    private State state;

    private enum State
    {
       WaitingToStart,
       Playing,
       Dead,
    }

    private void Awake()
    {
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }
    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }

    }
    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
       
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}

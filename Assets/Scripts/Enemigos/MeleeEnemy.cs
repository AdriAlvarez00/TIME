﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    // variables públicas (velocidad, daño rango de ataque y tiempo de ataque) y privadas (ángulo de movimiento y booleano que indica cuando ataca y cuando no)
    Pathfinder pathfinder;
    public float acc,maxspeed, attackrange, attackTime, casttime;
    public int damage;
    GameObject player;
    Rigidbody2D rb;
    private Vector2 angle;
    private bool attacking;

    void Start()
    {
        //inicialización de player
        player = GameManager.instance.GetPlayer();
        pathfinder = GetComponent<Pathfinder>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (pathfinder)
            //comparación de la posición del jugador y el enemigo       
            angle = pathfinder.Direction() - transform.position;
        else Debug.Log(gameObject.name + "No tiene pathfinder");
        if (Vector2.Distance(player.transform.position,transform.position) <=attackrange && !attacking)
        {
            Attack();
        }
    }
    private void FixedUpdate()
    {
        //mientras no está atacando va hacia el jugador
        if (rb)
        {
            if (!attacking && rb.velocity.magnitude <= maxspeed)
                rb.AddForce(angle*acc);
        }
        else
        {
            Debug.Log("Metele un Rigigbody a este " + gameObject.name);
        }
    }

    //Cambia la variable de ataque, y tras un pequeño lapso de tiempo comienza la animación de ataque, y tras ella, otro lapso de tiempo que precede al reseteo del movimiento
    public void Attack()
    {
        attacking = true;
        //aquí va la animación
        Invoke("Damage", casttime);
        Invoke("Resetmove", attackTime);
    }

    // Si tras el "casteo" del ataque el jugador se encuentra en rango, es dañado
    public void Damage()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= attackrange)
        {
            GameManager.instance.ChangeHealth(-damage, player.gameObject);
        }

    }

    //Despúes de atacar, haya dañado o no al jugador, vuelve a moverse
    public void Resetmove()
    {
        attacking = false;
    }


}

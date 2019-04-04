﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    private GeneradorOleadas generadorOleadas;
    private CienciaManager cientifico;
    public GameObject player;
    public int enemyCount;
    public LayerMask enemyLayer, wallLayer;
    [SerializeField]private int /*enemyCount,*/oleadaActual=-1;
    public float delayOleada;
    [SerializeField]
    UIManager UI;

    //Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

    }
    // Use this for initialization


    public void ChangeHealth(int value, GameObject target)
    {
        Health tgHealth = target.GetComponent<Health>();
        if (tgHealth){
            tgHealth.ChangeHealth(value);
            //si es el jugadir quien ha recibido daño
            if (tgHealth.GetComponent<PlayerController>())
            {
                UI.UpdateHealth(tgHealth.health);
            }
        }        
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
    public void SetUI(GameObject newUI)
    {
        UI = newUI.GetComponent<UIManager>();
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public void ChangeWeapon(Weapon weaponPrincipal, Weapon weaponSecondary)
    {
        UI.ChangeWeaponUI(weaponPrincipal, weaponSecondary);
    }
    //cada vez que muere un enemigo
    public void EnemySlain()
    {
        enemyCount--;
        //si era el último, genera una nueva oleada
        if (enemyCount <= 0)
        {
       
            if ((oleadaActual - cientifico.primeraAparicion) % cientifico.cadaX == 0)
            {
                //aquí van las visitas del CIENTEFRICO
                cientifico.Visita();
               // Invoke("GeneraOleada", delayOleada);
            }
            else
            {
                Invoke("GeneraOleada", delayOleada);
            }
        }
          
    }
    //Ordena generar una oleada
    public void GeneraOleada()
    {
        oleadaActual++;
        UI.UpdateOleada(oleadaActual);
        generadorOleadas.GeneraOleada(oleadaActual,out enemyCount);
    }
    //añade enemigos a la oleada(ej. portales)
    public void XtraEnemies(int n)
    {
        enemyCount += n;
    }
    public void UpdateAmmo(int ammo)
    {
        UI.UpdateCurrentAmmo(ammo);
    }
    public void UpdateMaxAmmo(int ammo)
    {
        UI.UpdateMaxAmmo(ammo);
    }
    public void DisableWeapon(int ammo, int maxAmmo)
    {
        UI.UpdateSecondaryWeapon(ammo, maxAmmo);
    }
    public void SwitchingWeaponUI()
    {
        UI.SwitchingWeapon();
    }
    public void ReloadingIconUI(bool state)
    {
        UI.ReloadingWeapon(state);
    }
    public void UpdateSecondaryAmmo(int ammo, int maxAmmo)
    {
        UI.UpdateSecondaryAmmo(ammo, maxAmmo);
    }
    public void UpdateMaxHealth(int newMax)
    {
        UI.UpdateMaxHealthVariable(newMax);
    }
    public void UpdatePerk(string perk)
    {
        UI.ActivatePerk(perk);
    }
    public LayerMask OnlyWalls(){ return wallLayer; }
    public LayerMask OnlyEnemies() { return enemyLayer; }
    public void SetCienciaManager(CienciaManager dis)
    {
        cientifico = dis;
    }
    public void SetGeneraOleadas(GeneradorOleadas olas)
    {
        generadorOleadas = olas;
    }
    public void OnButtonPressed(string button)
    {
        switch (button)
        {
            case "start":
                SceneManager.LoadScene("Hito02");
                break;
        }
    }
}

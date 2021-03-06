﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    Gun weapon;
    float changeTime;
    public bool isSwitching = false, first = true;
    [SerializeField] int currentWeapon = 0;
    IEnumerator reload, fb;
    Weapon[] equipadas = new Weapon[] { Weapon.pistola, Weapon.nothing };
    public GameObject fbBar, fbBarBg;
    public Color fbBarReload, fbBarChange;
    private void Start()
    {
        changeTime = GetComponentInParent<PlayerController>().changeTime;
        CancelReload();
    }
    public IEnumerator SwitchWeapon(float changeTime)
    {
        Debug.Log("Cambiando arma");
        //first = variable de seguridad para evitar errores la primera vez que se realiza(que siempre tiene nothing de secundaria)
        if (first || equipadas[(currentWeapon + 1) % equipadas.Length] != Weapon.nothing)
        {
            fb = FeedbackBar(changeTime, fbBarChange);            //Naranja
            StartCoroutine(fb);
            currentWeapon = (currentWeapon + 1) % equipadas.Length;
            weapon = GetComponentInChildren<Gun>();
            weapon.CannotShoot();
            //GameManager.instance.SwitchingWeaponUI();
            if (IsReloading())
            {
                Debug.Log("Recarga cancelada");
                CancelReload();
            }
            isSwitching = true;
            yield return new WaitForSeconds(changeTime);
            weapon.Switched();
            SelectWeapon(currentWeapon);
        }
    }
    public void ChangeWeapon(Collider2D wpCol)
    {
        if (!isSwitching)                                               //Evita recoger un arma cuando esté cambiando por la secundaria
        {
            WeaponPickup wpPick = wpCol.GetComponent<WeaponPickup>();
            if (wpPick)
            {
                if (equipadas[(currentWeapon + 1) % equipadas.Length] == Weapon.nothing)
                {
                    equipadas[(currentWeapon + 1) % equipadas.Length] = wpPick.WhatWeapon();
                    StartCoroutine("SwitchWeapon", 0);
                }
                else
                {
                    equipadas[currentWeapon] = wpPick.WhatWeapon();                   
                }
                SelectWeapon(currentWeapon);
                wpPick.OnPicked();
            }
        }

    }
    private void SelectWeapon(int weapn)                //Activa el arma weapn y desactiva todas las demás
    {
        foreach (Transform weap in transform)
        {
            if (weap.GetComponentInChildren<Gun>())         //No lo hace si el hijo no tiene el componente Gun, para evitar al Melee
            {
                if (weap.GetComponentInChildren<Gun>())
                {
                    if (equipadas[weapn] == weap.GetComponentInChildren<Gun>().iAmWeapon)
                    {
                        weap.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (weap.gameObject.name == equipadas[(currentWeapon + 1) % equipadas.Length].ToString())
                        {
                            weap.gameObject.GetComponentInChildren<Gun>().Disable();
                        }
                        weap.gameObject.SetActive(false);
                    }
                }
            }
        }
        isSwitching = false;
        GameManager.instance.ChangeWeapon(equipadas[weapn], equipadas[(weapn + 1) % equipadas.Length]);
    }
    public void CancelReload()
    {
        if (IsReloading())
        {
            weapon = GetComponentInChildren<Gun>();
            StopCoroutine(reload);
            StopCoroutine(fb);
            fbBarBg.SetActive(false);
            weapon.NotReload();
            GameManager.instance.ReloadingIconUI(false);
        }

    }
    public void StartReload()
    {
        if (!isSwitching && !CheckAmmo())
        {
            weapon = GetComponentInChildren<Gun>();
            fb = FeedbackBar(weapon.reload, fbBarReload);      //Verde
            StartCoroutine(fb);
            reload = weapon.Reload();
            StartCoroutine(reload);
            GameManager.instance.ReloadingIconUI(true);
        }
    }
    void Swap(int[] a)
    {
        int x = a[0];
        a[0] = a[1];
        a[1] = x;
    }
    public bool CheckAmmo()            //Le pregunta al arma equipada si tiene munición máxima en el cargador
    {
        weapon = GetComponentInChildren<Gun>();
        return weapon.MaxAmmo();
    }
    public bool IsReloading()          //Le pregunta al arma si está recargando
    {
        weapon = GetComponentInChildren<Gun>();
        return weapon.Reloading();
    }
    public void IsSwitching()
    {
        isSwitching = true;
    }
    public Weapon SecondaryWeapon()
    {
        return equipadas[0];
    }
    public void StartWeaponSwitch()
    {
        StartCoroutine("SwitchWeapon", changeTime);
    }
    public void UpgradeFireRate(float percentage)
    {
        foreach (Transform child in transform)
        {
            Gun gun = child.GetComponentInChildren<Gun>();
            if (gun)
            {
                gun.rate -= gun.rate * (percentage / 100);
            }
        }
    }
    public void UpgradeMagSize(float percentage)
    {
        //busca en todas las armas
        foreach (Transform child in transform)
        {
            Gun gun = child.GetComponentInChildren<Gun>();
            if (gun)
            {
                //aumenta la municion
                gun.magSize += Mathf.RoundToInt(gun.magSize * (percentage / 100));
                gun.UpdateAmmo();
                //Actualiza la munición del arma equipada
                if (gun.gameObject.activeInHierarchy)
                {
                    GameManager.instance.UpdateMaxAmmo(gun.magSize);
                    GameManager.instance.UpdateAmmo(gun.BulletsLeft());
                }
                else if (gun.iAmWeapon == equipadas[(currentWeapon + 1) % equipadas.Length])
                {
                    GameManager.instance.UpdateSecondaryAmmo(gun.BulletsLeft(), gun.magSize);
                }

                //Actualiza la munición del arma no equipada
                else if (gun.iAmWeapon == equipadas[(currentWeapon + 1) % equipadas.Length])

                {

                    GameManager.instance.UpdateSecondaryAmmo(gun.BulletsLeft(), gun.magSize);
                }
            }
        }

    }
    public Weapon equipedWeapon(int n)
    {
        return equipadas[n];
    }
    IEnumerator FeedbackBar(float time, Color color)
    {
        SpriteRenderer sp = fbBar.GetComponent<SpriteRenderer>();
        sp.color = color;                       //Le cambia el color
        fbBarBg.gameObject.SetActive(true);
        float timeMax = time + Time.time;       //Tiempo donde acaba de llenarse
        float startTime = Time.time;            //Tiempo inicial
        float progress = startTime;             //Lo que lleva
        while (progress < timeMax)
        {
            progress = Time.time;
            fbBar.transform.localScale = new Vector2(Mathf.Clamp01((progress - startTime) / time), 1);
            yield return null;
        }
        fbBarBg.gameObject.SetActive(false);
    }
}


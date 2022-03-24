using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void WeaponHolderReloadDelegate(ReloadState reloadState);
public class WeaponHolderComponent : ActorComponent
{
    protected Pawn owner;

    [SerializeField]
    protected Transform attachSocket;
    [SerializeField]
    protected List<WeaponActor> slots = new List<WeaponActor>();
    protected int maxAvailableSlots = 9;
    [SerializeField]
    protected List<bool> slotEquips = new List<bool>();
    [SerializeField]
    protected int currentSlot = -1;
    protected WeaponActor weapon;
    //[SerializeField]
    //protected AmmunitionComponent ammunition;

    protected bool isChanging;
    protected int toSlot;
    protected float nextChange;

    protected WeaponHolderReloadDelegate OnReload;

    public bool HasWeapon => currentSlot >= 0;
    public int MaxAvailableSlots => maxAvailableSlots;
    public void Restore()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].OnPop();
        }
        //ammunition.Restore();
    }
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }

    public virtual void SetFire(bool fire)
    {
        if (!isChanging && currentSlot >= 0)
        {
            weapon.SetFire(fire);
        }
    }
    public virtual ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (!isChanging && currentSlot >= 0)
        {
            return weapon.Shoot(position, direction);
        }
        return ShootState.none;
    }   
    public virtual void Stop()
    {
    }
    public bool Add(WeaponActor prefab, ref WeaponActor weapon, int clip, bool change = true)
    {
        if(Add(prefab,ref weapon, change))
        {          
            return true;
        }
        return false;
    }
    public bool Add(WeaponActor prefab, ref WeaponActor weapon, bool change = true)
    {
        if (!isChanging)
        {
            int slot = GetEmptySlot();
            weapon = GameInstance.Instance.PoolManager.Pop(prefab) as WeaponActor;
            weapon.SetOwner(owner);
            //weapon.SetAmmunition(ammunition);
            weapon.SetParent(attachSocket);
            weapon.Transform.localPosition = (Vector3.zero);
            weapon.Transform.localRotation = (Quaternion.identity);
            weapon.Transform.localScale = (Vector3.one);
            weapon.gameObject.SetActive(false);
           
            slots[slot] = weapon;
            slotEquips[slot] = true;
            if (change)
            {
                if (slot == currentSlot)
                    currentSlot = -1;
            }
            return true;
        }
        return false;
    }
    public bool Add(WeaponActor prefab, int slot, ref WeaponActor weapon,  bool change = true)
    {
        if (!isChanging)
        {
            weapon = GameInstance.Instance.PoolManager.Pop(prefab) as WeaponActor;
            weapon.SetOwner(owner);
            //weapon.SetAmmunition(ammunition);
            weapon.SetParent(attachSocket);
            weapon.Transform.localPosition = (Vector3.zero);
            weapon.Transform.localRotation = (Quaternion.identity);
            weapon.Transform.localScale = (Vector3.one);
            weapon.gameObject.SetActive(false);


         

            if (slots.Count > slot)
                slots[slot] = weapon;
            else
                slots.Insert(slot, weapon);
            if (slotEquips.Count > slot)
                slotEquips[slot] = true;
            else
                slotEquips.Insert(slot, true);


           
           

            if (change)
            {
                if (slot == currentSlot)
                    currentSlot = -1;
            }
            return true;
        }
        return false;
    }
    //public bool AddAmmo(AmmunitionType type, int ammo)
    //{
    //    if (!ammunition.IsFull(type))
    //    {
    //        ammunition.Add(type, ammo);
    //        return true;
    //    }
    //    return false;

    //}
    //public bool AmmoIsFull(AmmunitionType type)
    //{
    //    return ammunition.IsFull(type);
    //}
    protected int GetEmptySlot()
    {
        int slot = -1;
        if (slotEquips.Count < maxAvailableSlots)
        {
            slot = slotEquips.Count;
            slotEquips.Add(false);
            slots.Add(null);
            return slot;
        }
        else
        {
            for (int i = 0; i < slotEquips.Count; i++)
            {
                if (!slotEquips[i])
                {
                    slot = i;
                    return slot;
                }                 
            }
        }
        slot = currentSlot;
        return slot;
    }
    protected int GetEquipSlot()
    {
        int slot = -1;
        for (int i = 0; i < slotEquips.Count; i++)
        {
            if (slotEquips[i])
                slot = i;
        }
        return slot;
    }
    public WeaponActor GetWeapon() => weapon;
    public bool GetWeapon(int slot, ref WeaponActor weapon) 
    {
        if(slot >= 0 && slot < slotEquips.Count && slotEquips[slot])
        {
            weapon = slots[slot];
            return true;
        }
        return false;
    }
    public Transform GetShootPoint()
    {
        return weapon.ShootPoint;
    }
    public bool HasSameWeapon(WeaponActor weapon)
    {
        for (int i = 0; i < slotEquips.Count; i++)
        {
            if (slotEquips[i] && slots[i].Specifier.Equals(weapon.Specifier))
                return true;
        }
        return false;
    }
    public void SetDamageMultiply(float damageMultiply = 1.0f)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slotEquips[i])
                slots[i].DamageMultiply = damageMultiply;
        }
    }

    protected override void OnStart()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= slotEquips.Count || !slotEquips[i])
            {
                WeaponActor weapon = null;
                Add(Instantiate(slots[i]), ref weapon, false);
            }
            else
            {
                slots[i].SetOwner(owner);
                //slots[i].SetAmmunition(ammunition);
            }
            if (i == currentSlot)
            {
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
            if (currentSlot >= 0)
            {
                weapon = slots[currentSlot];
            }

        }
        if (slots.Count >= maxAvailableSlots)
            maxAvailableSlots = slots.Count;
    }
    protected void Start()
    {
        OnStart();
    }



    public void CallOnReload(ReloadState state)
    {
        OnReload?.Invoke(state);
    }
    public void BindOnReload(WeaponHolderReloadDelegate action)
    {
        OnReload += action;
    }
    public void UnbindOnReload(WeaponHolderReloadDelegate action)
    {
        OnReload -= action;
    }
    public void ClearOnReload()
    {
        OnReload = null;
    }

}

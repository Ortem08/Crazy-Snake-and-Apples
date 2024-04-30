using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerComponent : MonoBehaviour, IHurtable, IUser, IPushable
{
    [SerializeField]
    private float ItemPickUpRadius = 3;

    [SerializeField]
    private Transform handTransform;

    public Transform HandTransform => handTransform;

    [SerializeField]
    private Transform cameraTransform;

    public Transform CameraTransform => cameraTransform;

    public Transform SelfTransform => transform;

    private Inventory inventory;

    private QuakeCPMPlayerMovement QuakeMovenentController;

    [SerializeField]
    private float health = 100;

    public float Health => health;

    public Vector3 Velocity => QuakeMovenentController.GetVelocity();

    public void ConsumeDamage(float amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        health -= damageInfo.Amount;
        Debug.Log($"Player Hurt: health = {health}");
        if (health < 0)
        {
            Debug.Log("stop shooting! I`m already dead");
        }
    }

    private void Awake()
    {
        inventory = new Inventory();

        QuakeMovenentController = GetComponent<QuakeCPMPlayerMovement>();

        SingletonInputManager.instance.InputMap.Gameplay.PickItem.performed += PickItem_performed;
        SingletonInputManager.instance.InputMap.Gameplay.DropItem.performed += DropItem_performed;

        SingletonInputManager.instance.InputMap.Gameplay.UseItemPrimaryAction.performed += UseItemPrimaryAction_performed;
        SingletonInputManager.instance.InputMap.Gameplay.UseItemSecondaryAction.performed += UseItemSecondaryAction_performed;

        SingletonInputManager.instance.InputMap.Gameplay.ItemSelect.performed += ItemSelect_performed;
    }

    private void ItemSelect_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var input = (int)obj.ReadValue<float>() - 1;
        if (input < 0 || input >= inventory.Capacity)
        {
            throw new ArgumentException($"input must be an integer in range 1 to {input + 1}");
        }
        inventory.SelectedIndex = input;
    }

    private void UseItemSecondaryAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        inventory.SelectedItem?.TryUseSecondaryAction();
    }

    private void UseItemPrimaryAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        inventory.SelectedItem?.TryUsePrimaryAction();
    }

    private void DropItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        TryDropItem();
    }

    private void PickItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Debug.Log("attempt to pick up sth");
        TryPickUpItem();
    }

    private bool TryPickUpItem()
    {
        //use layers or tags to optimize
        foreach( var item in Physics.OverlapSphere(transform.position, ItemPickUpRadius))
        {
            //Debug.Log(item);
            if (item.TryGetComponent<ItemAvatar>(out var avatar))
            {
                if (inventory.SelectedItem == null)
                {
                    inventory.SelectedItem = avatar.PickUp();
                    inventory.SelectedItem.SetUser(this);
                    inventory.SelectedItem.OnSelect();
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryDropItem()
    {
        return inventory.TryDropCurrent(); ;
    }

    public void Push(Vector3 impulse)
    {
        QuakeMovenentController.AddVelocity(impulse);   // believe that player mass is 1
    }
}

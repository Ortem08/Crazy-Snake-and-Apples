
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public int Health { get; set; }
    public Inventory Inventory { get; private set; }
    public IInventoriable SelectedItem { get; set; }

    private void Start()
    {
        Inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        SelectedItem?.Update();
    }
}
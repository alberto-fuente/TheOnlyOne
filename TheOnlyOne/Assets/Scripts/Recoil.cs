using UnityEngine;
public class Recoil : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    private Weapon currentWeapon;

    [Header("Rotations")]
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
    }
    void Update()
    {
        //Check current weapon
        if (playerInventory.GetCurrentItem() == null || playerInventory.GetCurrentItem().typeOfItem != GameUtils.TypeOfItem.GUN)
            return;
        currentWeapon = playerInventory.GetCurrentItem().GetComponent<Weapon>();
        //Constantly tries to rotate back to initial rotation
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, currentWeapon.weaponData.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, currentWeapon.weaponData.recoilSpeed * Time.fixedDeltaTime);
        //applies rotation
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void RecoilFire(Vector3 recoil)
    {
        //add recoil in x and z axis
        targetRotation += new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
    }

}

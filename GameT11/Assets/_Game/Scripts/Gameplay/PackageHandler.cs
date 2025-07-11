using UnityEngine;
using UnityEngine.InputSystem;

namespace GameT11.Gameplay
{
    public class PackageHandler : MonoBehaviour
    {
        [SerializeField] private Transform packageHoldPoint;
        [SerializeField] private float interactionRange = 2f;
        
        private GameObject currentPackage;
        private bool hasPackage = false;
        
        private void Update()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (hasPackage)
                    DropPackage();
                else
                    TryPickupPackage();
            }
        }
        
        private void TryPickupPackage()
        {
            Collider[] packages = Physics.OverlapSphere(transform.position, interactionRange);
            
            foreach (var package in packages)
            {
                if (package.CompareTag("Package"))
                {
                    PickupPackage(package.gameObject);
                    break;
                }
            }
        }
        
        private void PickupPackage(GameObject package)
        {
            currentPackage = package;
            hasPackage = true;
            
            package.transform.SetParent(packageHoldPoint);
            package.transform.localPosition = Vector3.zero;
            
            Rigidbody packageRb = package.GetComponent<Rigidbody>();
            if (packageRb != null)
                packageRb.isKinematic = true;
        }
        
        private void DropPackage()
        {
            if (currentPackage == null) return;
            
            currentPackage.transform.SetParent(null);
            
            Rigidbody packageRb = currentPackage.GetComponent<Rigidbody>();
            if (packageRb != null)
                packageRb.isKinematic = false;
            
            Vector3 dropPosition = transform.position + transform.forward * 1.5f;
            currentPackage.transform.position = dropPosition;
            
            currentPackage = null;
            hasPackage = false;
        }
        
        public bool HasPackage => hasPackage;
    }
}
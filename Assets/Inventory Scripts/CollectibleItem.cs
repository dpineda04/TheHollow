using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{

    [Header("Visuals")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Info")]
    public string itemName;
    public float proximityRadius = 2f;
    public int slotIndex;

    [Header("Optional")]
    [SerializeField] private SphereCollider proximityTrigger;



    private void Reset()
    {
        // Auto-assign on drag-drop, obtains info from children
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        proximityTrigger = GetComponentInChildren<SphereCollider>();
    }

    public void Setup(string name, Mesh mesh, Material material, float radius)
        {
            itemName = name;

            if (meshFilter != null)
                meshFilter.mesh = mesh;

            if (meshRenderer != null && material != null)
                meshRenderer.material = material;

            if (proximityTrigger != null)
                proximityTrigger.radius = radius;

            Debug.Log($"Item '{itemName}' spawned with radius {radius}.");
        }

    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {

            Debug.Log($"Player is near {itemName}!");
            }

         }

    private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {

            Debug.Log($"Player has moved away from {itemName}!");
            }

         }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

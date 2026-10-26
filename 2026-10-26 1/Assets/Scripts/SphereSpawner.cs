using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float interval = 3f;
    [SerializeField] private float areaHalfSize = 4.5f;
    [SerializeField] private float minHeight = 8f;
    [SerializeField] private float maxHeight = 12f;
    [SerializeField] private Material sphereMaterial;

    private float timer;

    private void Update()
    {
        if (player != null && player.IsGameOver) return;
        timer += Time.deltaTime;
        if (timer < interval) return;
        timer -= interval;

        var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.name = "Sphere";
        s.transform.position = new Vector3(
            Random.Range(-areaHalfSize, areaHalfSize),
            Random.Range(minHeight, maxHeight),
            Random.Range(-areaHalfSize, areaHalfSize));
        s.AddComponent<Rigidbody>();
        s.AddComponent<FallingSphere>();
        if (sphereMaterial != null) s.GetComponent<Renderer>().sharedMaterial = sphereMaterial;
        Destroy(s, 30f);
    }
}

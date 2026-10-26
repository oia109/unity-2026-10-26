using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float baseInterval = 3f;
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float intervalDecreasePerStep = 0.2f;
    [SerializeField] private int baseCount = 1;
    [SerializeField] private int maxCount = 5;
    [SerializeField] private float stepSeconds = 10f;
    [SerializeField] private float areaHalfSize = 14f;
    [SerializeField] private float minHeight = 8f;
    [SerializeField] private float maxHeight = 12f;
    [SerializeField] private Material sphereMaterial;

    private float elapsed;
    private float timer;

    private void Update()
    {
        if (player != null && player.IsGameOver) return;
        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        int step = Mathf.FloorToInt(elapsed / stepSeconds);
        int count = Mathf.Min(baseCount + step, maxCount);
        float interval = Mathf.Max(baseInterval - intervalDecreasePerStep * step, minInterval);
        if (timer < interval) return;
        timer -= interval;

        for (int i = 0; i < count; i++) Spawn();
    }

    private void Spawn()
    {
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

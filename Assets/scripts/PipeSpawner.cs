using UnityEngine;

[DisallowMultipleComponent]
public class PipeSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject pipePairPrefab;

    [Header("Spawn Timing")]
    [SerializeField, Min(0.05f)] float spawnInterval = 1.75f;

    [Header("Spawn Position")]
    [SerializeField] Vector2 yRange = new Vector2(-1.5f, 2.0f);

    // When true, spawns just beyond the right edge of the camera view.
    [SerializeField] bool placeAtRightEdge = true;
    [SerializeField] float edgePadding = 1.0f;     // how far beyond the right edge (world units)

    // Used only if placeAtRightEdge == false
    [SerializeField] float xSpawn = 8f;

    [Header("Hierarchy (optional)")]
    [SerializeField] Transform spawnParent;

    float timer;

    void Update()
    {
        if (!ShouldRun()) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (!pipePairPrefab) return;

        float y = Random.Range(yRange.x, yRange.y);
        Vector3 pos = new Vector3(GetSpawnX(), y, 0f);
        Instantiate(pipePairPrefab, pos, Quaternion.identity, spawnParent);
    }

    float GetSpawnX()
    {
        if (placeAtRightEdge)
        {
            Camera cam = Camera.main;
            if (cam && cam.orthographic)
            {
                float rightEdge = cam.transform.position.x + cam.orthographicSize * cam.aspect;
                return rightEdge + edgePadding;
            }
        }
        return xSpawn;
    }

    bool ShouldRun()
    {
        // Works with or without the GameManager in the scene
        if (GameManager.Instance != null)
            return GameManager.Instance.GameRunning && !GameManager.Instance.GameOver;
        return true;
    }

#if UNITY_EDITOR
    // simple gizmos for spawn band
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float x = Application.isPlaying ? GetSpawnX() : (placeAtRightEdge ? transform.position.x : xSpawn);
        Vector3 a = new Vector3(x, yRange.x, 0f);
        Vector3 b = new Vector3(x, yRange.y, 0f);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, 0.05f);
        Gizmos.DrawSphere(b, 0.05f);
    }
#endif
}

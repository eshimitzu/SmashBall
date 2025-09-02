using UnityEngine;

public class BumperSpawner : MonoBehaviour
{
    [SerializeField] GameObject bumperPrefab;
    [SerializeField] float step;

        
    void Start()
    {
        SpawnRow(new Vector3(-6, 0, -12.5f), Quaternion.identity,  Vector3.right, 7);
        SpawnRow(new Vector3(-6, 0, 12.5f), Quaternion.Euler(0,180,0), Vector3.right, 7);
        
        SpawnRow(new Vector3(-7.5f, 0, -11f), Quaternion.Euler(0,90,0),  Vector3.forward, 12);
        SpawnRow(new Vector3(7.5f, 0, -11f), Quaternion.Euler(0,-90,0),  Vector3.forward, 12);
    }

    void SpawnRow(Vector3 position, Quaternion rotation, Vector3 dir, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(position.x, position.y, position.z) + dir * step * i;
            var prefab = Instantiate(bumperPrefab, pos, rotation, transform);
            prefab.transform.localPosition = pos;
        }
    }
}

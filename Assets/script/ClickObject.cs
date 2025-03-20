using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public GameObject[] prefabs;  // Array ของ prefabs ที่จะสร้าง
    private List<GameObject> createdPrefabs = new List<GameObject>();  // ลิสต์ที่เก็บ prefabs ที่ถูกสร้าง
    public float gridSize = 1.0f; // กำหนดขนาดของ Grid

    private int currentIndex = 0;  // ใช้ในการเลือก prefab ตัวถัดไปในอาเรย์

    void Update()
    {
        // ตรวจสอบการกดเมาส์ซ้าย (สร้าง prefab)
        if (Input.GetMouseButtonDown(0))  // 0 คือการคลิกซ้าย
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // ปรับตำแหน่งให้เป็นไปตาม Grid
                Vector3 gridPosition = GetGridPosition(hit.point);

                // ตรวจสอบว่ามี object อยู่ที่ตำแหน่งนี้แล้วหรือไม่
                GameObject existingObject = createdPrefabs.Find(obj => obj.transform.position == gridPosition);
                if (existingObject != null)
                {
                    createdPrefabs.Remove(existingObject);
                    Destroy(existingObject);
                }

                // สร้าง prefab ใหม่จากอาเรย์
                GameObject newPrefab = Instantiate(prefabs[currentIndex], gridPosition, Quaternion.identity);

                // เพิ่ม Collider เพื่อให้สามารถตรวจจับการคลิกได้
                if (newPrefab.GetComponent<Collider>() == null)
                {
                    newPrefab.AddComponent<BoxCollider>();
                }

                // เพิ่ม prefab ใหม่ลงในลิสต์
                createdPrefabs.Add(newPrefab);

                // เลือก prefab ตัวถัดไปในอาเรย์
                currentIndex = (currentIndex + 1) % prefabs.Length;  // ทำให้มันวนลูปไปเรื่อยๆ
            }
        }

        // ตรวจสอบการกดเมาส์ขวา (ลบ prefab ที่ถูกคลิก)
        if (Input.GetMouseButtonDown(1))  // 1 คือการคลิกขวา
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (createdPrefabs.Contains(clickedObject))
                {
                    createdPrefabs.Remove(clickedObject);
                    Destroy(clickedObject);
                }
            }
        }
    }

    // ฟังก์ชันสำหรับปรับตำแหน่งให้เป็นไปตาม Grid
    Vector3 GetGridPosition(Vector3 originalPosition)
    {
        float x = Mathf.Round(originalPosition.x / gridSize) * gridSize;
        float y = Mathf.Round(originalPosition.y / gridSize) * gridSize;
        float z = Mathf.Round(originalPosition.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }
}
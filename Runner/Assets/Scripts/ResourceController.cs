using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}

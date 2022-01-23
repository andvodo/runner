using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        rb.transform.Rotate(axis: new Vector3(1, 1, 1), 1);
    }
}

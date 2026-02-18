using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject player;
    [SerializeField]
    private int force = 10;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = this.gameObject;
        Debug.Log("Game Started");
    }

    // Update is called once per frame
    void Update()
    {
        player.transform.Translate(Vector3.forward * Time.deltaTime * force);
        // rb.AddForce(new Vector3(force, 0, 0)); 
    }
}

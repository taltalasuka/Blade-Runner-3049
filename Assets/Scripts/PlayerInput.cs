using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float verticalInput;
    public float horizontalInput;
    public bool isMouseDown;
    public bool isSpaceDown;
    void FixedUpdate()
    {
        if (!isMouseDown && Time.timeScale != 0)
        {
            isMouseDown = Input.GetMouseButtonDown(0);
        }

        if (!isMouseDown && Time.timeScale != 0)
        {
            isSpaceDown = Input.GetKeyDown(KeyCode.Space);
        }
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void OnDisable()
    {
        ClearCache();
    }

    public void ClearCache()
    {
        isSpaceDown = false;
        isMouseDown = false;
        verticalInput = 0;
        horizontalInput = 0;
    }
}

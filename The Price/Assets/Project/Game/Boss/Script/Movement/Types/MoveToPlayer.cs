using UnityEngine;

public class MoveToPlayer : TypeMovement {

    private bool inMove = false;
    private float speedMove = 0;
    private float timeToCancel;

    private void Start() { inMove = false; }
    private void Update()
    {
        if (inMove)
        {
            timeToCancel -= Time.deltaTime;

            if (timeToCancel <= 0) _boss.CancelMove();

            transform.position = Vector3.Lerp(transform.position, _playerStats.transform.position, 0.5f * speedMove * Time.deltaTime);

            if(Vector3.Distance(transform.position, _playerStats.transform.position) < 2f) { inMove = false; }
        }
    }
    public override void Move(float speed)
    {
        speedMove = speed;
        inMove = true;
    }
}

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VoidScript : MonoBehaviour
{
    BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (CharacterMovement.Instance.state == CharacterMovement.State.Dashing)
            boxCollider.isTrigger = true;
        else
            boxCollider.isTrigger = false;
    }
}

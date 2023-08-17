using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [field:SerializeField]
    public Transform AimAtPoint { get; private set; }
}

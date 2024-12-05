using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpearOwnerChange : MonoBehaviour
{
    private enum Actiontype : byte
    {
        GetSpear,
        LoseSpear
    }
    [SerializeField] private Actiontype m_Action;

    private void OnTriggerEnter(Collider _other)
    {
        //Layer 3 is player 
        if (_other.gameObject.layer != 3)
            return;
        _other.GetComponent<PlayerController>().OwnSpear = m_Action == Actiontype.GetSpear ? true : false;
    }
}
using UnityEngine;

public class CellHandler : MonoBehaviour
{
    [SerializeField] GameObject _circleObject;
    [SerializeField] GameObject _crossObject;

    public void OnActivateCircle()
    {
        _circleObject.gameObject.SetActive(true);
    }

    public void OnActivateCross()
    {
        _crossObject.gameObject.SetActive(true);
    }
}

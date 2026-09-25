
using UnityEngine;

public abstract class BasePreview : MonoBehaviour
{
    public abstract void Show(TerritoryType type, UnitStats troop, TerretoryData ter);
    public virtual void Hide() => gameObject.SetActive(false);
}

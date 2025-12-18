using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public abstract PopupId Id { get; }

    public virtual bool CanCloseByBackground => true;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
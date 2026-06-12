using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once CheckNamespace
public static class UIExtensions
{
    public static RectTransform AsRectTransform(this GameObject obj) =>
        (RectTransform)obj.transform;

    public static RectTransform AsRectTransform(this Component obj) =>
        (RectTransform)obj.transform;

    public static Canvas GetRootCanvas(this GameObject obj) =>
        obj.transform.GetRootCanvas();

    public static Canvas GetRootCanvas(this Component obj)
    {
        var tm = obj.transform;
        var result = tm.GetComponent<Canvas>();

        while (tm.parent != null)
        {
            tm = tm.parent;

            if (!tm)
                continue;

            var newCanvas = tm.GetComponent<Canvas>();
            if (newCanvas)
                result = newCanvas;
        }

        return result;
    }
}
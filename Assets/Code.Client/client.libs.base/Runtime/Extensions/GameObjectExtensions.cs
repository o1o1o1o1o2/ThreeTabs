using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
public static partial class GameObjectExtensions
{
    /// <summary>
    ///     Helper function that recursively sets all children with widgets' game objects layers to the specified value.
    /// </summary>
    public static void SetLayer(this Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (var i = 0; i < t.childCount; ++i)
        {
            var child = t.GetChild(i);
            child.gameObject.layer = layer;

            SetLayer(child, layer);
        }
    }

    public static void SetLayer(this GameObject obj, int layer)
    {
        obj.transform.SetLayer(layer);
    }

    public static void CopyPRS(this Component obj, GameObject item)
    {
        obj.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
        obj.transform.localScale = item.transform.localScale;
    }

    public static void CopyPRS(this GameObject obj, GameObject item)
    {
        obj.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
        obj.transform.localScale = item.transform.localScale;
    }

    public static void CopyPRS(this Component obj, Component item)
    {
        obj.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
        obj.transform.localScale = item.transform.localScale;
    }

    public static void CopyPRS(this GameObject obj, Component item)
    {
        obj.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
        obj.transform.localScale = item.transform.localScale;
    }

    public static void ResetPRS<T>(this T obj, bool resetPos = true, bool resetRot = true, bool resetScale = true) where T : Component
    {
        var childTm = obj.transform;

        if (resetPos)
            childTm.localPosition = Vector3.zero;
        if (resetRot)
            childTm.localRotation = Quaternion.identity;
        if (resetScale)
            childTm.localScale = Vector3.one;
    }

    public static void ResetPRS(this GameObject obj, bool resetPos = true, bool resetRot = true, bool resetScale = true)
    {
        var childTm = obj.transform;

        if (resetPos)
            childTm.localPosition = Vector3.zero;
        if (resetRot)
            childTm.localRotation = Quaternion.identity;
        if (resetScale)
            childTm.localScale = Vector3.one;
    }

    public static T GetComponentInParent<T>(this Component _this) => _this.gameObject.GetComponentInParent<T>();

    public static T GetComponentInParent<T>(this GameObject _this)
    {
        var p = _this.transform.parent;

        while (p != null)
        {
            var c = p.GetComponent<T>();

            if (c != null)
                return c;

            p = p.parent;
        }

        return default;
    }

    public static bool HasComponent<T>(this Component _this) where T : Component => _this.gameObject.GetComponent<T>() != null;

    public static bool HasComponent<T>(this GameObject _this) where T : Component => _this.GetComponent<T>() != null;

    public static T GetExistingComponent<T>(this Component _this) => _this.gameObject.GetExistingComponent<T>();

    public static T GetExistingComponent<T>(this GameObject _this)
    {
        var c = _this != null ? _this.GetComponent<T>() : default;

        if (c == null)
            throw new Exception($"{_this.GetFullPath()}: required component not found: {typeof(T).Name}");

        return c;
    }

    public static T[] GetComponents<T>(this GameObject _this) where T : class
    {
        var c = _this.GetComponents(typeof(T));

        return Array.ConvertAll(c, input => input as T);
    }

    public static T GetComponent<T>(this Component _this, string childName) => _this.gameObject.GetComponent<T>(childName);

    public static T GetComponent<T>(this GameObject _this, string childName)
    {
        var obj = _this.transform.Find(childName);
        return obj != null ? obj.GetComponent<T>() : default;
    }

    public static T GetOrAddComponent<T>(this Component _this) where T : Component => _this.gameObject.GetOrAddComponent<T>();

    public static T GetOrAddComponent<T>(this GameObject _this) where T : Component
    {
        var c = _this.GetComponent<T>();
        if (c == null)
            c = _this.AddComponent<T>();

        return c;
    }

    public static void DisableComponent<T>(this Component _this) where T : Behaviour
    {
        _this.gameObject.DisableComponent<T>();
    }

    public static void DisableComponent<T>(this GameObject _this) where T : Behaviour
    {
        var c = _this.GetComponent<T>();
        if (c != null)
            c.enabled = false;
    }

    public static T GetExistingComponent<T>(this Component _this, string childName) where T : Component => _this.gameObject.GetExistingComponent<T>(childName);

    public static T GetExistingComponent<T>(this GameObject _this, string childName) where T : Component
    {
        var obj = _this.transform.Find(childName);
        var c = obj != null ? obj.GetComponent<T>() : null;

        if (c == null)
            Debug.LogError(_this.name + ": required component or child object not found: '" + childName + "'." + typeof(T).Name);

        return c;
    }

    public static T GetComponentInChildren<T>(this Component _this, string childName) where T : Component => _this.gameObject.GetComponentInChildren<T>(childName);

    public static T GetComponentInChildren<T>(this GameObject _this, string childName) where T : Component
    {
        var obj = _this.transform.Find(childName);
        return obj != null ? obj.gameObject.GetComponent<T>() : null;
    }

    public static IEnumerable<Transform> EnumerateParents(this Component obj)
    {
        foreach (var parent in obj.gameObject.EnumerateParents())
            yield return parent;
    }

    public static IEnumerable<Transform> EnumerateParents(this GameObject obj)
    {
        var p = obj.transform.parent;

        while (p != null)
        {
            yield return p;
            p = p.parent;
        }
    }

    public static string GetFullPath(this GameObject obj)
    {
        if (obj == null)
            return string.Empty;

        return obj.transform.GetFullPath();
    }

    public static string GetFullPath(this Component obj)
    {
        if (obj == null)
            return string.Empty;

        var builder = new StringBuilder(256);

        foreach (var p in obj.EnumerateParents().Reverse())
        {
            builder.Append('/');
            builder.Append(p.name);
        }

        builder.Append('/');
        builder.Append(obj.name);

        return builder.ToString();
    }

    public static IEnumerable<T> AllChildrenOfType<T>(this Component obj, bool recursive = false) where T : Component
    {
        var t = obj.transform;
        var c = t.childCount;

        for (var i = 0; i < c; ++i)
        {
            var childTM = t.GetChild(i);
            if (!childTM)
                continue;

            var child = childTM.GetComponent<T>();
            if (child)
                yield return child;

            if (!recursive)
                continue;
            foreach (var childEnum in childTM.AllChildrenOfType<T>(true))
                yield return childEnum;
        }
    }

    public static void DestroyAllChildren(this Transform t)
    {
        var isPlaying = Application.isPlaying;

        while (t.childCount != 0)
        {
            var child = t.GetChild(0);

            if (isPlaying)
            {
                child.SetParent(null);
                Object.Destroy(child.gameObject);
            }
            else
                Object.DestroyImmediate(child.gameObject);
        }
    }

    public static void DestroyAllChildren(this Transform t, Func<Transform, bool> needRemoved)
    {
        var isPlaying = Application.isPlaying;

        for (var i = 0; i < t.childCount;)
        {
            var child = t.GetChild(i);

            if (!needRemoved(child))
            {
                ++i;
                continue;
            }

            if (isPlaying)
            {
                child.SetParent(null);
                Object.Destroy(child.gameObject);
            }
            else
                Object.DestroyImmediate(child.gameObject);
        }
    }

    public static void DestroyAll(this ICollection<GameObject> list)
    {
        var isPlaying = Application.isPlaying;
        foreach (var o in list)
        {
            if (isPlaying)
            {
                o.transform.SetParent(null);
                Object.Destroy(o);
            }
            else
                Object.DestroyImmediate(o);
        }

        list.Clear();
    }

    public static void DestroyAllComponents<TComponent>(this ICollection<TComponent> list) where TComponent : Component
    {
        var isPlaying = Application.isPlaying;
        foreach (var o in list)
        {
            if (isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o);
        }

        list.Clear();
    }

    public static void DestroyAllGameObjects<TComponent>(this ICollection<TComponent> list) where TComponent : Component
    {
        var isPlaying = Application.isPlaying;
        foreach (var o in list)
        {
            if (isPlaying)
            {
                o.transform.SetParent(null);
                Object.Destroy(o.gameObject);
            }
            else
                Object.DestroyImmediate(o.gameObject);
        }

        list.Clear();
    }

    /// <summary>
    ///     enumerate trough all transform children
    /// </summary>
    public static IEnumerable<Transform> AsEnumerable(this Transform tm)
    {
        for (var i = 0; i < tm.childCount; i++)
            yield return tm.GetChild(i);
    }

    public static void SetActive(this IEnumerable<GameObject> objects, bool isActive)
    {
        if (objects == null)
            return;

        foreach (var obj in objects)
        {
            if (obj)
                obj.SetActive(isActive);
        }
    }

    public static void SetActive<T>(this IEnumerable<T> objects, bool isActive) where T : Component
    {
        if (objects == null)
            return;

        foreach (var obj in objects)
        {
            if (obj)
                obj.gameObject.SetActive(isActive);
        }
    }

    public static void SetActive(this Component component, bool isActive)
    {
        if (component == null)
            return;
        var go = component.gameObject;
        if (go == null)
            return;
        if (go.activeSelf != isActive)
            go.SetActive(isActive);
    }

    public static void SetEnabled<T>(this IEnumerable<T> objects, bool enabled) where T : Behaviour
    {
        if (objects == null)
            return;

        foreach (var obj in objects)
        {
            if (obj)
                obj.enabled = enabled;
        }
    }
}
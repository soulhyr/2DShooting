using UnityEngine;

public interface IObjectProvider
{
    GameObject MakeObject(ObjectType type, Vector3? pos = null);
    GameObject[] GetPool(ObjectType type);
}
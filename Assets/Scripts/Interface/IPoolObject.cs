using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPoolObject
{
    GameObject GetObject();
    ObjectType GetObjectType();
    void Init();
    void ReturnObject();
}

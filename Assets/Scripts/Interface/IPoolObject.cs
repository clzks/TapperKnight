using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPoolObject
{
    GameObject GetObject();
    ObjectType GetObjectType();
    UniTask Init();
    UniTask ReturnObject();
}

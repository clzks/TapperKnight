using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    IPoolObject MakeObject();
    GameObject GetObject();
    ObjectType GetObjectType();
    void Init();
    void ReturnObject();
}

using UnityEngine;
using System.Collections;


public interface IPool<T,K>
{

    T GetPooledObject(K obj);

    void StoreObject(T obj);

    void DrainPool();

}

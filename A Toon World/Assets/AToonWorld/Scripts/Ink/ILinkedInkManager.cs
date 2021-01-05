using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkedObjectManager<T>
{
    void Link(T linkableObject);
    void Unlink(T linkableObject);
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CAT_Action_SetTransform : CAT_Action
{
    public List<Transform> objects;
    public bool setLocalPosition = false;
    public Vector3 newLocalPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public bool setLocalRotation = false;
    public Vector3 newLocalRotation = new Vector3(0.0f, 0.0f, 0.0f);
    public bool setLocalScale = false;
    public Vector3 newLocalScale = new Vector3(1.0f, 1.0f, 1.0f);

    public override CAT_Action InternalCopy()
    {
        CAT_Action_SetTransform copy = new CAT_Action_SetTransform();

        CopyCatActionMembers(copy);

        copy.objects = new List<Transform>();
        foreach (var obj in objects)
            copy.objects.Add(obj);

        copy.setLocalPosition = setLocalPosition;
        copy.newLocalPosition = new Vector3(newLocalPosition.x, newLocalPosition.y, newLocalPosition.z);
        copy.setLocalRotation = setLocalRotation;
        copy.newLocalRotation = new Vector3(newLocalRotation.x, newLocalRotation.y, newLocalRotation.z);
        copy.setLocalScale = setLocalScale;
        copy.newLocalScale = new Vector3(newLocalScale.x, newLocalScale.y, newLocalScale.z);

        return copy;
    }

    protected override void OnStateChanged_Running()
    {
        for (var i = 0; i < objects.Count; i++)
        {
            if (setLocalPosition)
                objects[i].localPosition = newLocalPosition;

            if (setLocalRotation)
                objects[i].localRotation = Quaternion.Euler(newLocalRotation);

            if (setLocalScale)
                objects[i].localScale = newLocalScale;
        }

        ChangeState(CATState.Finished);
    }
}

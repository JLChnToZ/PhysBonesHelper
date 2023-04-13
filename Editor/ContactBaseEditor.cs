using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;

[EditorTool("Contact Sender Tool", typeof(ContactBase))]
class ContactBaseEditor : EditorTool
{
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    List<ContactBase> senders = new List<ContactBase>();

    void OnEnable()
    {
        m_IconContent = new GUIContent("Contact Tool", m_ToolIcon, "Contact Tool");
    }

    public override GUIContent toolbarIcon => m_IconContent;

    public override void OnToolGUI(EditorWindow window)
    {
        foreach (var transfrom in Selection.transforms)
        {
            transfrom.gameObject.GetComponents(senders);
            foreach (var sender in senders)
            {
                EditorGUI.BeginChangeCheck();

                Transform senderTransform = sender.rootTransform;
                if (!senderTransform) senderTransform = sender.transform;

                Vector3 position = senderTransform.TransformPoint(sender.position);
                Quaternion rotation = senderTransform.rotation * sender.rotation;
                Vector2 scale = new Vector2(sender.radius, sender.height);
                position = Handles.PositionHandle(position, rotation);
                rotation = Handles.RotationHandle(rotation, position);
                scale.x = Handles.ScaleSlider(scale.x, position,rotation * Vector3.forward, rotation, HandleUtility.GetHandleSize(position)*0.7f, HandleUtility.GetHandleSize(position));
                if(sender.shapeType == ContactBase.ShapeType.Capsule)
                    scale.y = Handles.ScaleSlider(scale.y, position, rotation * Vector3.up, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    sender.position = senderTransform.InverseTransformPoint(position);
                    sender.rotation = (Quaternion.Inverse(senderTransform.rotation) * rotation).normalized;
                    sender.radius = Mathf.Max(0.0000001f, scale.x);
                    if (sender.shapeType == ContactBase.ShapeType.Capsule)
                        sender.height = Mathf.Max(0.0000001f, scale.y);
                }
            }
        }
    }
}

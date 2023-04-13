using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;

[EditorTool("Bone Tool",typeof(VRCPhysBoneColliderBase))]
class BoneColliderEditor : EditorTool
{
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    List<VRCPhysBoneColliderBase> senders = new List<VRCPhysBoneColliderBase>();

    void OnEnable()
    {
        m_IconContent = new GUIContent("Bone Tool", m_ToolIcon, "Bone Tool");
    }

    public override GUIContent toolbarIcon => m_IconContent;

    public override void OnToolGUI(EditorWindow window)
    {
        foreach (var transform in Selection.transforms)
        {
            transform.gameObject.GetComponents(senders);
            foreach (var sender in senders)
            {
                Transform senderTransform = sender.rootTransform;
                if (!senderTransform) senderTransform = sender.transform;

                EditorGUI.BeginChangeCheck();

                Vector3 position = senderTransform.TransformPoint(sender.position);
                Quaternion rotation = senderTransform.rotation * sender.rotation;
                Vector2 scale = new Vector2(sender.radius, sender.height);
                position = Handles.PositionHandle(position, rotation);
                rotation = Handles.RotationHandle(rotation, position);
                if (sender.shapeType != VRCPhysBoneColliderBase.ShapeType.Plane)
                    scale.x = Handles.ScaleSlider(scale.x, position, rotation * Vector3.forward, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));
                if (sender.shapeType == VRCPhysBoneColliderBase.ShapeType.Capsule)
                    scale.y = Handles.ScaleSlider(scale.y, position, rotation * Vector3.up, rotation, HandleUtility.GetHandleSize(position) * 0.7f, HandleUtility.GetHandleSize(position));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    sender.position = senderTransform.InverseTransformPoint(position);
                    sender.rotation = (Quaternion.Inverse(senderTransform.rotation) * rotation).normalized;
                    if (sender.shapeType != VRCPhysBoneColliderBase.ShapeType.Plane)
                        sender.radius = Mathf.Max(0.0000001f, scale.x);
                    if (sender.shapeType == VRCPhysBoneColliderBase.ShapeType.Capsule)
                        sender.height = Mathf.Max(0.0000001f, scale.y);
                }
            }
        }
    }
}

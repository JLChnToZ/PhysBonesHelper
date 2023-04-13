using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using VRC.Dynamics;

[EditorTool("PhysBones Tool", typeof(VRCPhysBoneBase))]
class PhysEndBoneEditor : EditorTool
{
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    List<VRCPhysBoneBase> senders = new List<VRCPhysBoneBase>();

    void OnEnable()
    {
        m_IconContent = new GUIContent("PhysBones Tool", m_ToolIcon, "PhysBones Tool");
    }

    public override GUIContent toolbarIcon => m_IconContent;

    public override void OnToolGUI(EditorWindow window)
    {
        foreach (var transform in Selection.transforms)
        {
            transform.gameObject.GetComponents(senders);
            foreach (VRCPhysBoneBase sender in senders)
            {
                Transform senderTransform = sender.rootTransform;
                if (!senderTransform) senderTransform = sender.transform;
                while (senderTransform.childCount > 0)
                    senderTransform = senderTransform.GetChild(0);
                EditorGUI.BeginChangeCheck();

                Vector3 position = senderTransform.TransformPoint(sender.endpointPosition);
                position = Handles.PositionHandle(position, senderTransform.rotation);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sender, "Move Contact");
                    sender.endpointPosition = senderTransform.InverseTransformPoint(position);
                }
            }
        }
    }
}
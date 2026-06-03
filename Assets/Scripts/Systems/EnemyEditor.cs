
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();

        if (property.NextVisible(true))
        {
            do
            {
                bool isShooterProperty =
                    property.name == "startingDirection" ||
                    property.name == "visionDistance" ||
                    property.name == "coneAngle" ||
                    property.name == "visionResolution" ||
                    property.name == "visionMask" ||
                    property.name == "bulletPrefab" ||
                    property.name == "firePoint" ||
                    property.name == "fireDelay";

                if (isShooterProperty)
                {
                    if (serializedObject.FindProperty("enemyType").enumValueIndex == (int)Enemy.EnemyType.Shooter)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
                else if (property.name == "staminaGiven")
                {
                    if (!serializedObject.FindProperty("restoreMaxStamina").boolValue)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }

            } while (property.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
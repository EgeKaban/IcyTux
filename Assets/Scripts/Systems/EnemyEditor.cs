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
                // Check if property belongs to Shooter settings
                bool isShooterProperty =
                    property.name == "startingDirection" || // --- BU SATIR EKLENDİ ---
                    property.name == "visionDistance" ||
                    property.name == "coneAngle" ||
                    property.name == "visionResolution" ||
                    property.name == "visionMask" ||
                    property.name == "bulletPrefab" ||
                    property.name == "firePoint" ||
                    property.name == "fireDelay";

                if (isShooterProperty)
                {
                    // Only draw shooter settings if type is Shooter
                    if (serializedObject.FindProperty("enemyType").enumValueIndex == (int)Enemy.EnemyType.Shooter)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
                else if (property.name == "staminaGiven")
                {
                    // Only draw the custom Stamina Given field if Restore Max Stamina is FALSE
                    if (!serializedObject.FindProperty("restoreMaxStamina").boolValue)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
                else
                {
                    // Draw everything else normally
                    EditorGUILayout.PropertyField(property, true);
                }

            } while (property.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
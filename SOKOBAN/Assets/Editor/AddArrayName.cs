// ---------------------------------------------------------  
// AddArrayName.cs  
//   
// 作成日:  
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AddArrayNameAttribute))]
public class AddArrayName : PropertyDrawer//インスペクターカスタマイズする為に継承
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(position,property,new GUIContent(((AddArrayNameAttribute)attribute).ArrayNames[pos]));
        }
        catch
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

// ---------------------------------------------------------  
// AddArrayNameAttribute.cs  
//   
// 作成日:  
// 作成者:  sasaki rio
// ---------------------------------------------------------  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddArrayNameAttribute : PropertyAttribute
{
    public readonly string[] ArrayNames;
    public AddArrayNameAttribute(string[] ArrayNames) 
    {
        this.ArrayNames = ArrayNames; 
    }
}

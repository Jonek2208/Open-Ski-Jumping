using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CTI_URP_ShaderGUI : ShaderGUI  {

    protected Color avrgCol = Color.gray;

    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        
        base.OnGUI (materialEditor, properties);
        Material targetMat = materialEditor.target as Material;

    //  Bark texture2D
        if (targetMat.HasProperty("_BumpOcclusionMap")) {
            if ( targetMat.GetTexture("_BumpOcclusionMap") == null) {
                targetMat.SetTexture("_BumpOcclusionMap", Resources.Load("CTI_Default_NST") as Texture2D );
            }
        }

    //  Leaves
        if (targetMat.HasProperty("_BumpSpecMap")){
            if (targetMat.GetTexture("_BumpSpecMap") == null) {
                targetMat.SetTexture("_BumpSpecMap", Resources.Load("CTI_Default_NST") as Texture2D ); 
            }
        }

        GUILayout.Space(8);

    }
}
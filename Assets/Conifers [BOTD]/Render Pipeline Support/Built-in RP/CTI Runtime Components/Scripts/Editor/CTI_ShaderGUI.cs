using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CTI_ShaderGUI : ShaderGUI  {

    protected Color avrgCol = Color.gray;

    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
    {

        EditorGUILayout.HelpBox("Please note that params marked with '*' must be synced across bark, leaf and maybe even the billboard material.", MessageType.Info);
        
        base.OnGUI (materialEditor, properties);
        Material targetMat = materialEditor.target as Material;

    //  Bark texture2D
        if (targetMat.HasProperty("_BumpSpecAOMap")) {
            if ( targetMat.GetTexture("_BumpSpecAOMap") == null) {
                targetMat.SetTexture("_BumpSpecAOMap", Resources.Load("CTI_default_normal_spec_ao") as Texture2D );
            }
        }
    //  Bark textureArray
        else if (targetMat.HasProperty("_BumpSpecAOMapArray")){
            if (targetMat.GetTexture("_BumpSpecAOMapArray") == null) {
                targetMat.SetTexture("_BumpSpecAOMapArray", Resources.Load("CTI_default_normal_spec_ao_Texture2DArray") as Texture2DArray );   
            }
        }

    //  Assign default combined detail texture
        if (targetMat.HasProperty("_DetailNormalMapX")){
            if (targetMat.GetTexture("_DetailNormalMapX") == null) {
                targetMat.SetTexture("_DetailNormalMapX", Resources.Load("CTI_default_normal_spec_ao") as Texture2D ); 
            }
        }

    //  Leaves
        if (targetMat.HasProperty("_BumpSpecMap")){
            if (targetMat.GetTexture("_BumpSpecMap") == null) {
                targetMat.SetTexture("_BumpSpecMap", Resources.Load("CTI_default_normal_spec") as Texture2D ); 
            }
        }
        if (targetMat.HasProperty("_TranslucencyMap")){
            if (targetMat.GetTexture("_TranslucencyMap") == null) {
                targetMat.SetTexture("_TranslucencyMap", Resources.Load("CTI_default_ao_trans_smoothness") as Texture2D ); 
            }
        }


    //  Get average color
        if (targetMat.HasProperty("_AverageCol")) {
            GUILayout.Space(8);
            if (GUILayout.Button("Get average Color")) {
                if (targetMat.HasProperty("_MainTex") && targetMat.GetTexture("_MainTex") != null) {
                    getAverageColor( targetMat.GetTexture("_MainTex") as Texture2D );
                }
                else if (targetMat.HasProperty("_MainTexArray") && targetMat.GetTexture("_MainTexArray") != null) {
                    Texture2DArray sourceTex = targetMat.GetTexture("_MainTexArray") as Texture2DArray;
                    Color32[] aColor = sourceTex.GetPixels32(0, 1);
                    avrgCol = aColor[0];
                }
                targetMat.SetColor("_AverageCol", avrgCol);
            }
        }


        GUILayout.Space(8);



    }

    private void getAverageColor(Texture2D sourceTex) {
        bool wasReadable = false;
        string path = AssetDatabase.GetAssetPath(sourceTex);
        TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(path);
        if (ti.isReadable == true) {
            wasReadable = true;
        }
        else {
            ti.isReadable = true;
            // refresh texture
            AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );  
        }

        if (sourceTex) {
            // read from lowest mipmaplevel
            int mip = sourceTex.mipmapCount - 1;
            // is array
            Color[] aColor = sourceTex.GetPixels(0, 0, 1, 1, mip);
            avrgCol = aColor[0];
        }
        else {  
            avrgCol = Color.gray;
            Debug.Log("No Texture assigned yet.");
        }

        // reset texture settings
        if (wasReadable == false) {
            ti.isReadable = false;
            AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate ); 
        }
    }
}
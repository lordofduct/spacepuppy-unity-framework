using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.spacepuppyeditor.Base
{
    
    public class ModelImportSettingsProcessor : AssetPostprocessor
    {

        private void OnPreprocessModel()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            if (modelImporter == null) return;
            if (System.IO.File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(modelImporter.assetPath))) return;

            if (SpacepuppySettings.SetMaterialSearchOnImport)
            {
                modelImporter.materialSearch = SpacepuppySettings.MaterialSearch;
            }

            if (!SpacepuppySettings.SetAnimationSettingsOnImport)
            {
                modelImporter.animationType = SpacepuppySettings.ImportAnimRigType;
            }
        }
        
    }

}

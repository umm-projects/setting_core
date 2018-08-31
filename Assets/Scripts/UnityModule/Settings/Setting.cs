﻿using System;
using System.IO;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityModule.Settings
{
    public interface ISetting
    {
    }

    public interface IEnvironmentSetting : ISetting
    {
    }

    public abstract class Setting : ScriptableObject, ISetting
    {
    }

    [PublicAPI]
    public abstract class Setting<TSetting> : Setting where TSetting : Setting<TSetting>, ISetting
    {
        public static TSetting GetOrDefault()
        {
            var setting = SettingContainer.ResolveContainerInstance<TSetting>().Get<TSetting>();
            return setting == null ? CreateInstance<TSetting>() : setting;
        }

#if UNITY_EDITOR
        private const string DefaultAssetPathFormat = "Assets/{1}Settings/{0}.asset";

        private const string BaseDirectoryName = "Settings";

        private const string EnvironmentDirectoryName = "Environment";

        protected static TSetting CreateAsset()
        {
            var projectSetting = CreateInstance<TSetting>();
            var assetDirectoryPath = Path.Combine(
                BaseDirectoryName,
                typeof(IEnvironmentSetting).IsAssignableFrom(typeof(TSetting)) ? EnvironmentDirectoryName : string.Empty
            );
            var assetName = typeof(TSetting).Name;
            if (!string.IsNullOrEmpty(Path.Combine(Application.dataPath, assetDirectoryPath)) && !Directory.Exists(Path.Combine(Application.dataPath, assetDirectoryPath)))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, assetDirectoryPath));
            }

            AssetDatabase.CreateAsset(projectSetting, AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", assetDirectoryPath, $"{assetName}.asset")));
            AssetDatabase.Refresh();
            SettingContainer.ResolveContainerInstance<TSetting>().Add(projectSetting);
            return projectSetting;
        }
#endif
    }
}
using System.IO;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityModule.Settings {

    public interface ISetting
    {
    }

    public abstract class Setting : ScriptableObject, ISetting
    {
    }

    [PublicAPI]
    public abstract class Setting<TSetting> : Setting where TSetting : Setting<TSetting>, ISetting
    {
        private const string DefaultAssetPathFormat = "Assets/Settings/{0}.asset";

        public static TSetting GetOrDefault()
        {
            var setting = SettingContainer.Instance.Get<TSetting>();
            return setting == null ? CreateInstance<TSetting>() : setting;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected static string GetAssetPath()
        {
            return DefaultAssetPathFormat;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected static string GetAssetName() {
            return typeof(TSetting).Name;
        }

#if UNITY_EDITOR
        protected static void CreateAsset() {
            var projectSetting = CreateInstance<TSetting>();
            var directoryPath = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), string.Format(GetAssetPath(), GetAssetName()))));
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            AssetDatabase.CreateAsset(projectSetting, string.Format(GetAssetPath(), GetAssetName()));
            AssetDatabase.Refresh();
        }
#endif
    }
}

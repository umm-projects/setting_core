using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UnityModule.Settings
{
    public interface ISettingContainer
    {
        bool Exists<TSetting>() where TSetting : ISetting;

        TSetting Get<TSetting>() where TSetting : ISetting;

        IEnumerable<TSetting> GetAll<TSetting>() where TSetting : ISetting;

        IEnumerable<TSetting> GetAll<TSetting>(Func<TSetting, bool> predicate) where TSetting : ISetting;

        void Add<TSetting>(TSetting setting) where TSetting : ISetting;
    }

    public class SettingContainer : ScriptableObject, ISettingContainer
    {
        private const string PrefixDefault = "Default";

        private const string PrefixEnvironment = "Environment";

        private const string PathFormat = "{0}SettingContainer";

        private const string Extension = ".asset";

        private static ISettingContainer instance;

        private static Dictionary<string, ISettingContainer> InstanceMap { get; } = new Dictionary<string, ISettingContainer>();

        public static ISettingContainer ResolveContainerInstance<TSetting>()
        {
            if (typeof(IEnvironmentSetting).IsAssignableFrom(typeof(TSetting)))
            {
                return GetInstance(PrefixEnvironment);
            }

            return GetInstance(PrefixDefault);
        }

        private static ISettingContainer GetInstance(string prefix)
        {
            if (!InstanceMap.ContainsKey(prefix))
            {
                InstanceMap[prefix] =
                    Resources.Load<SettingContainer>(string.Format(PathFormat, prefix))
#if UNITY_EDITOR
                    ?? CreateScriptableObject(prefix)
#endif
                    ;
            }

            return InstanceMap[prefix];
        }

        [SerializeField] private List<Setting> settingList = new List<Setting>();

        private IEnumerable<ISetting> SettingList => settingList;

        public bool Exists<TSetting>() where TSetting : ISetting
        {
            return SettingList?.Any(x => x is TSetting) ?? false;
        }

        public TSetting Get<TSetting>() where TSetting : ISetting
        {
            return (TSetting) SettingList?.FirstOrDefault(x => x is TSetting);
        }

        public IEnumerable<TSetting> GetAll<TSetting>() where TSetting : ISetting
        {
            return SettingList.OfType<TSetting>();
        }

        public IEnumerable<TSetting> GetAll<TSetting>(Func<TSetting, bool> predicate) where TSetting : ISetting
        {
            return GetAll<TSetting>().Where(predicate);
        }

        public void Add<TSetting>(TSetting setting) where TSetting : ISetting
        {
            settingList.Add(setting as Setting);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR

        /// <summary>
        /// 設定アセットを作成する
        /// [CreateAssetMenu] を用いても良かったが、パスを固定にしたかったので、敢えてヤヤコシイ処理を入れている
        /// </summary>
        [MenuItem("Assets/Create/SettingContainer")]
        public static void CreateScriptableObjects()
        {
            CreateScriptableObject(PrefixDefault);
            CreateScriptableObject(PrefixEnvironment);
        }

        private static SettingContainer CreateScriptableObject(string prefix)
        {
            if (File.Exists(Path.Combine(Application.dataPath, "Resources", $"{string.Format(PathFormat, prefix)}{Extension}")))
            {
                return Resources.Load<SettingContainer>(PathFormat);
            }

            var directoryPath = Path.Combine(Application.dataPath, "Resources");
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var settingContainer = CreateInstance<SettingContainer>();
            AssetDatabase.CreateAsset(settingContainer, Path.Combine("Assets", "Resources", $"{string.Format(PathFormat, prefix)}{Extension}"));
            AssetDatabase.Refresh();
            return settingContainer;
        }

#endif
    }
}
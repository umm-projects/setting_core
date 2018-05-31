using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;

#endif

namespace UnityModule.Settings
{
    public interface ISettingContainer
    {
        TSetting Get<TSetting>() where TSetting : ISetting;

        IEnumerable<TSetting> GetAll<TSetting>() where TSetting : ISetting;

        IEnumerable<TSetting> GetAll<TSetting>(Func<TSetting, bool> predicate) where TSetting : ISetting;

        void Add<TSetting>(TSetting setting) where TSetting : ISetting;
    }

    public class SettingContainer : ScriptableObject, ISettingContainer
    {
        private const string Path = "SettingContainer";

        private const string Extension = ".asset";

        private static ISettingContainer instance;

        // XXX: Should support to Dependency Injection...
        public static ISettingContainer Instance
        {
            get
            {
                return instance
                       // Try to load from Resources
                       ?? (instance = Resources.Load<SettingContainer>(Path))
                       // Create empty instance
                       ?? (instance = CreateInstance<SettingContainer>());
            }
            set { instance = value; }
        }

        [SerializeField] private List<Setting> settingList = new List<Setting>();

        private IEnumerable<ISetting> SettingList
        {
            get { return settingList; }
            set { settingList = value.Cast<Setting>().ToList(); }
        }

        public TSetting Get<TSetting>() where TSetting : ISetting
        {
            return (TSetting)SettingList?.First(x => x is TSetting);
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
            var list = settingList.ToList();
            list.Add(setting as Setting);
            SettingList = list;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 設定アセットを作成する
        /// [CreateAssetMenu] を用いても良かったが、パスを固定にしたかったので、敢えてヤヤコシイ処理を入れている
        /// </summary>
        [MenuItem("Assets/Create/SettingContainer")]
        public static void CreateAsset()
        {
            if (File.Exists(System.IO.Path.Combine(Application.dataPath, "Resources", $"{Path}{Extension}")))
            {
                return;
            }

            var directoryPath = System.IO.Path.Combine(Application.dataPath, "Resources");
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var projectSetting = CreateInstance<SettingContainer>();
            AssetDatabase.CreateAsset(projectSetting, System.IO.Path.Combine("Assets", "Resources", $"{Path}{Extension}"));
            AssetDatabase.Refresh();
        }

#endif
    }
}
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityModule.Settings {

    /// <summary>
    /// 設定クラスの基底クラス
    /// </summary>
    /// <remarks>Singleton アクセサを実装しています</remarks>
    /// <typeparam name="T">設定クラス</typeparam>
    public abstract class Setting<T> : ScriptableObject where T : Setting<T> {

        /// <summary>
        /// 設定アセットのパス
        /// </summary>
        private const string ASSET_PATH_FORMAT = "Assets/Resources/Settings/{0}.asset";

        /// <summary>
        /// 最も優先度が高いインスタンスの実体
        /// </summary>
        protected static T instance;

        /// <summary>
        /// 最も優先度が高いインスタンス
        /// </summary>
        public static T Instance {
            get {
                if (instance == default(T)) {
                    LoadSetting();
                }
                return instance;
            }
        }

        /// <summary>
        /// アセット名を取得
        /// </summary>
        /// <remarks>クラス名とします</remarks>
        /// <returns>アセット名</returns>
        protected static string GetAssetName() {
            return typeof(T).Name;
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        protected static void LoadSetting() {
            instance = Resources.Load<T>("Settings/" + GetAssetName());
            // もし、インスタンスが生成出来なかった (= アセットがない) 場合は、空のインスタンスを作る
            if (instance == default(T)) {
                instance = CreateInstance<T>();
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// 設定アセットを作成する
        /// </summary>
        protected static void CreateAsset() {
            T projectSetting = CreateInstance<T>();
            string directoryPath = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), string.Format(ASSET_PATH_FORMAT, GetAssetName()))));
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            AssetDatabase.CreateAsset(projectSetting, string.Format(ASSET_PATH_FORMAT, GetAssetName()));
            AssetDatabase.Refresh();
        }

#endif

    }

}

using System.Linq;
using UnityEngine;

namespace UnityModule.Settings {

    /// <summary>
    /// 優先度を持たせて複数アセット存在した場合でも解決を行えるようにする
    /// </summary>
    /// <remarks>Priority の最も大きいアセットが採用されます</remarks>
    /// <typeparam name="T">設定クラス</typeparam>
    public abstract class PrioritizeSetting<T> : Setting<T> where T : PrioritizeSetting<T> {

        /// <summary>
        /// 優先度の実体
        /// </summary>
        [SerializeField]
        private int priority;

        /// <summary>
        /// 優先度
        /// </summary>
        public int Priority {
            get {
                return this.priority;
            }
            set {
                this.priority = value;
            }
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        protected new static void LoadSetting() {
            LoadProjectSettingByPriority();
        }

        /// <summary>
        /// 最も優先度が高い ProjectSetting を取得する
        /// </summary>
        /// <returns>ProjectSetting のインスタンス</returns>
        private static void LoadProjectSettingByPriority() {
            T[] settings = Resources.LoadAll<T>(GetAssetName());
            if (settings.Length == 0) {
                return;
            }
            instance = settings.ToList().OrderByDescending(x => x.Priority).First();
        }

    }

}
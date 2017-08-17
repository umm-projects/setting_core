# What?

* `***Setting.asset` 的な設定を管理するためのアセットの基底クラスです。

# Why?

* 結構な頻度で使うクラスなので、共通化して切り出したくなりました。

# Install

```shell
$ npm install @umm/setting_core
```

# Usage

```csharp
using UnityEngine;
using UnityModule.Settings;

class HogeSetting : Setting<HogeSetting> {

    public string Fuga;

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Setting/Create HogeSetting")]
    public static void CreateHogeSetting() {
        CreateAsset();
    }

#endif

}

class PiyoSetting : PrioritizeSetting<PiyoSetting> {

    public float Ponyo;

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Setting/Create PiyoSetting")]
    public static void CreatePiyoSetting() {
        CreateAsset();
    }

#endif


}

class Sample : MonoBehaviour {

    void Start() {
        Debug.Log(HogeSetting.Instance.Fuga);
        Debug.Log(PiyoSetting.Instance.Ponyo);
    }

}

```

* `Setting<T>` は基本的な設定クラスの基底クラスです。
  * `.Instance` という Singleton Getter を提供します。
* `PrioritizeSetting<T>` は `Setting<T>` の機能に加えて、プロジェクト内に複数の設定アセットがあった場合でも、優先度を付けて一元化出来るようにしたクラスです。
  * 現在の所、値の継承は実装しておりません。
* `Setting<T>.CreateAsset()` を呼ぶことで、空の `***Setting.asset` が `Assets/Resources/Settings/` 以下に生成されます。
  * メニューに表示させるために、 `UnityEditor.MenuItemAttribute` を用いると良いでしょう。
  * `#if UNITY_EDITOR ... #endif` で囲まないと実機ビルドした際にコンパイルエラーになります。

# License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)


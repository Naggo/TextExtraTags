# Text Extra Tags

リッチテキスト風のタグを解析・変換するためのパーサーを追加します。

## Installation URL (for UPM)

Recent release version:
```
https://github.com/Naggo/TextExtraTags.git?path=src/TextExtraTags#v2.0.0
```

Latest developments:
```
https://github.com/Naggo/TextExtraTags.git?path=src/TextExtraTags
```

## 使い方

主な設定はProject Settingsの`Text Extra Tags`から行います。
以下はメニュー項目の説明です。
```md
* Create Asset:
設定用のアセットを作成します。
アセットは移動しても問題はありませんが、名前を変えずに`Resources`フォルダの中に含める必要があります。

- Name:
パーサーの名前を設定します。`Parser`オブジェクトを取得する際に使用します。

- Capacity Level:
内部バッファの初期容量を決めます。数値と容量の対応関係は`1:256, 2:512, 3:1024, 4:2048, 5:4096`となっています。
実行中に足りなくなった場合は自動で２倍になります。

- Iteration Limit:
解析処理における再帰回数の上限を決めます。`1`に設定した場合、タグの変換処理で追加された文字列は全て解析されなくなります。
`2`以上に設定することを推奨します。

- Features:
パーサーに追加されたFeatureの一覧です。
`ExtraTagFeature`の派生型にシリアライズ可能なフィールドがある場合、ここに表示されます。

* Add Feature:
パーサーに`ExtraTagFeature`を追加します。
`TypeCache.GetTypesDerivedFrom()`から取得した派生型を一覧表示します。

* Create new Parser:
パーサーを新規作成します。

```

## 標準機能

`TextExtraTags.Standards`には標準的な機能やサンプルが入っています。
以下はその説明です。
```md
- TextExtraParser
フィールドに入力された文字列を解析してTextMeshProに反映するコンポーネントです。
解析処理の動作検証にも利用できます。

- RichTextTagsSupport
文字列内のリッチテキスト用のタグを簡易的に判定します。
判定されたタグは文字数の計算から除外され、以降のフィルターの処理をスキップされます。
これより先にタグの処理を行いたい場合、`RichTextTagsSupport`より上にFeatureを配置することで可能です。

- RubyTagFeature
`<ruby=かんじ>漢字</ruby>`のようにルビを振るタグを追加するサンプルです。
漢字の開始地点に`RubyTag`が追加され、漢字とルビの文字数が記録されます。
正常に動作するには`RichTextTagsSupport`と、`Iteration Limit`を`2`以上に設定する必要があります。

- IPoolableExtraTag, PoolableExtraTagCollection
タグデータのプールを実装するためのインターフェイスとコレクションです。
`PoolableExtraTagCollection`から要素が削除される際、その要素が`IPoolableExtraTag`を継承していたらそれをプールに戻します。

```

## License

This project is licensed under the MIT License. For more details, see [LICENSE.md](LICENSE.md) and [NOTICE.md](NOTICE.md).

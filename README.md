# Text Extra Tags

リッチテキスト風のタグを解析・変換するためのパーサーを追加します。

## Installation URL (for UPM)

Recent release version:
```
https://github.com/Naggo/TextExtraTags.git?path=src/TextExtraTags#v1.1.0
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
設定用のアセットを作成します。作成後は移動しても問題はありませんが、`Resources`フォルダの中に含める必要があります。

- Name:
パーサーの名前を設定します。`Parser`オブジェクトを取得する際に使用します。

- Capacity Level:
内部バッファの初期容量を決めます。数値と容量の対応関係は`1:256, 2:512, 3:1024, 4:2048, 5:4096`となっています。
実行中に足りなくなった場合は自動で２倍になります。

- Iteration Limit:
解析処理における再帰回数の上限を決めます。`1`に設定した場合、タグの変換処理で追加された文字列は全て解析されなくなります。
`2`以上に設定することを推奨します。

- Features:
パーサーに追加された`ExtraTagFeature`の一覧です。
`ExtraTagFeature`の派生クラスにシリアライズ可能なフィールドがある場合、ここに表示されます。

* Add Feature:
パーサーに`ExtraTagFeature`を追加します。
`TypeCache.GetTypesDerivedFrom()`から取得した派生クラスを一覧表示します。

* Create new Parser
パーサーを新規作成します。

```

## TextExtraTags.Standards



## License

This project is licensed under the MIT License. For more details, see [LICENSE.md](LICENSE.md) and [NOTICE.md](NOTICE.md).

# RedmineNotification

Redmineの指定クエリをHipchat または Chatworkに整形して通知するツールです。

ツール自体は実行をすると実行時のクエリー内容をキャッシュします。
以降は実行毎に、そのキャッシュとの差分を比較しながら変更があった場合のみ
通知を行います。

# 実行方法

RedmineNotification settings.xml

# settings.xml

<redmine-url>RedmineのURL</redmine-url>
<redmine-project>Redmine上のプロジェクト名</redmine-project>
<redmine-apikey>RedmineのAPIKEY</redmine-apikey>
<redmine-query-id>利用するクエリーID</redmine-query-id>
<chatwork-title>チャットワークのタイトルに表示する文字列</chatwork-title>
<chatwork-apikey>チャットワークのAPIKEY</chatwork-apikey>
<chatwork-roomid>チャットワークのルーム番号</chatwork-roomid>
<hipchat-authtoken>hipchatのauthtoken</hipchat-authtoken>
<hipchat-roomid>hipchatのroomid</hipchat-roomid>
  
  

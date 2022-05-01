using System;
using System.Data.Odbc;

namespace cs_core_mysql_01
{
    class Program
    {
        static void Main(string[] args)
        {
            // *******************************
            // 接続文字列作成
            // *******************************

            // 新しい OdbcConnectionStringBuilder オブジェクトを作成
            OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder();

            // ドライバ文字列をセット ( 波型括弧{} は必要ありません ) 
            // 文字列を正確に取得するには、レジストリ : HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBCINST.INI
            builder.Driver = "MySQL ODBC 8.0 Unicode Driver";

            // 接続用のパラメータを追加
            builder.Add("server", "localhost");
            builder.Add("database", "lightbox");
            builder.Add("uid", "root");
            builder.Add("pwd", "");

            // 接続文字列の内容を確認
            Console.WriteLine(builder.ConnectionString);

            // 一旦停止
            Console.ReadLine();

            using (OdbcConnection myCon = new OdbcConnection())
            using (OdbcCommand myCommand = new OdbcCommand())
            {

                // *******************************
                // 接続
                // *******************************
                try
                {
                    myCon.ConnectionString = builder.ConnectionString;
                    myCon.Open();
                }
                catch (OdbcException ex)
                {
                    Console.WriteLine("接続エラーです");

                    string errorMessages = "";
                    int i = 0;

                    for (i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages +=
@$"Index #{i.ToString()}
Message: {ex.Errors[i].Message}
NativeError: {ex.Errors[i].NativeError.ToString()}
Source: {ex.Errors[i].Source}\r\nSQL: {ex.Errors[i].SQLState}
";
                    }

                    Console.WriteLine(errorMessages);
                    // 一旦停止
                    Console.ReadLine();

                    // アプリケーション終了
                    return;
                }

                // *******************************
                // レコードセットを取得する為の SQL
                // ※ DATE_FORMAT は MySQL 用
                // *******************************
                string myQuery = "SELECT 社員マスタ.*,DATE_FORMAT(生年月日,'%Y-%m-%d') as 誕生日 from 社員マスタ";

                // 実行する為に必要な情報をセット
                myCommand.CommandText = myQuery;
                myCommand.Connection = myCon;

                // 実行後にレコードセットを取得する為のオブジェクトを作成
                // ( SQL の実行結果 )
                OdbcDataReader myReader = myCommand.ExecuteReader();

                // *******************************
                // 読み出し
                // Read メソッドは、行が存在する場合は true、
                // それ以外の場合は false を返します
                // *******************************
                while (myReader.Read())
                {
                    string result = "";

                    result += @$"{GetValue(myReader, "社員コード")} : {GetValue(myReader, "氏名")} : {GetValue(myReader, "給与")} : {GetValue(myReader, "作成日")} : {GetValue(myReader, "更新日")} : {GetValue(myReader, "生年月日")} : {GetValue(myReader, "誕生日")}";

                    // 内容をコンソールに表示
                    Console.WriteLine(result);

                }

                // リーダーを閉じる
                myReader.Close();

                // 接続を閉じる
                myCon.Close();
            }
            // 接続の終わり

            // 一旦停止
            Console.ReadLine();

        }
        
        // ********************************************************
        // 列データ取得
        //
        // 列データを文字列として取得しますが、NULL の場合は
        // 空文字列を返します
        // ********************************************************
        static string GetValue(OdbcDataReader myReader, string strName)
        {

            string ret = "";
            int fld = 0;

            // 指定された列名より、テーブル内での定義順序番号を取得
            fld = myReader.GetOrdinal(strName);
            // 定義順序番号より、NULL かどうかをチェック
            if (myReader.IsDBNull(fld))
            {
                ret = "";
            }
            else
            {
                // NULL でなければ内容をオブジェクトとして取りだして文字列化する
                ret = myReader.GetValue(fld).ToString();
            }

            // 列の値を返す
            return ret;

        }
        
    }
}

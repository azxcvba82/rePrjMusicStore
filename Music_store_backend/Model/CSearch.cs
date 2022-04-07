using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Music_store_backend.Util;

namespace Music_store_backend.Model
{
    public class CSearch
    {
        public static IEnumerable<tAlbumResponse> ConstructAllAlbum(AppSettings Util)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            //SqlDataAdapter myCommand = new SqlDataAdapter("SELECT * FROM tAlbum where fStatus='2' Order by fYear desc", myConnection);
            //DataSet ds = new DataSet();
            //myCommand.Fill(ds, "t1");

            IEnumerable<tAlbum> tA = myConnection.Query<tAlbum>("SELECT * FROM tAlbum where fStatus='2' Order by fYear desc", new tAlbum());
            List<tAlbumType> tTy = CSearch.takeAllType(Util);
            List<tAlbumResponse> tARs = new List<tAlbumResponse>(){ };
            foreach (tAlbum album in tA)
            {
                tAlbumResponse tAR = new tAlbumResponse();
                tAR.fAccount = album.fAccount;
                tAR.fActivityID = album.fActivityID;
                tAR.fAlbumID = album.fAlbumID;
                tAR.fAlbumName = album.fAlbumName;
                tAR.fALPrice = album.fALPrice;
                tAR.fCoverPath = album.fCoverPath;
                tAR.fDiscount = album.fDiscount;
                tAR.fKinds = album.fKinds;
                tAR.fMaker = album.fMaker;
                tAR.fStatus = album.fStatus;
                tAR.fType = tTy.FirstOrDefault(p => p.fTypeID == album.fType).fTypeName;
                tAR.fYear = album.fYear;
                tARs.Add(tAR);
            }
            IEnumerable<tAlbumResponse> tARE = tARs;
            return tARE;
        }

        public static IEnumerable<tAlbumKind> takeAllKind(AppSettings Util)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            IEnumerable<tAlbumKind> tA = myConnection.Query<tAlbumKind>("SELECT * FROM tAlbumKind", new tAlbumKind());
            return tA;
        }

        public static List<tAlbumType> takeAllType(AppSettings Util)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tAlbumType> tAT = myConnection.Query<tAlbumType>("SELECT * FROM tAlbumType ").ToList();
            return tAT;
        }

        public static List<tAlbum> byKindPage(AppSettings Util, int kindID)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            string kind = myConnection.QueryFirstOrDefault<tAlbumKind>("SELECT * FROM tAlbumKind WHERE KindID = @KindID", new { KindID = kindID }).KindName;
            List<tAlbum> tAT = myConnection.Query<tAlbum>("SELECT * FROM tAlbum WHERE fKinds LIKE @fKinds AND fStatus = '2' Order by fYear desc", new { fKinds = "%"+kind+"%" }).ToList();
            return tAT;
        }

        public static List<KeywordsSearchResponse> byKeyword(AppSettings Util, string keyword)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<KeywordsSearchResponse> tA = myConnection.Query<KeywordsSearchResponse>("SELECT * FROM tProducts a left join tAlbum b on a.fAlbumID = b.fAlbumID where b.fStatus='2' and a.fProductName like @fProductName order by b.fYear", new { fProductName = "%"+keyword+"%" }).ToList();
            return tA;
        }

        //進階搜索，找的是符合條件的專輯
        public static IEnumerable<KeywordsSearchResponse> byAdvanced(AppSettings Util, CAdvancedSearchObject keyObj)
        {
            List<tProduct> tP = null;
            List<KeywordsSearchResponse> tA = null;
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            if (!string.IsNullOrEmpty(keyObj.adSong) ||
                !string.IsNullOrEmpty(keyObj.adSinger) ||
                !string.IsNullOrEmpty(keyObj.adComposer))
                {
                tP = myConnection.Query<tProduct>("SELECT * FROM tProducts a left join tAlbum b on a.fAlbumID = b.fAlbumID where a.fProductName like @fProductName AND a.fSinger like @fSinger AND a.fComposer like @fComposer AND b.fStatus='2'", new { fProductName = "%" + keyObj.adSong + "%", fSinger = "%" + keyObj.adSinger + "%", fComposer = "%" + keyObj.adComposer + "%" }).ToList();
            }
            if ((!string.IsNullOrEmpty(keyObj.adAlbum) ||
                !string.IsNullOrEmpty(keyObj.adGroup)) &&
                keyObj.adType != "1") {
                tA = myConnection.Query<KeywordsSearchResponse>("SELECT * FROM tAlbum where fAlbumName like @fAlbumName AND fMaker like @fMaker AND fType = @fType AND fStatus='2'", new { fAlbumName = "%" + keyObj.adAlbum + "%", fMaker = "%" + keyObj.adGroup + "%", fType = keyObj.adType }).ToList();
            }else if(!string.IsNullOrEmpty(keyObj.adAlbum) ||
                !string.IsNullOrEmpty(keyObj.adGroup))
            {
                tA = myConnection.Query<KeywordsSearchResponse>("SELECT * FROM tAlbum where fAlbumName like @fAlbumName AND fMaker like @fMaker AND fStatus='2'", new { fAlbumName = "%" + keyObj.adAlbum + "%", fMaker = "%" + keyObj.adGroup + "%"}).ToList();
            }

            //if (keyObj.adType != 1)//不指定的id為1，資料庫若有更改得做修正
            //{
            //    data.album = data.album.Where(a => a.fType == keyObj.adType).ToArray();
            //}
            if (keyObj.adKinds != null)
            {
                string[] adkinds = keyObj.adKinds.Split(',');
                for (int i = 0; i < adkinds.Length; i++)
                {
                    //lambda似乎不能直接接受陣列[i]的內容，必須先轉換成一個變數
                    string kind = adkinds[i];
                    List<KeywordsSearchResponse> ta = myConnection.Query<KeywordsSearchResponse>("SELECT * FROM tAlbum where fKinds like @fKinds AND fStatus='2'", new { fKinds = "%" + kind + "%"}).ToList();
                    if (tA != null)
                    {
                        tA.AddRange(ta);
                    }
                    else
                    {
                        tA = ta;
                    }
                    // tA = tA.Where(a => a.fKinds.Contains(kind)).ToList();
                }
            }

            //因為前面所挑出來的專輯可能會重複，因此要再過濾一次重複的專輯
            List<KeywordsSearchResponse> result = new List<KeywordsSearchResponse>();
            if (tA != null)
            {
                foreach (var a in tA)
                {
                    if (!result.Contains(a))
                    {
                        result.Add(a);
                    }
                }
            }
            if (tP != null)
            {
                foreach (var b in tP)
                {
                    KeywordsSearchResponse tAObj = myConnection.QueryFirstOrDefault<KeywordsSearchResponse>("SELECT * FROM tAlbum where fAlbumID = @fAlbumID", new { fAlbumID = b.fAlbumID });
                    if (!result.Contains(tAObj))
                    {
                        result.Add(tAObj);
                    }
                }
            }
            return result;
        }

        //以下後臺用
        //尋找屬於特定活動的專輯
        public static List<tAlbum> byEvent(AppSettings Util, int eventId)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tAlbum> tAT = myConnection.Query<tAlbum>("SELECT * FROM tAlbum WHERE fActivityID = @fActivityID AND fStatus = '2' Order by fYear desc", new { fActivityID = eventId }).ToList();
            return tAT;
        }
    }
}

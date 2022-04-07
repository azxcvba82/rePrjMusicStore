using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CAutoComplete
    {
        public static List<string> SearchSong(AppSettings Util, string term)
        {
            List<string> result = new List<string>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tProduct> tP = myConnection.Query<tProduct>("SELECT * FROM tProducts a left join tAlbum b on a.fAlbumID = b.fAlbumID where a.fProductName like @fProductName AND b.fStatus='2'", new { fProductName = "%" + term + "%" }).ToList();
            //var names = tP.Where(p => p.fProductName.Contains(term));
            foreach (tProduct t in tP)
            {
                if (!result.Contains(t.fProductName))
                {
                    result.Add(t.fProductName);
                }
            }
            result = result.Take(10).ToList();
            return result;
        }

        public static List<string> SearchAlbum(AppSettings Util, string term)
        {
            List<string> result = new List<string>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tAlbum> tA = myConnection.Query<tAlbum>("SELECT * FROM tAlbum where fAlbumName like @fAlbumName AND fStatus='2'", new { fAlbumName = "%" + term + "%" }).ToList();
            foreach (tAlbum t in tA)
            {
                if (!result.Contains(t.fAlbumName))
                {
                    result.Add(t.fAlbumName);
                }
            }
            result = result.Take(10).ToList();
            return result;
        }

        public static List<string> SearchSinger(AppSettings Util, string term)
        {
            List<string> result = new List<string>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tProduct> tP = myConnection.Query<tProduct>("SELECT * FROM tProducts a left join tAlbum b on a.fAlbumID = b.fAlbumID where a.fSinger like @fSinger AND b.fStatus='2'", new { fSinger = term + "%" }).ToList();
            foreach (tProduct t in tP)
            {
                if (!result.Contains(t.fSinger))
                {
                    result.Add(t.fSinger);
                }
            }
            result = result.Take(10).ToList();
            return result;
        }

        public static List<string> SearchGroup(AppSettings Util, string term)
        {
            List<string> result = new List<string>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tAlbum> tA = myConnection.Query<tAlbum>("SELECT * FROM tAlbum where fMaker like @fMaker AND fStatus='2'", new { fMaker = "%" + term + "%" }).ToList();
            foreach (tAlbum t in tA)
            {
                if (!result.Contains(t.fMaker))
                {
                    result.Add(t.fMaker);
                }
            }
            result = result.Take(10).ToList();
            return result;
        }

        public static List<string> SearchComposer(AppSettings Util, string term)
        {
            List<string> result = new List<string>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tProduct> tP = myConnection.Query<tProduct>("SELECT * FROM tProducts a left join tAlbum b on a.fAlbumID = b.fAlbumID where a.fComposer like @fComposer AND b.fStatus='2'", new { fComposer = "%" + term + "%" }).ToList();
            foreach (tProduct t in tP)
            {
                if (!result.Contains(t.fComposer))
                {
                    result.Add(t.fComposer);
                }
            }
            result = result.Take(10).ToList();
            return result;
        }
    }
}

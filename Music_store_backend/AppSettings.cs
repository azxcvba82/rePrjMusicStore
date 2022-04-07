using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Music_store_backend
{
    public class AppSettings
    {
        public string ConnectionString { get; set; } //DI injected 
        public bool EAQueryDataCacheEnabled { get; set; } //DI injected 
        public bool DebugMode { get; set; } //DI injected 

        //public enum enumPlaymode
        //{
        //    PlayThenOff = 0,
        //    Continue = 1,
        //    SingleSongContinue = -1,
        //    Random = 2,
        //    LastSong = 4,
        //}

        public string getEnumPlaymode(int value)
        {
            string result ="";
            switch (value)
            {
                case 1:
                    result = "Continue";
                    break;
                case 0:
                    result = "PlayThenOff";
                    break;
                case -1:
                    result = "SingleSongContinue";
                    break;
                case 2:
                    result = "Random";
                    break;
                case 4:
                    result = "LastSong";
                    break;
            }
            return result;
        }

        public SqlConnection GetSqlConnectionMBMan()
        {
            return new SqlConnection(this.ConnectionString);
        }

        public string GetSqlConnectionStringMBMan()
        {
            return this.ConnectionString;
        }

        public bool IsValidUserAndPassword(String userId, String password, out String Message)
        {

            SqlConnection myConnection = this.GetSqlConnectionMBMan();
            SqlDataAdapter myCommand = new SqlDataAdapter("SELECT * FROM tMember where fAccount=@userId", myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar, 50));
            myCommand.SelectCommand.Parameters["@userId"].Value = userId;
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");
            String loginErrorMessage = @"User name or password is incorrect, please try again";
            if (0 == ds.Tables[0].Rows.Count)
            {
                Message = loginErrorMessage;
                return false;

            }

            // for test not encry it
            //String passwordEntered = Encrypt(MD5(password.Trim()));
            String passwordEntered = password.Trim();

            if (passwordEntered != ds.Tables[0].Rows[0]["fPassword"].ToString())
            {
                Message = loginErrorMessage;
                return false;
            }
            if ("Y" != ds.Tables[0].Rows[0]["fisActive"].ToString())
            {
                Message = loginErrorMessage;
                return false;
            }

            Message = "Login Sucessfully";
            return true;
        }

        public byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public string Decrypt(string sValue)
        {
            // byte[] buffer = Convert.FromBase64String(sValue);
            byte[] buffer = StringToByteArray(sValue);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(Encoding.Default.GetBytes(mstrKey), Encoding.Default.GetBytes(mstrIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        const string mstrKey = "MUSICBEN";

        const string mstrIV = "COODOOEOOO";

        public string Encrypt(string sValue)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(sValue);

            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.Default.GetBytes(mstrKey), Encoding.Default.GetBytes(mstrIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();

            // return Convert.ToBase64String(ms.ToArray());
            return BytesToHexString(ms.ToArray());

        }

        public string MD5(string input)
        {
            string output = "";
            string sValue = input;
            byte[] data = System.Text.Encoding.Default.GetBytes(sValue);
            System.Security.Cryptography.MD5CryptoServiceProvider m = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] buffer;
            buffer = m.ComputeHash(data);
            output = BytesToHexString(buffer);
            return output;
        }

        public string BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);
            int counter;

            for (counter = 0; counter <= bytes.Length - 1; counter++)
            {
                hexString.Append(string.Format("{0:X2}", bytes[counter]));
            }
            return hexString.ToString();
        }

        public string GetDBString(object o)
        {

            if (Convert.IsDBNull(o))
            {
                return "";
            }
            else
            {
                return (String)o;
            }
        }

        public bool SQLExecute(SqlCommand myCommand, out String message)
        {
            try
            {
                bool bOK = true;
                SqlTransaction myTransaction = null;
                String outMessage = "";
                try
                {
                    myCommand.Connection.Open();
                    myTransaction = myCommand.Connection.BeginTransaction();
                    myCommand.Transaction = myTransaction;
                    myCommand.ExecuteNonQuery();

                }
                catch (System.Data.SqlClient.SqlException Ex)
                {
                    outMessage = Ex.Message;
                    myTransaction.Rollback();
                    bOK = false;
                }
                finally
                {
                    if (bOK)
                    {
                        myTransaction.Commit();
                    }
                    myCommand.Connection.Close();
                }
                if (bOK)
                {
                    message = "Data have been updated successfully.";
                }
                else
                {
                    message = outMessage;
                }
                return bOK;
            }
            catch (System.Exception Ex)
            {
                message = Ex.Message;
                return false;
            }
        }

    }
}

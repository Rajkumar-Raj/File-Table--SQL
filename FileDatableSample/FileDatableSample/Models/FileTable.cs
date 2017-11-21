using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FileDatableSample
{
    public class FileTable
    {

        public static bool InsertFile(HttpPostedFileBase file)
        {
            try
            {
                // sql connection
                SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConFileTableDB"].ConnectionString);

                // input filesream to byte conversion
                Stream fs = file.InputStream;
                BinaryReader br = new BinaryReader(fs);
                Byte[] bytes = br.ReadBytes((Int32)fs.Length);

                string fileName = file.FileName;

                // Sql insert operation
                using (sqlcon)
                {
                    sqlcon.Open();
                    string strQuery = "insert into FileTableTb(name, file_stream)" + " values(@Name, @Data)";
                    SqlCommand cmd = new SqlCommand(strQuery, sqlcon);
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = fileName;
                    cmd.Parameters.Add("@Data", SqlDbType.Binary).Value = bytes;
                    cmd.ExecuteNonQuery();
                    sqlcon.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static FileTableData GetFileData(string stream_id)
        {
            try
            {
               // SelectPhoto();
                FileTableData f = new FileTableData();

                // sql connection
                SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConFileTableDB"].ConnectionString);

                using (sqlcon)
                {
                    sqlcon.Open();
                    string strQuery = "select name,file_stream from FileTableTb where stream_id=@stream_id"; //file_stream.PhysicalPathName() AS physicalpath,FileTableRootPath() + file_stream.GetFileNamespacePath() as virtualpath,
                    SqlCommand cmd = new SqlCommand(strQuery, sqlcon);
                    cmd.Parameters.AddWithValue("@stream_id", stream_id);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        f.bytes = (byte[])sdr["file_stream"];
                        //f.physicalfilePath = sdr["physicalpath"].ToString();
                        //f.virtualfilePath = sdr["virtualpath"].ToString();
                        f.fileName = sdr["name"].ToString();
                    }
                    sqlcon.Close();
                }
                return f;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public static Image SelectPhoto()
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConFileTableDB"].ConnectionString);
                const string SelectTSql = @"SELECT name,file_stream.PathName(),GET_FILESTREAM_TRANSACTION_CONTEXT() FROM FileTableTb where stream_id='2EC9A750-AAC3-E711-9BFF-204747404668'";

                Image photo;
                string serverPath;
                string desc;
                byte[] serverTxn;
                using (TransactionScope ts = new TransactionScope())
                {
                    using (conn)
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand(SelectTSql, conn))
                        {
                            //cmd.Parameters.Add("@PhotoId", SqlDbType.Int).Value = photoId;

                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                rdr.Read();
                                desc = rdr.GetSqlString(0).Value;
                                serverPath = rdr.GetSqlString(1).Value;
                                serverTxn = rdr.GetSqlBinary(2).Value;
                                rdr.Close();
                            }
                        }
                        photo = LoadPhotoImage(serverPath, serverTxn);
                    }
                    ts.Complete();
                }


                return photo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Image LoadPhotoImage(string filePath, byte[] txnToken)
        {
            try
            {
                Image photo;

                using (SqlFileStream sfs =
                  new SqlFileStream(filePath, txnToken, FileAccess.Read))
                {
                    photo = Image.FromStream(sfs);
                    sfs.Close();
                }
                return photo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }


    public class FileTableData
    {
        public string fileName { get; set; }
        public string physicalfilePath { get; set; }
        public string virtualfilePath { get; set; }
        public byte[] bytes { get; set; }
    }

}
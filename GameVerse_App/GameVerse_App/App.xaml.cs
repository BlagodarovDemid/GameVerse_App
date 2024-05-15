using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Xamarin.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;
using MySqlConnector;

namespace GameVerse_App
{
    public partial class App : Application
    {
        public int UserID = 1;

        public MySqlConnection cn = new MySqlConnection(@"server=192.168.56.1;Port=3307;uid=admin;pwd=1234;database=gameverse;");
        public MySqlCommand cmd;
        public MySqlDataReader dr;

        static ConnectionInfo connection = new ConnectionInfo("192.168.56.1", 22, "MSII", new PasswordAuthenticationMethod("MSII", "5689"));
        public SftpClient sftpClient = new SftpClient(connection);

        public App()
        {
            InitializeComponent();

            try
            {
                cn.Close();
                cn.Open();
            }
            catch (Exception ex) { Console.WriteLine($"Проблема соединения с БД: {0}", ex.Message); }

            try
            {
                sftpClient.Connect();
                sftpClient.Disconnect();
            }
            catch (Exception ex) { Console.WriteLine($"Проблема соединения с БД: {0}", ex.Message); }

            MainPage = new NavigationPage(new Authorization());
        }

        public byte[] DownloadServerImageFromPath(string ImagePathInServer)
        {
            MemoryStream imageStream = new MemoryStream();
            try
            {
                sftpClient.Connect();
                sftpClient.DownloadFile(ImagePathInServer, imageStream);
                sftpClient.Disconnect();
                return imageStream.ToArray();
            }
            catch (SftpPathNotFoundException)
            {
                sftpClient.Disconnect();
                return new byte[0];
            }
        }
        public bool IsDirectoryInServerExists(string FolderPathInServer)
        {
            try
            {
                sftpClient.Connect();
                sftpClient.ChangeDirectory(FolderPathInServer);
                sftpClient.Disconnect();
                return true;
            }
            catch (SftpPathNotFoundException)
            {
                sftpClient.Disconnect();
                return false;
            }
        }

        public string GetHashString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = "";
            foreach (byte b in byteHash)
            {
                hash += string.Format("{0:x2}", b);
            }
            return hash;
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#№!$%^:;&?*()-=+_/|.,'[]{}<>`~";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public string CreateVerificationCode(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#№!$%^:;&?*()-=+_/|.,'[]{}<>`~";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

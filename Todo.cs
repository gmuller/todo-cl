using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Toodledo.Model;
using Toodledo.Model.API;
using Toodledo.Client;
using System.Net;
using System.Collections.Specialized;
using CommandLine;
using System.Configuration;
using System.Security;

namespace Todo
{
    public class Todo
    {
        public SecureString UserID = DecryptString(Properties.Settings.Default.userId);
        public SecureString password = DecryptString(Properties.Settings.Default.password);
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

        public static int Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                bool optionSet = false;
                // consume Options type fields
                if (options.username != null)
                {
                    optionSet = true;
                    Properties.Settings.Default.userId = EncryptString(ToSecureString(options.username));
                    Properties.Settings.Default.Save();
                    Console.Out.WriteLine("Successfully Set Username");
                }

                if (options.password != null)
                {
                    optionSet = true;
                    Properties.Settings.Default.password = EncryptString(ToSecureString(options.password));
                    Properties.Settings.Default.Save();
                    Console.Out.WriteLine("Successfully Set Password");
                }

                if (optionSet)
                {
                    Console.Out.WriteLine("Options set. Todo is ready to do it.");
                    return 0;
                }
            }

            Todo todo = new Todo();
            StringBuilder builder = new StringBuilder();
            foreach (String arg in args)
            {
                builder.Append(arg);
                builder.Append(" ");
            }
            try
            {
                todo.addTask(builder.ToString());
            } catch (Exception e) {
                Console.Out.WriteLine(e.Message);
            }
            return 0;
        }

        public void addTask(String taskName)
        {
            var task = new Task() { Name = taskName, Folder = getFolder() };
            var added = Tasks.AddTask(task);
        }

        public Folder getFolder()
        {
            Folder folder = General.GetFolders().First();
            return folder;
        }

        private Session _session = null;
        public Session Session
        {
            get { return _session ?? Session.Create(ToInsecureString(UserID), ToInsecureString(password), null); }
        }

        public ITasks Tasks
        {
            get { return (ITasks)this.Session; }
        }

        public IGeneral General
        {
            get { return (IGeneral)this.Session; }
        }

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
}

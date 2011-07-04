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
    class Todo
    {
        public SecureString UserID = DecryptString(Properties.Settings.Default.userId);
        public SecureString password = DecryptString(Properties.Settings.Default.password);
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

        static int Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                if (options.username != null)
                {
                    Properties.Settings.Default.userId = EncryptString(ToSecureString(options.username));
                    Properties.Settings.Default.Save();
                    Console.Out.WriteLine("Successfully Set Username");
                }

                if (options.password != null)
                {
                    Properties.Settings.Default.password = EncryptString(ToSecureString(options.password));
                    Properties.Settings.Default.Save();
                    Console.Out.WriteLine("Successfully Set Password");
                }

                if (options.settings != null)
                {
                    string[] settings = options.settings.Split(':');
                    if (settings.Length == 2 && settings[0].ToLower() == "folder")
                    {
                        Properties.Settings.Default.folder = settings[1];
                        Properties.Settings.Default.Save();
                    }

                    if (settings.Length == 2 && settings[0].ToLower() == "context" && settings[1] != null)
                    {
                        Properties.Settings.Default.folder = settings[1];
                        Properties.Settings.Default.Save();
                    }
                }

                Todo todoCL = new Todo();
                Task newTask = new Task();
                newTask.Name = string.Join(" ", options.task.Select(i => i.ToString()).ToArray());
                
                string folder = options.folder != null ? options.folder : Properties.Settings.Default.folder;
                if (folder != null) newTask.Folder = todoCL.getFolder(folder);

                string context = options.context != null ? options.context : Properties.Settings.Default.context;
                if (context != null) newTask.Context = todoCL.getContext(context);

                if (options.tags != null) newTask.Tag = string.Join(",", options.tags.Select(i => i.ToString()).ToArray());
                if (options.duedate != null)
                {
                    DateTime? dueDate = DateParser.Parse(options.duedate);
                    if (dueDate != null) newTask.Due = (DateTime)dueDate;
                }
                int taskLength;
                if (options.length != null && int.TryParse(options.length, out taskLength)) newTask.Length = taskLength;

                try
                {
                    todoCL.addTask(newTask);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                }
            }
            return 0;
        }

        private Context getContext(string contextName)
        {
            IEnumerable<Context> contextList = General.GetContexts();
            return contextList.FirstOrDefault(context => context.Name.ToLower() == contextName.ToLower());
        }

        public void addTask(Task task)
        {
            var added = Tasks.AddTask(task);
        }

        public Folder getFolder(string folderName)
        {
            IEnumerable<Folder> folderList = General.GetFolders();
            return folderList.FirstOrDefault(folder => folder.Name.ToLower() == folderName.ToLower());
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

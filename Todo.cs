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
using System.Configuration;

namespace Todo
{
    public class Todo
    {
        public string UserID = Properties.Settings.Default.userId;
        public string password = Properties.Settings.Default.password;

        public static void Main(string[] args)
        {
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
            get { return _session ?? Session.Create(UserID, password, null); }
        }

        public ITasks Tasks
        {
            get { return (ITasks)this.Session; }
        }

        public IGeneral General
        {
            get { return (IGeneral)this.Session; }
        }
    }
}

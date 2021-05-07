using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Kemorave.SQLite;
using Kemorave.SQLite.Options;
using Kemorave.SQLite.SQLiteAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class DBTest
    {
        private class Image : Kemorave.SQLite.ModelBase.Model
        {
            public Image()
            {
            }

            public Image(SQLiteDataBase dataBase, string path, string type, long itemId) : base(dataBase)
            {
                Path = path;
                Type = type;
                ItemId = itemId;
            }
            [Kemorave.SQLite.SQLiteAttribute.TableColumn("Path", SQLiteType.TEXT,false,false,false)]
            public string Path { get; set; }
            [Kemorave.SQLite.SQLiteAttribute.TableColumn("Type", SQLiteType.TEXT, false, false, false)]
            public string Type { get; set; }
            [Kemorave.SQLite.SQLiteAttribute.TableColumn("ItemId", SQLiteType.INTEGER, false, false, false)]
            public long ItemId { get; set; }
            public override string ToString()
            {
                return base.ToString();
            }

            protected override void OnDelete()
            {
                base.OnDelete();
            }

            protected override void OnLoad(SQLiteDataBase dataBase)
            {
                base.OnLoad(dataBase);
            }

            protected override void OnUpdate()
            {
                base.OnUpdate();
            }
        }
        private class Post : Kemorave.SQLite.IDBModel
        {
            public Post(SQLiteDataBase dataBase, string content) 
            {
                Content = content;
                DataBase = dataBase;
                Images = new List<Image>();
            }

            public Post()
            {
                Images = new List<Image>();
            }

            protected SQLiteDataBase DataBase;
            [Kemorave.SQLite.SQLiteAttribute.Property(Behavior.Populate)]
            [TableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            public string Content { get; set; }
            [Property(Behavior.Ignore)]
            public List<Image> Images { get; }
            [FillMethod]
            public void Fill(Kemorave.SQLite.DataBaseGetter getter)
            {
                Images.AddRange(getter.Include(this
                    ,new IncludeOptions<Post, Image>(forigenKey:"ItemId","Path",new string[] { "Path"},
                    new Where(WhereConditon.IsEqual("Type", "PostImage")))));
            }
            public void AddImage(string path)
            {
                new Image(DataBase,path, "PostImage", this.ID).Save();
            }
           
            public   void Save()
            {
                DataBase.DataSetter.Insert(this);
            }
        }
        private const string FILENAME = "MySQLiteDB.sqlite";

        private static readonly string FILEPATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + FILENAME;
        [STAThread]
        public static void Main()
        {
            try
            {
                // var file = Kemorave.Win.Shell.ShellItem.FromParsingName(@"F:\Musica\my_musica\Music\New folder\Beautiful Pain.mp3");
                using (Kemorave.SQLite.SQLiteDataBase db = new SQLiteDataBase(FILEPATH))
                {
                    //db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.Trace += Connection_Trace; ;

                    db.TableManager.CreateTable(typeof(Image));
                    var postT = new TableInfo("Post");
                    postT.Columns.Add(new ColumnInfo("ID", SQLiteType.INTEGER, true, true));
                    postT.Columns.Add(new ColumnInfo("Content", SQLiteType.TEXT));
                    db.TableManager.CreateTable(postT);
                    Stopwatch stopwatch = new Stopwatch();
                    var post = new Post(db, "Gola");
                    post.Save();
                    post.AddImage("AAAAA");
                    post = db.DataGetter.GetItemByID<Post>(post.ID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error : {e.Message}.\n{e}", "Errpr");
            }
        }

        private static void Connection_Trace(object sender, System.Data.SQLite.TraceEventArgs e)
        {

            Debug.WriteLine($"TRACE {e.Statement}");
        }

        private static void Connection_RollBack(object sender, EventArgs e)
        {
            Debug.WriteLine($"Rolled back");

        }

        private static void Connection_Commit(object sender, System.Data.SQLite.CommitEventArgs e)
        {
        }

        private static void Connection_Update(object sender, System.Data.SQLite.UpdateEventArgs e)
        {
            //  Debug.WriteLine($"Table {e.Table} Row {e.RowId} Event {e.Event} Database {e.Database}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Kemorave.SQLite;
using Kemorave.SQLite.SQLiteAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class DBTest
    {
        [Table("TestScheduler")]
        private class TestScheduler : IDBModel
        {
            [Property(PropertyAttribute.DefaultValueBehavior.Populate)]
            [TableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            //[Property]
            [TableColumn("TestID", SQLiteType.INTEGER, false, false, false, false, "0", "Test", "ID", SQLiteActions.CASCADE, SQLiteActions.CASCADE)]
            public long TestID { get; set; }

            [Property(PropertyAttribute.DefaultValueBehavior.Ignore)]
            public Test Test { get; set; }
            //[Property]
            [TableColumn("TestDate", SQLiteType.DATETIME, false, false, false)]
            public DateTime TestDate { get; set; }
            [FillMethod]
            public void SetNavigationProperties(DataBaseGetter getter)
            {
               // Test = getter.GetItemByID<Test>(this);
                if (Test != null)
                {
                    throw new Exception("WWWWWWWWWWAAA");
                }
            }
            public override string ToString()
            {
                return $"{Test} Date : {TestDate}";
            }
        }
        [Table("Test")]
        private class Test : IDBModel
        {
            [Property(PropertyAttribute.DefaultValueBehavior.Populate)]
            [TableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            [Property(PropertyAttribute.DefaultValueBehavior.PopulateAndInclude, "Test_Name")]
            [TableColumn("Test_Name", SQLiteType.VARCHAR, false, false, false)]
            public string Name { get; set; }
            //[Property]
            [TableColumn("Major", SQLiteType.VARCHAR, false, false, false)]
            public string Major { get; set; }
            public override string ToString()
            {
                return $"ID : {ID} Name : {Name} Major {Major}";
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
                Stopwatch stopwatch = new Stopwatch();
                using (Kemorave.SQLite.SQLiteDataBase db = new   SQLiteDataBase(FILEPATH))
                {
                    db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.RollBack += Connection_RollBack;

                    db.TableManager.CreateTable(typeof(Test)); 

                    db.TableManager.CreateTable(typeof(TestScheduler));

                    var names = db.TableManager.GetTablesNames();
                    var tt = db.TableManager.GetTableInfo("TestScheduler");

                    Test test = new Test
                    {
                        Name = "Hola",
                        Major = "CS"
                    };
                    db.DataSetter.Insert(test);
                    db.DataSetter.Insert(new TestScheduler() { TestID = test.ID, TestDate = new DateTime(2021, 6, 6) });
                  //  foreach (TestScheduler item in db.DataGetter.GetItems<TestScheduler>())
                    {
                     //   Debug.WriteLine(item.ToString());
                    }
                    List<Test> list = new List<Test>();
                    for (int i = 0; i != 1000000; i++)
                    {
                        list.Add(new Test() { Major = "CS", Name = $"Test {i}" });
                    }
                    stopwatch.Start();
                    db.DataSetter.Insert(list);
                    stopwatch.Stop();
                    //stopwatch.Reset();
                    Debug.Write($"Inserted {list.Count} items in {stopwatch.Elapsed}");

                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error : {e.Message}.\n{e}", "Errpr");
            }
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
            //   Debug.WriteLine($"Table {e.Table} Row {e.RowId} Event {e.Event} Database {e.Database}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Kemorave.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class DBTest
    {
        [SQLiteTable("TestScheduler")]
        private class TestScheduler : IDBModel
        {
            [SQLiteProperty(SQLitePropertyAttribute.DefaultValueBehavior.Populate)]
            [SQLiteTableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            [SQLiteProperty]
            [SQLiteTableColumn("TestID", SQLiteType.INTEGER, false, false, false, false, "0", "Test", "ID", ColumnInfo.SQLiteActions.CASCADE, ColumnInfo.SQLiteActions.CASCADE)]
            public long TestID { get; set; }
            public Test Test { get; private set; }
            [SQLiteProperty]
            [SQLiteTableColumn("TestDate", SQLiteType.DATETIME, false, false, false)]
            public DateTime TestDate { get; set; }
            [SQLiteFillMethod]
            public void SetNavigationProperties(IDataBaseGetter getter)
            {
                Test = getter.GetItemByID<Test>(TestID);
            }
            public override string ToString()
            {
                return $"{Test} Date : {TestDate}";
            }
        }

        [SQLiteTable("Test")]
        private class Test : IDBModel
        {
            [SQLiteProperty(SQLitePropertyAttribute.DefaultValueBehavior.Populate)]
            [SQLiteTableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            [SQLiteProperty(SQLitePropertyAttribute.DefaultValueBehavior.PopulateAndInclude, "Test_Name")]
            [SQLiteTableColumn("Test_Name", SQLiteType.VARCHAR, false, false, false)]
            public string Name { get; set; }
            [SQLiteProperty]
            [SQLiteTableColumn("Major", SQLiteType.VARCHAR, false, false, false)]
            public string Major { get; set; }
            public override string ToString()
            {
                return $"ID : {ID} Name : {Name} Major {Major}";
            }
        }




        private const string FILENAME = "MySQLiteDB.sqlite";

        private static readonly string FILEPATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + FILENAME;
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                using (DataBase db = new DataBase(FILEPATH))
                {
                    db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.RollBack += Connection_RollBack;
                    //    db.AutoCommitChanges = false;
                    Debug.WriteLine(db.Connection.State.ToString());
                    db.CreateTable(typeof(Test));
                    Debug.WriteLine(db.Connection.State.ToString());

                    db.CreateTable(typeof(TestScheduler));

                    Test test = new Test
                    {
                        Name = "Hola",
                        Major = "CS"
                    };
                    db.Insert(test);
                    db.Insert(new TestScheduler() { TestID = test.ID, TestDate = new DateTime(2021, 6, 6) });
                    foreach (TestScheduler item in db.GetItems<TestScheduler>())
                    {
                        Debug.WriteLine(item.ToString());
                    }
                    List<Test> list = new List<Test>();
                    for (int i = 0; i != 100000; i++)
                    {
                        list.Add(new Test() { Major = "CS", Name = $"Test {i}" });
                    }
                    stopwatch.Start();
                    db.Insert(list);
                    Debug.Write($"Inserted {list.Count} items in {stopwatch.Elapsed}");
                    stopwatch.Stop();
                    stopwatch.Reset();
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
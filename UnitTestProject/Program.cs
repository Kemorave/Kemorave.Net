using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Kemorave.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class Program
    {
        [SQLiteTable("TestScheduler")]
        class TestScheduler : IDBModel
        {
            [SQLiteProperty(SQLitePropertyAttribute.DefaultValueBehavior.Populate)]
            [SQLiteTableColumn("ID", SQLiteType.INTEGER, true, true, false)]
            public long ID { get; set; }
            [SQLiteProperty]
            [SQLiteTableColumn("TestID", SQLiteType.INTEGER, false, false, false, false, "0", "Test", "ID", ColumnInfo.SQLiteActions.CASCADE, ColumnInfo.SQLiteActions.CASCADE)]
            public long TestID { get; set; }
            public Test Test { get; set; }
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
            [SQLiteProperty(SQLitePropertyAttribute.DefaultValueBehavior.PopulateAndInclude,"Test_Name")]
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
                DataBaseInfo dBInfo = new DataBaseInfo(FILEPATH);
                dBInfo.AddTableFromType(typeof(Test));
                dBInfo.AddTableFromType(typeof(TestScheduler));
                using (DataBase db = new DataBase(FILEPATH))
                {
                    db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.RollBack += Connection_RollBack;
                    //    db.AutoCommitChanges = false;
                    db.Recreate(dBInfo);
                    Test test = new Test();
                    test.Name = "Hola";
                    test.Major = "CS";
                    db.Insert(test);
                    foreach (var item in db.GetItems<Test>())
                    {
                        test = item;
                        db.Insert(new TestScheduler() { TestID = item.ID, TestDate = new DateTime(2021,6,6) });
                    }
                    foreach (var item in db.GetItems<TestScheduler>())
                    {

                    }
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
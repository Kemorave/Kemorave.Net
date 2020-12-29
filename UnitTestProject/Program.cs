using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
        [Kemorave.SQLite.SQLiteTable("Test")]
        private class Test : Kemorave.SQLite.IDBModel
        {
            [Kemorave.SQLite.SQLiteColumn(Kemorave.SQLite.SQLiteColumnAttribute.DefaultValueBehavior.Populate)]
            public long ID { get; set; }
            [Kemorave.SQLite.SQLiteColumn]
            public string Test_Name { get; set; }
            [Kemorave.SQLite.SQLiteColumn]
            public string Major { get; set; } = "CS";
            public override string ToString()
            {
                return $"ID : {ID} Name : {Test_Name}";
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
                Kemorave.SQLite.DBInfo dBInfo = new Kemorave.SQLite.DBInfo(FILEPATH);
                Kemorave.SQLite.TableInfo testTab = new Kemorave.SQLite.TableInfo("Test");
                testTab.Columns.Add(new Kemorave.SQLite.ColumnInfo("ID", Kemorave.SQLite.SQLiteType.INTEGER, true) );
                testTab.Columns.Add(new Kemorave.SQLite.ColumnInfo("Test_Name", Kemorave.SQLite.SQLiteType.VARCHAR) {
                    DefaultValue ="Hello kitty" , IsNullable=true } );
                testTab.Columns.Add(new Kemorave.SQLite.ColumnInfo("Major", Kemorave.SQLite.SQLiteType.VARCHAR));
                dBInfo.Tables.Add(testTab);
                using (Kemorave.SQLite.SQLiteDb db = new Kemorave.SQLite.SQLiteDb(FILEPATH))
                {
                    db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.RollBack += Connection_RollBack;
                    db.AutoCommitChanges = false;
                    //Test test = null;
                    List<Test> tests = new List<Test>();
                    db.Recreate(dBInfo);
                    for (int i = 1; i <= 100; i++)
                    {
                        tests.Add(new Test() { Test_Name = "Hema " + i, Major = "CS " + i });
                    }
                    //stopwatch.Start();
                    db.Insert(tests);
                    //stopwatch.Stop();
                    //Debug.WriteLine($"Inseting {tests.Count} items in {stopwatch.Elapsed.ToString()}");
                    //stopwatch.Restart();
                    //test = db.GetItemByID<Test>(555);
                    //stopwatch.Stop();
                    ////Debug.WriteLine($"Get {test} item in {stopwatch.Elapsed.ToString()}");

                    //stopwatch.Restart();

                    //foreach (Test item in db.GetItems<Test>("Test"))
                    //{
                    //}
                    //stopwatch.Stop();
                    //Debug.WriteLine($"Get {tests.Count} items in {stopwatch.Elapsed.ToString()}");

                    //test.Test_Name = "Nigga";
                    //test.Major = "Hello kitty";
                    //db.Update( test);
                    //foreach (Test item in db.GetItems<Test>())
                    //{
                    //    test = item;
                    //    Debug.WriteLine(item.ToString());
                    //}
                    //db.Delete( test);
                    //foreach (Test item in db.GetItems<Test>())
                    //{
                    //    test = item;
                    // //   Debug.WriteLine(item.ToString());
                    //}
                    if(db.CanRollBack)
                    db.RollBack();
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
            Debug.WriteLine($"Table {e.Table} Row {e.RowId} Event {e.Event} Database {e.Database}");
        }
    }
}
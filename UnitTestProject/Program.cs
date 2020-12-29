﻿using System;
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
        [TestMethod]
        public void TestMethod1()
        {
        }
        [Kemorave.SQLite.SQLiteTable("Test")]
        private class Test : Kemorave.SQLite.IDBModel
        {
            [Kemorave.SQLite.SQLiteProperty(Kemorave.SQLite.SQLitePropertyAttribute.DefaultValueBehavior.Populate)]
            [SQLiteColumn("ID", SQLiteType.INTEGER,true,true,false)]
            public long ID { get; set; }
            [Kemorave.SQLite.SQLiteProperty]
            [Kemorave.SQLite.SQLiteColumn("Test_Name", Kemorave.SQLite.SQLiteType.VARCHAR, false, false, true)]
            public string Test_Name { get; set; }
            [Kemorave.SQLite.SQLiteProperty]
            [Kemorave.SQLite.SQLiteColumn("Major", Kemorave.SQLite.SQLiteType.VARCHAR,false,false,true)]
            public string Major { get; set; }
            public override string ToString()
            {
                return $"ID : {ID} Name : {Test_Name} Major {Major}";
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
                Kemorave.SQLite.DataBaseInfo dBInfo = new Kemorave.SQLite.DataBaseInfo(FILEPATH);
                dBInfo.AddTableFromType(typeof(Test));
                using (Kemorave.SQLite.DataBase db = new Kemorave.SQLite.DataBase(FILEPATH))
                {
                    db.Connection.Update += Connection_Update;
                    db.Connection.Commit += Connection_Commit;
                    db.Connection.RollBack += Connection_RollBack;
                    db.AutoCommitChanges = false;
                    //Test test = null;
                    List<Test> tests = new List<Test>();
                    db.Recreate(dBInfo);
                    for (int i = 1; i <= 10; i++)
                    {
                        tests.Add(new Test() { Test_Name = "Hema " + i});
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
                    foreach (Test item in db.GetItems<Test>())
                    {
                       // test = item;
                           Debug.WriteLine(item.ToString());
                    }
                    if (db.CanRollBack)
                    db.RollBack();
                    foreach (Test item in db.GetItems<Test>())
                    {
                        // test = item;
                        Debug.WriteLine(item.ToString());
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
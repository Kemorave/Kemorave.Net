using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Kemorave.SQLite;
using Kemorave.SQLite.ModelBase;
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
			[Kemorave.SQLite.SQLiteAttribute.TableColumn("Path", SQLiteType.TEXT, false, false, false)]
			public string Path { get; set; }
			[Kemorave.SQLite.SQLiteAttribute.TableColumn("Type", SQLiteType.TEXT, false, false, false)]
			public string Type { get; set; }
			[Kemorave.SQLite.SQLiteAttribute.TableColumn("ItemId", SQLiteType.INTEGER, false, false, false)]
			public long ItemId { get; set; }
			public override string ToString()
			{
				return base.ToString();
			}

		}
		private class Post : IDBModel
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
			[Kemorave.SQLite.SQLiteAttribute.Property(SqlPropertyHandling.Populate)]
			[TableColumn("ID", SQLiteType.INTEGER, true, true, false)]
			public long Id { get; set; }
			public string Content { get; set; }
			[Property(SqlPropertyHandling.Ignore)]
			public List<Image> Images { get; }
			[FillMethod]
			public void Fill(Kemorave.SQLite.DataGetter getter)
			{
				Images.AddRange(getter.FindIncluded(this
					, new IncludeOptions<Post, Image>(forigenKey: "ItemId", "Path", new string[] { "Path" },
					new Where(WhereConditon.IsEqual("Type", "PostImage")))));
			}
			public void AddImage(string path)
			{
				new Image(DataBase, path, "PostImage", this.Id).Save();
			}

			public void Save()
			{
				DataBase.Setter.Insert(this);
			}
		}

		private class User : Kemorave.SQLite.ModelBase.Model
		{
			public User()
			{ }

			public User(SQLiteDataBase dataBase) : base(dataBase)
			{
			}
			[TableColumn("Name", SQLiteType.TEXT, false, false, false)]
			public string Name { get; set; } = "ALI";
		}

		private class Profile : Kemorave.SQLite.ModelBase.Model
		{
			public Profile()
			{ }

			public Profile(SQLiteDataBase dataBase) : base(dataBase)
			{
			}
			[TableColumn("Nick", SQLiteType.TEXT, false, false, false)]
			public string Nick { get; set; } = "Moga";

		}

		private class UserProfile : Kemorave.SQLite.ModelBase.Model
		{
			public UserProfile()
			{ }

			public UserProfile(SQLiteDataBase dataBase) : base(dataBase)
			{
			}

			public UserProfile(SQLiteDataBase dataBase, long userId, long profileId) : this(dataBase)
			{
				UserId = userId;
				ProfileId = profileId;
			}

			[ForeignKey("UserId", SQLiteType.INTEGER, false, false, null, "UserProfile", "Id", SQLiteActions.CASCADE, SQLiteActions.CASCADE)]
			// [TableColumn("UserId", SQLiteType.INTEGER)]
			public long UserId { get; set; }
			[ForeignKey("ProfileId", SQLiteType.INTEGER, false, false, null, "Profile", "Id", SQLiteActions.CASCADE, SQLiteActions.CASCADE)]
			//[TableColumn("ProfileId", SQLiteType.INTEGER)]
			public long ProfileId { get; set; }
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
					// db.Connection.Commit += Connection_Commit;
					//db.Connection.Trace += Connection_Trace; ;
					Stopwatch stopwatch = new Stopwatch();
					db.Setter.ProgressChanged += DataSetter_ProgressChanged;
					db.TableManager.CreateTable(typeof(User));
					db.TableManager.CreateTable(typeof(Profile));
					db.TableManager.CreateTable(typeof(UserProfile));
					List<User> users = new List<User>();
					for (int i = 0; i < 1000000; i++)
					{
						users.Add(new User() { Name = $"Hola {i}" });
					}
					stopwatch.Start();
					db.Setter.Insert(users.ToArray());
					stopwatch.Stop();
					Console.WriteLine($"{users.Count} rows inserted in {stopwatch.Elapsed}");
					//   db.Backup(Environment.GetFolderPath( Environment.SpecialFolder.Desktop));
				}
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error : {e.Message}.\n{e}", "Errpr");
			}

			Console.WriteLine($"Press any to exit");
			Console.Read();
		}

		private static void DataSetter_ProgressChanged(object sender, SetterProgressArgs e)
		{
			Console.Clear();
			Console.WriteLine(e);
		}

		private static void Connection_Trace(object sender, System.Data.SQLite.TraceEventArgs e)
		{ 
			Console.WriteLine($"TRACE {e.Statement}");
		}

		private static void Connection_RollBack(object sender, EventArgs e)
		{
			Console.WriteLine($"Rolled back");
		}

		private static void Connection_Commit(object sender, System.Data.SQLite.CommitEventArgs e)
		{
		}

		private static void Connection_Update(object sender, System.Data.SQLite.UpdateEventArgs e)
		{
			//  Console.WriteLine($"Table {e.Table} Row {e.RowId} Event {e.Event} Database {e.Database}");
		}
	}
}
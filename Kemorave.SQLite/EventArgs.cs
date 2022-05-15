using System;

using Kemorave.SQLite.ModelBase;

namespace Kemorave.SQLite
{
	public class SetterProgressArgs : System.EventArgs
	{
		public SetterProgressArgs(string operation)
		{
			Operation = operation ;
		}
		public double Progress { get; set; }
		public string Operation { get; }
		public override string ToString()
		{
			return $"{Operation} {Progress}%";
		}
	}
	public class SetterOperationArgs<T> : System.EventArgs where T : IDBModel
	{
		public SetterOperationArgs(string operation, T item)
		{
			Item = item;
			Operation = operation ;
		}
		public T Item { get; set; }
		public string Operation { get; }

	}
}

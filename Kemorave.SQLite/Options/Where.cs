using System;
using System.Collections.Generic;
using System.Linq;

namespace Kemorave.SQLite.Options
{
    public class Where
    {
        public Where()
        {
            _Conditons = new List<Tuple<ConditionOperator, WhereConditon[]>>();
        }

        public Where( ConditionOperator conditionOperator ,params WhereConditon[] conditons) : this()
        {
            AddConditons(conditionOperator, conditons);
        }
        public Where(params WhereConditon[] conditons) : this()
        {
            AddConditons( ConditionOperator.AND, conditons);
        }

        public override string ToString()
        {
            return GetCommand();
        }

        public string GetCommand()
        {
            string cmd = $"WHERE ";

            for (int i = 0; i < _Conditons.Count; i++)
            {
                bool clau = false;
                if (i+1 < _Conditons.Count)
                {
                    if (_Conditons[i].Item1!= _Conditons[i+1].Item1)
                    {
                        clau = true;
                    }
                }
                if (clau)
                {
                    cmd += "(";
                }
                if (i > 0)
                {
                    cmd +=$" {_Conditons[i].Item1} ";
                }
                for (int k = 0; k < _Conditons[i].Item2.Length; k++)
                {
                    if (k == 0)
                    {
                        cmd += _Conditons[i].Item2[k].GetCommand();
                    }
                    else
                    {
                        cmd += $" {_Conditons[i].Item1} {_Conditons[i].Item2[k].GetCommand()} ";
                    }
                }
                if (clau)
                {
                    cmd += ")";
                }
            }
            return cmd;
        }
        public enum ConditionOperator { AND, OR }
        public void AddConditons(ConditionOperator conditionOperator, params WhereConditon[] conditons)
        {
            _Conditons.Add(new Tuple<ConditionOperator, WhereConditon[]>(conditionOperator, conditons));
        }
        public void AddConditons(params WhereConditon[] conditons)
        {
            AddConditons(ConditionOperator.AND, conditons);
        }

     readonly   List<Tuple<ConditionOperator, WhereConditon[]>>  _Conditons;
    public    IEnumerable<WhereConditon[]> Conditons { get => _Conditons.Select(c=>c.Item2); }
    }
}
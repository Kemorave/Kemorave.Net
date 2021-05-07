using System;
using System.Collections.Generic;

namespace Kemorave.SQLite.Options
{
    public class Where
    {
        public Where()
        {
            Conditons = new List<Tuple<ConditionOperator, WhereConditon[]>>();
        }

        public Where(WhereConditon[] conditons, ConditionOperator conditionOperator = ConditionOperator.AND) : this()
        {
            AddConditons(conditionOperator, conditons);
        }

        public override string ToString()
        {
            return GetCommand();
        }

        internal string GetCommand()
        {
            string cmd = $"WHERE ";

            for (int i = 0; i < Conditons.Count; i++)
            {
                foreach (var item in Conditons[i].Item2)
                {
                    if (i == 0)
                    {
                        cmd += item.GetCommand();
                    }
                    else
                    {
                        cmd += $"{Conditons[i].Item1} {item.GetCommand()}";
                    }
                }
            }
            return cmd;
        }
        public enum ConditionOperator { AND, OR }
        public void AddConditons(ConditionOperator conditionOperator, params WhereConditon[] conditons)
        {
            Conditons.Add(new Tuple<ConditionOperator, WhereConditon[]>(conditionOperator, conditons));
        }
        public void AddConditons(params WhereConditon[] conditons)
        {
            AddConditons(ConditionOperator.AND, conditons);
        }

        List<Tuple<ConditionOperator, WhereConditon[]>> Conditons { get; }
    }
}
using System;

namespace Core.Castle
{
    [Serializable]
    public class Resource
    {
        public string Name;
        public int Value;
        public int Income;

        public Resource(string name) : this(name, 5, 2)
        {
        }
        
        public Resource(string name, int value, int income)
        {
            Name = name;
            Value = value;
            Income = income;
        }

        public void AddResource(int value)
        {
            Value += value;
        }

        public void RemoveResource(int value)
        {
            Value -= value;

            if (Value < 0)
                Value = 0;
        }

        public void AddIncome(int income)
        {
            Income += income;
        }

        public void RemoveIncome(int income)
        {
            Income -= income;

            if (Income < 1)
                Income = 1;
        }
    }
}
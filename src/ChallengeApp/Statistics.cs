using System;
using System.Collections.Generic;
using System.IO;

namespace ChallengeApp
{
    public class Statistics
    {
        string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        private readonly IFileWrapper _fileWrapper;
        public double Maxgrade;
        public double Mingrade;
        public double Sum;
        public int Count;
        public List<string> employeeList;
        public Statistics()
        {
            Count = 0;
            Sum = 0.0;
            Maxgrade = double.MinValue;
            Mingrade = double.MaxValue;
        }
        public Statistics(IFileWrapper fileWrapper)
        {
            _fileWrapper = fileWrapper;
            Count = 0;
            Sum = 0.0;
            Maxgrade = double.MinValue;
            Mingrade = double.MaxValue;
        }
        public char Letter
        {
            get
            {
                switch (Average)
                {
                    case var d when d >= 90:
                        return 'A';

                    case var d when d >= 70:
                        return 'B';

                    case var d when d >= 50:
                        return 'C';

                    case var d when d >= 30:
                        return 'D';

                    case var d when d >= 10:
                        return 'E';

                    case var d when d >= 0:
                        return 'F';

                    default:
                        return 'Z';
                }
            }
        }
        public double Average
        {
            get
            {
                return Sum / Count;
            }

        }
        public void Add(double grade)
        {
            Sum += grade;
            Count += 1;
            Mingrade = Math.Min(grade, Mingrade);
            Maxgrade = Math.Max(grade, Maxgrade);
        }
        public void AddEmployee(string emp)
        {

            this.employeeList.Add(emp);

        }

        public List<string> ReturnEmployeeList()
        {

            var empList = new List<string>();

            if (File.Exists(@$"{currentDirectory}\EmployeeList.txt"))
            {
                using (var reader = File.OpenText(@$"{currentDirectory}\EmployeeList.txt"))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        empList.Add(line);
                        line = reader.ReadLine();
                    }
                }
                Console.WriteLine();
            }
            else
            {
                throw new FileNotFoundException("File does not exist. You need to create an employee.");
            }
            return empList;
        }
    }
}
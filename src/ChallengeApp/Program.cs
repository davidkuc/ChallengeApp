using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChallengeApp

{






    class Program
    {
        static void Main(string[] args)
        {
            InteractionMethod();
            Console.Read();
        }

        #region InteractionMethods
        private static void InteractionMethod()
        {
            Console.WriteLine($"Welcome!");
            Console.WriteLine();
            Console.WriteLine($"What do you want to do?");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("--------------------");
            Console.WriteLine($"List of commands: ");
            Console.WriteLine($" 'q' - exit the application ");
            Console.WriteLine($" 'n' - create new employee profile ");
            Console.WriteLine($" 'm' - modify existing employee data ");
            Console.WriteLine($" 'd' - employee statistics options ");
            Console.WriteLine("--------------------");
            Console.WriteLine();

            var input = Console.ReadLine();

            try
            {
                switch (input)
                {

                    case "q":
                        return;

                    case "n":
                        CreateEmployeeInteraction();
                        break;

                    case "m":
                        ModifyEmployeeInteraction();
                        break;

                    case "d":
                        StatisticsInteraction();
                        break;
                    default:
                        throw new ArgumentException("Invalid input");
                }
            }
            catch (ArgumentException argex)
            {
                Console.WriteLine(argex.Message);
            }
            InteractionMethod();
        }

        private static void StatisticsInteraction()
        {
            Console.WriteLine($"What do you want to do?");
            Console.WriteLine();
            Console.WriteLine($" 'q' - return to previous menu '");
            Console.WriteLine($" 's' - show employee statistics ");
            Console.WriteLine($" 'g' - show employee grades ");
            Console.WriteLine($" 'l' - show employee list ");
            Console.WriteLine();

            var input2 = Console.ReadLine();

            try
            {
                switch (input2)
                {

                    case "q":
                        break;

                    case "s":
                        ShowStatistics();
                        Console.Read();
                        break;
                    case "g":
                        Console.WriteLine($"Enter employee name ");
                        var empName = Console.ReadLine();
                        var emp = new SavedEmployee(empName);
                        emp.ShowEmployeeGrades();
                        Console.Read();
                        break;
                    case "l":
                        ShowEmployeeList();
                        Console.Read();
                        break;
                    default:
                        throw new ArgumentException("Invalid input");
                }
            }
            catch (ArgumentException argex)
            {

                Console.WriteLine(argex.Message);
            }

        }

        private static void ModifyEmployeeInteraction()
        {
            Console.WriteLine($"Enter employees name.");
            var empName = Console.ReadLine();

            var emp = new SavedEmployee(empName);
            emp.GradeAdded += OnGradeAdded;
            emp.GradeAddedBelow30 += OnGradeAddedBelow30;
            emp.GradeDeleted += OnGradeDeleted;

            Console.WriteLine($"What do you want to do?");
            Console.WriteLine();
            Console.WriteLine($" 'q' - return to previous menu ");
            Console.WriteLine($" 'add' - add new grades ");
            Console.WriteLine($" 'del' - delete grades ");
            Console.WriteLine($" 'ch' - change employee name ");
            Console.WriteLine();
            var input = Console.ReadLine();

            try
            {
                switch (input)
                {
                    case "q":
                        break;

                    case "add":
                        EnterGradeInteraction(emp);
                        break;

                    case "del":
                        DeleteGradeInteraction(emp);
                        break;

                    case "ch":
                        ChangeEmployeeNameInteraction(emp);
                        break;

                    default:
                        throw new ArgumentException($"Invalid input");

                }
            }
            catch (ArgumentException argex)
            {

                Console.WriteLine(argex.Message);
            }
        }

        private static void ChangeEmployeeNameInteraction(SavedEmployee emp)
        {
            try
            {
                Console.WriteLine($"Enter a new name for the employee {emp.Name} ");
                Console.WriteLine($"or enter 'q' to exit.");
                var name = Console.ReadLine();

                if (name == "q")
                {
                    return;
                }
                else
                {
                    emp.ChangeThisEmployeeName(name);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            Console.WriteLine($"Employee name changed.");
        }

        private static void CreateEmployeeInteraction()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Enter employees name");
                    Console.WriteLine($"or enter 'q' to exit.");
                    var name = Console.ReadLine();

                    foreach (var ch in name)
                    {
                        if (char.IsDigit(ch))
                        {
                            throw new ArgumentException($"Name cannot contain numbers.");
                        }
                    }

                    if (name == "q")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"Enter employees sex. (Choose M/F)");
                        var sex = Console.ReadLine();

                        if (sex == "q")
                        {
                            return;
                        }
                        else if (sex == "M" || sex == "F")
                        {

                            var emp = new SavedEmployee(name, sex);

                        }
                        else
                        {
                            throw new Exception($"Please enter M or F.");
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }
        }




        private static void EnterGradeInteraction(IEmployee emp)
        {
            Console.WriteLine($"Enter grades from 0 to 100 for {emp.Name}");
            Console.WriteLine($"or press 'q' to return");
            while (true)
            {
                var input = Console.ReadLine();

                if (input == "q")
                {
                    break;
                }
                else
                {
                    try
                    {
                        emp.AddGrade(input);

                    }
                    catch (ArgumentException argex)
                    {

                        Console.WriteLine(argex.Message);
                    }
                    catch (FormatException formex)
                    {

                        Console.WriteLine(formex.Message);
                    }

                    Console.WriteLine($"Enter grade from 0 to 100 for {emp.Name}");
                    Console.WriteLine($"or press 'q' to return");
                }
            }


        }


        private static void DeleteGradeInteraction(IEmployee emp)
        {
            while (true)
            {

                try
                {
                    emp.ShowEmployeeGrades();
                    Console.WriteLine($"Enter the index of the grade you want to remove.");
                    Console.WriteLine("Enter 'q' to exit.");
                    var input1 = Console.ReadLine();
                    if (input1 == "q")
                    {
                        break;
                    }
                    else
                    {
                        emp.DeleteGrade(input1);
                    }
                }
                catch (ArgumentException argex)
                {
                    Console.WriteLine(argex.Message);
                }
                catch (FormatException formex)
                {
                    Console.WriteLine(formex.Message);
                }
            }
        }

        #endregion

        #region InteractionMethodsExtensions
        private static void ShowEmployeeList()
        {
            var stats = new Statistics();

            try
            {
                Console.WriteLine();
                Console.WriteLine($"Employee list of {EmployeeBase.firm}");
                var empList = stats.ReturnEmployeeList();
                foreach (var employee in empList)
                {
                    Console.WriteLine(employee);
                }
            }
            catch (System.IO.FileNotFoundException IOFNFex)
            {

                Console.WriteLine(IOFNFex.Message);
            }
            Console.WriteLine();
        }



        private static void ShowStatistics()
        {
            Console.WriteLine($"Enter employee name ");
            var empName = Console.ReadLine();
            var emp = new SavedEmployee(empName);
            var empStats = emp.GetStatistics();
            Console.WriteLine();
            Console.WriteLine($"Statistics of employee |{emp.Name}| ");
            Console.WriteLine($"Max - {empStats.Maxgrade} ");
            Console.WriteLine($"Min - {empStats.Mingrade} ");
            Console.WriteLine($"Average - {empStats.Average:N2} ");
            Console.WriteLine($"Letter grade - {empStats.Letter} ");
            Console.WriteLine();
        }
        #endregion

        #region DelegateMethods
        static void OnGradeAdded(object sender, EventArgs args)
        {
            Console.WriteLine($"New grade added");
        }

        static void OnGradeAddedBelow30(object sender, EventArgs args)
        {
            Console.WriteLine($"Grade below 30!");
        }

        static void OnGradeDeleted(object sender, EventArgs args)
        {
            Console.WriteLine("Grade deleted");
        }

        #endregion

    }
}
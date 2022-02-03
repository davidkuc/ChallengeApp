using System;
using System.Collections.Generic;
using System.IO;

namespace ChallengeApp
{
    public class SavedEmployee : EmployeeBase
    {
        public SavedEmployee(string name) : base(name)
        { }

        public SavedEmployee(string name, string sex) : base(name, sex)
        {
            using (var writerToAudit = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}Audit.txt"))
            using (var writerToEmpGrades = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}.txt"))
            using (var writerToEmployeeList = File.AppendText(@$"{this.CurrentDirectory}\EmployeeList.txt"))
            {
                Console.WriteLine($"New employee |{this.Name}| created.");
                writerToEmployeeList.WriteLine($"Joined: {DateTime.UtcNow}, Employee name: {this.Name}, sex: {this.Sex}");
            }

            if (
            File.Exists(@$"{this.CurrentDirectory}\{this.Name}Audit.txt")
            &
            File.Exists(@$"{this.CurrentDirectory}\{this.Name}.txt")
            &
            File.Exists(@$"{this.CurrentDirectory}\EmployeeList.txt")
               )
            {
                EmployeeFilesExist = true;
            }
            else
            {
                EmployeeFilesExist = false;
            }
        }

        public override event GradeAddedDelegate GradeAdded;
        public override event GradeAddedDelegate GradeAddedBelow30;
        public override event GradeDeletedDelegate GradeDeleted;

        public override string AddGrade(string grade)
        {
            double result;
            bool canParse = double.TryParse(grade, out result);

            if (canParse)
            {
                using (var writerToEmpGrades = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}.txt"))
                using (var writerToAudit = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}Audit.txt"))
                {
                    if (0 <= result && result <= 100)
                    {
                        writerToEmpGrades.WriteLine(result);
                        writerToAudit.WriteLine($"Time: {DateTime.UtcNow}, Grade: {grade}");

                        if (GradeAdded != null)
                        {
                            GradeAdded(this, new EventArgs());
                        }
                        if (GradeAddedBelow30 != null && result < 30)
                        {
                            GradeAddedBelow30(this, new EventArgs());
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Wrong value of ({nameof(grade)}). Please input string from 0 to 100");
                    }
                }
            }
            else
            {
                throw new FormatException($"Invalid format of {nameof(grade)}");
            }
            return grade;
        }

        public override List<string> DeleteGrade(string gradeIndex)
        {
            int inputIndex;
            List<string> newGradesList = new List<string>();
            bool canParse = int.TryParse(gradeIndex, out inputIndex);

            if (canParse)
            {
                string tempFile = Path.GetTempFileName();
                var tempArr = ReturnEmployeeGradesAsList().ToArray();
                var deletedGrade = tempArr[inputIndex];
                tempArr[inputIndex] = "grade deleted";

                using (var writerToAudit = File.AppendText(@$"{this.CurrentDirectory}\{this.Name}Audit.txt"))
                using (var streamReaderFromEmpGrades = new StreamReader(@$"{this.CurrentDirectory}\{this.Name}.txt"))
                using (var writerToTempFile = new StreamWriter(tempFile))
                {
                    string line;
                    int index = 0;

                    writerToAudit.WriteLine($"Time: {DateTime.UtcNow}, Grade deleted: {deletedGrade} at index: {inputIndex}");
                    while ((line = streamReaderFromEmpGrades.ReadLine()) != null)
                    {
                        writerToTempFile.WriteLine(tempArr[index]);
                        newGradesList.Add(tempArr[index]);
                        index++;
                    }
                }
                File.Delete(@$"{this.CurrentDirectory}\{this.Name}.txt");
                File.Move(tempFile, @$"{this.CurrentDirectory}\{this.Name}.txt");

                if (GradeDeleted != null)
                {
                    GradeDeleted(this, new EventArgs());
                }
                Console.WriteLine();
            }
            else
            {
                throw new FormatException($"Invalid format of {nameof(gradeIndex)}");
            }
            return newGradesList;
        }

        public override string[] ShowEmployeeGrades()
        {

            Console.WriteLine();
            Console.WriteLine($"Grades of |{this.Name}|");
            Console.WriteLine();
            Console.WriteLine("Index  |  Grade");

            var listOfGradesToReturn = new List<string>();
            var tempList = ReturnEmployeeGradesAsList();
            var grades = tempList.ToArray();

            for (int i = 0; i < grades.Length; i++)
            {
                Console.WriteLine($"{i}  |  {grades[i]}");

            }
            return grades;
        }

        private List<string> ReturnEmployeeGradesAsList()
        {
            var tempList = new List<string>();
            using (var reader = File.OpenText(@$"{this.CurrentDirectory}\{this.Name}.txt"))
            {
                var line = reader.ReadLine();

                while (line != null)
                {

                    tempList.Add(line);
                    line = reader.ReadLine();

                }
            }
            return tempList;
        }

        public override Statistics GetStatistics()
        {
            var result = new Statistics();
            if (File.Exists(@$"{this.CurrentDirectory}\{this.Name}.txt"))
            {
                using (var streamReaderFromEmpGrades = File.OpenText(@$"{this.CurrentDirectory}\{this.Name}.txt"))
                {
                    var line = streamReaderFromEmpGrades.ReadLine();

                    while (line != null)
                    {
                        bool CanParse;
                        double parseResult;
                        if (CanParse = double.TryParse(line, out parseResult))
                        {
                            result.Add(parseResult);
                        }
                        line = streamReaderFromEmpGrades.ReadLine();
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File with this name does not exist.");
            }
            return result;
        }

        public void ChangeThisEmployeeName(string newname)
        {
            var nameToCheck = newname;

            foreach (var ch in nameToCheck)
            {
                if (char.IsDigit(ch))
                {
                    throw new ArgumentException($"Name cannot contain numbers.");
                }
            }

            var oldName = this.Name;
            this.Name = newname;

            ChangeEmployeeFilesName(oldName);
            ReplaceOldNameInEmployeeListFile(oldName, this.Name);
        }

        public void ChangeEmployeeFilesName(string oldName)
        {
            System.IO.File.Move(@$"{this.CurrentDirectory}\{oldName}Audit.txt", @$"{this.CurrentDirectory}\{this.Name}Audit.txt");
            System.IO.File.Move(@$"{this.CurrentDirectory}\{oldName}.txt", @$"{this.CurrentDirectory}\{this.Name}.txt");
        }

        public void ReplaceOldNameInEmployeeListFile(string oldName, string currentName)
        {
            string text = File.ReadAllText(@$"{this.CurrentDirectory}\EmployeeList.txt");
            text = text.Replace(oldName, currentName);
            File.WriteAllText(@$"{this.CurrentDirectory}\EmployeeList.txt", text);
        }
    }
}
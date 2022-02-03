using System;
using System.Collections.Generic;
using System.IO;


public interface IFileWrapper
{
    string Text { get; set; }

    bool FileExists(string filePath);

    string[] ReturnStringArray();

    StreamReader StreamReaderFromTextFile(string filePath);
    StreamWriter StreamWriterToEmployeeGrades(string filePath);
    StreamWriter StreamWriterToAuditFile(string filePath);
    
    
}








using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;

namespace DVDNameReplacer
{
    public partial class Form1 : Form
    {
        private DirectoryInfo[] movieList;
        private FileInfo[] xmlFiles;
        private bool updatedname;
        private string movieTitle;
        private bool firstTime = true;

        public Form1()
        {
            InitializeComponent();

            string xmlLocation = @"C:\Users\Danger Is Go\AppData\Roaming\Microsoft\eHome\DvdInfoCache";
            string movieLocation = @"Z:\HD Movies";

            xmlFiles = new DirectoryInfo(xmlLocation).GetFiles("*.xml");
            movieList = new DirectoryInfo(movieLocation).GetDirectories();
        }

        private void btn_Go_Click(object sender, EventArgs e)
        {
            foreach (FileInfo xmlFile in xmlFiles)
            {
                StreamReader sr = new StreamReader(xmlFile.FullName);
                string line = sr.ReadLine().Trim();

                while (!sr.EndOfStream)
                {
                    if (line.Contains("<dvdTitle>"))
                    {
                        movieTitle = line.Substring(10, line.Length - 21);

                        string[] delim = {" "};
                        string[] splitstring = movieTitle.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                        if (splitstring.Length == 1)
                        {
                            if (Regex.Matches(splitstring[0], @"\.").Count > 1)
                            {
                                // Already done this file
                                updatedname = true;
                            }
                            else
                            {
                                // Means its a one word name
                                sr.Close();
                                updatedname = ParseMovieName(splitstring, xmlFile.FullName);
                            }
                        }
                        else
                        {
                            sr.Close();
                            updatedname = ParseMovieName(splitstring, xmlFile.FullName);
                        }
                        break;
                    }
                    line = sr.ReadLine().Trim();
                }
                sr.Close();

                if (!updatedname)
                {
                    if (firstTime)
                    {
                        if (File.Exists(@"C:\movieLog.log"))
                            File.Delete(@"C:\movieLog.log");
                        firstTime = false;
                    }

                    StreamWriter sw = new StreamWriter(@"C:\movieLog.log", true);

                    sw.WriteLine(string.Concat(movieTitle, " ", xmlFile.Name));
                    sw.Close();
                }
            }
        }

        private bool ParseMovieName(string[] splitString, string filePath)
        {
            if (splitString.Length == 1)
            {
                foreach (DirectoryInfo dir in movieList)
                {
                    if (dir.Name.ToLower().Contains(splitString[0].ToLower()))
                    {
                        // Ask for replacing here
                        DialogResult result = MessageBox.Show(string.Concat("Replace ", splitString[0], " with ", dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Add the tags and replace
                            string oldText = string.Concat("<dvdTitle>", splitString[0], "</dvdTitle>");
                            string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                            ReplaceContent(filePath, oldText, newText);
                            return true;
                        }
                        else
                        {
                            // Continue parsing movie folders
                            continue;
                        }
                    }
                }
            }
            else
            {
                foreach (DirectoryInfo dir in movieList)
                {
                    string newName;

                    if (splitString[1].Contains("["))
                    {
                        newName = splitString[0].ToLower();
                    }
                    else
                        newName = string.Concat(splitString[0], ".", splitString[1]).ToLower();

                    if (dir.Name.ToLower().Contains(newName))
                    {
                        // Ask for replacing here
                        DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                            " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Add the tags and replace
                            string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                            string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                            ReplaceContent(filePath, oldText, newText);
                            return true;
                        }
                        else
                        {
                            // Continue parsing movie folders
                            continue;
                        }
                    }
                    else
                    {
                        if (newName.Contains("'"))
                        {
                            newName = newName.Replace("'", "");
                        }
                        if (dir.Name.ToLower().Contains(newName))
                        {
                            // Ask for replacing here
                            DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                                " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                // Add the tags and replace
                                string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                                string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                                ReplaceContent(filePath, oldText, newText);
                                return true;
                            }
                            else
                            {
                                // Continue parsing movie folders
                                continue;
                            }
                        }
                        else
                        {
                            string temp = newName;
                            if (newName.Contains(":"))
                            {
                                temp = newName.Replace(":", ".");
                            }
                            if (dir.Name.ToLower().Contains(temp))
                            {
                                // Ask for replacing here
                                DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                                    " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (result == DialogResult.Yes)
                                {
                                    // Add the tags and replace
                                    string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                                    string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                                    ReplaceContent(filePath, oldText, newText);
                                    return true;
                                }
                                else
                                {
                                    // Continue parsing movie folders
                                    continue;
                                }
                            }
                            else
                            {
                                if (newName.Contains(":"))
                                {
                                    newName = newName.Replace(":", "");
                                }
                                if (dir.Name.ToLower().Contains(newName))
                                {
                                    // Ask for replacing here
                                    DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                                        " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (result == DialogResult.Yes)
                                    {
                                        // Add the tags and replace
                                        string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                                        string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                                        ReplaceContent(filePath, oldText, newText);
                                        return true;
                                    }
                                    else
                                    {
                                        // Continue parsing movie folders
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (newName.Contains("&amp;"))
                                    {
                                        newName = newName.Replace("&amp;", "&");
                                    }
                                    if (dir.Name.ToLower().Contains(newName))
                                    {
                                        // Ask for replacing here
                                        DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                                            " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                        if (result == DialogResult.Yes)
                                        {
                                            // Add the tags and replace
                                            string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                                            string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                                            ReplaceContent(filePath, oldText, newText);
                                            return true;
                                        }
                                        else
                                        {
                                            // Continue parsing movie folders
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (newName.Contains("-"))
                                        {
                                            newName = newName.Replace("-", ".");
                                        }
                                        if (dir.Name.ToLower().Contains(newName))
                                        {
                                            // Ask for replacing here
                                            DialogResult result = MessageBox.Show(string.Concat("Replace ", Environment.NewLine, ConcatSplitString(splitString), Environment.NewLine,
                                                                " with ", Environment.NewLine, dir.Name, "?"), "Replace Name?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                            if (result == DialogResult.Yes)
                                            {
                                                // Add the tags and replace
                                                string oldText = string.Concat("<dvdTitle>", ConcatSplitString(splitString), "</dvdTitle>");
                                                string newText = string.Concat("<dvdTitle>", dir.Name, "</dvdTitle>");
                                                ReplaceContent(filePath, oldText, newText);
                                                return true;
                                            }
                                            else
                                            {
                                                // Continue parsing movie folders
                                                continue;
                                            }
                                        }
                                    }
                                }

                            }
                            
                        }
                    }
                }
            }
            return false;
        }

        private string ConcatSplitString(string[] splitString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i<splitString.Length; i++)
            {
                sb.Append(splitString[i]);
                if (i < splitString.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        private void ReplaceContent(string filePath, string searchText, string replaceText)
        {
            searchText = searchText.Replace("[", @"\[");
            searchText = searchText.Replace("^", @"\^");
            searchText = searchText.Replace("$", @"\$");
            searchText = searchText.Replace(".", @"\.");
            searchText = searchText.Replace("+", @"\+");
            searchText = searchText.Replace("(", @"\(");
            searchText = searchText.Replace(")", @"\)");

            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            Match match = Regex.Match(content, searchText);

            content = Regex.Replace(content, searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
    }
}

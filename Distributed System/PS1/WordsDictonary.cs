using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem
{
    public class PS1
    {
        private WordsDictonary _words;
        public PS1(string wordsFilePath,string meanFilePath)
        {
            _words=new WordsDictonary();
            if (File.Exists(wordsFilePath))
            {
                using (StreamReader reader = new StreamReader(wordsFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        _words.AddWord(line.Trim());
                    }
                }
            }
            if (File.Exists(meanFilePath))
            {
                using (StreamReader reader = new StreamReader(meanFilePath))
                {
                    string line;
                    CharMap wordMap = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(IsAllCapitalsAlpha(line.Trim()))
                        {
                            var word = line.Trim();
                            wordMap = _words.FindWordMap(word);
                        }
                        else
                        {
                            if(wordMap!=null)
                            {
                                wordMap.AppendMean(line);
                            }
                        }
                    }
                }
            }
        }
        public bool IsAllCapitalsAlpha(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                foreach(var c in str)
                {
                    if (c >= 'A' && c <= 'Z')
                        continue;
                    else
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckWord(string str,out string mean)
        {
            return _words.IsWord(str,out mean);
        }
    }
    public class WordsDictonary
    {
        private CharMap Dict;
        public WordsDictonary()
        {
            Dict = new CharMap();
        }
        public void AddWord(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                var newwrod = word.Replace("-", "");//remove - in word
                Dict.Add(newwrod.ToUpper());
            }
        }
        public bool IsWord(string check,out string mean)
        {
            if (string.IsNullOrEmpty(check))
            {
                mean = null;
                return false;
            }
            return Dict.Exist(check.ToUpper(),out mean);
        }
        public CharMap FindWordMap(string word)
        {
            return Dict.FindWord(word.ToUpper());
        }
    }
    public class CharMap
    {
        private bool[] Characters;   //length 27 a-z is exist or not, the last one means is it end of the words
        private CharMap[] NextChars; //
        private string Meaning;// mean of the word
        public CharMap()
        {
            Characters = new bool[27];
            NextChars = new CharMap[27];
        }
        public void Add(string str)//str is uppercase
        {
            if (!string.IsNullOrEmpty(str))
            {
                Characters[str[0] - 'A'] = true;
                if (NextChars[str[0] - 'A'] == null)
                {
                    NextChars[str[0] - 'A'] = new CharMap();
                }
                NextChars[str[0] - 'A'].Add(str.Substring(1));
            }
            else
            {
                Characters[26] = true;
            }
        }
        public bool Exist(string str,out string mean)
        {
            mean = null;
            if (!string.IsNullOrEmpty(str))
            {
                return Characters[str[0] - 'A'] && NextChars[str[0] - 'A'] != null && NextChars[str[0] - 'A'].Exist(str.Substring(1),out mean);
            }
            else
            {
                if (Characters[26])
                    mean = Meaning;
                else
                    mean = null;
                return Characters[26];
            }
        }
        public CharMap FindWord(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if(Characters[word[0] - 'A'] && NextChars[word[0] - 'A'] != null)
                {
                    return NextChars[word[0] - 'A'].FindWord(word.Substring(1));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (Characters[26])
                    return this;
                else
                    return null;
            }
        }
        public void AppendMean(string mean)
        {
            if (string.IsNullOrEmpty(Meaning))
            {
                Meaning = mean;
            }
            else
            {
                Meaning += "\r\n";
                Meaning += mean;
            }
        }
    }


    /*
     class Program
    {
        static void Main(string[] args)
        {
            PS1 ps1 = new PS1(@"words.txt", @"dictionary.txt");
            bool exist=false;
            do
            {
                Console.Write("Input a word:");
                string str = Console.ReadLine();
                string mean;
                exist = ps1.CheckWord(str,out mean);
                if(exist)
                {
                    Console.WriteLine("Find {0}\r\nMean:\r\n{1}", str,mean);
                }
                else
                {
                    Console.WriteLine("Not Find {0}", str);
                }
            }
            while (!exist);
            Console.ReadLine();
        }
    }
     */
}

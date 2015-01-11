using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CountLines;


namespace InterfaceLocalizer.Classes
{
    //! Словарь <QuestID, CQuest>
    using QuestDict = Dictionary<int, CQuest>;
    //! Словарь <ID - TextData>
    using TextDict = Dictionary<int, CTextData>;
    //! Словарь <NPCName, <DialogID, CDialog>>
    using DialogDict = Dictionary<string, Dictionary<int, CDialog>>;

    public class CTextData
    {
        public string phrase;
        public string oldPhrase;
        public string sourcePhrase;
        public string filename;
        public Stack<string> tags;

        public CTextData()
        {         
        }
        public CTextData(string _phrase, string _eng, string _filename, Stack<string> _tags, string _sourcePhrase = "")
        {
            phrase = _phrase;
            filename = _filename;
            tags = new Stack<string>();
            tags = _tags;
            oldPhrase = _eng;
            sourcePhrase = _sourcePhrase;
        }
    }

    public class CDataManager
    {
        private TextDict textsDict = new TextDict();
        private TextDict dialogsDict = new TextDict();
        private TextDict questsDict = new TextDict();
        private TextDict newsDict = new TextDict();

        private QuestDict sourceQuestDict = new QuestDict();
        private DialogDict sourceDialogDict = new DialogDict();
        private QuestDict oldQuestDict = new QuestDict();
        private DialogDict oldDialogDict = new DialogDict();
        private string path1 = "old\\Interfaces\\";
        private string path2 = "old\\Quests\\";
        public int undoneDialogs = 0;
        public int undoneQuests = 0;
        public int totalDialogs = 0;
        int id = 0;
        
        public CDataManager()
        {
            id = 0;
        }
 
        public Dictionary<int, CTextData> getTextsDict()
        {
            return textsDict;
        }
        public Dictionary<int, CTextData> getDialogsDict()
        {
            return dialogsDict;
        }
        public Dictionary<int, CTextData> getQuestsDict()
        {
            return questsDict;
        }
        public Dictionary<int, CTextData> getNewsDict()
        {
            return newsDict;
        }

        public void clearAllData()
        {
            textsDict.Clear();
            id = 0;
        }

        public void addFileToManager(string filename)
        {   
            string phrase = "";
            string eng = "";

            Stack<string> tags = new Stack<string>();
            //string newPath = path1 + "\\new\\" + filename;
            string newPath = @"..\..\..\..\res\local\English\" + filename;
            string oldPath = path1 + filename;
            XmlTextReader reader = new XmlTextReader (newPath);
            XDocument engDoc = new XDocument();
            bool exists = true;
            if (File.Exists(oldPath))
                engDoc = XDocument.Load(oldPath);
            else
                exists = false;
            bool gotten = false;
            
            while (reader.Read())  
            {
                switch (reader.NodeType)  
                {
                    case XmlNodeType.Element: // Узел является элементом.
                        tags.Push(reader.Name);
                        if (reader.IsEmptyElement)
                        {
                            eng = "";
                            Stack<string> copy = new Stack<string>(tags.ToArray());
                            //texts.Add(new CTextData(phrase, eng, filename, copy));
                            textsDict.Add(id++, new CTextData(phrase, eng, filename, copy));
                            phrase = "";
                            eng = "";
                            tags.Pop();                            
                        }
                        break;
                    case XmlNodeType.Text: // Вывести текст в каждом элементе.
                        phrase = reader.Value.Trim();
                        gotten = true;
                        break;
                    case XmlNodeType.EndElement: // Вывести конец элемента.
                        //if (phrase != "")
                        if (gotten)
                        {
                            eng = getValueFromXml(engDoc, tags);
                            Stack<string> copy = new Stack<string>(tags.ToArray());
                            //texts.Add(new CTextData(phrase, eng, filename, copy));
                            textsDict.Add(id++, new CTextData(phrase, eng, filename, copy));
                            gotten = false;
                            phrase = "";
                            eng = "";
                        }
                        tags.Pop();
                        break;
                }
            }
        }

        public string getValueFromXml(XDocument doc, Stack<string> tags)
        {
            string result = "";
            //Stack<string> ntags = invertStack(copy);            
            Stack<string> ntags = new Stack<string>(tags);

            try
            {
                if (ntags.Count == 1)
                    result = doc.Element(ntags.Pop()).Value.ToString();
                else if (ntags.Count == 2)
                    result = doc.Element(ntags.Pop()).Element(ntags.Pop()).Value.ToString();
                else if (ntags.Count == 3)
                    result = doc.Element(ntags.Pop()).Element(ntags.Pop()).Element(ntags.Pop()).Value.ToString();
            }
            catch 
            {
                result = "<NO DATA>";
            }
            result = result.Trim();
            return result;        
        }

        public void addDialogsToManager(string filename)
        {
            Stack<string> tags = new Stack<string>();
            const int adder = 10000;
            int adder2 = 20000;
            undoneDialogs = 0;
            totalDialogs = 0;

            string newPath = @"..\..\..\..\res\scripts\common\data\Quests\ENG\" + filename;
            string oldPath = path2 + filename;
            string sourcePath = @"..\..\..\..\res\scripts\common\data\Quests\RUS\" + filename;
            addDialogsToDict(sourcePath, sourceDialogDict);
            addDialogsToDict(oldPath, oldDialogDict);

            XDocument doc = XDocument.Load(newPath);            
            XDocument oldDoc = new XDocument();
            if (File.Exists(oldPath))
                oldDoc = XDocument.Load(oldPath);
            totalDialogs = doc.Root.Elements().Count();
            foreach (XElement item in doc.Root.Elements())
            {
                Regex reg = new Regex("^[a-zA-Z0-9\\W]*$", RegexOptions.IgnoreCase);
                Regex regRus = new Regex("^[а-яА-Я0-9\\W]*$", RegexOptions.IgnoreCase);
                Regex regSquare = new Regex("[А-Я][\\d]", RegexOptions.IgnoreCase);
                int DialogID = int.Parse(item.Element("DialogID").Value);
                string title = item.Element("Title").Value.Trim();
                string text = item.Element("Text").Value.Trim();
                string holder = item.Element("Holder").Value.Trim();
                
                if (!reg.IsMatch(title) || !reg.IsMatch(text))
                {
                    if (item.Element("NodeCoordinates").Element("Active").Value.ToString() == "1")
                        if (title.Length > 1 || text.Length > 1)
                        {
                            if (!(regSquare.Match(text).Index > 0))
                            {
                                undoneDialogs++;

                            }
                            
                        }
                    continue;
                }
                
                 
                /*
                if (regRus.IsMatch(title) )
                    if ( regRus.IsMatch(text))
                {
                    if (item.Element("NodeCoordinates").Element("Active").Value.ToString() == "1")
                        if (title.Length > 1 || text.Length > 1)
                            undoneDialogs++;
                    continue;
                }
                */

                tags.Push(DialogID.ToString());
                Stack<string> copy = new Stack<string>(tags.ToArray());

                // getting old text
                CDialog old = new CDialog();
                if (oldDialogDict.ContainsKey(holder))
                    if (oldDialogDict[holder].ContainsKey(DialogID))
                     old = oldDialogDict[holder][DialogID];
                // getting source text
                CDialog source = sourceDialogDict[holder][DialogID];

                // костыли от одинаковых ID -ужоснах
                if (!dialogsDict.ContainsKey(DialogID))
                    dialogsDict.Add(DialogID, new CTextData(title, old.title, filename, copy, source.title));
                else
                    dialogsDict.Add(adder2++, new CTextData(title, old.title, filename, copy, source.title));
                if (!dialogsDict.ContainsKey(DialogID+adder))
                    dialogsDict.Add(DialogID+adder, new CTextData(text, old.text, filename, copy, source.text));
                else
                    dialogsDict.Add(adder2++, new CTextData(text, old.text, filename, copy, source.text));
                tags.Pop();
            }
        }

        public void addQuestsToManager(string filename)
        {
            Stack<string> tags = new Stack<string>();
            const int adder = 10000;
            const int adderW = 20000;
            const int adderF = 30000;
            undoneQuests = 0;

            string newPath = @"..\..\..\..\res\scripts\common\data\Quests\ENG\" + filename;
            string oldPath = path2 + filename;
            string sourcePath = @"..\..\..\..\res\scripts\common\data\Quests\RUS\" + filename;
            addQuestsToDict(sourcePath, sourceQuestDict);
            addQuestsToDict(oldPath, oldQuestDict);

            XDocument doc = XDocument.Load(newPath);
            XDocument oldDoc = new XDocument();
            if (File.Exists(oldPath))
                oldDoc = XDocument.Load(oldPath);

            foreach (XElement item in doc.Root.Elements())
            {
                Regex reg = new Regex("^[a-zA-Z0-9\\W]*$", RegexOptions.IgnoreCase);
                int QuestID = int.Parse(item.Element("QuestID").Value);
                string title = item.Element("QuestInformation").Element("Title").Value.Trim();
                string desc = item.Element("QuestInformation").Element("Description").Value.Trim();
                string win = item.Element("QuestInformation").Element("onWin").Value.Trim();
                string fail = item.Element("QuestInformation").Element("onFailed").Value.Trim();
                string holder = item.Element("Additional").Element("Holder").Value.Trim();
                if (!reg.IsMatch(title) || !reg.IsMatch(desc) || !reg.IsMatch(win) || !reg.IsMatch(fail))
                {
                    undoneQuests++;
                    continue;
                }

                tags.Push(QuestID.ToString());
                Stack<string> copy = new Stack<string>(tags.ToArray());

                // getting old text
                CQuest old = new CQuest();
                if (oldQuestDict.ContainsKey(QuestID))
                    old = oldQuestDict[QuestID];
                // getting source text
                CQuest source = sourceQuestDict[QuestID];

                questsDict.Add(QuestID, new CTextData(title, old.title, filename, copy, source.title));
                questsDict.Add(QuestID + adder, new CTextData(desc, old.description, filename, copy, source.description));
                questsDict.Add(QuestID + adderW, new CTextData(win, old.onWin, filename, copy, source.onWin));
                questsDict.Add(QuestID + adderF, new CTextData(fail, old.onFailed, filename, copy, source.onFailed));
                tags.Pop();
            }
        }

        public void addQuestsToDict(string fullPath, QuestDict targetDict)
        {
            if (!File.Exists(fullPath))
                return;
            XDocument doc = new XDocument();
            doc = XDocument.Load(fullPath);

            foreach (XElement item in doc.Root.Elements())
            {
                int QuestID = int.Parse(item.Element("QuestID").Value);
                int Version = 0;
                if (item.Descendants("Version").ToList().Count > 0)
                    if (!item.Element("Version").Value.Equals(""))
                        Version = int.Parse(item.Element("Version").Value);
                String Description = item.Element("QuestInformation").Element("Description").Value.Trim();
                String Title = item.Element("QuestInformation").Element("Title").Value.Trim();
                String onWin = item.Element("QuestInformation").Element("onWin").Value.Trim();
                String onFailed = item.Element("QuestInformation").Element("onFailed").Value.Trim();
                String Holder = item.Element("Additional").Element("Holder").Value.Trim();

                if (!targetDict.ContainsKey(QuestID))
                    targetDict.Add(QuestID, new CQuest(QuestID,Version, Description, Title, onWin, onFailed, Holder));
            }
        }

        private void addDialogsToDict(string fullPath, DialogDict targetDict)
        {
            if (!File.Exists(fullPath))
                return;
            XDocument doc = new XDocument();
            doc = XDocument.Load(fullPath);
            foreach (XElement item in doc.Root.Elements())
            {
                int DialogID = int.Parse(item.Element("DialogID").Value);
                string Title = item.Element("Title").Value.Trim();
                string Text = item.Element("Text").Value.Trim();
                string holder = item.Element("Holder").Value.Trim();

                int Version = 0;
                if (item.Descendants("Version").ToList().Count > 0)
                    if (!item.Element("Version").Value.Equals(""))
                        Version = int.Parse(item.Element("Version").Value);
                /*
                NodeCoordinates nodeCoord = new NodeCoordinates();
                if (item.Descendants("NodeCoordinates").ToList().Count > 0)
                {
                    if (item.Element("NodeCoordinates").Descendants().Any(itm2 => itm2.Name == "Active"))
                    {
                        if (item.Element("NodeCoordinates").Element("Active").Value.Equals("1"))
                            nodeCoord.Active = true;
                        else
                            nodeCoord.Active = false;
                    }
                }
                */

                if (targetDict.Keys.Contains(holder))
                {
                    if (!targetDict[holder].Keys.Contains(DialogID))
                        targetDict[holder].Add(DialogID, new CDialog(DialogID, Version, Title, Text, holder));    // , nodeCoord
                }
                else
                {
                    targetDict.Add(holder, new Dictionary<int, CDialog>());
                    targetDict[holder].Add(DialogID, new CDialog(DialogID, Version, Title, Text, holder));    // , nodeCoord
                }
            }
        }

        public void addNewsToManager()
        {
            string newPath = @"..\..\..\..\res\scripts\base\data\";
            string fileNews = "BulletinBoardNews.txt";
            string fileGossip = "BulletinBoardQuest.txt";
            string news = File.ReadAllText(newPath + fileNews);
            string gossip = File.ReadAllText(newPath + fileGossip);
            newsDict.Add(0, new CTextData(news, "", fileNews, new Stack<string>()));
            newsDict.Add(1, new CTextData(gossip, "", fileGossip, new Stack<string>()));            
        }
    
    }
}

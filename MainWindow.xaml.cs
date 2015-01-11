using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

using InterfaceLocalizer.Classes;

namespace CountLines
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CDataManager dataManager = new CDataManager();

        int QuestsSymbolsNow = 0;
        int DialogsSymbolsNow = 0;
        int InterfacesSymbolsNow = 0;
        int NewsSymbolsNow = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        public class Item
        {
            public int id { get; set; }
            public string filename { get; set; }
            public string path { get; set; }
            public string sourceText { get; set; }
            public string oldText{ get; set; }
            public string newText { get; set; }
        }

        private void addDataToGridView(int id, CTextData td)
        {
            Stack<string> copy = new Stack<string>(td.tags);
            copy = CFileList.invertStack(copy);
            string temp = "";
            while (copy.Count != 0)
                temp += copy.Pop() + " -> ";

            Item item = new Item();
            item.id = id;
            item.filename = Path.GetFileName(td.filename);
            item.path = temp;
            item.sourceText = td.sourcePhrase;
            item.oldText = td.oldPhrase;
            item.newText = td.phrase;            
            DataGridDiff.Items.Add(item);            
        }

        private void addInfoToGridView(String line1, int num1, String line2, int num2, String line3 = "", int num3 = 0)
        {
            DataGridDiff.Items.Clear();
            Item item1 = new Item();
            item1.sourceText = line1;
            item1.oldText = "" + num1.ToString();
            DataGridDiff.Items.Add(item1);
            Item item2 = new Item();
            item2.sourceText = line2;
            item2.oldText = "" + num2.ToString();
            DataGridDiff.Items.Add(item2);
            if (line3 != "" && num3 != 0)
            {
                Item item3 = new Item();
                item3.sourceText = line3;
                item3.oldText = "" + num3.ToString();
                DataGridDiff.Items.Add(item3);
            }
        }

        //**************INTERFACES add, show and count
        //***********************************************
        private void AddAllFiles_Click(object sender, RoutedEventArgs e)
        {
            dataManager.clearAllData();
            string path1 = "old\\Interfaces\\";
            int count = 0;
            IEnumerable<string> files = System.IO.Directory.EnumerateFiles(path1, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string filepath in files)
            {
                if (Path.GetFileName(filepath) == "soCheckBox.xml")    // костыль, так как файл содержит только пустую строку
                    continue;
                dataManager.addFileToManager(Path.GetFileName(filepath));
                count++;
            }
            OutNumber.Header = "Добавлено: " + count.ToString() + " файлов";
            AddAllFiles.IsEnabled = false;
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getTextsDict();
            DataGridDiff.BeginInit();
            foreach (int id in textDict.Keys)
                addDataToGridView(id, textDict[id]);
            DataGridDiff.EndInit();
            OutNumber.Header = "Выведено: " + DataGridDiff.Items.Count.ToString();
        }

        private void ShowChanged_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getTextsDict();
            DataGridDiff.BeginInit();
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                    addDataToGridView(id, textDict[id]);
            }
            DataGridDiff.EndInit();
            OutNumber.Header = "Выведено: " + DataGridDiff.Items.Count.ToString();
        }

        private void Count_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, CTextData> textDict = dataManager.getTextsDict();
            InterfacesSymbolsNow = 0;
            int symbolsWas = 0;
            int phrases = 0;
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                {
                    InterfacesSymbolsNow += textDict[id].phrase.Length;
                    if (textDict[id].oldPhrase != "<NO DATA>")
                        symbolsWas += textDict[id].oldPhrase.Length;
                    phrases++;
                }
            }
            addInfoToGridView("Фраз переведено:", phrases, "Символов переведено:", InterfacesSymbolsNow);
        }

        //**************DIALOGS add, show and count
        //***********************************************
        private void AddDialogs_Click(object sender, RoutedEventArgs e)
        {
            dataManager.addDialogsToManager("Dialogs.xml");
            OutNumber.Header = "Диалоги добавлены";
            AddDialogs.IsEnabled = false;
        }

        private void ShowChangedDialogs_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getDialogsDict();
            DataGridDiff.BeginInit();
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                    addDataToGridView(id, textDict[id]);
            }
            DataGridDiff.EndInit();
            OutNumber.Header = "Выведено: " + DataGridDiff.Items.Count.ToString();
        }

        private void CountDialogs_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getDialogsDict();
            DialogsSymbolsNow = 0;
            int symbolsWas = 0;
            int phrases = 0;
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                {
                    DialogsSymbolsNow += textDict[id].phrase.Length;
                    if (textDict[id].oldPhrase != "<NO DATA>")
                        symbolsWas += textDict[id].oldPhrase.Length;
                    phrases++;
                }
            }
            addInfoToGridView("Фраз переведено:", phrases, "Символов переведено:", DialogsSymbolsNow);  // , "Фраз осталось перевести:", dataManager.undoneDialogs
        }

        //**************QUESTS add, show and count
        //***********************************************
        private void AddQuests_Click(object sender, RoutedEventArgs e)
        {
            dataManager.addQuestsToManager("Quests.xml");
            OutNumber.Header = "Квесты добавлены";
            AddQuests.IsEnabled = false;
        }

        private void ShowQuests_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getQuestsDict();
            DataGridDiff.BeginInit();
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                    addDataToGridView(id, textDict[id]);
            }
            DataGridDiff.EndInit();
            OutNumber.Header = "Выведено: " + DataGridDiff.Items.Count.ToString();
        }

        private void CountQuests_Click(object sender, RoutedEventArgs e)
        {
            DataGridDiff.Items.Clear();
            Dictionary<int, CTextData> textDict = dataManager.getQuestsDict();
            QuestsSymbolsNow = 0;
            int symbolsWas = 0;
            int phrases = 0;
            foreach (int id in textDict.Keys)
            {
                if (textDict[id].phrase != textDict[id].oldPhrase)
                {
                    QuestsSymbolsNow += textDict[id].phrase.Length;
                    if (textDict[id].oldPhrase != "<NO DATA>")
                        symbolsWas += textDict[id].oldPhrase.Length;
                    phrases++;
                }
            }

            addInfoToGridView("Фраз переведено:", phrases, "Символов переведено:", QuestsSymbolsNow, "Фраз осталось перевести:", dataManager.undoneQuests);
        }

        //**************NEWS and GOSSIP add and count
        //***********************************************
        private void AddNews_Click(object sender, RoutedEventArgs e)
        {
            dataManager.addNewsToManager();
            OutNumber.Header = "Новости добавлены";
            AddNews.IsEnabled = false;
        }

        private void CountNews_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, CTextData> textDict = dataManager.getNewsDict();
            int newsSymb = 0;
            int gossipSymb = 0;
            foreach (int id in textDict.Keys)
            {                
                if (textDict[id].filename == "BulletinBoardNews.txt")
                    newsSymb = textDict[id].phrase.Length;
                else if (textDict[id].filename == "BulletinBoardQuest.txt")
                    gossipSymb = textDict[id].phrase.Length;
            }
            NewsSymbolsNow = newsSymb + gossipSymb;

            addInfoToGridView("Переведено символов новостей:", newsSymb, "Переведено символов слухов:", gossipSymb);
        }

        private void CountTotal_Click(object sender, RoutedEventArgs e)
        {
            int total = InterfacesSymbolsNow + DialogsSymbolsNow + QuestsSymbolsNow + NewsSymbolsNow;
            double charge = (double)total / 1000.0 * 350;

            addInfoToGridView("Символов суммарно переведено:", total, "Суммарная стоимость:", (int) charge);
        }

    }
}

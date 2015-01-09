using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLocalizer.Classes
{
    public class CFileList
    {
        public static List<string> allFiles = new List<string>();
        public static List<string> checkedFiles = new List<string>();

        /*
        public static void setAllFilesList(List<string> files)
        {
            allFiles = files;
        }

        public static void setCheckedFilesList(List<string> files)
        {
            checkedFiles = files;
        }
         * */
        public static string getFilenameFromPath(string path)
        {
            string filename = "";
            int last = path.LastIndexOf("\\");
            filename = path.Substring(last + 1);
            return filename;
        }

        public static string getListAsString(List<string> list)
        {
            string result = "";
            foreach (string str in list)
                result += str + ";";
            return result;
        }
        public static List<string> getListFromString(string value)
        {
            List<string> result = new List<string>();
            string []arr = value.Split(';');
            foreach (string str in arr)
            {
                result.Add(str);
            }
            return result;
        }
        
        public static Stack<string> invertStack(Stack<string> stack)
        {
            Stack<string> result = new Stack<string>(stack);
            //while (stack.Count > 0)
            //    result.Push(stack.Pop());
            return result;
        }
          
    }
}

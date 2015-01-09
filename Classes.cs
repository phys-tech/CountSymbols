using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountLines
{
    public class CQuest
    {
        int questID;
        int version;
        public string description;
        public string title;
        public string onWin;
        public string onFailed;
        public string holder;

        public CQuest()
        {

        }
        public CQuest(int id, int vers, string _desc, string _title, string _onWin, string _onFailed, string _holder)
        {
            questID = id;
            version = vers;
            description = _desc;
            title = _title;
            onWin = _onWin;
            onFailed = _onFailed;
            holder = _holder;
        }
    }
}

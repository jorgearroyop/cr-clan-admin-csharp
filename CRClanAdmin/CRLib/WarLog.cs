using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRLib
{
    public class WarLog
    {
        public War[] items;        
    }

    public class War
    {
        public int seasonId;
        public string createdDate;
        public Participants[] participants;


        public int FindParticipant(string aname)
        {
            int ind = -1;
            for (int i = 0; i < participants.Length; i++)
            {
                if (participants[i].name.Equals(aname))
                {
                    ind = i;
                    return ind;
                }
            }
            return ind;
        }
    }
}



/* Model
WarLog {
items(Array[inline_model], optional)
}
inline_model {
seasonId(integer, optional),
createdDate(string, optional),
participants(Array[WarParticipant], optional)
}
WarParticipant {
tag(string, optional),
name(string, optional),
cardsEarned(integer, optional),
battlesPlayed(integer, optional),
wins(integer, optional)
}
*/
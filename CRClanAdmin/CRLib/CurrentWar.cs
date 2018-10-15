using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRLib
{
    public class CurrentWar
    {
        public string state;
        public string warEndTime;
        public string collectionEndTime;            // only on collection day otherwise null
        public Clan clan;
        public Participants[] participants;
    }    
}


// Note: make sure the labels match exactly the members of the class
// warEndTime/collectionEndTime depending if it is collection day or war day
/* Model
{
  "state": "string",
  "warEndTime": "string",
  "clan": {
    "tag": "string",
    "name": "string",
    "badgeId": 0,
    "clanScore": 0,
    "participants": 0,
    "battlesPlayed": 0,
    "wins": 0,
    "crowns": 0
  },
  "participants": [
    {
      "tag": "string",
      "name": "string",
      "cardsEarned": 0,
      "battlesPlayed": 0,
      "wins": 0
    }
  ]
}


CurrentWar {
state (string, optional),
warEndTime (string, optional),
clan (inline_model_0, optional),
participants (Array[WarParticipant], optional)
}
inline_model_0 {
tag (string, optional),
name (string, optional),
badgeId (integer, optional),
clanScore (integer, optional),
participants (integer, optional),
battlesPlayed (integer, optional),
wins (integer, optional),
crowns (integer, optional)
}
WarParticipant {
tag (string, optional),
name (string, optional),
cardsEarned (integer, optional),
battlesPlayed (integer, optional),
wins (integer, optional)
}
*/

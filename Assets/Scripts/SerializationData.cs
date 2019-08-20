using System.Collections;
using System.Collections.Generic;
using System;

/* Multi-languages */
namespace LanguagesSerialization
{
    [System.Serializable]
    public class ListWrapper
    {
        public List<string> l;
        public ListWrapper() { }
        public ListWrapper(List<string> _list)
        {
            l = _list;
        }
    }

    [System.Serializable]
    public class SerializablePhrase
    {
        public List<ListWrapper> translations;
        public string id
        {
            get
            {
                return translations[0].l[0];
            }
            set
            {
                if (translations.Count == 0)
                {
                    translations.Add(new ListWrapper());
                }
                translations[0].l = new List<string>() { value };
            }

        }
    }

    [System.Serializable]

    public class DictionaryData
    {
        public List<SerializablePhrase> phrases;
    }

    public class Phrase
    {
        public string id;
        public Dictionary<string, string> translations;

        public Phrase(SerializablePhrase x)
        {
            translations = new Dictionary<string, string>();
            id = x.id;
            foreach (var it in x.translations)
            {
                translations.Add(it.l[0], it.l[1]);
            }
        }
    }

    public class GameDictionary
    {
        public Dictionary<string, bool> languages;
        public Dictionary<string, Phrase> phrases;
        public GameDictionary(DictionaryData x)
        {
            foreach (var it in x.phrases)
            {
                phrases.Add(it.id, new Phrase(it));
                foreach (var tr in it.translations)
                {
                    languages.Add(tr.l[0], true);
                }
            }
        }
    }
}


/*********************************************************** */

namespace JumpersSerialization
{
    [System.Serializable]
    public class Country
    {
        public string id;
        public string nameId;
        public string ioc;

        public Country() { }
        public Country(string _id, string _nameId, string _ioc)
        {
            id = _id;
            nameId = _nameId;
            ioc = _ioc;
        }
    }

    [System.Serializable]
    public class SerializableJumper
    {
        public int code;
        public string firstName;
        public string lastName;
        public string gender;
        public string dateOfBirth;
        public string countryId;

        public SerializableJumper() { }
        public SerializableJumper(int _code, string _firstName, string _lastName, string _gender, DateTime _dateOfBirth, string _countryId)
        {
            code = _code;
            firstName = _firstName;
            lastName = _lastName;
            gender = _gender;
            dateOfBirth = _dateOfBirth.ToString(new System.Globalization.CultureInfo("pl-PL"));
            countryId = _countryId;
        }
    }

    [System.Serializable]
    public class Jump
    {
        public float dist;
        public List<float> judgesMarks;
        public int gate;
        public float speed;
        public float wind;
        public int rank;
        public int bib;
        public Jump()
        {
            judgesMarks = new List<float>();
        }
        public Jump(float _dist, float _a, float _b, float _c, float _d, float _e, int _gate, float _speed, float _wind, int _rank, int _bib)
        {
            dist = _dist;
            judgesMarks = new List<float>() { _a, _b, _c, _d, _e };
            gate = _gate;
            speed = _speed;
            wind = _wind;
            rank = _rank;
            bib = _bib;
        }
    }

    [System.Serializable]
    public class Result
    {
        public List<Jump> jumps;
        public SerializableJumper competitor;
        public float total;
        public int rank;
    }

    [System.Serializable]
    public class RoundInfo
    {
        public string startTime;
        public string endTime;
        public int startGate;

        public RoundInfo(){}
        public RoundInfo(DateTime _startTime, DateTime _endTime, int _startGate)
        {
            startTime = _startTime.ToString(new System.Globalization.CultureInfo("pl-PL"));
            endTime = _endTime.ToString(new System.Globalization.CultureInfo("pl-PL"));
            startGate = _startGate;
        }
    }

    [System.Serializable]
    public class Competition
    {
        public List<RoundInfo> roundsInfo;
        public List<Result> results;
    }


    [System.Serializable]
    public class SerializableJumperData
    {
        SerializableJumper jp;
        List<Result> before;
        Result current;

    }

    // [System.Serializable]

    // public class SerializableCompetitionData
    // {
    //     public
    // }
}
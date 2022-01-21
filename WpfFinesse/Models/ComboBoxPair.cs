using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinesse.Models
{
    public class ComboBoxPair
    {
        public string _Key { get; set; }
        public string _Value { get; set; }

        public string _Type { get; set; }
        public ComboBoxPair(string _key, string _value)
        {
            _Key = _key;
            _Value = _value;
        }

        public ComboBoxPair(string _key, string _value,string _type)
        {
            _Key = _key;
            _Value = _value;
            _Type = _type;
        }
    }
}

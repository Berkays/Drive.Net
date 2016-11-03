using System;
using System.Collections.Generic;

namespace DriveNET
{
    public enum Field
    {
        Name,
        FullText,
        mimeType,
        modifiedTime,
        Thrashed,
        Starred,
        Parents,
        Owners,
        Properties
    }
    public enum MimeType
    {
        Application,
        Audio,
        Image,
        Text,
        Video
    }

    enum Operator
    {
        AND,
        OR,
        NOT,
        EQUAL,
        CONTAINS,
        NOTEQUAL,
        SMALLER,
        E_SMALLER,
        BIGGER,
        E_BIGGER,
        IN,
        HAS
    }

    internal abstract class AbstractField
    {
        public abstract string Key { get;}
        internal abstract List<Operator> AllowedOperators{get;}
    }

    internal sealed class Name : AbstractField
    {
        public override string Key
        {
            get{return "name";}
        }
        internal override List<Operator> AllowedOperators
        {
            get{return new List<Operator>() { Operator.CONTAINS,Operator.EQUAL,Operator.NOTEQUAL};}
        }
    }
    internal sealed class FullText : AbstractField
    {
        public override string Key
        {
            get { return " fullText "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.CONTAINS }; }
        }
    }
    internal sealed class MIMEType : AbstractField
    {
        public override string Key
        {
            get { return " mimeType "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.CONTAINS, Operator.EQUAL, Operator.NOTEQUAL }; }
        }
    }
    internal sealed class ModifiedTime : AbstractField
    {
        public override string Key
        {
            get { return " modifiedTime "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.E_SMALLER,Operator.SMALLER, Operator.EQUAL,Operator.NOTEQUAL,Operator.BIGGER,Operator.E_BIGGER}; }
        }
    }
    internal sealed class Thrashed : AbstractField
    {
        public override string Key
        {
            get { return " thrashed "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.EQUAL, Operator.NOTEQUAL }; }
        }
    }
    internal sealed class Starred : AbstractField
    {
        public override string Key
        {
            get { return " starred "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.EQUAL, Operator.NOTEQUAL }; }
        }
    }
    internal sealed class Parents : AbstractField
    {
        public override string Key
        {
            get { return " parents "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.CONTAINS, Operator.EQUAL, Operator.NOTEQUAL }; }
        }
    }
    internal sealed class Owners : AbstractField
    {
        public override string Key
        {
            get { return " owners "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.CONTAINS, Operator.EQUAL, Operator.NOTEQUAL }; }
        }
    }
    internal sealed class Properties : AbstractField
    {
        public override string Key
        {
            get { return " properties "; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.HAS}; }
        }
    }
    internal sealed class ValueField : AbstractField
    {
        public override string Key
        {
            get { return string.Empty; }
        }
        internal override List<Operator> AllowedOperators
        {
            get { return new List<Operator>() { Operator.AND,Operator.BIGGER,Operator.CONTAINS,Operator.EQUAL,Operator.E_BIGGER,Operator.E_SMALLER,Operator.HAS,Operator.IN,Operator.NOTEQUAL,Operator.OR,Operator.SMALLER }; }
        }
    }

    public sealed class NetGSearchBuilder
    {
        public string Search { get; set; }
        private AbstractField lastField;
        private ValueField valueField = new ValueField();
     
        #region Dictionaries

        private Dictionary<Operator, string> operatorList = new Dictionary<Operator, string>()
        {
            {Operator.AND," and "},
            {Operator.OR," or "},
            {Operator.NOT," not "},
            {Operator.EQUAL," = "},
            {Operator.CONTAINS,"contains "},
            {Operator.NOTEQUAL," != "},
            {Operator.SMALLER," < "},
            {Operator.E_SMALLER," <= "},
            {Operator.BIGGER," > "},
            {Operator.E_BIGGER," >= "},
            {Operator.IN," in "},
            {Operator.HAS," has "},
        };

        private Dictionary<Field, AbstractField> FieldList = new Dictionary<Field, AbstractField>()
        {
            {Field.Name,new Name()},
            {Field.FullText,new FullText()},
            {Field.mimeType,new MIMEType()},
            {Field.modifiedTime,new ModifiedTime()},
            {Field.Thrashed,new Thrashed()},
            {Field.Starred,new Starred()},
            {Field.Parents,new Parents()},
            {Field.Owners,new Owners()},
            {Field.Properties,new Properties()}
        };

        private Dictionary<MimeType, string> MimeList = new Dictionary<MimeType, string>()
        {
            {MimeType.Application,"application/" },
            {MimeType.Audio,"audio/" },
            {MimeType.Image,"image/" },
            {MimeType.Text,"text/" },
            {MimeType.Video,"video/" }

        };
        #endregion

        public NetGSearchBuilder AddField(Field f)
        {
            Search += FieldList[f].Key;
            lastField = FieldList[f];
            return this;
        }

        #region Operator Methods

        public NetGSearchBuilder PStart()
        {
            Search += '(';
            return this;
        }
        public NetGSearchBuilder PEnd()
        {
            Search += ')';
            return this;
        }

        public NetGSearchBuilder And()
        {
            Operator op = Operator.AND;

            Search += operatorList[op];
            StringCheck(op);
            return this;
        }
        public NetGSearchBuilder Or()
        {
            Operator op = Operator.OR;
            Search += operatorList[Operator.OR];
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Not()
        {
            Operator op = Operator.NOT;
            Search += operatorList[Operator.NOT];
            StringCheck(op);

            return this;
        }

        public NetGSearchBuilder Equal(string value)
        {
            Operator op = Operator.EQUAL;
            Search += (operatorList[Operator.EQUAL] + "'" + value +"'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Equal(bool value)
        {
            Operator op = Operator.EQUAL;
            Search += (operatorList[Operator.EQUAL] + value);
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Equal(DateTime value)
        {
            this.Equal(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder NotEqual(string value)
        {
            Operator op = Operator.NOTEQUAL;
            Search += (operatorList[Operator.NOTEQUAL] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder NotEqual(bool value)
        {
            Operator op = Operator.NOTEQUAL;
            Search += (operatorList[Operator.NOTEQUAL] + value);
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder NotEqual(DateTime value)
        {
            this.NotEqual(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder Contains(string value)
        {
            Operator op = Operator.CONTAINS;
            Search += (operatorList[Operator.CONTAINS] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Contains(MimeType mime)
        {
            this.Contains(MimeList[mime]);
            return this;
        }

        public NetGSearchBuilder Smaller(string value)
        {
            Operator op = Operator.SMALLER;
            Search += (operatorList[Operator.SMALLER] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Smaller(DateTime value)
        {
            this.Smaller(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder Equal_Smaller(string value)
        {
            Operator op = Operator.E_SMALLER;
            Search += (operatorList[Operator.E_SMALLER] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Equal_Smaller(DateTime value)
        {
            this.Equal_Smaller(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder Bigger(string value)
        {
            Operator op = Operator.BIGGER;
            Search += (operatorList[Operator.BIGGER] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Bigger(DateTime value)
        {
            this.Bigger(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder Equal_Bigger(string value)
        {
            Operator op = Operator.E_BIGGER;
            Search += (operatorList[Operator.E_BIGGER] + "'" + value + "'");
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Equal_Bigger(DateTime value)
        {
            this.Equal_Bigger(value.ToRFC());
            return this;
        }

        public NetGSearchBuilder In(string value)
        {
            Operator op = Operator.IN;
            Search += (operatorList[Operator.IN] + value);
            StringCheck(op);

            return this;
        }
        public NetGSearchBuilder Has(string value)
        {
            Operator op = Operator.HAS;
            Search += (operatorList[Operator.HAS] + value);
            StringCheck(op);

            return this;
        }

        private void StringCheck(Operator o)
        {
            lastField = valueField;
            if (o == Operator.NOT)
                return;
            if (lastField.AllowedOperators.Contains(o) == false)
            {
                throw new Exception(string.Format("Operator {0} is not allowed for {1}",o.ToString(),lastField.Key), new InvalidOperationException());
            }
        }

        #endregion
    }


    public static class DateTimeExtension
    {
        public static string ToRFC(this DateTime date)
        {
            string format = "yyyy-MM-ddT12:HH:mm";
            return date.ToString(format);
        }
    }

}

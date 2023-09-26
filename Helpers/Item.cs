namespace Inzynierka.Helpers
{
    public class Item
    {
        int intValue;
        string stringValue;
        DateTime dateTimeValue;
        char charValue;
        Kind kind;

        public object Value
        {
            get
            {
                switch (kind)
                {
                    case Kind.Int:
                        return intValue;
                    case Kind.String:
                        return stringValue;
                    case Kind.DateTime:
                        return dateTimeValue;
                    case Kind.Char:
                        return charValue;
                    default:
                        return null;
                }

            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        // Implicit construction
        public static implicit operator Item(int i)
        {
            return new Item { intValue = i, kind = Kind.Int };
        }
        public static implicit operator Item(string s)
        {
            return new Item { stringValue = s, kind = Kind.String };
        }
        public static implicit operator Item(DateTime dt)
        {
            return new Item { dateTimeValue = dt, kind = Kind.DateTime };
        }
        public static implicit operator Item(char c)
        {
            return new Item { charValue = c, kind = Kind.Char };
        }

        // Implicit value reference
        public static implicit operator int(Item item)
        {
            if (item.kind != Kind.Int) // Optionally, you could validate the usage
            {
                throw new InvalidCastException("Trying to use a " + item.kind + " as an int");
            }
            return item.intValue;
        }
        public static implicit operator string(Item item)
        {
            return item.stringValue;
        }
        public static implicit operator DateTime(Item item)
        {
            return item.dateTimeValue;
        }
        public static implicit operator char(Item item)
        {
            return item.charValue;
        }
    }

    enum Kind
    {
        Int,
        String,
        DateTime,
        Char
    }
}

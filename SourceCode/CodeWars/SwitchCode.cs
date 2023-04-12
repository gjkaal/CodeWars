namespace CodeWars
{
    public class SwitchCode
    {
        public int SwitchCase4(char v)
        {
            switch (v)
            {
                case 'a': return 65;
                case 'b': return 66;
                case 'c': return 67;
                case 'd': return 68;

                default: return 20;
            }
        }

        public int SwitchCase7(string v)
        {
            switch (v)
            {
                default: return 20;
                case "a": return 65;
                case "b": return 66;
                case "c": return 67;
                case "d": return 68;

                case "ex": return 69;
                case "fx": return 70;
                case "gx": return 71;
            }
        }

        public int SwitchCase10(char v)
        {
            switch (v)
            {
                case 'a': return 65;
                case 'b': return 66;
                case 'c': return 67;
                case 'd': return 68;

                case 'e': return 69;
                case 'f': return 70;
                case 'g': return 71;
                case 'h': return 72;

                case 'i': return 73;
                case 'j': return 74;
                default: return 20;
            }
        }
    }
}

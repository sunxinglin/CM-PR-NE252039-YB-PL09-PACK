using System;


namespace RogerTech.Tool
{
    public class TagResult
    {
        public object Value { get; private set; }
        public bool Available { get; private set; }
        internal void SetValue(object value)
        {

            this.Value = value; 
        }
        internal void SetAviliable(bool aviliabled)
        {
            this.Available = aviliabled;
        }
    }
}
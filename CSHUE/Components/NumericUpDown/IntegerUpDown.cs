namespace CSHUE.Components.NumericUpDown
{
    public class IntegerUpDown : CommonNumericUpDown<int>
    {
        #region Constructors

        static IntegerUpDown()
        {
            UpdateMetadata(typeof(IntegerUpDown), 1, int.MinValue, int.MaxValue);
        }

        public IntegerUpDown() : base(int.TryParse, decimal.ToInt32, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
        }

        #endregion

        #region Base Class Overrides

        protected override int IncrementValue(int value, int increment)
        {
            return value + increment;
        }

        protected override int DecrementValue(int value, int increment)
        {
            return value - increment;
        }

        #endregion
    }
}
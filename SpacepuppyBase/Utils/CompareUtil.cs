namespace com.spacepuppy.Utils
{
    public static class CompareUtil
    {

        private const int GT = (int)(ComparisonOperator.GreaterThan);
        private const int GT_EQUAL = (int)(ComparisonOperator.GreaterThan | ComparisonOperator.Equal);
        private const int LT = (int)(ComparisonOperator.LessThan);
        private const int LT_EQUAL = (int)(ComparisonOperator.LessThan | ComparisonOperator.Equal);
        private const int EQUAL = (int)(ComparisonOperator.Equal);
        private const int NOT_EQUAL = (int)(ComparisonOperator.NotEqual);
        private const int NOT_EQUAL_ALT = (int)(ComparisonOperator.LessThan | ComparisonOperator.GreaterThan);
        private const int ALWAYS_EQUAL = (int)(ComparisonOperator.GreaterThan | ComparisonOperator.LessThan | ComparisonOperator.Equal);

        public static bool Compare(ComparisonOperator op, int a, int b)
        {
            switch ((int)op)
            {
                case GT:
                    return a > b;
                case GT_EQUAL:
                    return a >= b;
                case LT:
                    return a < b;
                case LT_EQUAL:
                    return a <= b;
                case EQUAL:
                    return a == b;
                case NOT_EQUAL:
                case NOT_EQUAL_ALT:
                    return a != b;
                case ALWAYS_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Compare(ComparisonOperator op, long a, long b)
        {
            switch ((int)op)
            {
                case GT:
                    return a > b;
                case GT_EQUAL:
                    return a >= b;
                case LT:
                    return a < b;
                case LT_EQUAL:
                    return a <= b;
                case EQUAL:
                    return a == b;
                case NOT_EQUAL:
                case NOT_EQUAL_ALT:
                    return a != b;
                case ALWAYS_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Compare(ComparisonOperator op, float a, float b)
        {
            switch ((int)op)
            {
                case GT:
                    return a > b;
                case GT_EQUAL:
                    return a >= b;
                case LT:
                    return a < b;
                case LT_EQUAL:
                    return a <= b;
                case EQUAL:
                    return a == b;
                case NOT_EQUAL:
                case NOT_EQUAL_ALT:
                    return a != b;
                case ALWAYS_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Compare(ComparisonOperator op, double a, double b)
        {
            switch ((int)op)
            {
                case GT:
                    return a > b;
                case GT_EQUAL:
                    return a >= b;
                case LT:
                    return a < b;
                case LT_EQUAL:
                    return a <= b;
                case EQUAL:
                    return a == b;
                case NOT_EQUAL:
                case NOT_EQUAL_ALT:
                    return a != b;
                case ALWAYS_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Compare(ComparisonOperator op, decimal a, decimal b)
        {
            switch ((int)op)
            {
                case GT:
                    return a > b;
                case GT_EQUAL:
                    return a >= b;
                case LT:
                    return a < b;
                case LT_EQUAL:
                    return a <= b;
                case EQUAL:
                    return a == b;
                case NOT_EQUAL:
                case NOT_EQUAL_ALT:
                    return a != b;
                case ALWAYS_EQUAL:
                    return true;
                default:
                    return false;
            }
        }

    }
}

namespace BlogSample.Models
{
    public static class Utilities
    {
        public static string CutText(string text, int wordCount = 35)
        {
            string[] words = text.Split(' ');
            if (words.Length <= wordCount)
            {
                return text;
            }

            ReadOnlySpan<string> strings = new Span<string>(words);
            ReadOnlySpan<string> shortened = strings[..wordCount];

            return string.Join(' ', shortened.ToArray()) + "...";
        }

        public static T CreateObjectBasedOn<S, T>(S source, bool passOnNullValues = true) where T : class, new()
                                               where S : class
        {
            T newObj = new T();
            Type newtype = newObj.GetType();
            var newProps = newtype.GetProperties();

            Type sourceType = source.GetType();
            var sourceProps = sourceType.GetProperties();

            foreach (var sourceProp in sourceProps)
            {
                var newPropInfo = newProps.SingleOrDefault(np => np.Name == sourceProp.Name && np.PropertyType == sourceProp.PropertyType);

                if (newPropInfo == null)
                {
                    continue;
                }

                var sourceValue = sourceProp.GetValue(source, null);

                if (!passOnNullValues && IsEmpty(sourceValue, sourceProp.PropertyType))
                {
                    continue;
                }

                newPropInfo.SetValue(newObj, sourceValue, null);
            }

            return newObj;
        }

        private static bool IsEmpty(object value, Type propertyType)
        {
            return value == null || (propertyType == typeof(DateTime) && Convert.ToDateTime(value) == default(DateTime));
        }
    }
}

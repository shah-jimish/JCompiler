namespace JCompiler.Helper.Token
{
    public static class TokenIdentifyHelper
    {
        private static readonly Lazy<Dictionary<string, int>> KeywordDictionary = new(InitializeKeywordDictionary);

        public static TokenEnum CheckIfTokenIsKeyword(string tokenText)
        {
            if (KeywordDictionary.Value.TryGetValue(tokenText, out int intValue))
            {
                return intValue >= 100 && intValue < 200 ? (TokenEnum)intValue : TokenEnum.NONE;
            }
            return TokenEnum.NONE;
        }

        private static Dictionary<string, int> InitializeKeywordDictionary()
        {
            var dictionary = new Dictionary<string, int>();

            foreach (var kindValue in Enum.GetValues(typeof(TokenEnum)))
            {
                string kindName = Enum.GetName(typeof(TokenEnum), kindValue);
                int intValue = (int)kindValue;
                if (intValue >= 100 && intValue < 200) // Only add the relevant keywords
                {
                    dictionary[kindName] = intValue;
                }
            }

            return dictionary;
        }
    }
}

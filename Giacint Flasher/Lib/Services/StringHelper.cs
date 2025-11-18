namespace GiacintFlasher.Lib.Services
{
    internal static class StringHelper
    {
        internal static string[] ParseArgs(string text)
        {
            string[] vars = text.Split("--");
            for (int i = 0; i < vars.Length; i++)
                vars[i] = vars[i].Trim().Trim("--".ToCharArray());
            return vars;
        }

        internal static string IsHaveArg(string[] vars, string var)
        {
            foreach (string v in vars)
                if (v.ToLower() == var.ToLower())
                    return v;
            return null;
        }

        internal static string ReplaceContexts(Dictionary<string, string> contexts, string text)
        {
            try
            {
                foreach (KeyValuePair<string, string> context in contexts)
                    text = text.Replace($"{context.Key}", context.Value);
                return text;
            }
            catch
            {
                return text;
            }
        }
    }
}

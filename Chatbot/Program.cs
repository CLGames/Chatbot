using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Chatbot chatbot = new Chatbot();
        }
    }

    public class Chatbot
    {
        private string lastOutput;
        private List<(string input, string response)> pairs = new List<(string input, string response)>();
        private string[] fallbackResponses = new string[]
{
    "Interesting.",
    "Oh, really?",
    "I see.",
    "Hmm, that’s cool.",
    "Got it.",
    "Ah, I understand.",
    "Okay.",
    "Nice!",
    "That’s good to know.",
    "I hear you.",
    "Sounds good.",
    "Right.",
    "Makes sense.",
    "I see what you mean.",
    "Gotcha.",
    "Hmm, okay.",
    "Huh?",
    "What?",
    "What are you talking about!?",
    "Umm what?",
    "I don't fully understand.",
    "This is confusing me."
};
        private string[] conversationStarters = new string[]
{
    "So, what have you been up to lately?",
    "Have you seen any good movies or shows recently?",
    "Do you like listening to music? What's your favorite song right now?",
    "Have you read any interesting books recently?",
    "What's your favorite hobby or way to spend free time?",
    "Do you like traveling? Any places you'd love to visit?",
    "What's the most exciting thing that's happened to you this week?",
    "Do you enjoy cooking or trying new foods?",
    "Are you into sports or fitness activities?",
    "Do you enjoy playing games? Board games or video games?",
    "What's something fun or unusual you've done recently?",
    "Do you like pets? Do you have any?",
    "Have you learned or tried anything new lately?",
    "What's your favorite way to relax after a long day?",
    "Do you like art or creative projects? Painting, drawing, writing?"
};     
        public Chatbot()
        {
            Console.WriteLine("Loading data...");
            string[] data = File.ReadAllLines("data.txt");
            foreach (string line in data)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // skip empty lines

                string[] split = line.Split(new string[] { "|||" }, StringSplitOptions.None);
                if (split.Length == 2) // only add valid pairs
                {
                    pairs.Add((split[0].Trim(), split[1].Trim()));
                }
            }

            Run();
        }

        private void Run()
        {
            Console.Clear();
            lastOutput = null;
            while (true)
            {
                string input = GetInput();
                if (lastOutput != null)
                {
                    File.AppendAllText("data.txt", lastOutput + "|||" + input + Environment.NewLine); // Learn from the user
                }
                string output = GetOutput(Normalise(input));
                Console.WriteLine("Bot: " + output);
                lastOutput = output;
            }
        }

        private string GetOutput(string userInput)
        {
            const double threshold = 0.2;
            List<double> scores = new List<double>();
            List<string> choices = new List<string>();
            Random random = new Random();
            Dictionary<string, int> frequencyTable = new Dictionary<string, int>();

            foreach (var pair in pairs)
            {
                scores.Add(Compare(userInput, Normalise(pair.input)));
            }

            double max = scores.Max();
            

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] == max)
                {
                    string n_response = Normalise(pairs[i].response);
                    if (n_response != Normalise(lastOutput) && n_response != "" && n_response != null & n_response != ".")
                    {
                        choices.Add(pairs[i].response);
                        if (frequencyTable.ContainsKey(n_response)) frequencyTable[n_response]++;
                        else frequencyTable.Add(n_response, 1);
                    }

                }
            }


            if (max < threshold || choices.Count == 0)
            {
                if (random.Next(0, 2) == 1)
                {
                    return fallbackResponses[random.Next(fallbackResponses.Length)];
                }
                else
                {
                    return conversationStarters[random.Next(conversationStarters.Length)];
                }

            }
            else
            {
                List<string> finalChoices = new List<string>();
                int maxWeight = frequencyTable.Values.Max();
                if (maxWeight > 1)
                {
                    foreach (string choice in choices)
                    {
                        if (frequencyTable[Normalise(choice)] == maxWeight) finalChoices.Add(choice);
                    }
                    return finalChoices[random.Next(finalChoices.Count)];
                }
                else
                {
                    //return choices[random.Next(choices.Count)] + $" [{(int) (max * 100)}% confidence]";
                    return choices[random.Next(choices.Count)];
                }
            }
            
        }
        private string GetInput()
        {
            Console.Write("User: ");
            return Console.ReadLine();
        }

        private string Normalise(string text)
        {
            if (text == null) return null;
            Dictionary<string, string> contractions = new Dictionary<string, string>
            {

                {"arent", "are not"},
                {"cant", "cannot"},
                {"couldnt", "could not"},
                {"didnt", "did not"},
                {"doesnt", "does not"},
                {"dont", "do not"},
                {"hadnt", "had not"},
                {"hasnt", "has not"},
                {"havent", "have not"},
                {"hed", "he would"},
                {"hell", "he will"},
                {"hes", "he is"},
                {"hows", "how is"},
                {"id", "I would"},
                {"ill", "I will"},
                {"im", "I am"},
                {"ive", "I have"},
                {"isnt", "is not"},
                {"itd", "it would"},
                {"itll", "it will"},
                {"its", "it is"},
                {"lets", "let us"},
                {"mightnt", "might not"},
                {"mustnt", "must not"},
                {"shant", "shall not"},
                {"shed", "she would"},
                {"shell", "she will"},
                {"shes", "she is"},
                {"shouldnt", "should not"},
                {"thats", "that is"},
                {"theres", "there is"},
                {"theyd", "they would"},
                {"theyll", "they will"},
                {"theyre", "they are"},
                {"theyve", "they have"},
                {"wasnt", "was not"},
                {"wed", "we would"},
                {"well", "we will"},
                {"were", "we are"},
                {"weve", "we have"},
                {"werent", "were not"},
                {"whatll", "what will"},
                {"whatre", "what are"},
                {"whats", "what is"},
                {"whatve", "what have"},
                {"wheres", "where is"},
                {"whod", "who would"},
                {"wholl", "who will"},
                {"whos", "who is"},
                {"wont", "will not"},
                {"wouldnt", "would not"},
                {"youd", "you would"},
                {"youll", "you will"},
                {"youre", "you are"},
                {"youve", "you have"},
                {"gonna", "going to" },
                {"dunno", "don't know" }
            };
            Dictionary<string, string> spellingConversions = new Dictionary<string, string>
        {

    {"maths", "math"},
    {"colour", "color"},
    {"flavour", "flavor"},
    {"honour", "honor"},
    {"labour", "labor"},
    {"rumour", "rumor"},
    {"favour", "favor"},
    {"neighbour", "neighbor"},
    {"harbour", "harbor"},
    {"valour", "valor"},
    {"saviour", "savior"},

    {"centre", "center"},
    {"theatre", "theater"},
    {"metre", "meter"},
    {"litre", "liter"},
    {"kilometre", "kilometer"},
    {"millimetre", "millimeter"},
    {"centimetre", "centimeter"},
    {"tonne", "ton"},

    {"catalogue", "catalog"},
    {"dialogue", "dialog"},
    {"monologue", "monolog"},
    {"analogue", "analog"},

    {"mum", "mom"},
    {"pyjamas", "pajamas"},
    {"jewellery", "jewelry"},
    {"aluminium", "aluminum"},
    {"cheque", "check"},
    {"tyre", "tire"},
    {"aeroplane", "airplane"},
    {"sceptical", "skeptical"},

    {"defence", "defense"},
    {"licence", "license"},
    {"offence", "offense"},
    {"pretence", "pretense"},

    {"organise", "organize"},
    {"realise", "realize"},
    {"recognise", "recognize"},
    {"apologise", "apologize"},
    {"civilise", "civilize"},
    {"criticise", "criticize"},
    {"emphasise", "emphasize"},
    {"finalise", "finalize"},
    {"mobilise", "mobilize"},
    {"specialise", "specialize"},
    {"standardise", "standardize"},
    {"symbolise", "symbolize"}

        };
            Dictionary<string, string> textingAbbreviations = new Dictionary<string, string>
{
    {"u", "you"},
    {"ur", "your"},
    {"r", "are"},
    {"idk", "i dont know"},
    {"brb", "be right back"},
    {"btw", "by the way"},
    {"omg", "oh my god"},
    {"lol", "laughing out loud"},
    {"lmao", "laughing my ass off"},
    {"rofl", "rolling on the floor laughing"},
    {"smh", "shaking my head"},
    {"tbh", "to be honest"},
    {"imo", "in my opinion"},
    {"imho", "in my humble opinion"},
    {"jk", "just kidding"},
    {"np", "no problem"},
    {"thx", "thanks"},
    {"ty", "thank you"},
    {"yw", "youre welcome"},
    {"bff", "best friends forever"},
    {"gtg", "got to go"},
    {"g2g", "got to go"},
    {"omw", "on my way"},
    {"ily", "i love you"},
    {"hbu", "how about you"},
    {"wbu", "what about you"},
    {"wyd", "what are you doing"},
    {"wya", "where are you at"},
    {"idc", "i dont care"},
    {"ikr", "i know right"},
    {"nvm", "never mind"},
    {"tmi", "too much information"},
    {"fyi", "for your information"},
    {"afaik", "as far as i know"},
    {"asap", "as soon as possible"},
    {"irl", "in real life"},
    {"gg", "good game"},
    {"gl", "good luck"},
    {"hf", "have fun"},
    {"wb", "welcome back"},
    {"dm", "direct message"},
    {"pm", "private message"},
    {"omfg", "oh my fucking god"},
    {"wtf", "what the fuck"},
    {"wth", "what the hell"},
    {"xoxo", "hugs and kisses"},
    {"bc", "because"},
    {"bday", "birthday"},
    {"gr8", "great"},
    {"l8r", "later"},
    {"pls", "please"},
    {"plz", "please"},
    {"ppl", "people"},
    {"sry", "sorry"},
    {"faq", "frequently asked questions"},
    {"ftw", "for the win"},
    {"msg", "message"},
    {"sec", "second"},
    {"tmrw", "tomorrow"},
    {"txt", "text"},
    {"vip", "very important person"},
    {"yolo", "you only live once"},
    {"hru", "how are you"},
    {"wdym", "what do you mean" }
};

            text = text.ToLower();
            text = text.Trim();
            text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
            if (text == "") return null;
            string[] words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                word = word.Trim();
                if (spellingConversions.ContainsKey(word))
                {
                    words[i] = spellingConversions[word];
                }
                else if (contractions.ContainsKey(word))
                {
                    words[i] = contractions[word];
                }
                else if (textingAbbreviations.ContainsKey(word))
                {
                    words[i] = textingAbbreviations[word];
                }
            }

            text = string.Join(" ", words);
            
            
            return text;
        }

        private static int Levenshtein(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }
            return dp[a.Length, b.Length];
        }

        // Convert Levenshtein distance to similarity between 0 and 1
        private static double LevenshteinSimilarity(string a, string b)
        {
            int distance = Levenshtein(a, b);
            int maxLen = Math.Max(a.Length, b.Length);
            if (maxLen == 0) return 1.0; // both strings empty
            return 1.0 - (double)distance / maxLen;
        }

        // Main Compare function
        public static double Compare(string a, string b)
        {
            if (a == null || b == null) return 0;
            const double alpha = 0.6; // weight for Cosine
            const double beta = 0.2;  // weight for Jaccard
            const double gamma = 0.2; // weight for Levenshtein

            // Preprocess (lowercase + split by space)
            var wordsA = a.ToLower().Split(' ');
            var wordsB = b.ToLower().Split(' ');

            // --- Jaccard similarity ---
            var setA = new HashSet<string>(wordsA);
            var setB = new HashSet<string>(wordsB);
            double jaccard = setA.Intersect(setB).Count() / (double)setA.Union(setB).Count();

            // --- Cosine similarity ---
            var freqA = wordsA.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var freqB = wordsB.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var allWords = new HashSet<string>(freqA.Keys.Concat(freqB.Keys));

            double dot = allWords.Sum(w => (freqA.ContainsKey(w) ? freqA[w] : 0) *
                                           (freqB.ContainsKey(w) ? freqB[w] : 0));
            double magA = Math.Sqrt(freqA.Values.Sum(v => v * v));
            double magB = Math.Sqrt(freqB.Values.Sum(v => v * v));
            double cosine = dot / (magA * magB + 1e-9); // avoid division by zero

            // --- Levenshtein similarity ---
            double levSim = LevenshteinSimilarity(a.ToLower(), b.ToLower());

            // --- Weighted combination ---
            return alpha * cosine + beta * jaccard + gamma * levSim;
        }
    }

    
}

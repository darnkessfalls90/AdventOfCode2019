namespace FlawedFrequencyTransmission
{
    public class FlawedFrequencyTransmission
    {
        static int[] BasePattern = new int[] { 0, 1, 0, -1 };

        public static void Main(string[] args)
        {
            var signalReceivedListOrig = File.ReadAllText("input.txt").Select(c => int.Parse(c.ToString())).ToList();

            string offset = "";
            for (int i = 0; i < 7; i++)
            {
                offset += signalReceivedListOrig[i].ToString();
            }

            var offsetInt = int.Parse(offset);

            var signalReceivedList = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                signalReceivedList.AddRange(signalReceivedListOrig);
            }

            var signalReceived = signalReceivedList.ToArray();

            for (int i = 0; i < 100; i++)
            {
                signalReceived = Phase(signalReceived);
            }

            
            var message = signalReceived.Skip(offsetInt+1).Take(8).ToArray();

            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
            }

            Console.ReadLine();
        }

        public static int[] Phase(int[] input)
        {
            var result = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                var pattern = CreatePattern(i + 1, input.Length);
                var newVal = 0;
                for (int n = 0; n < input.Length; n++)
                {
                    newVal += input[n] * pattern[n];
                }

                newVal = int.Parse(newVal.ToString().Last().ToString());
                result[i] = newVal;
            }
            return result;
        }

        static Dictionary<int, int[]>  existingPatterns = new Dictionary<int, int[]>();
        public static int[] CreatePattern(int place, int length)
        {
            if (existingPatterns.ContainsKey(place)) return existingPatterns[place];

            List<int> pattern = new List<int>();
            for(int i = 0; i < BasePattern.Length; i++)
            {
                for(int n = 0; n < place; n++)
                {
                    pattern.Add(BasePattern[i]);
                }
            }

            while(pattern.Count < length + 1)
            {
                pattern.AddRange(pattern);
            }

            var patternArr = pattern.Skip(1).Take(length).ToArray();
            existingPatterns.Add(place, patternArr);
            return patternArr;
        }
    }
}
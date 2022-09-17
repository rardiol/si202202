// See https://aka.ms/new-console-template for more information

public class Program
{
    public static void Main(String[] args)
    {
        int populationSize = 100;
        //int numberOfItems = 100;
        int bagCapacity = 500;
        int selectedCrossoverMembers = populationSize/2; 
        int selectedEliteMembers = populationSize/20;
        int maxReps = 10;
        double mutationPercentage = 0.1;

        List<int> allItems = new List<int>() { 11, 29, 2, 17, 19, 5, 14, 28, 23, 18, 16, 5, 2, 22, 20, 12, 3, 13, 16, 27, 22, 9, 13, 24, 8, 1, 4, 29, 22, 29, 21, 18, 4, 2, 25, 10, 23, 6, 17, 16, 5, 27, 20, 16, 25, 19, 24, 20, 26, 25, 6, 3, 11, 21, 12, 24, 9, 2, 5, 19, 3, 7, 28, 21, 9, 27, 26, 6, 3, 21, 25, 1, 10, 23, 10, 29, 24, 2, 28, 12, 13, 24, 27, 19, 7, 9, 17, 3, 12, 9, 19, 10, 6, 0, 21, 13, 29, 18, 18, 3 };

        List<List<bool>> population = InitializePopulation(populationSize, allItems.Count);

        int? bestFitness = null;
        int repeatedBest = 0;

        do
        {
            List<(int, List<bool>)> populationFitness = new List<(int, List<bool>)>();

            Console.WriteLine("Population");
            foreach (var member in population)
            {
                //string combinedString = string.Join(",", member);
                Console.WriteLine(MeasureFitness(member, allItems, bagCapacity));

                populationFitness.Add((MeasureFitness(member, allItems, bagCapacity), member));

            }

            int bestGenerationFitness = populationFitness.OrderByDescending(x => x.Item1).First().Item1;
            List<(int, List<bool>)> crossoverMembers = populationFitness.OrderByDescending(x => x.Item1).Take(selectedCrossoverMembers).ToList();
            List<(int, List<bool>)> eliteMembers = populationFitness.OrderByDescending(x => x.Item1).Take(selectedEliteMembers).ToList();
            Console.WriteLine("EliteMembers");
            foreach (var member in eliteMembers)
            {
                string combinedString = string.Join(",", member.Item2);
                Console.WriteLine(member.Item1);
            }

            if (bestGenerationFitness > bestFitness|| bestFitness == null)
            {
                bestFitness = bestGenerationFitness;
                repeatedBest = 0;
            }
            else
            {
                repeatedBest++;
                if(maxReps == repeatedBest)
                {
                    break;
                }
            }



            //Console.WriteLine("CrossoverMembers");
            //foreach (var member in crossoverMembers)
            //{
            //    string combinedString = string.Join(",", member.Item2);
            //    Console.WriteLine(member.Item1 + ":" + combinedString);
            //}



            Random rnd = new Random();

            List<List<bool>> newMembers = new List<List<bool>>();

            for (int i = 0; i < populationSize - selectedEliteMembers; i++)
            {
                int rand = rnd.Next();

                List<bool> parent1 = crossoverMembers[rand % (populationSize - selectedCrossoverMembers)].Item2;

                rand = rnd.Next();

                List<bool> parent2 = crossoverMembers[rand % (populationSize - selectedCrossoverMembers)].Item2;

                newMembers.Add(Recombination(parent1, parent2, mutationPercentage));

            }

            newMembers.AddRange(eliteMembers.Select(x => x.Item2).ToList());

            //Console.WriteLine("NewPopulation");

            //foreach (var member in newMembers)
            //{
            //    string combinedString = string.Join(",", member);
            //    Console.WriteLine(MeasureFitness(member, allItems, bagCapacity) + ":" + combinedString);
            //}

            population = newMembers;

        }while (repeatedBest < maxReps);

    }

    //private static List<int> InitializeBag(int numberOfItems)
    //{
    //    List<int> bag = new List<int>();

    //    return bag;
    //}

    private static List<List<bool>> InitializePopulation(int populationSize, int totalNumberOfItems)
    {
        List<List<bool>> population = new List<List<bool>>();
        Random rnd = new Random();
        for (int i = 0; i < populationSize; i++)
        {
            List<bool> indviduo = new List<bool>();
            for(int j= 0; j < totalNumberOfItems; j++)
            {
                int rand = rnd.Next();
                indviduo.Add(rand % 2 == 0);
            }
            population.Add(indviduo);
        }

        return population;
    }

    public static int MeasureFitness(List<bool> member, List<int> items, int bagCapacity)
    {
        int memberTotal = 0;

        for(int i = 0; i < member.Count; i++)
        {
            if (member[i])
            {
                memberTotal += items[i];   
            }
        }

        return memberTotal > bagCapacity ? (bagCapacity-memberTotal) : memberTotal;
    }

    public static List<bool> Recombination(List<bool> parent1, List<bool>parent2, double mutationPercentage)
    {
        List<bool> result = new List<bool>();
        Random rnd = new Random();
        int rand;

        if (parent1.Count %2 == 0)
        {
            for(int i = 0; i < (parent1.Count)/2; i++)
            {
                result.Add(parent1[i]);
                result.Add(parent2[i+1]);
            }
        }
        else
        {
            rand = rnd.Next();

            for (int i = 0; i < ((parent1.Count) / 2); i++)
            {
                result.Add(parent1[i]);
                result.Add(parent2[i + 1]);
            }

            if(rand % 2 == 0)
            {
                result.Add(parent1[parent1.Count - 1]);
            }
            else
            {
                result.Add(parent2[parent2.Count - 1]);
            }
        }

        double randDouble = rnd.NextDouble();

        if (randDouble< mutationPercentage)
        {
            Console.WriteLine("mutação");
            Mutation(result);
        }

        return result;
    }

    public static List<bool> Mutation(List<bool> member)
    {
        Random rnd = new Random();
        int rand = rnd.Next();

        member[rand%member.Count] = !member[rand % member.Count];

        return member;
    }
}



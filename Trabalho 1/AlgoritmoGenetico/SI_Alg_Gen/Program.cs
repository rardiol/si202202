// See https://aka.ms/new-console-template for more information

using System.Data;

public class Program
{
    class TestsConfig
    {
        public int listOfItemsSize { get; set; }
        public int populationSize { get; set; }
        public double elitePercentage { get; set; }
        public double mutationPercentage { get; set; }

    }

    public static void Main(String[] args)
    {
        List<int> populationsSizes = new List<int>() { 100, 1000, 10000};
        List<int> listOfItemsSizes = new List<int>() { 100, 1000, 10000 };
        List<double> mutationPercentages = new List<double>() { 0.01, 0.05, 0.1 };
        List<double> elitismPercentages = new List<double>() { 0.01, 0.05, 0.1 };

        int bagCapacity = 50000;
        int configNumber = 1;

        double crossoverPercentage = 0.5;


        List<TestsConfig> testsConfigs = new List<TestsConfig>();
        
        foreach(var listOfItemsSize in listOfItemsSizes)
        {
            foreach(var populationSize in populationsSizes)
            {
                foreach (var mutationPercentage in mutationPercentages)
                {
                    foreach(var elitePercentage in elitismPercentages)
                    {
                        Console.WriteLine($"");
                        Console.WriteLine($"########");
                        Console.WriteLine($"");
                        Console.WriteLine($"Configurações de teste");
                        Console.WriteLine($"");
                        Console.WriteLine($"Tamanho da lista de itens: {listOfItemsSize}");
                        Console.WriteLine($"Tamanho da população: {populationSize}");
                        Console.WriteLine($"Porcentagem de mutação: {mutationPercentage}");
                        Console.WriteLine($"Porcentagem de elitismo: {elitePercentage}");
                        Console.WriteLine($"");
                        Console.WriteLine($"########");
                        Console.WriteLine($"");

                        int maximumBestFitnessRepetition = 20;
                        int numberOfTests = 100;
                        int selectedCrossoverMembers = ((int)(populationSize * crossoverPercentage));
                        int selectedEliteMembers = ((int)(populationSize * elitePercentage));


                        string textFile = System.IO.File.ReadAllText(@"C:\Repos\Faculdade\si202202\Trabalho 1\lista de numeros.txt");

                        List<int> allItems = textFile.Split(',').Select(x => int.Parse(x)).Take(listOfItemsSize).ToList();

                        List<TestResult> testResults = new List<TestResult>();

                        var tableResult = new DataTable("Resultados do teste");

                        tableResult.Columns.Add(new DataColumn("Sucesso"));

                        tableResult.Columns.Add(new DataColumn("Melhor Fitness"));
                        tableResult.Columns.Add(new DataColumn("Numero de gerações"));



                        for (int i = 1; i <= numberOfTests; i++)
                        {
                            //Console.WriteLine($"Teste {i}");

                            List<List<bool>> population = InitializePopulation(populationSize, allItems.Count);

                            int? bestFitness = null;
                            int repeatedBest = 0;
                            int currentGeneration = 1;
                            int bestGenerationFitness;

                            do
                            {

                                List<(int, List<bool>)> populationFitness = GetPopulationFitness(bagCapacity, allItems, population);

                                bestGenerationFitness = populationFitness.OrderByDescending(x => x.Item1).First().Item1;

                                List<(int, List<bool>)> crossoverMembers = populationFitness.OrderByDescending(x => x.Item1).Take(selectedCrossoverMembers).ToList();
                                List<(int, List<bool>)> eliteMembers = populationFitness.OrderByDescending(x => x.Item1).Take(selectedEliteMembers).ToList();

                                if (bestGenerationFitness > bestFitness || bestFitness == null)
                                {
                                    bestFitness = bestGenerationFitness;
                                    repeatedBest = 0;
                                }
                                else
                                {
                                    repeatedBest++;
                                    if (maximumBestFitnessRepetition == repeatedBest)
                                    {
                                        break;
                                    }
                                }
                                //Console.WriteLine($"Geração: {currentGeneration}");
                                //Console.WriteLine($"Melhor membro: {bestFitness}");
                                //Console.WriteLine($"");
                                //Console.WriteLine($"");

                                currentGeneration++;

                                population = Crossover(populationSize, selectedCrossoverMembers, selectedEliteMembers, mutationPercentage, crossoverMembers, eliteMembers);

                            } while (repeatedBest < maximumBestFitnessRepetition && bestFitness != bagCapacity);

                            testResults.Add(
                                new TestResult()
                                {
                                    BestFitness = bestFitness.GetValueOrDefault(),
                                    NumberOfGenerations = currentGeneration,
                                    Success = bestFitness == bagCapacity
                                }
                                );

                            //Console.WriteLine($"");
                            //Console.WriteLine($"########");
                            //Console.WriteLine($"");
                            //Console.WriteLine($"Sucesso: {bestFitness == bagCapacity}");
                            //Console.WriteLine($"");
                            //Console.WriteLine($"Numero de gerações: {currentGeneration}");
                            //Console.WriteLine($"");
                            //Console.WriteLine($"Melhor do teste: {bestFitness}");
                            //Console.WriteLine($"");
                            //Console.WriteLine($"########");
                            //Console.WriteLine($"");

                        }

                        foreach (var testResult in testResults)
                        {
                            tableResult.Rows.Add(testResult.Success, testResult.BestFitness, testResult.NumberOfGenerations);
                        }

                        ToCSV(tableResult, $@"C:\Repos\Faculdade\si202202\Trabalho 1\AlgoritmoGenetico\SI_Alg_Gen\testResults\{configNumber.ToString("###")}-listSize-{listOfItemsSize}-pop{populationSize}-elitism{elitePercentage}-mutation-{mutationPercentage}.csv");
                        configNumber++;
                    }
                }
            }
        }



    }



    private static List<(int, List<bool>)> GetPopulationFitness(int bagCapacity, List<int> allItems, List<List<bool>> population)
    {
        List<(int, List<bool>)> populationFitness = new List<(int, List<bool>)>();

        foreach (var member in population)
        {
            populationFitness.Add((MeasureFitness(member, allItems, bagCapacity), member));
        }

        return populationFitness;
    }

    private static List<List<bool>> Crossover(int populationSize, int selectedCrossoverMembers, int selectedEliteMembers, double mutationPercentage, List<(int, List<bool>)> crossoverMembers, List<(int, List<bool>)> eliteMembers)
    {
        List<List<bool>> population;
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

        population = newMembers;
        return population;
    }

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
    public static void ToCSV(DataTable dtDataTable, string strFilePath)
    {
        StreamWriter sw = new StreamWriter(strFilePath, false);
        //headers    
        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        {
            sw.Write(dtDataTable.Columns[i]);
            if (i < dtDataTable.Columns.Count - 1)
            {
                sw.Write(",");
            }
        }
        sw.Write(sw.NewLine);
        foreach (DataRow dr in dtDataTable.Rows)
        {
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                if (!Convert.IsDBNull(dr[i]))
                {
                    string value = dr[i].ToString();
                    if (value.Contains(','))
                    {
                        value = String.Format("\"{0}\"", value);
                        sw.Write(value);
                    }
                    else
                    {
                        sw.Write(dr[i].ToString());
                    }
                }
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
        }
        sw.Close();
    }
}

class TestResult
{
    public bool Success { get; set; }
    public int BestFitness { get; set; }
    public int NumberOfGenerations { get; set; }
}



using System.Diagnostics;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Travelling_Salesman
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        private List<PointF> cities = new List<PointF>();
        private List<int> bestSolution;
        private float bestDistance;
        private Stopwatch stopwatch = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            bestSolution = new List<int>();
            bestDistance = float.MaxValue;
            buttonLoad.Click += btnLoadCities_Click;
            buttonCalculate.Click += btnRunAlgorithm_Click;
        }

        private void btnLoadCities_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Title = "Select a City Data File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadCities(openFileDialog.FileName);
                }
            }
        }

        private async void btnRunAlgorithm_Click(object? sender, EventArgs e)
        {
            // Parameters for the genetic algorithm
            int populationSize = 100;
            int generations = 500;
            double mutationRate = 0.02;
            double crossoverRate = 0.85;

            stopwatch.Restart();
            lblStatus.Text = "Running...";

            // Initialize population
            List<List<int>> population = InitializePopulation(populationSize, cities.Count);

            // Run the genetic algorithm asynchronously
            await Task.Run(() => RunGeneticAlgorithm(population, generations, mutationRate, crossoverRate));

            stopwatch.Stop();
            lblStatus.Text = $"Completed in {stopwatch.ElapsedMilliseconds / 1000.0:F2} sec";
        }

        private void RunGeneticAlgorithm(List<List<int>> population, int generations, double mutationRate, double crossoverRate)
        {
            // Genetic Algorithm
            for (int generation = 0; generation < generations; generation++)
            {
                // Evaluate fitness
                List<(List<int> path, float fitness)> fitnessPopulation = EvaluateFitness(population, cities);

                // Find the best path found in this generation
                (List<int> path, float fitness) bestPath = fitnessPopulation.OrderBy(p => p.fitness).First();

                // Update the best solution if found
                if (bestPath.fitness < bestDistance)
                {
                    bestDistance = bestPath.fitness;
                    bestSolution = bestPath.path;

                    // Update UI with the current best path
                    this.Invoke((Action)(() =>
                    {
                        DrawPath(bestSolution);
                        lblShortestDistance.Text = String.Format("Shortest Distance: {0:F2}", bestDistance);
                        lblRoute.Text = string.Join(" -> ", bestSolution.Select(i => (i + 1).ToString()))
                                        + $" -> {bestSolution[0] + 1}";

                        // Kiírás StringBuilderrel
                        StringBuilder distancesText = new StringBuilder();

                        // 1. Minden város közötti távolság
                        distancesText.AppendLine("All City-to-City Distances:");
                        for (int i = 0; i < cities.Count; i++)
                        {
                            for (int j = i + 1; j < cities.Count; j++)
                            {
                                PointF city1 = cities[i];
                                PointF city2 = cities[j];
                                float dist = Distance(city1, city2);
                                distancesText.AppendLine($"Distance between City {i + 1} and City {j + 1}: {dist:F2}");
                            }
                        }

                        // 2. Üres sor
                        distancesText.AppendLine();

                        // 3. Aktuális legjobb útvonal menti távolságok
                        distancesText.AppendLine("Distances Along Best Path:");
                        for (int i = 0; i < bestSolution.Count - 1; i++)
                        {
                            int fromIndex = bestSolution[i];
                            int toIndex = bestSolution[i + 1];
                            PointF fromCity = cities[fromIndex];
                            PointF toCity = cities[toIndex];
                            float dist = Distance(fromCity, toCity);
                            distancesText.AppendLine($"Distance between City {fromIndex + 1} and City {toIndex + 1}: {dist:F2}");
                        }

                        // Visszazárás az utolsó városból az elsõbe
                        int lastCityIndex = bestSolution[bestSolution.Count - 1];
                        int firstCityIndex = bestSolution[0];
                        float returnDist = Distance(cities[lastCityIndex], cities[firstCityIndex]);
                        distancesText.AppendLine($"Distance between City {lastCityIndex + 1} and City {firstCityIndex + 1}: {returnDist:F2}");

                        lblDistances.Text = distancesText.ToString();
                    }));
                }

                // Selection
                List<List<int>> selectedPopulation = Selection(fitnessPopulation);

                // Crossover
                List<List<int>> offspringPopulation = Crossover(selectedPopulation, crossoverRate);

                // Mutation
                population = Mutation(offspringPopulation, mutationRate);
            }
        }

        // Draw the best path on the PictureBox
        private void DrawPath(List<int> path)
        {
            Bitmap bitmap = new Bitmap(pbCanvas.Width, pbCanvas.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                // Meghatározzuk a szélsõ pontokat
                float minX = cities.Min(c => c.X);
                float maxX = cities.Max(c => c.X);
                float minY = cities.Min(c => c.Y);
                float maxY = cities.Max(c => c.Y);

                // Elõre meghatározzuk a legfelsõ vonalat
                float minYLine = float.MaxValue;
                (int, int) topEdge = (-1, -1);
                for (int i = 0; i < path.Count; i++)
                {
                    int index1 = path[i];
                    int index2 = path[(i + 1) % path.Count];

                    PointF city1 = cities[index1];
                    PointF city2 = cities[index2];

                    float topY = Math.Min(city1.Y, city2.Y);

                    if (topY < minYLine)
                    {
                        minYLine = topY;
                        topEdge = (index1, index2);
                    }
                }

                Font font = new Font("Arial", 8);
                Pen pen = new Pen(Color.Blue, 2);

                // Városok kirajzolása
                for (int i = 0; i < cities.Count; i++)
                {
                    PointF city = cities[i];
                    g.FillEllipse(Brushes.Red, city.X - 3, city.Y - 3, 6, 6);

                    string label = (i + 1).ToString();
                    SizeF textSize = g.MeasureString(label, font);

                    PointF labelPosition = city;
                    float padding = 6;
                    float edgePadding = 1; // Szélsõ pontokhoz kisebb eltolás

                    bool isInTopLine = (i == topEdge.Item1);

                    if (isInTopLine)
                    {
                        labelPosition = new PointF(city.X - textSize.Width / 2 + 6, city.Y - textSize.Height + 2);
                    }
                    else
                    {
                        bool isLeftMost = Math.Abs(city.X - minX) < 1e-2;
                        bool isRightMost = Math.Abs(city.X - maxX) < 1e-2;
                        bool isTopMost = Math.Abs(city.Y - minY) < 1e-2;
                        bool isBottomMost = Math.Abs(city.Y - maxY) < 1e-2;

                        if (isLeftMost)
                        {
                            labelPosition = new PointF(city.X - textSize.Width - edgePadding, city.Y - textSize.Height / 2);
                        }
                        else if (isRightMost)
                        {
                            labelPosition = new PointF(city.X + 3 + edgePadding, city.Y - textSize.Height / 2);
                        }
                        else if (isTopMost)
                        {
                            labelPosition = new PointF(city.X - textSize.Width / 2, city.Y - textSize.Height - edgePadding);
                        }
                        else if (isBottomMost)
                        {
                            labelPosition = new PointF(city.X - textSize.Width / 2 + 6, city.Y + edgePadding);
                        }
                        else
                        {
                            int currentIndex = path.IndexOf(i);
                            int prevCityIndex = path[(currentIndex - 1 + path.Count) % path.Count];
                            int nextCityIndex = path[(currentIndex + 1) % path.Count];

                            PointF prevCity = cities[prevCityIndex];
                            PointF nextCity = cities[nextCityIndex];

                            float dxPrev = city.X - prevCity.X;
                            float dyPrev = city.Y - prevCity.Y;
                            float dxNext = nextCity.X - city.X;
                            float dyNext = nextCity.Y - city.Y;

                            if (Math.Abs(dxPrev) > Math.Abs(dyPrev) && Math.Abs(dxNext) > Math.Abs(dyNext))
                            {
                                if (dxPrev > 0)
                                    labelPosition = new PointF(city.X - textSize.Width / 2, city.Y + padding);
                                else
                                    labelPosition = new PointF(city.X - textSize.Width / 2, city.Y - textSize.Height - padding);
                            }
                            else
                            {
                                labelPosition = new PointF(city.X - textSize.Width / 2, city.Y + padding);
                            }
                        }
                    }

                    g.DrawString(label, font, Brushes.Black, labelPosition);
                }

                // Útvonal kirajzolása
                for (int i = 0; i < path.Count; i++)
                {
                    PointF city1 = cities[path[i]];
                    PointF city2 = cities[(path[(i + 1) % path.Count])];
                    g.DrawLine(pen, city1, city2);
                }
            }
            pbCanvas.Image = bitmap;
        }

        // Asynchronous method to load cities
        private async void LoadCities(string filePath)
        {
            try
            {
                cities = await LoadCitiesFromFileAsync(filePath);

                // Optionally, update the UI or state based on successful load
                MessageBox.Show("Cities loaded successfully!");
            }
            catch (Exception ex)
            {
                // Handle errors gracefully and inform the user
                MessageBox.Show($"Error loading cities: {ex.Message}");
            }
        }

        // Load city coordinates from a text file asynchronously
        private async Task<List<PointF>> LoadCitiesFromFileAsync(string filePath)
        {
            List<PointF> cities = new List<PointF>();

            // Read all lines asynchronously from the file
            string[] lines = await File.ReadAllLinesAsync(filePath);

            // Check if the file has at least one line (for the number of cities)
            if (lines.Length == 0)
            {
                throw new Exception("File is empty or missing.");
            }

            // Parse the number of cities from the first line
            if (!int.TryParse(lines[0].Trim(), out int N))
            {
                throw new Exception("The first line should contain the number of cities.");
            }

            // Ensure there are enough lines for the specified number of cities
            if (lines.Length < N + 1)
            {
                throw new Exception("The file does not contain enough lines for the specified number of cities.");
            }

            // Parse each city's coordinates
            for (int i = 1; i <= N; i++)
            {
                string line = lines[i].Trim();

                // Check if the line is empty
                if (string.IsNullOrEmpty(line))
                {
                    throw new Exception($"Line {i + 1} is empty.");
                }

                // Split the line by commas to get coordinates
                string[] coords = line.Split(',');

                // Ensure the line contains exactly two coordinates
                if (coords.Length != 2)
                {
                    throw new Exception($"Line {i + 1} does not contain exactly two coordinates.");
                }

                // Parse the x and y coordinates
                if (!float.TryParse(coords[0], out float x) || !float.TryParse(coords[1], out float y))
                {
                    throw new Exception($"Line {i + 1} contains invalid coordinates.");
                }

                // Add the city coordinates to the list
                cities.Add(new PointF(x, y));
            }

            return cities;
        }

        // Initialize the population with random paths
        private List<List<int>> InitializePopulation(int populationSize, int numberOfCities)
        {
            List<List<int>> population = new List<List<int>>();
            for (int i = 0; i < populationSize; i++)
            {
                List<int> path = Enumerable.Range(0, numberOfCities).ToList();
                Shuffle(path);
                population.Add(path);
            }
            return population;
        }

        // Shuffle the list to create a random path
        private void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Evaluate the fitness of each path in the population
        private List<(List<int> path, float fitness)> EvaluateFitness(List<List<int>> population, List<PointF> cities)
        {
            List<(List<int> path, float fitness)> fitnessPopulation = new List<(List<int> path, float fitness)>();
            foreach (List<int> path in population)
            {
                float distance = CalculateTotalDistance(path, cities);
                fitnessPopulation.Add((path, distance));
            }
            return fitnessPopulation;
        }

        // Calculate the total distance of a given path
        private float CalculateTotalDistance(List<int> path, List<PointF> cities)
        {
            float totalDistance = 0.0f;
            for (int i = 0; i < path.Count; i++)
            {
                PointF city1 = cities[path[i]];
                PointF city2 = cities[path[(i + 1) % path.Count]];
                totalDistance += Distance(city1, city2);
            }
            return totalDistance;
        }

        // Calculate the distance between two cities
        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        // Select parents for crossover using tournament selection
        private List<List<int>> Selection(List<(List<int> path, float fitness)> fitnessPopulation)
        {
            List<List<int>> selectedPopulation = new List<List<int>>();
            int tournamentSize = 5;
            for (int i = 0; i < fitnessPopulation.Count; i++)
            {
                List<(List<int> path, float fitness)> tournament = new List<(List<int> path, float fitness)>();
                for (int j = 0; j < tournamentSize; j++)
                {
                    int randomIndex = rand.Next(fitnessPopulation.Count);
                    tournament.Add(fitnessPopulation[randomIndex]);
                }
                (List<int> path, float fitness) bestInTournament = tournament.OrderBy(p => p.fitness).First();
                selectedPopulation.Add(bestInTournament.path);
            }
            return selectedPopulation;
        }

        // Perform crossover to create offspring
        private List<List<int>> Crossover(List<List<int>> selectedPopulation, double crossoverRate)
        {
            List<List<int>> offspringPopulation = new List<List<int>>();
            for (int i = 0; i < selectedPopulation.Count; i += 2)
            {
                List<int> parent1 = selectedPopulation[i];
                List<int> parent2 = selectedPopulation[(i + 1) % selectedPopulation.Count];
                if (rand.NextDouble() < crossoverRate)
                {
                    // Perform ordered crossover (OX)
                    (List<int> child1, List<int> child2) = OrderedCrossover(parent1, parent2);
                    offspringPopulation.Add(child1);
                    offspringPopulation.Add(child2);
                }
                else
                {
                    // No crossover, just add the parents
                    offspringPopulation.Add(new List<int>(parent1));
                    offspringPopulation.Add(new List<int>(parent2));
                }
            }
            return offspringPopulation;
        }

        // Ordered Crossover (OX) for TSP
        private (List<int>, List<int>) OrderedCrossover(List<int> parent1, List<int> parent2)
        {
            int size = parent1.Count;
            int start = rand.Next(size);
            int end = rand.Next(start, size);

            List<int> child1 = new List<int>(new int[size]);
            List<int> child2 = new List<int>(new int[size]);

            // Copy the selected segment from the first parent to the child
            for (int i = start; i <= end; i++)
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }

            // Fill the remaining positions from the second parent
            FillRemainingPositions(child1, parent2, start, end);
            FillRemainingPositions(child2, parent1, start, end);

            return (child1, child2);
        }

        // Fill remaining positions for crossover
        private void FillRemainingPositions(List<int> child, List<int> parent, int start, int end)
        {
            int size = parent.Count;
            int currentIndex = (end + 1) % size;
            int parentIndex = (end + 1) % size;
            while (currentIndex != start)
            {
                while (child.Contains(parent[parentIndex]))
                {
                    parentIndex = (parentIndex + 1) % size;
                }
                child[currentIndex] = parent[parentIndex];
                currentIndex = (currentIndex + 1) % size;
            }
        }

        // Perform mutation on the offspring
        private List<List<int>> Mutation(List<List<int>> population, double mutationRate)
        {
            foreach (List<int> path in population)
            {
                if (rand.NextDouble() < mutationRate)
                {
                    if (rand.NextDouble() < 0.5)
                    {
                        // Perform swap mutation
                        /*int index1 = rand.Next(path.Count);
                        int index2 = rand.Next(path.Count);
                        int temp = path[index1];
                        path[index1] = path[index2];
                        path[index2] = temp;*/

                        if (rand.NextDouble() < 0.5)
                        {
                            int index1 = rand.Next(path.Count);
                            int index2 = rand.Next(path.Count);
                            (path[index1], path[index2]) = (path[index2], path[index1]);
                        }
                        else
                        {
                            int index1 = rand.Next(path.Count - 1);
                            path.Reverse(index1, 2);
                        }
                    }
                }
            }
            return population;
        }
    }
}
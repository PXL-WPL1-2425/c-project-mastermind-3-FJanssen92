using Microsoft.VisualBasic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MastermindCSProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Een klasse die de kleuren van de pogingen en de borders van de gekozen kleuren bijhoudt.
        /// Voor elke kleur wordt de kleur, de kleur van de border en de dikte van de border bijgehouden.
        /// </summary>
        private class Attempt
        {
            public Brush ChosenColor1 { get; set; }
            public Brush ChosenColor2 { get; set; }
            public Brush ChosenColor3 { get; set; }
            public Brush ChosenColor4 { get; set; }
            public Brush Color1BorderBrush { get; set; }
            public Thickness Color1BorderThickness { get; set; }
            public Brush Color2BorderBrush { get; set; }
            public Thickness Color2BorderThickness { get; set; }
            public Brush Color3BorderBrush { get; set; }
            public Thickness Color3BorderThickness { get; set; }
            public Brush Color4BorderBrush { get; set; }
            public Thickness Color4BorderThickness { get; set; }
        }

        public class Player
        {
            public string Name { get; set; }
            public int Score { get; set; }
            public int Attempts { get; set; }

            public Player(string name)
            {
                Name = name;
                Score = 100;
                Attempts = 1;
            }
        }

        private List<Attempt> attemptsList;
        private string color1, color2, color3, color4;
        private int attempts = 1;
        private DispatcherTimer timer;
        private int startTime;
        private int score = 100;
        private Color[] colors = { Colors.White, Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Orange };
        private int[] colorIndex = { 0, 0, 0, 0 };
        private string inputName;
        private List<string> highscores = new List<string>();
        private int maxAttempts = 10;
        private List<Player> players = new List<Player>();
        private int playerIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            attemptsList = new List<Attempt>();
            timer = new DispatcherTimer();
            RandomColors(out color1, out color2, out color3, out color4);
            secretCodeTextBox.Text = $"Kleur 1: {color1}, Kleur 2: {color2}, Kleur 3:{color3}, Kleur 4:{color4}";
            Title = $"Mastermind - Poging: {attempts}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();
            inputName = players[playerIndex].Name;
            StartCountdown();

        }

        /// <summary>
        /// De naam van de speler wordt gevraagd en een nieuw Player object wordt aangemaakt met de ingevoerde naam.
        /// Daarna wordt er aan de speler gevraagd of er nog een speler toegevoegd moet worden. Dit blijft herhaald totdat de speler op nee klikt.
        /// </summary>
        public void StartGame()
        {
            string name;
            do
            {
                name = Interaction.InputBox("Geef uw naam in.", "Speler Naam");
            }
            while (string.IsNullOrEmpty(name));
            players.Add(new Player(name));

            while (true)
            {
                MessageBoxResult result = MessageBox.Show("Wilt u nog een speler toevoegen?", "Extra Speler Toevoegen", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    break;
                }
                name = Interaction.InputBox("Naam nieuwe speler: ", "Speler Toevoegen");
                if (!string.IsNullOrEmpty(name))
                {
                    players.Add(new Player(name));
                }
            }
        }

        

        /// <summary>
        /// Timer wordt geinitialiseerd en gestart.
        /// de start tijd wordt ingesteld op 10, zodat de timer vanaf 10 kan aftellen in de Timer_Tick methode.
        /// </summary>
        private void StartCountdown()
        {
            startTime = 10;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick -= Timer_Tick;
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        /// <summary>
        /// De timer telt af van 10 naar 0, zolang de starttijd groter is dan 0.
        /// Zoniet, wordt de StopTimer methode aangeroepen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (startTime >= 0)
            {
                timerLabel.Content = "Timer: " + startTime.ToString();
                startTime--;
            }
            else
            {
                StopTimer();
            }
        }


        /// <summary>
        /// De timer wordt gestopt en het aantal pogingen wordt verhoogd en in de titel weergegeven.
        /// Daarna wordt de StartCountdown methode weer opgeroepen zodat er een nieuwe beurt begint.
        /// Er wordt gekeken of de tijd verstreken is en of de speler nog pogingen over heeft.
        /// </summary>
        private void StopTimer()
        {
            timer.Stop();
            Player currentPlayer = players[playerIndex];

            if (currentPlayer.Attempts < maxAttempts)
            {
                currentPlayer.Attempts++;
                Title = $"Mastermind - Poging: {attempts}";
                timerLabel.Content = "Tijd is op! Beurt verloren!";
                currentPlayer.Score -= 8;
                scoreLabel.Content = "Score: " + score;
                StartCountdown();
            }

            else
            {
                highscores.Add($"Naam: {currentPlayer.Name} - Aantal pogingen: {currentPlayer.Attempts} - Score: {currentPlayer.Score}");
                string nextPlayerName = players[(playerIndex + 1) % players.Count].Name;
                MessageBox.Show($"Game Over! De correcte code was: {color1} - {color2} - {color3} - {color4}\nVolgende speler: {nextPlayerName}", currentPlayer.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                NextPlayer();
                
            }


        }

        /// <summary>
        /// De game wordt gereset en de geheime code wordt opnieuw gegenereerd.
        /// </summary>
        private void ResetGame()
        {
            Player currentPlayer = players[playerIndex];
            currentPlayer.Attempts = 1;
            currentPlayer.Score = 100;
            RandomColors(out color1, out color2, out color3, out color4);
            secretCodeTextBox.Text = $"Kleur 1: {color1}, Kleur 2: {color2}, Kleur 3:{color3}, Kleur 4:{color4}";
            Title = $"Mastermind - Poging: {attempts}";
            timerLabel.Content = "Timer: 10";

            color1Ellipse.Fill = Brushes.White;
            color2Ellipse.Fill = Brushes.White;
            color3Ellipse.Fill = Brushes.White;
            color4Ellipse.Fill = Brushes.White;

            color1Border.BorderBrush = Brushes.Black;
            color1Border.BorderThickness = new Thickness(1);
            color2Border.BorderBrush = Brushes.Black;
            color2Border.BorderThickness = new Thickness(1);
            color3Border.BorderBrush = Brushes.Black;
            color3Border.BorderThickness = new Thickness(1);
            color4Border.BorderBrush = Brushes.Black;
            color4Border.BorderThickness = new Thickness(1);

            attemptsList.Clear();
            attemptsListBox.ItemsSource = null;
            StartCountdown();
        }

        private void NextPlayer()
        {
            playerIndex = (playerIndex + 1) % players.Count;
            ResetGame();
        }

        /// <summary>
        /// Methode die laat zien wat de geheime code is als de speler CTRL + F12 indrukt.
        /// Als de toetscombinatie daarna weer wordt ingedrukt, verdwijnt de textbox weer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleDebug(object sender, KeyEventArgs e)
        {
            //Als CTRL + F12 wordt ingedrukt EN de texbox is verborgen, laat deze dan zien.
            if ((e.Key == Key.F12 && e.KeyboardDevice.Modifiers == ModifierKeys.Control) & secretCodeTextBox.Visibility == Visibility.Hidden)
            {
                secretCodeTextBox.Visibility = Visibility.Visible;
            }

            //Hetzelfde gebeurd hier, maar omgekeerd. Als de textbox zichtbaar is, wordt deze verborgen.
            else if ((e.Key == Key.F12 && e.KeyboardDevice.Modifiers == ModifierKeys.Control) & secretCodeTextBox.Visibility == Visibility.Visible)
            {
                secretCodeTextBox.Visibility= Visibility.Hidden;
            }
        }


        /// <summary>
        /// Er wordt gekeken op welke ellipse geklikt wordt door de index te bepalen.
        /// Daarna wordt de kleur van de ellipse veranderd naar de volgende kleur in de array.
        /// Er wordt modulus gebruikt om ervoor te zorgen dat de index niet buiten de array valt.
        /// Daarna wordt de kleur van de geselecteerde ellipse veranderd naar de volgende kleur in de colors array.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            int index = 0;

            if (ellipse == color1Ellipse) index = 0;
            else if (ellipse == color2Ellipse) index = 1;
            else if (ellipse == color3Ellipse) index = 2;
            else if (ellipse == color4Ellipse) index = 3;

            colorIndex[index] = (colorIndex[index] + 1) % colors.Length;
            ellipse.Fill = new SolidColorBrush(colors[colorIndex[index]]);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void HighScores_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string highscore in highscores)
            {
                sb.AppendLine(highscore);
            }

            MessageBox.Show(sb.ToString(), "Mastermind Highscores");
        }

        private void AantalPogingen_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("Geef het maximum aantal pogingen in.", "Aantal Pogingen");
            if (!int.TryParse(input, out maxAttempts) || maxAttempts <3 || maxAttempts > 20)
            {
                MessageBox.Show("Geef een getal in tussen 3 en 20!", "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult close = MessageBox.Show("Bent u zeker dat u het spel wilt verlaten?", "Afsluiten", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (close == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public void RandomColors(out string color1, out string color2, out string color3, out string color4)
        {
            Random newColor = new Random();
            List<string> colorList = new List<string>();
            string randomColor;

            for (int i = 1; i <= 4; i++)
            {
                randomColor = (newColor.Next(1, 7)).ToString();
                switch (randomColor)
                {
                    case "1":
                        randomColor = "White";
                        break;
                    case "2":
                        randomColor = "Red";
                        break;
                    case "3":
                        randomColor = "Blue";
                        break;
                    case "4":
                        randomColor = "Green";
                        break;
                    case "5":
                        randomColor = "Yellow";
                        break;
                    case "6":
                        randomColor = "Orange";
                        break;
                }

                colorList.Add(randomColor);
            }

            color1 = colorList[0];
            color2 = colorList[1];
            color3 = colorList[2];
            color4 = colorList[3];

        }

        /// <summary>
        /// De gekozen en gegenereerde kleuren worden vergeleken met elkaar. 
        /// Er wordt een Attempt object aangemaakt en de gekozen kleuren worden toegevoegd.
        /// Daarna krijgen de ellipses een border afhankelijk van de situatie.
        /// Deze borders worden ook toegevoegd aan het attempt object.
        /// Het volledige attempt object wordt toegevoegd aan de attemptsList en de listbox wordt geupdate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkCodeButton_Click(object sender, RoutedEventArgs e)
        {
            Player currentPlayer = players[playerIndex];

            //Gekozen kleuren opslaan in variabelen en in een lijst zetten.
            Brush chosenColor1, chosenColor2, chosenColor3, chosenColor4;
            chosenColor1 = color1Ellipse.Fill;
            chosenColor2 = color2Ellipse.Fill;
            chosenColor3 = color3Ellipse.Fill;
            chosenColor4 = color4Ellipse.Fill;

            List<Brush> chosenColors = new List<Brush> { chosenColor1, chosenColor2, chosenColor3, chosenColor4 };

            //Omdat de gegenereerde kleuren een string zijn, moeten deze omgezet worden naar een Brush.
            List<Brush> generatedColors = new List<Brush> 
            { new SolidColorBrush((Color)ColorConverter.ConvertFromString(color1)), 
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(color2)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(color3)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(color4))
            };

            //Een nieuwe poging aanmaken en de gekozen kleuren toevoegen.
            var attempt = new Attempt
            {
                ChosenColor1 = chosenColor1,
                ChosenColor2 = chosenColor2,
                ChosenColor3 = chosenColor3,
                ChosenColor4 = chosenColor4,
            };

            bool allCorrect = true;

            //Border wordt gelijkgesteld aan de juiste border, afhankelijk van de index.
            for (int i = 0; i < 4; i++) 
            {
                Border targetBorder = null;

                switch (i)
                {
                    case 0: targetBorder = color1Border;
                        break;
                    case 1:
                        targetBorder = color2Border;
                        break;
                    case 2:
                        targetBorder = color3Border;
                        break;
                    case 3:
                        targetBorder = color4Border;
                        break;
                }

                //Als de targetBorder niet null is, wordt er gekeken of de kleur van de gekozen kleur gelijk is aan de gegenereerde kleur.
                //Een gekleurde border wordt toegevoegd afhankelijk van de situatie.
                //De score wordt ook al gelijk aangepast in dezelfde vergelijking.
                if (targetBorder != null)
                {
                    if (((SolidColorBrush)chosenColors[i]).Color == ((SolidColorBrush)generatedColors[i]).Color)
                    {
                        targetBorder.BorderBrush = Brushes.DarkRed;
                        targetBorder.BorderThickness = new Thickness(5);
                    }
                    else if (generatedColors.Any(gc => ((SolidColorBrush)gc).Color == ((SolidColorBrush)chosenColors[i]).Color))
                    {
                        targetBorder.BorderBrush = Brushes.Wheat;
                        targetBorder.BorderThickness = new Thickness(5);
                        score -= 1;
                        allCorrect = false;
                    }
                    else
                    {
                        targetBorder.BorderBrush = Brushes.Transparent;
                        targetBorder.BorderThickness = new Thickness(5);
                        score -= 2;
                        allCorrect = false;
                    }

                    //De juiste border wordt dan ook meegegeven aan de attempt.
                    switch (i)
                    {
                        case 0:
                            attempt.Color1BorderBrush = targetBorder.BorderBrush;
                            attempt.Color1BorderThickness = targetBorder.BorderThickness;
                            break;
                        case 1:
                            attempt.Color2BorderBrush = targetBorder.BorderBrush;
                            attempt.Color2BorderThickness = targetBorder.BorderThickness;
                            break;
                        case 2:
                            attempt.Color3BorderBrush = targetBorder.BorderBrush;
                            attempt.Color3BorderThickness = targetBorder.BorderThickness;
                            break;
                        case 3:
                            attempt.Color4BorderBrush = targetBorder.BorderBrush;
                            attempt.Color4BorderThickness = targetBorder.BorderThickness;
                            break;
                    
                    }
                }
            }

            //De attempt wordt toegevoegd aan een lijst, die op zijn beurt weer wordt toegevoegd aan de listbox.
            attemptsList.Add(attempt);
            attemptsListBox.ItemsSource = null;
            attemptsListBox.ItemsSource = attemptsList;

            //Als alle kleuren correct zijn geraden, wordt de speler gefeliciteerd en stopt de timer.
            //Dit gebeurt door de Boolean allCorrect. Als alle kleuren correct zijn, blijft deze true en wordt het spel beeindigt.
            if (allCorrect)
            {
                highscores.Add($"Naam: {currentPlayer.Name} - Aantal pogingen: {currentPlayer.Attempts} - Score: {currentPlayer.Score}");
                string nextPlayerName = players[(playerIndex + 1) % players.Count].Name;
                MessageBox.Show($"Gefeliciteerd, {currentPlayer.Name}! Je hebt de code geraden! Score: {currentPlayer.Score} Pogingen: {currentPlayer.Attempts}\r\n Volgende speler: {nextPlayerName}", currentPlayer.Name);
                StopTimer();
            }
            else if (attempts < maxAttempts)
            {
                attempts++;
                this.Title = $"Mastermind - Poging: {attempts}";
                StartCountdown();
            }
            else
            {
                StopTimer();
            }

            scoreLabel.Content = "Score: " + score;

        }
    }
}
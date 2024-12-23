﻿using Microsoft.VisualBasic;
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

        private List<Attempt> _attemptsList;
        private string _color1, _color2, _color3, _color4;
        private DispatcherTimer _timer;
        private int _startTime;
        private Color[] _colors = { Colors.White, Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Orange };
        private int[] _colorIndex = { 0, 0, 0, 0 };
        private List<string> _highscores = new List<string>();
        private int _maxAttempts = 10;
        private List<Player> _players = new List<Player>();
        private int _playerIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            _attemptsList = new List<Attempt>();
            _timer = new DispatcherTimer();
            RandomColors(out _color1, out _color2, out _color3, out _color4);
            secretCodeTextBox.Text = $"Kleur 1: {_color1}, Kleur 2: {_color2}, Kleur 3:{_color3}, Kleur 4:{_color4}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();
            StartCountdown();
            Title = $"Mastermind - Poging: {_players[_playerIndex].Attempts}";

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
            _players.Add(new Player(name));
            playerNameLabel.Content = "Speler: " + _players[_playerIndex].Name;

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
                    _players.Add(new Player(name));
                }
            }

        }

        

        /// <summary>
        /// Timer wordt geinitialiseerd en gestart.
        /// de start tijd wordt ingesteld op 10, zodat de timer vanaf 10 kan aftellen in de Timer_Tick methode.
        /// </summary>
        private void StartCountdown()
        {
            _startTime = 10;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick -= Timer_Tick;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }


        /// <summary>
        /// De timer telt af van 10 naar 0, zolang de starttijd groter is dan 0.
        /// Zoniet, wordt de StopTimer methode aangeroepen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_startTime >= 0)
            {
                timerLabel.Content = "Timer: " + _startTime.ToString();
                _startTime--;
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
            _timer.Stop();
            Player currentPlayer = _players[_playerIndex];

            if (currentPlayer.Attempts < _maxAttempts)
            {
                currentPlayer.Attempts++;
                Title = $"Mastermind - Poging: {currentPlayer.Attempts}";
                timerLabel.Content = "Tijd is op! Beurt verloren!";
                currentPlayer.Score -= 8;
                scoreLabel.Content = "Score: " + currentPlayer.Score;
                StartCountdown();
            }

            else
            {
                _highscores.Add($"Naam: {currentPlayer.Name} - Aantal pogingen: {currentPlayer.Attempts} - Score: {currentPlayer.Score}");
                string nextPlayerName = _players[(_playerIndex + 1) % _players.Count].Name;
                MessageBox.Show($"Game Over! De correcte code was: {_color1} - {_color2} - {_color3} - {_color4}\nVolgende speler: {nextPlayerName}", currentPlayer.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                scoreLabel.Content = "Score: 100";
                NextPlayer();
                
            }


        }

        /// <summary>
        /// De game wordt gereset en de geheime code wordt opnieuw gegenereerd.
        /// </summary>
        private void ResetGame()
        {
            Player currentPlayer = _players[_playerIndex];
            currentPlayer.Attempts = 1;
            currentPlayer.Score = 100;
            RandomColors(out _color1, out _color2, out _color3, out _color4);
            secretCodeTextBox.Text = $"Kleur 1: {_color1}, Kleur 2: {_color2}, Kleur 3:{_color3}, Kleur 4:{_color4}";
            Title = $"Mastermind - Poging: {currentPlayer.Attempts}";
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

            _attemptsList.Clear();
            attemptsListBox.ItemsSource = null;
            StartCountdown();
        }

        private void NextPlayer()
        {
            _playerIndex = (_playerIndex + 1) % _players.Count;
            playerNameLabel.Content = "Speler: " + _players[_playerIndex].Name;
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

            _colorIndex[index] = (_colorIndex[index] + 1) % _colors.Length;
            ellipse.Fill = new SolidColorBrush(_colors[_colorIndex[index]]);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void HighScores_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string highscore in _highscores)
            {
                sb.AppendLine(highscore);
            }

            MessageBox.Show(sb.ToString(), "Mastermind Highscores");
        }

        private void AantalPogingen_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("Geef het maximum aantal pogingen in.", "Aantal Pogingen");
            if (!int.TryParse(input, out _maxAttempts) || _maxAttempts <3 || _maxAttempts > 20)
            {
                MessageBox.Show("Geef een getal in tussen 3 en 20!", "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Error);
            }  
        }

        /// <summary>
        /// Er wordt gevraagd aan de speler of hij een hint wilt kopen, op basis van de selectie wordt de juiste methode uitgevoerd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buyHintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Wilt u een hint kopen voor zowel de kleur en de locatie? (Yes, -25 punten) of alleen de kleur? (No, -15 punten)", "Hint Kopen", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                BuyHintColorLocation();
            }
            else if (result == MessageBoxResult.No)
            {
                BuyHintColor();
            }
        }


        /// <summary>
        /// De methode geeft een hint aan de speler.
        /// Er wordt een willekeurige index gekozen en een eerste check wordt gedaan om te kijken als de speler de kleur op deze index al juist heeft gekozen.
        /// Als dit niet het geval is, wordt gekeken of de kleur op een andere index wel voorkomt. Er wordt een hint voorzien aan de speler waarin de kleur staat, maar niet de positie.
        /// </summary>
        private void BuyHintColor()
        {
            Player currentPlayer = _players[_playerIndex];

            List<Brush> generatedColors = new List<Brush>
            { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color1)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color2)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color3)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color4))
            };

            List<Brush> chosenColors = new List<Brush> {
                color1Ellipse.Fill,
                color2Ellipse.Fill,
                color3Ellipse.Fill,
                color4Ellipse.Fill };

            Dictionary<Color, string> colorNames = new Dictionary<Color, string>
            {
                { Colors.White, "White" },
                { Colors.Red, "Red" },
                { Colors.Blue, "Blue" },
                { Colors.Green, "Green" },
                { Colors.Yellow, "Yellow" },
                { Colors.Orange, "Orange" }
            };

            Random random = new Random();
            int randomIndex;
            bool hintGiven = false;

            while (!hintGiven)
            {
                randomIndex = random.Next(0, 4);
                if (((SolidColorBrush)chosenColors[randomIndex]).Color != ((SolidColorBrush)generatedColors[randomIndex]).Color)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (randomIndex != i && ((SolidColorBrush)chosenColors[randomIndex]).Color == ((SolidColorBrush)generatedColors[i]).Color)
                        {
                            Color hintColor = ((SolidColorBrush)generatedColors[randomIndex]).Color;
                            MessageBox.Show($"Hint: Kleur {colorNames[hintColor]} is een van de kleuren!");
                            currentPlayer.Score -= 15;
                            scoreLabel.Content = "Score: " + currentPlayer.Score;
                            hintGiven = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// De methode geeft een hint aan de speler.
        /// Er wordt een willekeure index gegenereerd en wordt vergeleken met de gegenereerde kleuren.
        /// Dit blijft herhaald worden totdat er een verschil is tussen de gekozen en gegenereerde kleuren, zodat de hint niet al een kleur geeft die de speler al juist heeft.
        /// Vervolgens wordt de hint weergegeven. Zowel de kleur als de locatie.
        /// </summary>
        private void BuyHintColorLocation()
        {
            Player currentPlayer = _players[_playerIndex];

            List<Brush> generatedColors = new List<Brush>
            { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color1)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color2)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color3)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color4))
            };

            List<Brush> chosenColors = new List<Brush> {
                color1Ellipse.Fill,
                color2Ellipse.Fill,
                color3Ellipse.Fill,
                color4Ellipse.Fill };

            Dictionary<Color, string> colorNames = new Dictionary<Color, string>
            {
                { Colors.White, "White" },
                { Colors.Red, "Red" },
                { Colors.Blue, "Blue" },
                { Colors.Green, "Green" },
                { Colors.Yellow, "Yellow" },
                { Colors.Orange, "Orange" }
            };

            Random random = new Random();
            int randomIndex;

            //Er wordt een willekeurige index gekozen en gekeken of de kleur op deze index al juist is gekozen.
            do
            {
                randomIndex = random.Next(0, 4);
            } while (((SolidColorBrush)chosenColors[randomIndex]).Color == ((SolidColorBrush)generatedColors[randomIndex]).Color);

            //Als de kleur op de willekeurige index niet juist is gekozen, wordt de hint weergegeven.
            Color hintColor = ((SolidColorBrush)generatedColors[randomIndex]).Color;
            MessageBox.Show($"Hint: Kleur {randomIndex + 1} is {colorNames[hintColor]}.");
            currentPlayer.Score -= 25;
            scoreLabel.Content = "Score: " + currentPlayer.Score;

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
            Player currentPlayer = _players[_playerIndex];

            //Gekozen kleuren opslaan in variabelen en in een lijst zetten.
            Brush chosenColor1, chosenColor2, chosenColor3, chosenColor4;
            chosenColor1 = color1Ellipse.Fill;
            chosenColor2 = color2Ellipse.Fill;
            chosenColor3 = color3Ellipse.Fill;
            chosenColor4 = color4Ellipse.Fill;

            List<Brush> chosenColors = new List<Brush> { chosenColor1, chosenColor2, chosenColor3, chosenColor4 };

            //Omdat de gegenereerde kleuren een string zijn, moeten deze omgezet worden naar een Brush.
            List<Brush> generatedColors = new List<Brush> 
            { new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color1)), 
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color2)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color3)),
              new SolidColorBrush((Color)ColorConverter.ConvertFromString(_color4))
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
                Label targetLabel = null;

                switch (i)
                {
                    case 0:
                        targetBorder = color1Border;
                        targetLabel = ellipse1Label;
                        break;
                    case 1:
                        targetBorder = color2Border;
                        targetLabel = ellipse2Label;
                        break;
                    case 2:
                        targetBorder = color3Border;
                        targetLabel = ellipse3Label;
                        break;
                    case 3:
                        targetBorder = color4Border;
                        targetLabel = ellipse4Label;
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
                        targetLabel.Content = "Juiste kleur, juiste positie!";
                    }
                    else if (generatedColors.Any(gc => ((SolidColorBrush)gc).Color == ((SolidColorBrush)chosenColors[i]).Color))
                    {
                        targetBorder.BorderBrush = Brushes.Wheat;
                        targetBorder.BorderThickness = new Thickness(5);
                        targetLabel.Content = "Juiste kleur, foute positie!";
                        currentPlayer.Score -= 1;
                        allCorrect = false;
                    }
                    else
                    {
                        targetBorder.BorderBrush = Brushes.Transparent;
                        targetBorder.BorderThickness = new Thickness(5);
                        targetLabel.Content = "Foute kleur!";
                        currentPlayer.Score -= 2;
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
            _attemptsList.Add(attempt);
            attemptsListBox.ItemsSource = null;
            attemptsListBox.ItemsSource = _attemptsList;

            //Als alle kleuren correct zijn geraden, wordt de speler gefeliciteerd en stopt de timer.
            //Dit gebeurt door de Boolean allCorrect. Als alle kleuren correct zijn, blijft deze true en wordt het spel beeindigt.
            if (allCorrect)
            {
                _highscores.Add($"Naam: {currentPlayer.Name} - Aantal pogingen: {currentPlayer.Attempts} - Score: {currentPlayer.Score}");
                string nextPlayerName = _players[(_playerIndex + 1) % _players.Count].Name;
                MessageBox.Show($"Gefeliciteerd, {currentPlayer.Name}! Je hebt de code geraden! Score: {currentPlayer.Score} Pogingen: {currentPlayer.Attempts}\r\n Volgende speler: {nextPlayerName}", currentPlayer.Name);
                scoreLabel.Content = "Score: 100";
                NextPlayer();
            }
            else if (currentPlayer.Attempts < _maxAttempts && currentPlayer.Score >= 0)
            {
                currentPlayer.Attempts++;
                this.Title = $"Mastermind - Poging: {currentPlayer.Attempts}";
                scoreLabel.Content = "Score: " + currentPlayer.Score;
                StartCountdown();
            }
            else
            {
                StopTimer();
            }


        }
    }
}
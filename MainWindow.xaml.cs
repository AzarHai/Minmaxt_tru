using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimaxAlphaBeta
{
    public partial class MainWindow : Window
    {
        private Random _random = new Random();
        private Node _root;
        private Point _lastMousePosition;
        private bool _isDragging = false;
        List<string> strings = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            TreeCanvas.MouseLeftButtonDown += TreeCanvas_MouseLeftButtonDown;
            TreeCanvas.MouseLeftButtonUp += TreeCanvas_MouseLeftButtonUp;
            TreeCanvas.MouseMove += TreeCanvas_MouseMove;

            // Центрирование Canvas при загрузке
            this.Loaded += (s, e) =>
            {
                var scrollViewer = TreeCanvas.Parent as ScrollViewer;
                if (scrollViewer != null)
                {
                    double initialX = (scrollViewer.ActualWidth - TreeCanvas.Width * TreeScaleTransform.ScaleX) / 2;
                    double initialY = (scrollViewer.ActualHeight - TreeCanvas.Height * TreeScaleTransform.ScaleY) / 2;
                    TreeTranslateTransform.X = initialX > 0 ? initialX : 0;
                    TreeTranslateTransform.Y = initialY > 0 ? initialY : 0;
                }
            };
        }

        private void TreeCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            _isDragging = true;
            TreeCanvas.CaptureMouse();
        }

        private void TreeCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = false;
            TreeCanvas.ReleaseMouseCapture();
        }

        private void TreeCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - _lastMousePosition.X;
                double offsetY = currentPosition.Y - _lastMousePosition.Y;

                TreeTranslateTransform.X += offsetX;
                TreeTranslateTransform.Y += offsetY;

                _lastMousePosition = currentPosition;
            }
        }
        private void OnNewButtonClick(object sender, RoutedEventArgs e)
        {
            Minmaxt_tru.MainWindow2 window2 = new Minmaxt_tru.MainWindow2();
            window2.Show();
            this.Close();
        }

        private void OnBuildTreeClick(object sender, RoutedEventArgs e)
        {
            // Clear previous tree and conditions
            PruningConditionsTextBox.Clear();
            TreeCanvas.Children.Clear();

            int depth = DepthComboBox.SelectedIndex + 1; // Get depth from ComboBox
            bool isMax = ((ComboBoxItem)FirstPlayerComboBox.SelectedItem).Content.ToString() == "MAX";

            // Parse branching range
            if (!int.TryParse(MinBranchTextBox.Text, out int minBranch) || !int.TryParse(MaxBranchTextBox.Text, out int maxBranch) ||
                minBranch <= 0 || maxBranch < minBranch)
            {
                MessageBox.Show("Укажите корректный диапазон потомков.");
                return;
            }

            _root = GenerateTree(depth, isMax, minBranch, maxBranch);
            DrawTree(_root, TreeCanvas.ActualWidth / 2, 20, TreeCanvas.ActualWidth / 4, 0);
        }

        private Node GenerateTree(int depth, bool isMax, int minBranch, int maxBranch, int level = 0)
        {
            Node node = new Node { IsMax = isMax };

            // Base case: if depth is 0, generate a random value for the leaf
            if (depth == 0)
            {
                node.Value = _random.Next(-10, 10);
                return node;
            }

            // Determine the number of children for the node
            int branching = _random.Next(minBranch, maxBranch + 1);

            for (int i = 0; i < branching; i++)
            {
                node.Children.Add(GenerateTree(depth - 1, !isMax, minBranch, maxBranch, level + 1));
            }

            return node;
        }





        private void OnRunAlgorithmClick(object sender, RoutedEventArgs e)
        {
            if (_root == null)
            {
                MessageBox.Show("Сначала постройте дерево!");
                return;
            }
            PruningConditionsTextBox.Clear();
            bool isMax = ((ComboBoxItem)FirstPlayerComboBox.SelectedItem).Content.ToString() == "MAX";
            int depth = DepthComboBox.SelectedIndex + 1;
            bool orderLeftToRight = ((ComboBoxItem)OrderComboBox.SelectedItem).Content.ToString() == "Слева-направо";

            int bestValue = Minimax(_root, depth, int.MinValue, int.MaxValue, isMax, orderLeftToRight);
            HighlightOptimalPath(_root);
            MessageBox.Show($"Лучшая оценка первого хода: {bestValue}");
        }

        private Node GenerateTree(int depth, bool isMax, int level = 0)
        {
            Node node = new Node { IsMax = isMax };

            if (depth == 0)
            {
                node.Value = _random.Next(-10, 10);
                return node;
            }

            if (BranchingInputPanel.Children.Count > level)
            {
                var inputStack = BranchingInputPanel.Children[level] as StackPanel;
                var comboBox = inputStack?.Children[1] as ComboBox;
                int branching = comboBox?.SelectedIndex + 1 ?? 2; // Получение выбранного количества потомков (от 1 до 4)

                for (int i = 0; i < branching; i++)
                {
                    node.Children.Add(GenerateTree(depth - 1, !isMax, level + 1));
                }
            }

            return node;
        }
        private int Minimax(Node node, int depth, int alpha, int beta, bool isMax, bool orderLeftToRight)
        {
            if (depth == 0 || node.Children.Count == 0)
            {
                UpdateNodeValueDisplay(node); // обновление отображения листового узла
                return node.Value;
            }

            var children = orderLeftToRight ? node.Children : node.Children.AsEnumerable().Reverse();
            int value = isMax ? int.MinValue : int.MaxValue;
            bool pruned = false; // Флаг для отсечения

            foreach (var child in children)
            {
                if (pruned) // Пропускаем оставшиеся итерации, но выделяем узлы
                {
                    MarkAsPruned(child);
                    HighlightPrunedSubtree(child);
                    continue;
                }

                int childValue = Minimax(child, depth - 1, alpha, beta, !isMax, orderLeftToRight);

                if (isMax)
                {
                    value = Math.Max(value, childValue);
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    value = Math.Min(value, childValue);
                    beta = Math.Min(beta, value);
                }

                if (beta <= alpha) // Условие отсечения
                {
                    // Отметим текущий узел и все его потомки как отсеченные
                    //MarkAsPruned(child);

                    

                    // Выделим поддерево, которое отсечено
                    HighlightPrunedSubtree(child);
                    string condition = isMax
                        ? $"α ({alpha}) ≥ β ({beta}) — узел отсечён"
                        : $"β ({beta}) ≤ α ({alpha}) — узел отсечён";
                    AddPruningCondition(condition);
                    pruned = true; // Устанавливаем флаг отсечения
                }
            }

            node.Value = value;
            UpdateNodeValueDisplay(node); // обновление отображения промежуточного узла
            return value;
        }

        private void MarkAsPruned(Node node)
        {
            node.Pruned = true;
            foreach (var child in node.Children)
            {
                MarkAsPruned(child);
            }
        }

        private void HighlightPrunedSubtree(Node node)
        {
            if (node.Pruned)
            {
                node.Color = Brushes.Red; // Красный цвет для отсечённых узлов
                UpdateNodeVisual(node);   // Обновление визуализации узла
            }

            foreach (var child in node.Children)
            {
                HighlightPrunedSubtree(child); // Рекурсивное выделение
            }
        }

        private void UpdateNodeVisual(Node node)
        {
            if (node.UIElement is Ellipse ellipse)
            {
                ellipse.Fill = node.Color; // Применение нового цвета
            }
        }
        private void UpdateNodeValueDisplay(Node node)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var child in TreeCanvas.Children.OfType<TextBlock>())
                {
                    if (child.Tag == node)
                    {
                        child.Text = node.Value.ToString();
                        break;
                    }
                }
            });
        }
        private void DrawTree(Node node, double x, double y, double offset, int level)
        {
            

            

            double currentOffset = level == 0 ? offset * 8.5 :
                                    level == 1 ? offset * 1.6 :
                                    level == 2? offset *  1.4:
                                    level == 3 ? offset * 1.6: offset;

            double childX = x - currentOffset * (node.Children.Count - 1) / 2;

            foreach (var child in node.Children)
            {
                if (child.Pruned)
                {
                    DrawLine(x, y, childX, y + 80, Brushes.Gray, DashStyles.Dash); // Пунктирная линия
                }
                else
                {
                    DrawLine(x, y, childX, y + 80, Brushes.Black, DashStyles.Solid); // Обычная линия
                }

                DrawTree(child, childX, y + 100, currentOffset / 5, level +1); // Рекурсивный вызов
                childX += currentOffset;
                
            }
            // Рисуем узел
            DrawNode(node, x, y, level);

        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush color, DashStyle dashStyle)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = color,
                StrokeThickness = 2,
                StrokeDashArray = dashStyle == DashStyles.Dash ? new DoubleCollection { 4, 2 } : null
            };
            TreeCanvas.Children.Add(line);
        }

        private void HighlightOptimalPath(Node node)
        {
            if (node == null || node.Children.Count == 0) return;

            var optimalChild = node.Children.FirstOrDefault(c => c.Value == node.Value && !c.Pruned);
            if (optimalChild != null)
            {
                // Найти линию между текущим узлом и оптимальным потомком
                HighlightLineBetweenNodes(node, optimalChild);
                HighlightOptimalPath(optimalChild);
            }
        }

        private void HighlightLineBetweenNodes(Node parent, Node child)
        {
            foreach (var line in TreeCanvas.Children.OfType<Line>())
            {
                if (IsLineBetweenNodes(line, parent, child))
                {
                    line.Stroke = Brushes.Green; // Зеленая линия
                    break;
                }
            }

            foreach (var ellipse in TreeCanvas.Children.OfType<Ellipse>())
            {
                if (Math.Abs(Canvas.GetLeft(ellipse) + ellipse.Width / 2 - parent.X) < 1e-2 &&
                    Math.Abs(Canvas.GetTop(ellipse) + ellipse.Height / 2 - parent.Y) < 1e-2)
                {
                    ellipse.Fill = Brushes.LightGreen; // Изменение цвета узла родителя
                }

                if (Math.Abs(Canvas.GetLeft(ellipse) + ellipse.Width / 2 - child.X) < 1e-2 &&
                    Math.Abs(Canvas.GetTop(ellipse) + ellipse.Height / 2 - child.Y) < 1e-2)
                {
                    ellipse.Fill = Brushes.LightGreen; // Изменение цвета узла потомка
                }
            }
        }



        private bool IsLineBetweenNodes(Line line, Node parent, Node child)
        {
            // Определить, соединяет ли линия два узла
            // Предположим, вы добавили координаты узлов в сами узлы (например, node.X, node.Y)
            return Math.Abs(line.X1 - parent.X) < 1e-2 &&
                   Math.Abs(line.Y1 - parent.Y) < 1e-2 &&
                   Math.Abs(line.X2 - child.X) < 1e-2 &&
                   Math.Abs(line.Y2 - child.Y) < 1e-2;
        }

        private void DrawNode(Node node, double x, double y, int level)
        {
            node.X = x;
            node.Y = y;

            Ellipse ellipse = new Ellipse
            {
                Width = 40,
                Height = 40,
                Stroke = Brushes.Black,
                Fill = node.IsMax ? Brushes.LightBlue : Brushes.LightPink
            };

            // Сохранение UIElement в узле
            node.UIElement = ellipse;

            Canvas.SetLeft(ellipse, x - 20);
            Canvas.SetTop(ellipse, y - 20);
            TreeCanvas.Children.Add(ellipse);

            TextBlock text = new TextBlock
            {
                Text = node.Value.ToString(),
                Foreground = Brushes.Black,
                Tag = node
            };
            Canvas.SetLeft(text, x - 10);
            Canvas.SetTop(text, y - 10);
            TreeCanvas.Children.Add(text);
        }


        private void OnSetBranchingLevelsClick(object sender, RoutedEventArgs e)
        {
            BranchingInputPanel.Children.Clear();
            int depth = DepthComboBox.SelectedIndex + 1; // Глубина начинается с 1

            for (int i = 0; i < depth; i++)
            {
                var levelStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                var label = new TextBlock { Text = $"Потомки на уровне {i + 1}:", Margin = new Thickness(5) };
                var comboBox = new ComboBox { Width = 50, Margin = new Thickness(5) };

                // Добавление значений от 1 до 4 для выбора количества потомков
                for (int j = 1; j <= 4; j++)
                {
                    comboBox.Items.Add(j);
                }

                comboBox.SelectedIndex = 1; // Значение по умолчанию (2 потомка)
                levelStack.Children.Add(label);
                levelStack.Children.Add(comboBox);
                BranchingInputPanel.Children.Add(levelStack);
            }
        }
        private void TreeCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            const double zoomFactor = 0.1; // Коэффициент масштабирования
            double zoomDelta = e.Delta > 0 ? zoomFactor : -zoomFactor; // Увеличение или уменьшение

            // Ограничение масштаба
            double newScaleX = TreeScaleTransform.ScaleX + zoomDelta;
            double newScaleY = TreeScaleTransform.ScaleY + zoomDelta;
            if (newScaleX < 0.1 || newScaleY < 0.1 || newScaleX > 5 || newScaleY > 5)
                return;

            // Центрирование относительно текущей позиции мыши
            Point mousePosition = e.GetPosition(TreeCanvas);
            double relativeX = mousePosition.X / TreeCanvas.ActualWidth;
            double relativeY = mousePosition.Y / TreeCanvas.ActualHeight;

            TreeTranslateTransform.X -= (mousePosition.X * zoomDelta) * relativeX;
            TreeTranslateTransform.Y -= (mousePosition.Y * zoomDelta) * relativeY;

            // Применение масштабирования
            TreeScaleTransform.ScaleX = newScaleX;
            TreeScaleTransform.ScaleY = newScaleY;
        }
        private void AddPruningCondition(string condition)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PruningConditionsTextBox.Text += condition + Environment.NewLine;
            });
        }


        public class Node
        {
            public int Value { get; set; }
            public bool IsMax { get; set; }
            public bool Pruned { get; set; }
            public double X { get; set; } // Координата X узла
            public double Y { get; set; } // Координата Y узла
            public List<Node> Children { get; } = new List<Node>();
            public UIElement UIElement { get; set; } // Связанный UI-элемент
            public Brush Color { get; set; } // Цвет узла
        }

    }
}
